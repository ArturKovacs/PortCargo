using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Threading.Tasks;
using Nakama;

public class NakamaClient : MonoBehaviour
{
    private const string AuthTokenName = "nakama.session";

    public IClient Client { get; private set; }
    //public ISocket Socket { get; private set; }
    public IMatch Match { get; private set; }

    private Task<ISession> sessionTask;
    private ISocket socket;

    List<IUserPresence> connectedOpponents;

    public void JoinMatch(string id)
    {
        var matchTask = socket.JoinMatchAsync(id);
        matchTask.Wait();
        Match = matchTask.Result;
        connectedOpponents.Remove(Match.Self);
    }

    public void CreateMatch()
    {
        var matchTask = socket.CreateMatchAsync();
        matchTask.Wait();
        Match = matchTask.Result;
        connectedOpponents.Remove(Match.Self);
    }

    private Task<ISession> AuthenticateAsync()
    {
        // Modify to fit the authentication strategy you want within your game.
        // EXAMPLE:
        const string deviceIdPrefName = "deviceid";
        var deviceId = PlayerPrefs.GetString(deviceIdPrefName, SystemInfo.deviceUniqueIdentifier);
#if UNITY_EDITOR
        Debug.LogFormat("Device id: {0}", deviceId);
#endif
        // With device IDs save it locally in case of OS updates which can change the value on device.
        PlayerPrefs.SetString(deviceIdPrefName, deviceId);
        return Client.AuthenticateDeviceAsync(deviceId);
    }

    private async void Awake()
    {
        DontDestroyOnLoad(this);
        Client = new Client("http", "127.0.0.1", 7350, "defaultkey")
        {
#if UNITY_EDITOR
            Logger = new UnityLogger()
#endif
        };
        socket = Client.NewSocket();

        // Restore session or create a new one.
        var authToken = PlayerPrefs.GetString(AuthTokenName);
        //var session = Session.Restore(authToken);
        ISession session = null;
        var expiredDate = DateTime.UtcNow.AddDays(-1);
        if (session == null || session.HasExpired(expiredDate))
        {
            sessionTask = AuthenticateAsync();
            await sessionTask.ContinueWith(t =>
            {
                if (t.IsCompleted)
                {
                    PlayerPrefs.SetString(AuthTokenName, t.Result.AuthToken);
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
            session = await sessionTask;
        }
        Debug.LogFormat("Active Session: {0}", session);
        var account = await Client.GetAccountAsync(session);
        Debug.LogFormat("Account id: {0}", account.User.Id);

        socket.Closed += () => Debug.Log("Socket closed.");
        socket.Connected += () => Debug.Log("Socket connected.");
        socket.ReceivedError += Debug.LogError;
        await socket.ConnectAsync(session);
        connectedOpponents =  new List<IUserPresence>(2);
        socket.ReceivedMatchPresence += presenceEvent =>
        {
            foreach (var presence in presenceEvent.Leaves)
            {
                connectedOpponents.Remove(presence);
            }
            connectedOpponents.AddRange(presenceEvent.Joins);
            // Remove yourself from connected opponents.
            if (Match != null) connectedOpponents.Remove(Match.Self);
            Debug.LogFormat("Connected opponents: [{0}]", string.Join(",\n  ", connectedOpponents));
        };
    }

    private void OnApplicationQuit() => socket?.CloseAsync();
}
