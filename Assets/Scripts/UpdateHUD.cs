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
        // Disable the pulsar jet if we stopped casting at the pulsar.
        if (previousHit != null)
        {
            previousHit.transform.GetChild(0).gameObject.SetActive(false);
            previousHit = null;
        }

        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.transform.gameObject.GetComponentInChildren<Pulsar>())
            {
                previousHit = hit.transform.gameObject;

                Pulsar p = hit.transform.gameObject.GetComponent<Pulsar>();
                p.transform.Rotate(p.rotationAxis, Mathf.Rad2Deg * (2.0f * Mathf.PI * p.f0 * 0.00001f));

                Transform objectHit = hit.transform;

                // Enable the pulsar jet.
                hit.transform.GetChild(0).gameObject.SetActive(true);

                // Update HUD information.
                pulsarName.text = objectHit.gameObject.name;
                pulsarDescription.text = "Distance: " + Mathf.Round(Vector3.Distance(player.transform.position, objectHit.position)).ToString() + " [kpc]\n";
                pulsarDescription.text += "Frequency: " + p.f0.ToString() + " [Hz]\n";
                pulsarDescription.text += "B surface: " + p.bsurf.ToString() + " [G]";
            }
        }
        else
        {
            pulsarName.text = "";
            pulsarDescription.text = "";
        }

        velocityText.text = Mathf.Round(player.GetComponent<Rigidbody>().velocity.magnitude).ToString() + " [kpc/s]";
    }
}
