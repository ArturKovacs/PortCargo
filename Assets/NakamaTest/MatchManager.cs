using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchManager : MonoBehaviour
{
    public InputField MatchName;

    NakamaClient client;

    private void Start()
    {
        client = FindObjectOfType<NakamaClient>();
    }

    private void Update()
    {
        
    }

    public void Join()
    {
        client.JoinMatch(MatchName.text);
        LoadGame();
    }

    public void Create()
    {
        client.CreateMatch();
        LoadGame();
    }

    private void LoadGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("NakamaTestGame");
    }
}
