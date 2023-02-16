using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(transform.position);
        Debug.Log(transform.localScale);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public Transform TransformToRelative(Transform transform)
    {
        // returns a difference in transforms that can be used to move origins
        Transform newTransform = new GameObject().transform;
        newTransform.position = transform.position - this.transform.position;
        newTransform.rotation = transform.rotation * Quaternion.Inverse(this.transform.rotation); ;
        newTransform.localScale = new Vector3(
            transform.localScale.x / this.transform.localScale.x, 
            transform.localScale.y / this.transform.localScale.y, 
            transform.localScale.z / this.transform.localScale.z
        );
        return newTransform;
    }
}
