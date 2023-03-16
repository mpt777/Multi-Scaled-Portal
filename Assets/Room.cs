using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{

    // Start is called before the first frame update
    public PortalManager portalManager;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void TransformPortalFromManager(PortalManager portalManager)
    {
        this.portalManager = portalManager;

        if (portalManager.originRoom != null & portalManager.targetRoom != null)
        {

        }
    }

    public void ReparentObject(GameObject obj)
    {
        Vector3 originalScale = obj.transform.localScale;
        obj.transform.parent = this.gameObject.transform;
        obj.transform.localScale = originalScale;
    }

    public void ReparentObjectToChild(GameObject obj, string name)
    {
        Vector3 originalScale = obj.transform.localScale;
        obj.transform.parent = obj.transform.parent = gameObject.transform.Find(name);
        obj.transform.localScale = originalScale;
    }

    public void TransformObjTo(Room room, GameObject obj)
    {
        //obj offset from self
        Vector3 offset = obj.transform.position - this.transform.position;

        //transform other room to relative self
        Transform targetRoom_relative_transform = this.TransformToRelative(room.gameObject.transform);

        // move OBJ to new position
        obj.transform.position = this.transform.position + targetRoom_relative_transform.position;

        //scale offset portal from new room origin
        offset.Scale(targetRoom_relative_transform.localScale);
        obj.transform.position += offset;
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
