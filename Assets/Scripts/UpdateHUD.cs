using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateHUD : MonoBehaviour
{
    private GameObject previousHit;
    private readonly Vector3 viewportCenter = new Vector3(0.5f, 0.5f, 0.0f);

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

                // Rotate pulsar + companion to create feeling of orbit.
                hit.transform.Rotate(new Vector3(0.0f, 0.5f, 0.0f), Space.Self);

                // Rotate pulsar around itself.
                hit.transform.GetChild(0).Rotate(p.rotationAxis, Mathf.Rad2Deg * (2.0f * Mathf.PI * p.f0 * 0.00001f));

                // Enable the pulsar jet.
                hit.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);

                // Enable the companion.
                hit.transform.GetChild(1).gameObject.SetActive(true);

                // Visualize orbital.
                hit.transform.GetChild(2).gameObject.SetActive(true);
                hit.transform.GetChild(3).gameObject.SetActive(true);

                // Update HUD information.
                pulsarName.text = objectHit.gameObject.name;
                pulsarDescription.text = "Distance: " + Mathf.Round(Vector3.Distance(player.transform.position, objectHit.position)).ToString() + " [kpc]\n";
                pulsarDescription.text += "Frequency: " + p.f0.ToString() + " [Hz]\n";
                pulsarDescription.text += "B surface: " + p.bsurf.ToString() + " [G]";
            }
        }
        else
        {
            // Disable the pulsar jet if we stopped casting at the pulsar.
            if (previousHit != null)
            {
                // Disable the pulsar jet.
                previousHit.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
                // Disable the companion.
                previousHit.transform.GetChild(1).gameObject.SetActive(false);
                // Disable orbital visualization.
                previousHit.transform.GetChild(2).gameObject.SetActive(false);
                previousHit.transform.GetChild(3).gameObject.SetActive(false);

                previousHit = null;
            }

            pulsarName.text = "";
            pulsarDescription.text = "";
        }

        velocityText.text = Mathf.Round(player.GetComponent<Rigidbody>().velocity.magnitude).ToString() + " [kpc/s]";
    }
}
