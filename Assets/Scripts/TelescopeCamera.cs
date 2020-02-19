using UnityEngine;
using System.Collections;

public class TelescopeCamera : MonoBehaviour
{
    // Speed for the radial movement.
    public float mainSpeed = 32.0f;
    // Speed for rotation movement.
    public float rotationSpeed = 10.0f;

    // Right ascension angle [deg].
    public float rightAscension;
    // Declination angle [deg].
    public float declination;
    // Distance from origin [pc].
    private float distance;

    private void Start()
    {
        rightAscension = 0.0f;
        declination = 0.0f;
        distance = 1.0f;
    }

    void Update()
    {
        // Declination and ascension movements.
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            float turnSpeed = rotationSpeed * Time.deltaTime;
            rightAscension += turnSpeed;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            float turnSpeed = rotationSpeed * Time.deltaTime;
            rightAscension -= turnSpeed;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            float turnSpeed = rotationSpeed * Time.deltaTime;
            declination -= turnSpeed;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            float turnSpeed = rotationSpeed * Time.deltaTime;
            declination += turnSpeed;
        }

        // Radial movement.
        if (Input.GetKey(KeyCode.W))
        {
            distance += Time.deltaTime * mainSpeed;
        }
        if (Input.GetKey(KeyCode.S))
        {
            if (distance > (1.0f + mainSpeed))
            {
                distance -= Time.deltaTime * mainSpeed;
            }
        }

        // Update camera position with celestial coordinates.
        transform.position = CelestialToCartesian(rightAscension, declination, distance);

        // Update camera rotation to look at the current direction.
        transform.rotation = Quaternion.LookRotation(transform.position, transform.up);
    }

    private Vector3 CelestialToCartesian(
            float rightAscension,
            float declination,
            float distance)
    {
        Vector3 cartesian = Vector3.zero;

        cartesian.x = distance * Mathf.Cos(declination * Mathf.Deg2Rad) * Mathf.Cos(rightAscension * Mathf.Deg2Rad);
        cartesian.y = distance * Mathf.Cos(declination * Mathf.Deg2Rad) * Mathf.Sin(rightAscension * Mathf.Deg2Rad);
        cartesian.z = distance * Mathf.Sin(declination * Mathf.Deg2Rad);

        return cartesian;
    }
}