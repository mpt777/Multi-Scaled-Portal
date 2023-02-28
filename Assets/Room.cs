using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public Vector3 relativeScale = new Vector3(1, 1, 1);

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    //public scaleRelativeScale(Vector3 scale)
    //{
    //    this.relativeScale = scale;
    //}

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
