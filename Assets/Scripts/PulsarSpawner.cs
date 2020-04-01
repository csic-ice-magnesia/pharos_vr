// Copyright(C) 2020 MAGNESIA (ICE-CSIC)
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.If not, see<https://www.gnu.org/licenses/>.
//
// Contributors to this file:
//  * Alberto Garcia Garcia (garciagarcia @ ice.csic.es)
//

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component to spawn a pulsar population.
/// </summary>
public class PulsarSpawner : MonoBehaviour
{
    // Prefab to spawn pulsar instances.
    public GameObject mPulsarPrefab;
    // CSV text file with an ATNF pulsar database.
    public TextAsset mPulsarDatabase;
    // Object responsible for 3D tagging the pulsars in the scene.
    public GameObject mTagger;
    // Number of random tags to spawn.
    public int mNumTags;

    // Distance beyond which the pulsars' apparent size will be kept constant.
    private const float CUTOFF_DISTANCE = 256.0f;
    // Minimum radius for the pulsars.
    private const float PULSAR_RADIUS = 1.0f;
    // Minimum apparent angular resolution for the pulsars.
    private static readonly float MINIMUM_ANGULAR_RESOLUTION = Mathf.Atan(
        PULSAR_RADIUS / CUTOFF_DISTANCE);
    // Current scale of the jet child of the pulsar prefab.
    private Vector3 mJetScale;
    // Data structure to hold pulsar data.
    struct PulsarData
    {
        public string name;
        public Vector3 rightAscension; // [h:m:s]
        public Vector3 declination; // [h:m:s]
        public Vector3 rotationAxis;
        public float distance; // [kpc]
        public float f0; // [Hz]
        public float bsurf; // [G]
    }
    // List of pulsar raw data.
    private List<PulsarData> mPulsars;
    // List of spawned pulsar instances.
    private List<GameObject> mPulsarInstances;
    // Gradient for the pulsar color depending on its bfield.
    private Gradient mGradient;
    // Gradient color key for the pulsar color.
    private GradientColorKey[] mColorKey;
    // Gradient alpha key for the pulsar color.
    private GradientAlphaKey[] mAlphaKey;

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    void Start()
    {
        mJetScale = mPulsarPrefab.transform.GetChild(0).localScale;

        // Read pulsar database to fetch raw data.
        mPulsars = new List<PulsarData>();
        ReadDatabase();

        // Spawn pulsars.
        {
            mPulsarInstances = new List<GameObject>();

            foreach (PulsarData pulsar in mPulsars)
            {
                // Do not plot pulsars for which we don't know the distance.
                if (pulsar.distance == 0.0f)
                {
                    continue;
                }

                // Create position according to its raw celestial coordinates
                // and pick a random rotation in 3D space.
                Vector3 position = CelestialToCartesian(
                    pulsar.rightAscension,
                    pulsar.declination,
                    pulsar.distance * 128.0f
                );
                Quaternion rotation = Quaternion.Euler(
                    Random.Range(0.0f, 360.0f),
                    Random.Range(0.0f, 360.0f),
                    Random.Range(0.0f, 360.0f)
                );

                // Create pulsar instance and fetch its pulsar component.
                var instance = Instantiate(
                    mPulsarPrefab,
                    position,
                    rotation
                );
                Pulsar p = instance.GetComponent<Pulsar>();

                // Fill pulsar details in the object and its component.
                instance.name = pulsar.name;
                p.mRightAscension = pulsar.rightAscension;
                p.mDeclination = pulsar.declination;
                p.mFrequency = pulsar.f0;
                p.mRotationAxis = pulsar.rotationAxis;
                p.mDistance = pulsar.distance;
                p.mSurfaceMagneticIntensity = pulsar.bsurf;
                p.Create();

                // Enable the instance and add to spawned list.
                instance.SetActive(true);
                mPulsarInstances.Add(instance);
            }
        }

        // Set pulsar material color depending on the bfield intensity.
        {
            // Create a gradient for the pulsar color depending on its bfield.
            mGradient = new Gradient();
            // Populate the gradient color keys at the positions 0.0 and 1.0.
            mColorKey = new GradientColorKey[2];
            mColorKey[0].color = Color.green;
            mColorKey[0].time = 0.0f;
            mColorKey[1].color = Color.blue;
            mColorKey[1].time = 1.0f;
            // Populate the gradient alpha keys at the positions 0.0 and 1.0.
            mAlphaKey = new GradientAlphaKey[2];
            mAlphaKey[0].alpha = 1.0f;
            mAlphaKey[0].time = 0.0f;
            mAlphaKey[1].alpha = 0.0f;
            mAlphaKey[1].time = 1.0f;
            // Add the color and alpha keys to the gradient.
            mGradient.SetKeys(mColorKey, mAlphaKey);

            // Determine max/min magnetic intensity in log10 scale.
            float minMagneticIntensity = float.MaxValue;
            float maxMagneticIntensity = float.MinValue;

            foreach (PulsarData pulsar in mPulsars)
            {
                if (pulsar.bsurf == 0.0f)
                {
                    continue;
                }

                minMagneticIntensity = Mathf.Min(
                    minMagneticIntensity,
                    pulsar.bsurf
                );
                maxMagneticIntensity = Mathf.Max(
                    maxMagneticIntensity,
                    pulsar.bsurf
                );
            }

            minMagneticIntensity = Mathf.Log10(minMagneticIntensity);
            maxMagneticIntensity = Mathf.Log10(maxMagneticIntensity);

            // Convert each pulsar's magnetic field intensity to log10 scale
            // and normalize to [0,1] range to find its corresponding color
            // according to the generated gradient and apply it.
            foreach (var p in mPulsarInstances)
            {
                // Rescale bfield intensity to log10 scale.
                float magneticIntensity = Mathf.Log10(
                    p.GetComponent<Pulsar>().mSurfaceMagneticIntensity
                );
                // Normalize to [0,1] range.
                float gradientPosition = 
                    (magneticIntensity - minMagneticIntensity) /
                    (maxMagneticIntensity - minMagneticIntensity);
                // Find color in gradient.
                Color color = mGradient.Evaluate(gradientPosition);

                // If no bfield intensity, we set a distinguishable color.
                if (p.GetComponent<Pulsar>().mSurfaceMagneticIntensity == 0.0f)
                {
                    color = Color.white;
                }

                // Apply the color to the pulsar material.
                p.transform.GetChild(0).GetChild(1).GetComponent<Renderer>().material.SetColor(
                    "_Color",
                    color
                );
            }
        }

        // Set up culling distances for the different pulsar layers with
        // spherical rather than planar distance calculation.
        {
            float[] cullingDistances = new float[32];
            cullingDistances[9] = 256.0f;
            Camera.main.layerCullDistances = cullingDistances;
            Camera.main.layerCullSpherical = true;
        }

        // Add tags to certain random pulsars and create them.
        {
            HashSet<int> tagIds = new HashSet<int>();
            Tagger tagger = mTagger.GetComponent<Tagger>();

            for (int i = 0; i < mNumTags; i++)
            {
                int p;

                do
                {
                    p = Random.Range(0, mPulsars.Count);
                }
                while (!tagIds.Add(p));

                PulsarData pd = mPulsars[p];
                tagger.AddTag(
                    pd.name,
                    CelestialToCartesian(
                        pd.rightAscension,
                        pd.declination,
                        pd.distance * 128.0f
                    )
                );
            }

            tagger.CreateTags();
        }
    }

