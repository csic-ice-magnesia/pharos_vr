using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraToHUD : MonoBehaviour
{
    public enum CameraAttributes
    {
        RightAscension,
        Declination
    };

    private Text text;
    public CameraAttributes cameraAttribute;

    // Use this for initialization
    void Start()
    {
        text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (cameraAttribute == CameraAttributes.Declination)
        {
            text.text = ((int)Camera.main.GetComponent<TelescopeCamera>().declination).ToString();
        }

        if (cameraAttribute == CameraAttributes.RightAscension)
        {
            text.text = ((int)Camera.main.GetComponent<TelescopeCamera>().rightAscension).ToString();
        }

    }
}
