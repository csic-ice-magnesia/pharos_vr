using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pulsar : MonoBehaviour
{
    public Vector3 rightAscension;
    public Vector3 declination;
    public Vector3 rotationAxis;
    public float distance;
    public float f0;
    public float bsurf;

    // Start is called before the first frame update
    void Start()
    {
        enabled = false;
        this.transform.GetChild(0).gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
