using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircumferenceDrawer : MonoBehaviour
{
    public Material parallelMaterial;
    public Material meridianMaterial;
    public Material equatorMaterial;

    public float[] parallels = { -90.0f, -80.0f, -70.0f, -60.0f, -50.0f, -40.0f, -30.0f, -20.0f, -10.0f, 10.0f, 20.0f, 30.0f, 40.0f, 50.0f, 60.0f, 70.0f, 80.0f, 90.0f };
    public float[] meridians = { -90.0f, -80.0f, -70.0f, -60.0f, -50.0f, -40.0f, -30.0f, -20.0f, -10.0f, 0.0f, 10.0f, 20.0f, 30.0f, 40.0f, 50.0f, 60.0f, 70.0f, 80.0f, 90.0f };

    public float lineWidth = 0.5f;

    void Start()
    {
        // Parallels.
        for (int i = 0; i < parallels.Length; ++i)
        {
            Debug.Log("Parallel " + parallels[i].ToString());
            GameObject go = new GameObject("Parallel_" + i.ToString());
            go.transform.SetParent(this.transform);

            LineRenderer lineRenderer = go.AddComponent<LineRenderer>();

            List<Vector3> parallelPoints = ComputeParallel(parallels[i], 128.0f, 64);
            Debug.Log(parallelPoints.ToArray().Length);
            lineRenderer.positionCount = parallelPoints.Count;
            lineRenderer.SetPositions(parallelPoints.ToArray());
            lineRenderer.material = parallelMaterial;
            lineRenderer.material.color = new Color(0.25f, 0.28f, 0.58f, 1.0f);
            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth;
        }

        // Meridians.
        for (int i = 0; i < meridians.Length; ++i)
        {
            GameObject go = new GameObject("Meridian_" + i.ToString());
            go.transform.SetParent(this.transform);

            List<Vector3> meridianPoints = ComputeMeridian(meridians[i], 128.0f, 64);

            LineRenderer lineRenderer = go.AddComponent<LineRenderer>();
            lineRenderer.positionCount = meridianPoints.Count;
            lineRenderer.SetPositions(meridianPoints.ToArray());
            lineRenderer.material = meridianMaterial;
            lineRenderer.material.color = new Color(0.25f, 0.28f, 0.58f, 1.0f);
            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth;
        }

        // Equator.
        {
            GameObject go = new GameObject("CelestialEquator");
            go.transform.SetParent(this.transform);

            LineRenderer lineRenderer = go.AddComponent<LineRenderer>();

            List<Vector3> parallelPoints = ComputeParallel(0.0f, 128.0f, 64);

            lineRenderer.positionCount = parallelPoints.Count;
            lineRenderer.SetPositions(parallelPoints.ToArray());
            lineRenderer.material = parallelMaterial;
            lineRenderer.material.color = new Color(0.24f, 0.50f, 0.57f, 1.0f);
            lineRenderer.startWidth = lineWidth * 1.25f;
            lineRenderer.endWidth = lineWidth * 1.25f;
        }
    }

    void Update()
    {
        
    }

    private List<Vector3> ComputeParallel(
        float declination,
        float radius,
        int divisions)
    {
        List<Vector3> parallelPoints = new List<Vector3>();

        float rightAscensionStep = 360.0f / divisions;
        for (float rightAscension = 0.0f; rightAscension <= 360.0f; rightAscension += rightAscensionStep)
        {
            Vector3 point = CelestialToCartesian(rightAscension, declination, radius);
            parallelPoints.Add(point);
        }

        return parallelPoints;
    }

    private List<Vector3> ComputeMeridian(
        float rightAscension,
        float radius,
        int divisions)
    {
        List<Vector3> meridianPoints = new List<Vector3>();

        float declinationStep = 360.0f / divisions;
        for (float declination = 0.0f; declination <= 360.0f; declination += declinationStep)
        {
            Vector3 point = CelestialToCartesian(rightAscension, declination, radius);
            meridianPoints.Add(point);
        }

        return meridianPoints;
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
