using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pulsar : MonoBehaviour
{
    const int PULSAR_CHILD_ID = 0;
    const int COMPANION_CHILD_ID = 1;
    const int ORBIT_LINE_CHILD_ID = 2;
    const int ORBIT_CENTER_CHILD_ID = 3;
    readonly Vector3 ORBITAL_SPEED = new Vector3( 0.0f, 0.5f, 0.0f);
    const float ROTATION_MULTILPIER = 0.00001f;

    public enum PulsarType
    {
        NS_NS,
        NS_HM,
        ISOLATED
    };

    public Vector3 mRightAscension;
    public Vector3 mDeclination;
    public Vector3 mRotationAxis;
    public float mDistance;
    public float mFrequency;
    public float mSurfaceMagneticIntensity;
    public float mOrbitalRadius;
    public PulsarType mType;

    public Material mOrbitalMaterial;

    // Start is called before the first frame update.
    void Start()
    {
        // Astrophysical jet is disabled by default for effiency.
        SetJetState(false);
        // Disable the companion for the moment.
        SetCompanionState(false);
        // Orbital is disabled for efficiency and clutter.
        SetOrbitVisuals(false);
        // Disable updating for efficiency.
        enabled = false;
    }

    public void Create()
    {
        // Randomly pick a type for the pulsar.
        PickType();

        // Companion setup.
        if (mType == PulsarType.ISOLATED)
        {
            transform.GetChild(COMPANION_CHILD_ID).transform.localScale *= 0.0f;
        }
        else if (mType == PulsarType.NS_HM)
        {
            float companionScaleMultiplier = Random.Range(1.0f, 4.0f);
            transform.GetChild(COMPANION_CHILD_ID).transform.localScale *= companionScaleMultiplier;
        }

        // Orbit setup.
        if (mType == PulsarType.ISOLATED)
        {
            mOrbitalRadius = 0.0f;
            SetOrbit();
        }
        else
        {
            mOrbitalRadius = Random.Range(2.0f, 8.0f);
            SetOrbit();
            CreateOrbitLine();
        }
    }

    /// <summary>
    /// Enables the pulsar jet, the companion, and the orbital visualization.
    /// </summary>
    public void Activate()
    {
        // Enable the pulsar jet.
        SetJetState(true);
        // Enable the companion.
        SetCompanionState(true);
        // Visualize orbital.
        SetOrbitVisuals(true);
    }

    /// <summary>
    /// Disables the pulsar jet, the companion, and the orbital visualization.
    /// </summary>
    public void Deactivate()
    {
        // Disable the pulsar jet.
        SetJetState(false);
        // Disable the companion.
        SetCompanionState(false);
        // Disable orbital visualization.
        SetOrbitVisuals(false);
    }

    /// <summary>
    /// Assigns this pulsar a random type of pulsar with equal probability.
    /// </summary>
    public void PickType()
    {
        mType = (PulsarType)Random.Range(0, System.Enum.GetNames(typeof(PulsarType)).Length);
    }

    /// <summary>
    /// Rotates the whole object around the center to create an illusion of the pulsar and
    /// its companion orbiting around themselves. It also rotates just the pulsar around its
    /// rotation axis with its corresponding frequency.
    /// </summary>
    public void OrbitStep()
    {
        // Rotate pulsar + companion to create feeling of orbit.
        if (mType != PulsarType.ISOLATED)
        {
            transform.Rotate(ORBITAL_SPEED, Space.Self);
        }
        // Rotate just the pulsar around its axis.
        transform.GetChild(PULSAR_CHILD_ID).Rotate(
            mRotationAxis,
            Mathf.Rad2Deg * (2.0f * Mathf.PI * mFrequency * ROTATION_MULTILPIER)
        );
    }

    /// <summary>
    /// Moves the pulsar, its companion, the orbit center and the orbit line to the adequate
    /// positions to create a circular orbit around the (0,0,0) origin. By convention, the
    /// pulsar and the companion are displaced to opposite ends of the Z axis.
    /// 
    /// Note: This approach does not allow us to tackle complex orbits but for now it's ok.
    /// 
    /// </summary>
    /// <param name="radius">The radius of the orbit in [kpc]</param>
    public void SetOrbit()
    {
        // Move pulsar, companion, and orbit center to the appropriate positions.
        transform.GetChild(PULSAR_CHILD_ID).transform.localPosition = new Vector3(0.0f, 0.0f, mOrbitalRadius);
        transform.GetChild(COMPANION_CHILD_ID).transform.localPosition = new Vector3(0.0f, 0.0f, -1.0f * mOrbitalRadius);
        transform.GetChild(ORBIT_LINE_CHILD_ID).transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        transform.GetChild(ORBIT_CENTER_CHILD_ID).transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);

        // Displace global sphere collider to fit the pulsar only.
        GetComponent<SphereCollider>().center = transform.GetChild(PULSAR_CHILD_ID).transform.localPosition;
    }

    /// <summary>
    /// Creates a circular line for the orbit with a given material. A line renderer is added
    /// to the "OrbitLine" object in the pulsar hierarchy and segments are added to generate
    /// a circle of predefined width and color around the XY plane.
    /// </summary>
    /// <param name="orbitMaterial">The material for the line to render.</param>
    public void CreateOrbitLine()
    {
        LineRenderer orbitRenderer = transform.GetChild(ORBIT_LINE_CHILD_ID).gameObject.AddComponent<LineRenderer>();

        float thetaScale = 0.01f;
        float sizeValue = (2.0f * Mathf.PI) / thetaScale;
        int size = (int)sizeValue;
        size++;

        orbitRenderer.useWorldSpace = false;
        orbitRenderer.startWidth = 0.05f;
        orbitRenderer.endWidth = 0.05f;
        orbitRenderer.material = mOrbitalMaterial;
        orbitRenderer.material.color = new Color(0.75f, 0.75f, 0.75f, 0.5f);
        orbitRenderer.positionCount = size;

        Vector3 pos;
        float theta = 0f;
        for (int i = 0; i < size; i++)
        {
            theta += (2.0f * Mathf.PI * thetaScale);
            float x = mOrbitalRadius * Mathf.Cos(theta);
            float y = mOrbitalRadius * Mathf.Sin(theta);
            x += transform.GetChild(ORBIT_LINE_CHILD_ID).gameObject.transform.localPosition.x;
            y += transform.GetChild(ORBIT_LINE_CHILD_ID).gameObject.transform.localPosition.y;
            pos = new Vector3(x, y, 0);
            orbitRenderer.SetPosition(i, pos);
        }
    }

    /// <summary>
    /// Enables or disables the orbit center and orbit line objects.
    /// </summary>
    /// <param name="state">Whether to enable or not the orbital objects.</param>
    public void SetOrbitVisuals(bool state)
    {
        transform.GetChild(ORBIT_LINE_CHILD_ID).gameObject.SetActive(state);
        transform.GetChild(ORBIT_CENTER_CHILD_ID).gameObject.SetActive(state);
    }

    /// <summary>
    /// Enables or disables the companion.
    /// </summary>
    /// <param name="state">Whether to enable or not the companion.</param>
    public void SetCompanionState(bool state)
    {
        transform.GetChild(COMPANION_CHILD_ID).gameObject.SetActive(state);
    }

    /// <summary>
    /// Enables or disables the jet.
    /// </summary>
    /// <param name="state">Whether or not to enable the jet.</param>
    public void SetJetState(bool state)
    {
        transform.GetChild(PULSAR_CHILD_ID).GetChild(0).gameObject.SetActive(state);
    }
}
