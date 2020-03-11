using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbiter : MonoBehaviour
{
    public Vector3 mAxis = Vector3.up;
    public float mRotationSpeed = 0.0f;
    public float mOrbitalDistance = 0.0f;
    public Transform mOrbitAround;
    public float mOrbitalSpeed = 0.0f;

    private Vector3 mDesiredPosition;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = (transform.position - mOrbitAround.position).normalized * mOrbitalDistance + mOrbitAround.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(mOrbitAround.position, mAxis, mRotationSpeed * Time.deltaTime);
        mDesiredPosition = (transform.position - mOrbitAround.position).normalized * mOrbitalDistance + mOrbitAround.position;
        transform.position = Vector3.MoveTowards(transform.position, mDesiredPosition, Time.deltaTime * mOrbitalSpeed);
    }
}
