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

    // Start is called before the first frame update.
    void Start()
    {
        // Astrophysical jet is disabled by default for effiency.
        this.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        // Disable the companion for the moment.
        this.transform.GetChild(1).gameObject.SetActive(false);
        // Orbital is disabled for efficiency and clutter.
        this.transform.GetChild(2).gameObject.SetActive(false);
        this.transform.GetChild(3).gameObject.SetActive(false);
        // Disable updating for efficiency.
        enabled = false;
    }
}
