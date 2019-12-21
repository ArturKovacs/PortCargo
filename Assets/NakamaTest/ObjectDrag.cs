using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakama;
using Nakama.Snippets;


public class ObjectDrag : MonoBehaviour
{
    NakamaClient client;
    bool initialized;

    private void Awake()
    {
        initialized = false;
    }

    private void Start()
    {
        client = FindObjectOfType<NakamaClient>();
        foreach (var presence in client.Match.Presences)
        {
        }
    }

    void Update()
    {
        if (!initialized)
        {
            
        }
    }
}
