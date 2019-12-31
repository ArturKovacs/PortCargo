using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPush : MonoBehaviour
{
    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float power = 2;
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            rb.AddForce(Vector3.left * power, ForceMode.Impulse);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            rb.AddForce(-Vector3.left * power, ForceMode.Impulse);
        }
    }
}
