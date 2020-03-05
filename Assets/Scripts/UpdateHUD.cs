using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateHUD : MonoBehaviour
{
    readonly Vector3 viewportCenter = new Vector3(0.5f, 0.5f, 0.0f);

    public GameObject player;

    public Text velocityText;
    public Text pulsarName;
    public Text pulsarDescription;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Transform objectHit = hit.transform;
            pulsarName.text = objectHit.gameObject.name;

            pulsarDescription.text = "Distance: " + Mathf.Round(Vector3.Distance(player.transform.position, objectHit.position)).ToString() + " [kpc]";
        }
        else
        {
            pulsarName.text = "";
            pulsarDescription.text = "";
        }

        velocityText.text = Mathf.Round(player.GetComponent<Rigidbody>().velocity.magnitude).ToString() + " [kpc/s]";
    }
}
