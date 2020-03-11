using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulsarSpawner : MonoBehaviour
{
    private static float cutoffDistance = 256.0f;
    private static float radius = 1.0f;
    private static float minimumAlpha = Mathf.Atan(radius / cutoffDistance);

    private Vector3 jetScale;

    struct PulsarData
    {
        public string name;
        public Vector3 rightAscension;
        public Vector3 declination;
        public Vector3 rotationAxis;
        public float distance;
        public float f0;
        public float bsurf;
    }

    List<PulsarData> pulsars;
    List<GameObject> pulsarInstances;
    public GameObject pulsarPrefab;
    public TextAsset pulsarDatabase;

    Gradient mGradient;
    GradientColorKey[] mColorKey;
    GradientAlphaKey[] mAlphaKey;

    // Start is called before the first frame update
    void Start()
    {
        mGradient = new Gradient();

        // Populate the color keys at the relative time 0 and 1 (0 and 100%)
        mColorKey = new GradientColorKey[2];
        mColorKey[0].color = Color.green;
        mColorKey[0].time = 0.0f;
        mColorKey[1].color = Color.blue;
        mColorKey[1].time = 1.0f;

        // Populate the alpha  keys at relative time 0 and 1  (0 and 100%)
        mAlphaKey = new GradientAlphaKey[2];
        mAlphaKey[0].alpha = 1.0f;
        mAlphaKey[0].time = 0.0f;
        mAlphaKey[1].alpha = 0.0f;
        mAlphaKey[1].time = 1.0f;

        mGradient.SetKeys(mColorKey, mAlphaKey);

        jetScale = pulsarPrefab.transform.GetChild(0).localScale;

        pulsars = new List<PulsarData>();
        ReadDatabase();

        pulsarInstances = new List<GameObject>();
        foreach (PulsarData pulsar in pulsars)
        {
            // Do not plot pulsars for which we don't know the distance.
            if (pulsar.distance == 0.0f)
            {
                continue;
            }

            Vector3 position = CelestialToCartesian(pulsar.rightAscension, pulsar.declination, pulsar.distance * 128.0f);
            Quaternion rotation = Quaternion.Euler(Random.Range(0.0f, 360.0f), Random.Range(0.0f, 360.0f), Random.Range(0.0f, 360.0f));
            //Quaternion rotation = Quaternion.identity;
            var pulsarInstance = Instantiate(pulsarPrefab, position, rotation);

            pulsarInstance.name = pulsar.name;

            // Fill pulsar details in the object.
            Pulsar pulsarInstancePulsar = pulsarInstance.GetComponent<Pulsar>();
            pulsarInstancePulsar.rightAscension = pulsar.rightAscension;
            pulsarInstancePulsar.declination = pulsar.declination;
            pulsarInstancePulsar.f0 = pulsar.f0;
            pulsarInstancePulsar.rotationAxis = pulsar.rotationAxis;
            pulsarInstancePulsar.distance = pulsar.distance;
            pulsarInstancePulsar.bsurf = pulsar.bsurf;

            // Setup for orbiting.
            pulsarInstance.transform.GetChild(0).transform.localPosition = new Vector3(0.0f, 0.0f, 4.0f);
            pulsarInstance.transform.GetChild(1).transform.localPosition = new Vector3(0.0f, 0.0f, -4.0f);
            pulsarInstance.transform.GetChild(2).transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);

            // Displace sphere collider to fit the pulsar only.
            pulsarInstance.GetComponent<SphereCollider>().center = pulsarInstance.transform.GetChild(0).transform.localPosition;

            pulsarInstance.SetActive(true);

            pulsarInstances.Add(pulsarInstance);
        }

        // Set pulsar material color depending on the surface magnetic field
        // intensity.

        float minMagneticIntensity = float.MaxValue;
        float maxMagneticIntensity = float.MinValue;

        foreach (PulsarData pulsar in pulsars)
        {
            if (pulsar.bsurf == 0.0f)
            {
                continue;
            }

            minMagneticIntensity = Mathf.Min(minMagneticIntensity, pulsar.bsurf);
            maxMagneticIntensity = Mathf.Max(maxMagneticIntensity, pulsar.bsurf);
        }

        minMagneticIntensity = Mathf.Log10(minMagneticIntensity);
        maxMagneticIntensity = Mathf.Log10(maxMagneticIntensity);

        foreach (var p in pulsarInstances)
        {
            float magneticIntensity = Mathf.Log10(p.GetComponent<Pulsar>().bsurf);
            float gradientPosition = (magneticIntensity - minMagneticIntensity) / (maxMagneticIntensity - minMagneticIntensity);
            Color color = mGradient.Evaluate(gradientPosition);

            if (p.GetComponent<Pulsar>().bsurf == 0.0f)
            {
                color = Color.white;
            }

            p.transform.GetChild(0).GetChild(1).GetComponent<Renderer>().material.SetColor(
                "_Color",
                color
            );
        }

        // Set up culling distances for the different pulsar layers with
        // spherical rather than planar distance calculation.

        float[] cullingDistances = new float[32];
        cullingDistances[9] = 256.0f;
        Camera.main.layerCullDistances = cullingDistances;
        Camera.main.layerCullSpherical = true;
    }

    // Update is called once per frame
    void Update()
    {
        
        /*foreach (GameObject pi in pulsarInstances)
        {
            
            //Pulsar p = pi.GetComponent<Pulsar>();

            // Keep constant pulsar size beyond the specified distance by scaling it.
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

            p.transform.GetChild(0).localScale = jetScale * Mathf.Clamp(16.0f / distanceToCamera, 0.0f, 1.0f);
            

            // Rotate pulsar according to its frequency.
            //p.transform.Rotate(p.rotationAxis, Mathf.Rad2Deg * (2.0f * Mathf.PI * p.f0 * 0.00001f));
        }*/
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

            PulsarData pulsar;

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

            float f0 = float.Parse(lineData[8]);
            float distance = float.Parse(lineData[10]);
            float bsurf = float.Parse(lineData[13]);

            pulsar.name = name;
            pulsar.rightAscension = rightAscension;
            pulsar.declination = declination;
            pulsar.rotationAxis = new Vector3(Random.Range(0.0f, 360.0f), Random.Range(0.0f, 360.0f), Random.Range(0.0f, 360.0f));
            pulsar.distance = distance;
            pulsar.f0 = f0;
            pulsar.bsurf = bsurf;

            pulsars.Add(pulsar);
        }
    }
}
