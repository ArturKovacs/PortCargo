using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using System.Threading.Tasks;
using Nakama;
using Newtonsoft.Json;

public class NakamaClient : MonoBehaviour
{
    private enum MessageId
    {
        RequestRedistributeOwnership = 0,
        ApplyOwership = 1,
        SynchronizeObject = 2
    }

    private const string AuthTokenName = "nakama.session";

    public IClient Client { get; private set; }
    //public ISocket Socket { get; private set; }
    public IMatch Match { get; private set; }

    private Task<ISession> sessionTask;
    private ISocket socket;

    private Dictionary<NetworkedObjectId, SynchronizedObject> synchronizedById;
    private HashSet<SynchronizedObject> primeval;
    private int instantiatedSeqNum = 0;

    private Dictionary<NetworkedObjectId, SynchronizedObject> owned;

    List<IUserPresence> connectedOpponents;

    public IEnumerator JoinMatch(string id)
    {
        var matchTask = socket.JoinMatchAsync(id);
        yield return StartCoroutine(FinishMatchCreateOrJoin(matchTask));
    }

    public IEnumerator CreateMatch()
    {
        var matchTask = socket.CreateMatchAsync();
        yield return StartCoroutine(FinishMatchCreateOrJoin(matchTask));
    }

    public IEnumerator FinishMatchCreateOrJoin(Task<IMatch> matchTask)
    {
        while (!matchTask.IsCompleted)
        {
            Debug.Log("Waiting to match to be ready.");
            yield return new WaitForSeconds(0.5f);
        }
        Match = matchTask.Result;
        connectedOpponents.Remove(Match.Self);
        StartCoroutine(SynchronizationLoop());
    }

    public void ObjectCreated(SynchronizedObject obj)
    {
        if (primeval.Contains(obj)) return;

        var id = new InstantiatedObjectId()
        {
            UserId = Match.Self.UserId,
            SequenceNum = instantiatedSeqNum++,
        };
    }

    public void RequestRedistributeOwnership()
    {
        if (!TryRedistributeOwnership())
        {
            socket.SendMatchStateAsync(Match.Id, (int)MessageId.RequestRedistributeOwnership, (byte[])null);
        }
    }

    /// <summary>
    /// Returns true if self is the decision maker. False otherwise.
    /// </summary>
    /// <returns></returns>
    public bool TryRedistributeOwnership()
    {
        if (!SelfIsTheDecisionMaker()) return false;

        Dictionary<NetworkedObjectId, string> newOwnderships = new Dictionary<NetworkedObjectId, string>();
        var presences = Match.Presences.ToArray();
        int presenceId = 0;
        foreach (var item in synchronizedById)
        {
            newOwnderships.Add(item.Key, presences[presenceId].UserId);
            presenceId = (presenceId + 1) % presences.Length;
        }

        var ownershipsJson = JsonConvert.SerializeObject(newOwnderships, Formatting.None);
        socket.SendMatchStateAsync(Match.Id, (int)MessageId.ApplyOwership, ownershipsJson);
        return true;
    }

    private bool SelfIsTheDecisionMaker()
    {
        foreach (var presence in Match.Presences)
        {
            if (presence.UserId.CompareTo(Match.Self.UserId) < 0)
            {
                // The one with the "lowest" userid is the decision maker
                return false;
            }
        }
        return true;
    }

    private void ApplyIncomingOwnerships(string ownershipsJson)
    {
        var data = JsonConvert.DeserializeObject<Dictionary<NetworkedObjectId, string>>(ownershipsJson);
        owned.Clear();
        foreach (var ownership in data)
        {
            if (ownership.Value == Match.Self.UserId)
            {
                owned.Add(ownership.Key, synchronizedById[ownership.Key]);
            }
        }
    }

    private void SendSynchronizedStates()
    {
        Dictionary<NetworkedObjectId, string> data = new Dictionary<NetworkedObjectId, string>();
        foreach (var item in owned)
        {
            data.Add(item.Key, item.Value.GetCurrentState());
        }
        string message = JsonConvert.SerializeObject(data, Formatting.None);
        socket.SendMatchStateAsync(Match.Id, (int)MessageId.SynchronizeObject, message);
    }

    private void ApplySynchronizedStates(string json)
    {
        var data = JsonConvert.DeserializeObject<Dictionary<NetworkedObjectId, string>>(json);
        foreach (var item in data)
        {
            synchronizedById[item.Key].ApplyIncomingState(item.Value);
        }
    }

    private void ReceivedMatchState(IMatchState state)
    {
        var encoding = System.Text.Encoding.UTF8;
        switch ((MessageId)state.OpCode)
        {
            case MessageId.RequestRedistributeOwnership:
                TryRedistributeOwnership();
                break;
            case MessageId.ApplyOwership:
                ApplyIncomingOwnerships(encoding.GetString(state.State));
                break;
            case MessageId.SynchronizeObject:
                ApplySynchronizedStates(encoding.GetString(state.State));
                break;
        }
    }

    private IEnumerator SynchronizationLoop()
    {
        while (true)
        {
            if (Match != null)
            {
                SendSynchronizedStates();
            }
            yield return new WaitForSeconds(0.1f);
        }
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

    private void OnLevelWasLoaded(int level)
    {
        Debug.Log("OnLevelWasLoaded");
        synchronizedById = new Dictionary<NetworkedObjectId, SynchronizedObject>();
        primeval = new HashSet<SynchronizedObject>();

        var rootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (var item in rootObjects)
        {
            CollectPrimevalObjectsBelow(item.transform);
        }

        if (Match != null) RequestRedistributeOwnership();
    }

    private void CollectPrimevalObjectsBelow(Transform parent)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            var target = parent.GetChild(i);
            var sync = target.GetComponent<SynchronizedObject>();
            if (sync != null)
            {
                var id = new PrimevalObjectId()
                {
                    Id = synchronizedById.Count
                };
                synchronizedById.Add(id, sync);
                primeval.Add(sync);
            }
            CollectPrimevalObjectsBelow(target);
        }
    }

    private async void Awake()
    {
        DontDestroyOnLoad(this);

        owned = new Dictionary<NetworkedObjectId, SynchronizedObject>();

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
        socket.ReceivedMatchState += ReceivedMatchState;
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

    private static int GetHierarchyDepth(Transform transform)
    {
        if (transform.parent == null) return 0;
        return 1 + GetHierarchyDepth(transform.parent);
    }
}
