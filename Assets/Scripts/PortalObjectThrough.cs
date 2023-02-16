using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalObjectThrough : MonoBehaviour
{
  private GameObject portalOrigin;
  private GameObject portalTarget;

  private GameObject portalHand_left;
  private GameObject portalHand_right;

  private GameObject myHand_left;
  private GameObject myHand_right;

  private GameObject myArm_left;
  private GameObject myArm_right;

    private GameObject objCopy_portal2MyHand_right;
    private GameObject objCopy_portal2MyHand_left;

    private GameObject objCopy_my2PortalHand_right;
    private GameObject objCopy_my2PortalHand_left;

    public void OnEnable(){
        portalHand_left =GameObject.FindWithTag("LeftHand_portal");
        portalHand_right = GameObject.FindWithTag("RightHand_portal");

        myHand_left = transform.GetComponent<PortalManager>().realHand_Left;
        myHand_right = transform.GetComponent<PortalManager>().realHand_Right;
        myArm_left = transform.GetComponent<PortalManager>().realArm_Left;
        myArm_right = transform.GetComponent<PortalManager>().realArm_Right;
    }

    public void DestroyCopiedObjects(){
        if(objCopy_portal2MyHand_right!=null){
            myHand_right.GetComponent<Hand>().RemoveContactInteractables(objCopy_portal2MyHand_right);
        }
        
        if(objCopy_portal2MyHand_left!=null){
        myHand_left.GetComponent<Hand>().RemoveContactInteractables(objCopy_portal2MyHand_left);
        }
        if(objCopy_my2PortalHand_right!=null){
        portalHand_right.GetComponent<Hand>().RemoveContactInteractables(objCopy_my2PortalHand_right);        
        }

        if(objCopy_my2PortalHand_left!=null){
        portalHand_left.GetComponent<Hand>().RemoveContactInteractables(objCopy_my2PortalHand_left);
        }

        Destroy(objCopy_portal2MyHand_right);
        Destroy(objCopy_portal2MyHand_left);
        Destroy(objCopy_my2PortalHand_right);
        Destroy(objCopy_my2PortalHand_left);
    }

    private void RightHandObjectManager_fromPortal2MyHand(){
        GameObject grabObject = null;

        if(!portalHand_right.GetComponent<Hand>().isActiveAndEnabled && objCopy_portal2MyHand_right!=null){
          
            if(!myArm_right.GetComponent<ArmExtensionFromHeadToHand>().GetIsCollidingToPortal() && 
                !myHand_right.GetComponent<MyPortalHandHandler>().GetIsCollidingToPortal() &&
                portalHand_right.GetComponent<FixedJoint>().connectedBody != null &&
                !objCopy_portal2MyHand_right.GetComponent<PortalObject>().GetIsCollidingToPortal()
                ){

                grabObject = portalHand_right.GetComponent<FixedJoint>().connectedBody.gameObject;
                portalHand_right.GetComponent<Hand>().RemoveContactInteractables(grabObject);

                Destroy(grabObject);
                portalHand_right.GetComponent<FixedJoint>().connectedBody = null;
                
              
                objCopy_portal2MyHand_right = null;
                grabObject = null;
            }
        }
        else{
            if(portalHand_right.GetComponent<FixedJoint>().connectedBody != null && myHand_right.GetComponent<FixedJoint>().connectedBody == null){
                // 1. create an copy
                grabObject = portalHand_right.GetComponent<FixedJoint>().connectedBody.gameObject;
                objCopy_portal2MyHand_right = GameObject.Instantiate(grabObject);
                
                // 2. locate the copy obj to my hand
                Vector3 portalHand2Obj = grabObject.transform.position - portalHand_right.transform.position;
                Vector3 objCopyPos = portalHand2Obj + myHand_right.transform.position;
                objCopy_portal2MyHand_right.transform.position = objCopyPos;

                // // 3. remove collider and render(or just keep it) if it is not collided to the portal collider
                objCopy_portal2MyHand_right.GetComponent<MeshCollider>().enabled = true;
                Physics.IgnoreCollision(objCopy_portal2MyHand_right.GetComponent<MeshCollider>(), GameObject.Find("PortalCircleSurface").GetComponent<MeshCollider>());

                // 4. add the object to my hand
                myHand_right.GetComponent<FixedJoint>().connectedBody = objCopy_portal2MyHand_right.GetComponent<Rigidbody>();
            
            }else if(portalHand_right.GetComponent<FixedJoint>().connectedBody == null && myHand_right.GetComponent<FixedJoint>().connectedBody != null){
                // when the portal hand is beyond the portal
                if((myHand_right.GetComponent<MyPortalHandHandler>().GetWasCollidingToPortal() && myArm_right.GetComponent<ArmExtensionFromHeadToHand>().GetIsCollidingToPortal()) || myHand_right.GetComponent<MyPortalHandHandler>().GetIsCollidingToPortal()){
                    myHand_right.GetComponent<FixedJoint>().connectedBody = null;
                    Destroy(objCopy_portal2MyHand_right);
                    objCopy_portal2MyHand_right = null;
                }
            }
        }
    }

    private void LeftHandObjectManager_fromPortal2MyHand(){
        GameObject grabObject = null;

        if(!portalHand_left.GetComponent<Hand>().isActiveAndEnabled&& objCopy_portal2MyHand_left!=null){
            if(!myArm_left.GetComponent<ArmExtensionFromHeadToHand>().GetIsCollidingToPortal() && 
                !myHand_left.GetComponent<MyPortalHandHandler>().GetIsCollidingToPortal() &&
                portalHand_left.GetComponent<FixedJoint>().connectedBody != null &&
                !objCopy_portal2MyHand_left.GetComponent<PortalObject>().GetIsCollidingToPortal()
                ){

                grabObject = portalHand_left.GetComponent<FixedJoint>().connectedBody.gameObject;
                portalHand_left.GetComponent<Hand>().RemoveContactInteractables(grabObject);

                Destroy(grabObject);
                portalHand_left.GetComponent<FixedJoint>().connectedBody = null;
                
                objCopy_portal2MyHand_left = null;
                grabObject = null;
            }
        }
        else{
            if(portalHand_left.GetComponent<FixedJoint>().connectedBody != null && myHand_left.GetComponent<FixedJoint>().connectedBody == null){
                // 1. create an copy
                grabObject = portalHand_left.GetComponent<FixedJoint>().connectedBody.gameObject;
                objCopy_portal2MyHand_left = GameObject.Instantiate(grabObject);
                
                // 2. locate the copy obj to my hand
                Vector3 portalHand2Obj = grabObject.transform.position - portalHand_left.transform.position;
                Vector3 objCopyPos = portalHand2Obj + myHand_left.transform.position;
                objCopy_portal2MyHand_left.transform.position = objCopyPos;

                // // 3. remove collider and render(or just keep it) if it is not collided to the portal collider
                objCopy_portal2MyHand_left.GetComponent<MeshCollider>().enabled = true;
                Physics.IgnoreCollision(objCopy_portal2MyHand_left.GetComponent<MeshCollider>(), GameObject.Find("PortalCircleSurface").GetComponent<MeshCollider>());

                // 4. add the object to my hand
                myHand_left.GetComponent<FixedJoint>().connectedBody = objCopy_portal2MyHand_left.GetComponent<Rigidbody>();
            
            }else if(portalHand_left.GetComponent<FixedJoint>().connectedBody == null && myHand_left.GetComponent<FixedJoint>().connectedBody != null){
                // when the portal hand is beyond the portal
                if((myHand_left.GetComponent<MyPortalHandHandler>().GetWasCollidingToPortal() && myArm_left.GetComponent<ArmExtensionFromHeadToHand>().GetIsCollidingToPortal()) || myHand_left.GetComponent<MyPortalHandHandler>().GetIsCollidingToPortal()){
                    myHand_left.GetComponent<FixedJoint>().connectedBody = null;
                    Destroy(objCopy_portal2MyHand_left);
                    objCopy_portal2MyHand_left = null;
                }
            }
        }

    }
 

     private void RightHandObjectManager_fromMy2PortalHand(){
        GameObject grabObject = null;

        if(!myHand_right.GetComponent<Hand>().isActiveAndEnabled){
            if(myArm_right.GetComponent<ArmExtensionFromHeadToHand>().GetIsCollidingToPortal() &&
                myHand_right.GetComponent<FixedJoint>().connectedBody != null &&
                !myHand_right.GetComponent<FixedJoint>().connectedBody.gameObject.GetComponent<PortalObject>().GetIsCollidingToPortal()
            ){

                grabObject = myHand_right.GetComponent<FixedJoint>().connectedBody.gameObject;
                myHand_right.GetComponent<Hand>().RemoveContactInteractables(grabObject);

                Destroy(grabObject);
                myHand_right.GetComponent<FixedJoint>().connectedBody = null;

                objCopy_my2PortalHand_right = null;    
                grabObject = null;
            }
        }else{
            if(myHand_right.GetComponent<FixedJoint>().connectedBody != null && portalHand_right.GetComponent<FixedJoint>().connectedBody == null){
                // 1. create an copy
                grabObject = myHand_right.GetComponent<FixedJoint>().connectedBody.gameObject;
                objCopy_my2PortalHand_right = GameObject.Instantiate(grabObject);
                // 2. locate the copy obj to my hand
                Vector3 portalHand2Obj = grabObject.transform.position - myHand_right.transform.position;
                Vector3 objCopyPos = portalHand2Obj + portalHand_right.transform.position;
                objCopy_my2PortalHand_right.transform.position = objCopyPos;

                // // 3. remove collider and render(or just keep it) if it is not collided to the portal collider
                objCopy_my2PortalHand_right.GetComponent<MeshCollider>().enabled = true;
                Physics.IgnoreCollision(objCopy_my2PortalHand_right.GetComponent<MeshCollider>(), GameObject.Find("PortalCircleSurface").GetComponent<MeshCollider>());
                Physics.IgnoreCollision(grabObject.GetComponent<MeshCollider>(), GameObject.Find("PortalCircleSurface").GetComponent<MeshCollider>());

                // 4. add the object to my hand
                portalHand_right.GetComponent<FixedJoint>().connectedBody = objCopy_my2PortalHand_right.GetComponent<Rigidbody>();

            }else if(myHand_right.GetComponent<FixedJoint>().connectedBody == null && portalHand_right.GetComponent<FixedJoint>().connectedBody != null){
                    if(!myArm_right.GetComponent<ArmExtensionFromHeadToHand>().GetIsCollidingToPortal()){
                        portalHand_right.GetComponent<FixedJoint>().connectedBody = null;
                        portalHand_right.GetComponent<Hand>().RemoveContactInteractables(objCopy_my2PortalHand_right);

                        Destroy(objCopy_my2PortalHand_right);
                        objCopy_my2PortalHand_right = null;
                    }
            }
        }
    }


     private void LeftHandObjectManager_fromMy2PortalHand(){
        GameObject grabObject = null;

        if(!myHand_left.GetComponent<Hand>().isActiveAndEnabled){
            if(myArm_left.GetComponent<ArmExtensionFromHeadToHand>().GetIsCollidingToPortal() &&
                myHand_left.GetComponent<FixedJoint>().connectedBody != null &&
                !myHand_left.GetComponent<FixedJoint>().connectedBody.gameObject.GetComponent<PortalObject>().GetIsCollidingToPortal()
            ){

                grabObject = myHand_left.GetComponent<FixedJoint>().connectedBody.gameObject;
                myHand_left.GetComponent<Hand>().RemoveContactInteractables(grabObject);

                Destroy(grabObject);
                myHand_left.GetComponent<FixedJoint>().connectedBody = null;

                objCopy_my2PortalHand_left = null;    
                grabObject = null;
            }
        }else{

            if(myHand_left.GetComponent<FixedJoint>().connectedBody != null && portalHand_left.GetComponent<FixedJoint>().connectedBody == null){
                // 1. create an copy
                grabObject = myHand_left.GetComponent<FixedJoint>().connectedBody.gameObject;
                objCopy_my2PortalHand_left = GameObject.Instantiate(grabObject);
                // 2. locate the copy obj to my hand
                Vector3 portalHand2Obj = grabObject.transform.position - myHand_left.transform.position;
                Vector3 objCopyPos = portalHand2Obj + portalHand_left.transform.position;
                objCopy_my2PortalHand_left.transform.position = objCopyPos;

                // // 3. remove collider and render(or just keep it) if it is not collided to the portal collider
                objCopy_my2PortalHand_left.GetComponent<MeshCollider>().enabled = true;
                Physics.IgnoreCollision(objCopy_my2PortalHand_left.GetComponent<MeshCollider>(), GameObject.Find("PortalCircleSurface").GetComponent<MeshCollider>());
                Physics.IgnoreCollision(grabObject.GetComponent<MeshCollider>(), GameObject.Find("PortalCircleSurface").GetComponent<MeshCollider>());

                // 4. add the object to my hand
                portalHand_left.GetComponent<FixedJoint>().connectedBody = objCopy_my2PortalHand_left.GetComponent<Rigidbody>();

            }else if(myHand_left.GetComponent<FixedJoint>().connectedBody == null && portalHand_left.GetComponent<FixedJoint>().connectedBody != null){
                    if(!myArm_left.GetComponent<ArmExtensionFromHeadToHand>().GetIsCollidingToPortal()){
                        portalHand_left.GetComponent<FixedJoint>().connectedBody = null;
                        portalHand_left.GetComponent<Hand>().RemoveContactInteractables(objCopy_my2PortalHand_left);

                        Destroy(objCopy_my2PortalHand_left);
                        objCopy_my2PortalHand_left = null;
                    }
            }
        }
    }


    void Update(){
        if(myHand_left.GetComponent<FixedJoint>().connectedBody != null 
                && myHand_left.GetComponent<FixedJoint>().connectedBody.gameObject.GetComponent<PortalGrabCursor>() != null){
                return;
        }else if(myHand_right.GetComponent<FixedJoint>().connectedBody != null 
                && myHand_right.GetComponent<FixedJoint>().connectedBody.gameObject.GetComponent<PortalGrabCursor>() != null){
                return;
        }else{
            RightHandObjectManager_fromMy2PortalHand();
            LeftHandObjectManager_fromMy2PortalHand();

            RightHandObjectManager_fromPortal2MyHand();
            LeftHandObjectManager_fromPortal2MyHand();
        }   
    }
}
