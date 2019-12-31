using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NakamaInfo : MonoBehaviour
{
    public TMP_InputField InfoText;

    NakamaClient client;

    // Start is called before the first frame update
    void Start()
    {
        client = FindObjectOfType<NakamaClient>();
    }

    // Update is called once per frame
    void Update()
    {
        if (client.Match != null)
        {
            InfoText.text = string.Format("Match Id: {0}\nUser Id: {1}", client.Match.Id, client.Match.Self.UserId);
        }
    }
}
