using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;

public class SynchronizedObject : MonoBehaviour
{
    [Serializable]
    private struct SynchronizationData
    {
        // Doing this uglyness because Unity's Vector3 and Quaternion are not
        // serializeable
        public float PosX, PosY, PosZ;
        public float RotX, RotY, RotZ, RotW;

        public Vector3 Position
        {
            get => new Vector3(PosX, PosY, PosZ);
            set
            {
                PosX = value.x;
                PosY = value.y;
                PosZ = value.z;
            }
        }

        public Quaternion Rotation
        {
            get => new Quaternion(RotX, RotY, RotZ, RotW);
            set
            {
                RotX = value.x;
                RotY = value.y;
                RotZ = value.z;
                RotW = value.w;
            }
        }
    }

    NakamaClient client;

    /// <summary>
    /// Just keep the state buffer around so that it does not need to be allocated every time.
    /// </summary>
    private byte[] stateBuffer;

    // Start is called before the first frame update
    void Start()
    {
        // TODO Notify the client that this object has been created
        // This is only relevant if the object was instantiated during runtime.
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ApplyIncomingState(string json)
    {
        var data = JsonConvert.DeserializeObject<SynchronizationData>(json);
        transform.position = data.Position;
        transform.rotation = data.Rotation;
    }

    public string GetCurrentState()
    {
        SynchronizationData data = new SynchronizationData();
        data.Position = transform.position;
        data.Rotation = transform.rotation;

        return JsonConvert.SerializeObject(data, Formatting.None);
    }
}
