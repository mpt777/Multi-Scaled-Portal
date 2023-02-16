using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalObject : MonoBehaviour
{
    private bool wasCollidingToPortal = false;
    private bool isCollidingToPortal = false;

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
        }
    }
}
