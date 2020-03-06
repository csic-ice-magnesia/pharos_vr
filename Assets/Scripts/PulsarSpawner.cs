using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulsarSpawner : MonoBehaviour
{
    private static float cutoffDistance = 256.0f;
    private static float radius = 1.0f;
    private static float minimumAlpha = Mathf.Atan(radius / cutoffDistance);

    struct Pulsar
    {
        public string name;
        public Vector3 rightAscension;
        public Vector3 declination;
        public float distance;
    }

    List<Pulsar> pulsars;
    List<GameObject> pulsarInstances;
    public GameObject pulsarPrefab;
    public TextAsset pulsarDatabase;

    // Start is called before the first frame update
    void Start()
    {
        pulsars = new List<Pulsar>();
        ReadDatabase();

        pulsarInstances = new List<GameObject>();
        foreach (Pulsar pulsar in pulsars)
        {
            // Do not plot pulsars for which we don't know the distance.
            if (pulsar.distance == 0.0f)
            {
                continue;
            }

            Vector3 position = CelestialToCartesian(pulsar.rightAscension, pulsar.declination, pulsar.distance);
            Quaternion rotation = Quaternion.Euler(Random.Range(0.0f, 360.0f), Random.Range(0.0f, 360.0f), Random.Range(0.0f, 360.0f));
            var pulsarInstance = Instantiate(pulsarPrefab, position, rotation);
            pulsarInstance.name = pulsar.name;
            pulsarInstance.SetActive(true);
            pulsarInstances.Add(pulsarInstance);
        }

        float[] cullingDistances = new float[32];
        cullingDistances[9] = 10.0f;
        Camera.main.layerCullDistances = cullingDistances;
        Camera.main.layerCullSpherical = true;
    }

    // Update is called once per frame
    void Update()
    {
        foreach (GameObject p in pulsarInstances)
        {
            float distanceToCamera = Vector3.Distance(p.transform.position, Camera.main.transform.position);
            float alpha = Mathf.Atan(radius / distanceToCamera);
            float alphaCoefficient = minimumAlpha / alpha;

            if (alphaCoefficient > 1.0f)
            {
                p.transform.localScale = new Vector3(radius * alphaCoefficient, radius * alphaCoefficient, radius * alphaCoefficient);
            }
            else
            {
                p.transform.localScale = Vector3.one;
            }
        }
    }

    private Vector3 CelestialToCartesian(
        Vector3 rightAscension,
        Vector3 declination,
        double distance)
    {
        Vector3 cartesian = Vector3.zero;

        double a = (rightAscension.x * 15.0) + (rightAscension.y * 0.25) + (rightAscension.z * 0.004166);
        double b = (Mathf.Abs(declination.x) + (declination.y / 60.0) + (declination.z / 3600)) * Mathf.Sign(declination.x);
        double c = distance;

        cartesian.x = (float)c * Mathf.Cos((float)b) * Mathf.Cos((float)a);
        cartesian.y = (float)c * Mathf.Cos((float)b) * Mathf.Sin((float)a);
        cartesian.z = (float)c * Mathf.Sin((float)b);

        return cartesian;
    }

    private void ReadDatabase()
    {
        var filedata = pulsarDatabase.text;
        string[] lines = filedata.Split('\n');

        for (int i = 1; i < lines.Length; ++i)
        {
            string[] lineData = (lines[i].Trim()).Split(';');

            Pulsar pulsar = new Pulsar();

            Vector3 rightAscension = Vector3.zero;
            Vector3 declination = Vector3.zero;

            string name = (lineData[1]);

            string[] raData = (lineData[2].Trim()).Split(':');

            int k = 2;
            for (int j = raData.Length - 1; j >= 0; --j)
            {
                rightAscension[k] = float.Parse(raData[j]);
                --k;
            }


            string[] decData = (lineData[3].Trim()).Split(':');

            k = 2;
            for (int j = decData.Length - 1; j >= 0; --j)
            {
                declination[k] = float.Parse(decData[j]);
                --k;
            }

            float distance = float.Parse(lineData[8]) * 128.0f;

            pulsar.name = name;
            pulsar.rightAscension = rightAscension;
            pulsar.declination = declination;
            pulsar.distance = distance;

            pulsars.Add(pulsar);
        }
    }
}
