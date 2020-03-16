using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tagger : MonoBehaviour
{
    public Transform mCameraToLookAt;
    public GameObject mTagObjectPrefab;

    struct Tag
    {
        public string name;
        public Vector3 position;
    };

    List<Tag> mTags;
    List<GameObject> mTagObjects;

    // Start is called before the first frame update
    void Start()
    {
        mTags = new List<Tag>();
        mTagObjects = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (GameObject t in mTagObjects)
        {
            t.transform.rotation = Camera.main.transform.rotation;
            Vector3 objectNormal = t.transform.rotation * Vector3.forward;
            Vector3 cameraToText = t.transform.position - Camera.main.transform.position;
            float f = Vector3.Dot(objectNormal, cameraToText);
            if (f < 0f)
            {
                t.transform.Rotate(0f, 180f, 0f);
            }
        }
    }

    public void AddTag(string name, Vector3 position)
    {
        Tag tag = new Tag
        {
            name = name,
            position = position
        };

        mTags.Add(tag);
    }

    public void AddTags(List<Vector3> positions, List<string> names)
    {
        for (int i = 0; i < names.Count; ++i)
        {
            Tag tag = new Tag
            {
                name = names[i],
                position = positions[i]
            };

            mTags.Add(tag);
        }
    }

    public void CreateTags()
    {
        foreach (Tag t in mTags)
        {
            var tagInstance = Instantiate(mTagObjectPrefab, t.position, Quaternion.identity);

            tagInstance.name = "Tag_" + t.name;
            tagInstance.transform.SetParent(this.transform);
            tagInstance.GetComponent<Text>().text = t.name;
            tagInstance.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

            mTagObjects.Add(tagInstance);
        }
    }
}
