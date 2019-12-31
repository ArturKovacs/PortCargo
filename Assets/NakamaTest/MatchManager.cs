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
        StartCoroutine(JoinCoroutine());
    }

    public void Create()
    {
        StartCoroutine(CreateCoroutine());
    }

    private IEnumerator JoinCoroutine()
    {
        yield return client.JoinMatch(MatchName.text);
        LoadGame();
    }

    private IEnumerator CreateCoroutine()
    {
        yield return client.CreateMatch();
        LoadGame();
    }

    private void LoadGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("NakamaTestGame");
    }
}
