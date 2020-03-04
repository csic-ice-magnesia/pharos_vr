using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControllerRay : MonoBehaviour
{
    public LineRenderer laserLineRenderer;
    public float laserWidth = 0.1f;
    public float laserMaxLength = 5f;
    public OvrAvatar ovrAvatar;

    void Start()
    {
        Vector3[] initLaserPositions = new Vector3[2] { Vector3.zero, Vector3.zero };
        laserLineRenderer.SetPositions(initLaserPositions);
        laserLineRenderer.startWidth = laserWidth;
        laserLineRenderer.endWidth = laserWidth / 2.0f;
        laserLineRenderer.startColor = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        laserLineRenderer.endColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        laserLineRenderer.material.SetColor("_TintColor", new Color(1, 1, 1, 0.15f));
    }
    void Update()
    {
        if (OVRInput.Get(OVRInput.RawTouch.RIndexTrigger))
        {
            ShootLaserFromTargetPosition(/*cameraRig.rightHandAnchor.localPosition*/
                ovrAvatar.HandRight.transform.position, ovrAvatar.HandRight.transform.forward, laserMaxLength);
            laserLineRenderer.enabled = true;
        }
        else
        {
            laserLineRenderer.enabled = false;
        }
    }
    void ShootLaserFromTargetPosition(Vector3 targetPosition, Vector3 direction, float length)
    {
        Ray ray = new Ray(targetPosition, direction);
        Vector3 endPosition = targetPosition + (length * direction);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, length))
        {
            endPosition = raycastHit.point;
        }
        laserLineRenderer.SetPosition(0, targetPosition);
        laserLineRenderer.SetPosition(1, endPosition);
    }
}