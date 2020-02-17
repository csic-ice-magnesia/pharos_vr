using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulsarSpawner : MonoBehaviour
{
    struct Pulsar
    {
        public Vector3 rightAscension;
        public Vector3 declination;
        public float distance;
    }

    List<Pulsar> pulsars;
    public GameObject pulsarPrefab;

    // Start is called before the first frame update
    void Start()
    {
        pulsars = new List<Pulsar>();
        ReadDatabase("Assets/Resources/ATNF_short.csv");

        foreach (Pulsar pulsar in pulsars)
        {
            Debug.Log("Spawning pulsar");

            Vector3 position = CelestialToCartesian(pulsar.rightAscension, pulsar.declination, pulsar.distance);
            var pulsarInstance = Instantiate(pulsarPrefab, position, Quaternion.identity);
            pulsarInstance.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
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

    private void ReadDatabase(
        string filename)
    {
        var filedata = System.IO.File.ReadAllText(filename);
        string[] lines = filedata.Split('\n');

        for (int i = 1; i < lines.Length; ++i)
        {
            string[] lineData = (lines[i].Trim()).Split(';');

            Pulsar pulsar = new Pulsar();

            Vector3 rightAscension = Vector3.zero;
            Vector3 declination = Vector3.zero;

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

            pulsar.rightAscension = rightAscension;
            pulsar.declination = declination;
            pulsar.distance = 1000;

            pulsars.Add(pulsar);
        }
    }
}
