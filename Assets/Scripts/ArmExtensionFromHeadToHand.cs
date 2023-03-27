using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmExtensionFromHeadToHand : MonoBehaviour
{
    public GameObject Shoulder;
    public GameObject Hand;
    
    private Vector3 bodyPos;
    private Vector3 handPos;

    private bool wasCollidingToPortal = false;
    private bool isCollidingToPortal = false;

    private List<GameObject> collidingObjects = new();

    private void Update()
    {
        // bodyPos = CameraUtil.instance.GetChestPos();
        bodyPos = Shoulder.transform.position;
        handPos = Hand.transform.position;

        transform.position = (bodyPos + handPos) / 2;
        transform.LookAt(Hand.transform, Vector3.up);
        transform.localScale = new Vector3(0.01f, 0.01f, (Vector3.Distance(bodyPos, handPos)));

    }

    public bool IsColliding(GameObject gameObject)
    {
        return collidingObjects.Contains(gameObject);
    }

    public bool GetIsCollidingToPortal(){
        return isCollidingToPortal;
    }

    public bool GetWasCollidingToPortal(){
        return wasCollidingToPortal;
    }

    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Portal"){
            isCollidingToPortal = true;
            wasCollidingToPortal = false;

            if (!collidingObjects.Contains(other.gameObject))
            {
                collidingObjects.Add(other.gameObject);
            }
        }
    }

    private void OnTriggerStay(Collider other) {
        if(other.tag ==  "Portal"){
            isCollidingToPortal = true;
        }
    }

    private void OnTriggerExit(Collider other){
        if(other.tag ==  "Portal"){
            isCollidingToPortal = false;
            wasCollidingToPortal = true;

            if (collidingObjects.Contains(other.gameObject))
            {
                collidingObjects.Remove(other.gameObject);
            }
        }
    }
}
