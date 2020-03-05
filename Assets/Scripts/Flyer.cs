using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flyer : MonoBehaviour
{
    public float speed = 3.0f;

    public GameObject direction;
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float axisY = OVRInput.Get(OVRInput.RawAxis2D.RThumbstick).y;
        if (Input.GetKeyDown(KeyCode.W))
        {
            axisY = 4.0f;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            axisY = -4.0f;
        }
        float axisX = OVRInput.Get(OVRInput.RawAxis2D.RThumbstick).x;

        rb.velocity += direction.transform.forward * speed * axisY + direction.transform.right * speed * axisX;
        rb.velocity *= 0.98f;
    }
}
