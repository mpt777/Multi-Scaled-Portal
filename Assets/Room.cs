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

    public void ReparentObject(GameObject obj)
    {
        Vector3 originalScale = obj.transform.localScale;
        obj.transform.SetParent(gameObject.transform, true);
        obj.transform.localScale = originalScale;
    }
    public void ReparentObject(GameObject obj, bool keepScale)
    {
        Vector3 originalScale = obj.transform.localScale;
        obj.transform.SetParent(gameObject.transform, true);
        if (keepScale)
        {
            obj.transform.localScale = originalScale;
        }
    }

    public void ReparentObjectToChild(GameObject obj, string name)
    {
        Vector3 originalScale = obj.transform.localScale;
        obj.transform.parent = gameObject.transform.Find(name);
        obj.transform.localScale = originalScale;
    }
    public Vector3 PositionInRoom(GameObject room, GameObject obj)
    {
        Vector3 offset = obj.transform.position - room.transform.position;
        offset.Scale(TransformToRelativeScale(transform));
        return gameObject.transform.position + offset;
    }
    public void TransformObjTo(Room room, GameObject obj)
    {
        //obj offset from self
        Vector3 offset = obj.transform.position - this.transform.position;

        //transform other room to relative self
        //Transform targetRoom_relative_transform = this.TransformToRelative(room.gameObject.transform);

        // move OBJ to new position
        obj.transform.position = this.transform.position + TransformToRelativePosition(room.gameObject.transform);

        //scale offset portal from new room origin
        offset.Scale(TransformToRelativeScale(room.gameObject.transform));
        obj.transform.position += offset;
    }

    public Vector3 TransformToRelativePosition(Transform transform)
    {
        return transform.position - this.transform.position;
    }
    public Quaternion TransformToRelativeRotation(Transform transform)
    {
        // returns a difference in transforms that can be used to move origins
        return transform.rotation * Quaternion.Inverse(this.transform.rotation); ;
    }

    public Vector3 TransformToRelativeScale(Transform transform)
    {
        // returns a difference in transforms that can be used to move origins
        return new Vector3(
            transform.localScale.x / this.transform.localScale.x, 
            transform.localScale.y / this.transform.localScale.y, 
            transform.localScale.z / this.transform.localScale.z
        );

    }
}
