using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateHUD : MonoBehaviour
{
    public GameObject player;

    public Text velocityText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        velocityText.text = player.GetComponent<Rigidbody>().velocity.magnitude.ToString() + " [kpc/s]";
    }
}
