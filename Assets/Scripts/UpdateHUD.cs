using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateHUD : MonoBehaviour
{
    private GameObject previousHit;

    public GameObject player;
    public Text velocityText;
    public Text pulsarName;
    public Text pulsarDescription;

    // Start is called before the first frame update
    void Start()
    {
        previousHit = null;
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.transform.gameObject.GetComponentInChildren<Pulsar>())
            {
                previousHit = hit.transform.gameObject;

                Pulsar p = hit.transform.gameObject.GetComponent<Pulsar>();
                Transform objectHit = hit.transform;

                // Enable the pulsar jet.
                p.SetJetState(true);
                // Enable the companion.
                p.SetCompanionState(true);
                // Visualize orbital.
                p.SetOrbitVisuals(true);
                // Orbit for one step.
                p.OrbitStep();

                // Update HUD information.
                pulsarName.text = objectHit.gameObject.name;
                pulsarDescription.text = "Distance: " + Mathf.Round(Vector3.Distance(player.transform.position, objectHit.position)).ToString() + " [kpc]\n";
                pulsarDescription.text += "Frequency: " + p.mFrequency.ToString() + " [Hz]\n";
                pulsarDescription.text += "B surface: " + p.mSurfaceMagneticIntensity.ToString() + " [G]";
            }
        }
        else
        {
            // Disable the pulsar jet if we stopped casting at the pulsar.
            if (previousHit != null)
            {
                Pulsar p = previousHit.transform.gameObject.GetComponent<Pulsar>();

                // Disable the pulsar jet.
                p.SetJetState(false);
                // Disable the companion.
                p.SetCompanionState(false);
                // Disable orbital visualization.
                p.SetOrbitVisuals(false);

                previousHit = null;
            }

            pulsarName.text = "";
            pulsarDescription.text = "";
        }

        velocityText.text = Mathf.Round(player.GetComponent<Rigidbody>().velocity.magnitude).ToString() + " [kpc/s]";
    }
}
