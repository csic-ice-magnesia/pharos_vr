using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaycastSelector : MonoBehaviour
{
    private Text text;
    readonly Vector3 viewportCenter = new Vector3(0.5f, 0.5f, 0.0f);

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

        if (Physics.Raycast(ray, out hit))
        {
            Transform objectHit = hit.transform;
            text.text = objectHit.gameObject.name;
        }
        else
        {
            text.text = "";
        }
    }
}