    /// <summary>
    /// Update is called once per frame.
    /// 
    /// Right now, it is disabled for efficiency purposes.
    /// 
    /// (1) Keep pulsars apparent size constant beyond a specified distance by
    ///     scaling it.
    /// (2) Rotate the pulsars around itself according to its frequency.
    /// </summary>
    /*void Update()
    {
        foreach (GameObject pi in pulsarInstances)
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
        }
    }*/

    /// <summary>
    /// Convert celestial coordinates to 3D cartesian.
    /// </summary>
    /// <param name="rightAscension">Right ascension [h:m:s].</param>
    /// <param name="declination">Declination [h:m:s].</param>
    /// <param name="distance">Distance to Earth [kpc].</param>
    /// <returns>3D point in cartesian coordinates.</returns>
    private Vector3 CelestialToCartesian(
        Vector3 rightAscension,
        Vector3 declination,
        double distance)
    {
        Vector3 cartesian = Vector3.zero;

        double a = (rightAscension.x * 15.0) + 
                   (rightAscension.y * 0.25) +
                   (rightAscension.z * 0.004166);
        double b = (Mathf.Abs(declination.x) +
                   (declination.y / 60.0) +
                   (declination.z / 3600)) * 
                   Mathf.Sign(declination.x);
        double c = distance;

        cartesian.x = (float)c *
                      Mathf.Cos((float)b) *
                      Mathf.Cos((float)a);
        cartesian.y = (float)c *
                      Mathf.Cos((float)b) *
                      Mathf.Sin((float)a);
        cartesian.z = (float)c *
                      Mathf.Sin((float)b);

        return cartesian;
    }

    /// <summary>
    /// Read ATNF pulsar database.
    /// 
    /// Read a CSV file from the ATNF pulsar database and fill the raw pulsar
    /// data list with the extracted information. Right now:
    ///     * Name.
    ///     * Right ascension [h:m:s].
    ///     * Declination [h:m:s].
    ///     * Frequency [Hz].
    ///     * Surface magnetic field intensity [G].
    ///     * Distance to Earth [kpc].
    ///     * Random rotation axis.
    /// </summary>
    private void ReadDatabase()
    {
        var filedata = mPulsarDatabase.text;
        string[] lines = filedata.Split('\n');

        for (int i = 1; i < lines.Length; ++i)
        {
            // The field delimiter in the ATNF format is ';'.
            string[] lineData = (lines[i].Trim()).Split(';');

            // Read name (column 1).
            string name = (lineData[1]);

            // Read right ascension data (column 2 in h:m:s).
            Vector3 rightAscension = Vector3.zero;
            {
                string[] raData = (lineData[2].Trim()).Split(':');

                int k = 2;
                for (int j = raData.Length - 1; j >= 0; --j)
                {
                    rightAscension[k] = float.Parse(raData[j]);
                    --k;
                }
            }

            // Read declination data (column 3 in h:m:s).
            Vector3 declination = Vector3.zero;
            {
                string[] decData = (lineData[3].Trim()).Split(':');

                int k = 2;
                for (int j = decData.Length - 1; j >= 0; --j)
                {
                    declination[k] = float.Parse(decData[j]);
                    --k;
                }
            }

            // Read frequency data (column 8).
            float f0 = float.Parse(lineData[8]);

            // Read distance to Earth (column 10).
            float distance = float.Parse(lineData[10]);

            // Read surface magnetic field intensity (column 13).
            float bsurf = float.Parse(lineData[13]);

            // Fill pulsar raw data and add to list.
            {
                PulsarData pulsar;

                pulsar.name = name;
                pulsar.rightAscension = rightAscension;
                pulsar.declination = declination;
                pulsar.rotationAxis = new Vector3(
                    Random.Range(0.0f, 360.0f),
                    Random.Range(0.0f, 360.0f),
                    Random.Range(0.0f, 360.0f)
                );
                pulsar.distance = distance;
                pulsar.f0 = f0;
                pulsar.bsurf = bsurf;

                mPulsars.Add(pulsar);
            }
        }
    }
}
