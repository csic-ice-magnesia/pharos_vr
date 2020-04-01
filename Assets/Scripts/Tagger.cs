using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tagger : MonoBehaviour
{
    // Camera to which the tags should always face to.
    public Transform mCameraToLookAt;
    // Prefab to spawn tags.
    public GameObject mTagObjectPrefab;
    // Distance at which the tag will be invisble [kpc].
    public float mNearDistance = 16.0f;
    // Distance at which the tag will be fully visible [kpc].
    public float mFarDistance = 64.0f;

    // Data struct to hold tag information.
    [System.Serializable]
    public struct Tag
    {
        public string name;
        public Vector3 position;
    };
    //
    public List<Tag> mDefaultTags;
    // List of tag data.
    private List<Tag> mTags;
    // List of tag instances.
    private List<GameObject> mTagInstances;

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    void Start()
    {
        mTags = new List<Tag>();
        mTags.AddRange(mDefaultTags);
        mTagInstances = new List<GameObject>();
    }

    /// <summary>
    /// Update is called once per frame.
    /// 
    /// Make sure that all tags are facing the camera and update their alpha
    /// value depending on the distance to the camera.
    /// </summary>
    void Update()
    {
        foreach (GameObject t in mTagInstances)
        {
            // Update tag rotation so that it faces the camera.
            {
                t.transform.rotation = Camera.main.transform.rotation;
                Vector3 objectNormal = t.transform.rotation * 
                                       Vector3.forward;
                Vector3 cameraToText = t.transform.position -
                                       Camera.main.transform.position;
                float f = Vector3.Dot(objectNormal, cameraToText);
                if (f < 0f)
                {
                    t.transform.Rotate(0f, 180f, 0f);
                }
            }
            // Update tag opacity according to its distance to camera.
            /*{
                float distanceToCamera = Vector3.Distance(
                    t.GetComponent<RectTransform>().transform.position,
                    mCameraToLookAt.transform.position
                );

                Debug.Log("Tag " + t.name);
                Debug.Log("Position " + t.GetComponent<RectTransform>().transform.position);
                Debug.Log("Distance " + distanceToCamera);


                Color tagColor = Color.white;
                tagColor.a = Mathf.Clamp(distanceToCamera / mNearDistance, 0.0f, 1.0f);

                Debug.Log("Opacity " + tagColor.a);
                t.GetComponent<Text>().material.color = tagColor;
            }*/
        }
    }

    /// <summary>
    /// Add tag data to the list.
    /// </summary>
    /// <param name="name">Name of the pulsar.</param>
    /// <param name="position">3D position in space [kpc, kpc, kpc].</param>
    public void AddTag(string name, Vector3 position)
    {
        Tag tag = new Tag
        {
            name = name,
            position = position
        };

        mTags.Add(tag);
    }

    /// <summary>
    /// Add tags data to the list.
    /// </summary>
    /// <param name="names">Names of the pulsars.</param>
    /// <param name="positions">3D positions in space [kpc, kpc, kpc].</param>
    public void AddTags(List<string> names, List<Vector3> positions)
    {
        for (int i = 0; i < names.Count; ++i)
        {
            AddTag(names[i], positions[i]);
        }
    }

    /// <summary>
    /// Create and instantiate tags.
    /// 
    /// Instantiate each tag with its corresponding data.
    /// </summary>
    public void CreateTags()
    {
        foreach (Tag t in mTags)
        {
            var tagInstance = Instantiate(
                mTagObjectPrefab,
                t.position,
                Quaternion.identity
            );

            tagInstance.name = "Tag_" + t.name;
            tagInstance.transform.SetParent(this.transform);
            tagInstance.GetComponent<Text>().text = t.name;
            tagInstance.GetComponent<Text>().alignByGeometry = true;
            tagInstance.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
            tagInstance.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

            mTagInstances.Add(tagInstance);
        }
    }
}
