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

  private PortalManager portalManager;

  private GameObject myArm_left;
  private GameObject myArm_right;

    private GameObject objCopy_portal2MyHand_right;
    private GameObject objCopy_portal2MyHand_left;

    private GameObject objCopy_my2PortalHand_right;
    private GameObject objCopy_my2PortalHand_left;


    private GameObject objCopy;

    public void OnEnable(){
        portalHand_left =GameObject.FindWithTag("LeftHand_portal");
        portalHand_right = GameObject.FindWithTag("RightHand_portal");

        portalManager = transform.GetComponent<PortalManager>();

        myHand_left = transform.GetComponent<PortalManager>().realHand_Left;
        myHand_right = transform.GetComponent<PortalManager>().realHand_Right;
        myArm_left = transform.GetComponent<PortalManager>().realArm_Left;
        myArm_right = transform.GetComponent<PortalManager>().realArm_Right;

        // set the boolean for is grabbing
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

    private void RightHandObjectManager_fromPortal2MyHand()
    {
        GameObject grabObject = null;

        if (!portalHand_right.GetComponent<Hand>().isActiveAndEnabled && objCopy_portal2MyHand_right != null)
        {

            if (!myArm_right.GetComponent<ArmExtensionFromHeadToHand>().GetIsCollidingToPortal() &&
                !myHand_right.GetComponent<MyPortalHandHandler>().GetIsCollidingToPortal() &&
                portalHand_right.GetComponent<FixedJoint>().connectedBody != null &&
                !objCopy_portal2MyHand_right.GetComponent<PortalObject>().GetIsCollidingToPortal()
                )
            {

                grabObject = portalHand_right.GetComponent<FixedJoint>().connectedBody.gameObject;
                portalHand_right.GetComponent<Hand>().RemoveContactInteractables(grabObject);

                Destroy(grabObject);
                portalHand_right.GetComponent<FixedJoint>().connectedBody = null;


                objCopy_portal2MyHand_right = null;
                grabObject = null;
            }
        }
        else
        {
            if (portalHand_right.GetComponent<FixedJoint>().connectedBody != null && myHand_right.GetComponent<FixedJoint>().connectedBody == null)
            {
                // 1. create an copy
                grabObject = portalHand_right.GetComponent<FixedJoint>().connectedBody.gameObject;
                objCopy_portal2MyHand_right = GameObject.Instantiate(grabObject);

                portalManager.ReparetToOriginRoom(objCopy_portal2MyHand_right, "Objects");

                // 2. locate the copy obj to my hand
                Vector3 portalHand2Obj = grabObject.transform.position - portalHand_right.transform.position;
                Vector3 objCopyPos = portalHand2Obj + myHand_right.transform.position;
                objCopy_portal2MyHand_right.transform.position = objCopyPos;


                // // 3. remove collider and render(or just keep it) if it is not collided to the portal collider
                objCopy_portal2MyHand_right.GetComponent<BoxCollider>().enabled = true;
                Physics.IgnoreCollision(objCopy_portal2MyHand_right.GetComponent<BoxCollider>(), GameObject.Find("PortalCircleSurface").GetComponent<MeshCollider>());

                // 4. add the object to my hand
                myHand_right.GetComponent<FixedJoint>().connectedBody = objCopy_portal2MyHand_right.GetComponent<Rigidbody>();

            }
            else if (portalHand_right.GetComponent<FixedJoint>().connectedBody == null && myHand_right.GetComponent<FixedJoint>().connectedBody != null)
            {
                // when the portal hand is beyond the portal
                if ((myHand_right.GetComponent<MyPortalHandHandler>().GetWasCollidingToPortal() && myArm_right.GetComponent<ArmExtensionFromHeadToHand>().GetIsCollidingToPortal()) || myHand_right.GetComponent<MyPortalHandHandler>().GetIsCollidingToPortal())
                {
                    myHand_right.GetComponent<FixedJoint>().connectedBody = null;
                    Destroy(objCopy_portal2MyHand_right);
                    objCopy_portal2MyHand_right = null;
                }
            }
        }
    }

    private void LeftHandObjectManager_fromPortal2MyHand()
    {
        GameObject grabObject = null;

        if (!portalHand_left.GetComponent<Hand>().isActiveAndEnabled && objCopy_portal2MyHand_left != null)
        {
            if (!myArm_left.GetComponent<ArmExtensionFromHeadToHand>().GetIsCollidingToPortal() &&
                !myHand_left.GetComponent<MyPortalHandHandler>().GetIsCollidingToPortal() &&
                portalHand_left.GetComponent<FixedJoint>().connectedBody != null &&
                !objCopy_portal2MyHand_left.GetComponent<PortalObject>().GetIsCollidingToPortal()
                )
            {

                grabObject = portalHand_left.GetComponent<FixedJoint>().connectedBody.gameObject;
                portalHand_left.GetComponent<Hand>().RemoveContactInteractables(grabObject);

                Destroy(grabObject);
                portalHand_left.GetComponent<FixedJoint>().connectedBody = null;

                objCopy_portal2MyHand_left = null;
                grabObject = null;
            }
        }
        else
        {
            if (portalHand_left.GetComponent<FixedJoint>().connectedBody != null && myHand_left.GetComponent<FixedJoint>().connectedBody == null)
            {
                // 1. create an copy
                grabObject = portalHand_left.GetComponent<FixedJoint>().connectedBody.gameObject;
                objCopy_portal2MyHand_left = GameObject.Instantiate(grabObject);

                portalManager.ReparetToOriginRoom(objCopy_portal2MyHand_left, "Objects");

                // 2. locate the copy obj to my hand
                Vector3 portalHand2Obj = grabObject.transform.position - portalHand_left.transform.position;
                Vector3 objCopyPos = portalHand2Obj + myHand_left.transform.position;
                objCopy_portal2MyHand_left.transform.position = objCopyPos;

                // // 3. remove collider and render(or just keep it) if it is not collided to the portal collider
                objCopy_portal2MyHand_left.GetComponent<BoxCollider>().enabled = true;
                Physics.IgnoreCollision(objCopy_portal2MyHand_left.GetComponent<BoxCollider>(), GameObject.Find("PortalCircleSurface").GetComponent<MeshCollider>());

                // 4. add the object to my hand
                myHand_left.GetComponent<FixedJoint>().connectedBody = objCopy_portal2MyHand_left.GetComponent<Rigidbody>();

            }
            else if (portalHand_left.GetComponent<FixedJoint>().connectedBody == null && myHand_left.GetComponent<FixedJoint>().connectedBody != null)
            {
                // when the portal hand is beyond the portal
                if ((myHand_left.GetComponent<MyPortalHandHandler>().GetWasCollidingToPortal() && myArm_left.GetComponent<ArmExtensionFromHeadToHand>().GetIsCollidingToPortal()) || myHand_left.GetComponent<MyPortalHandHandler>().GetIsCollidingToPortal())
                {
                    myHand_left.GetComponent<FixedJoint>().connectedBody = null;
                    Destroy(objCopy_portal2MyHand_left);
                    objCopy_portal2MyHand_left = null;
                }
            }
        }

    }


    private void RightHandObjectManager_fromMy2PortalHand()
    {
        GameObject grabObject = null;

        // if has something in hand return

        if (!myHand_right.GetComponent<Hand>().isActiveAndEnabled)
        {
            if (myArm_right.GetComponent<ArmExtensionFromHeadToHand>().GetIsCollidingToPortal() && myHand_right.GetComponent<FixedJoint>().connectedBody != null)
            {
                if (myHand_right.GetComponent<FixedJoint>().connectedBody.gameObject.GetComponent<PortalObject>() != null)
                {
                    if (!myHand_right.GetComponent<FixedJoint>().connectedBody.gameObject.GetComponent<PortalObject>().GetIsCollidingToPortal())
                    {

                        grabObject = myHand_right.GetComponent<FixedJoint>().connectedBody.gameObject;
                        myHand_right.GetComponent<Hand>().RemoveContactInteractables(grabObject);

                        Destroy(grabObject);
                        myHand_right.GetComponent<FixedJoint>().connectedBody = null;

                        objCopy_my2PortalHand_right = null;
                        grabObject = null;
                    }
                }
            }
        }
        else
        {
            if (myHand_right.GetComponent<FixedJoint>().connectedBody != null && portalHand_right.GetComponent<FixedJoint>().connectedBody == null)
            {
                // 1. create an copy
                grabObject = myHand_right.GetComponent<FixedJoint>().connectedBody.gameObject;
                objCopy_my2PortalHand_right = GameObject.Instantiate(grabObject);
                // 2. locate the copy obj to my hand
                Vector3 portalHand2Obj = grabObject.transform.position - myHand_right.transform.position;
                Vector3 objCopyPos = portalHand2Obj + portalHand_right.transform.position;
                objCopy_my2PortalHand_right.transform.position = objCopyPos;

                // // 3. remove collider and render(or just keep it) if it is not collided to the portal collider
                objCopy_my2PortalHand_right.GetComponent<BoxCollider>().enabled = true;
                Physics.IgnoreCollision(objCopy_my2PortalHand_right.GetComponent<BoxCollider>(), GameObject.Find("PortalCircleSurface").GetComponent<MeshCollider>());
                Physics.IgnoreCollision(grabObject.GetComponent<BoxCollider>(), GameObject.Find("PortalCircleSurface").GetComponent<MeshCollider>());

                // 4. add the object to my hand
                portalHand_right.GetComponent<FixedJoint>().connectedBody = objCopy_my2PortalHand_right.GetComponent<Rigidbody>();

            }
            else if (myHand_right.GetComponent<FixedJoint>().connectedBody == null && portalHand_right.GetComponent<FixedJoint>().connectedBody != null)
            {
                if (!myArm_right.GetComponent<ArmExtensionFromHeadToHand>().GetIsCollidingToPortal())
                {
                    portalHand_right.GetComponent<FixedJoint>().connectedBody = null;
                    portalHand_right.GetComponent<Hand>().RemoveContactInteractables(objCopy_my2PortalHand_right);

                    Destroy(objCopy_my2PortalHand_right);
                    objCopy_my2PortalHand_right = null;
                }
            }
        }
    }


    private void LeftHandObjectManager_fromMy2PortalHand()
    {
        GameObject grabObject = null;

        // create item from here

        if (!myHand_left.GetComponent<Hand>().isActiveAndEnabled)
        {
            if (myArm_left.GetComponent<ArmExtensionFromHeadToHand>().GetIsCollidingToPortal() &&
                myHand_left.GetComponent<FixedJoint>().connectedBody != null)
            {
                if (myHand_left.GetComponent<FixedJoint>().connectedBody.gameObject.GetComponent<PortalObject>() != null)
                {
                    if (!myHand_left.GetComponent<FixedJoint>().connectedBody.gameObject.GetComponent<PortalObject>().GetIsCollidingToPortal())
                    {

                        grabObject = myHand_left.GetComponent<FixedJoint>().connectedBody.gameObject;
                        myHand_left.GetComponent<Hand>().RemoveContactInteractables(grabObject);

                        Destroy(grabObject);
                        myHand_left.GetComponent<FixedJoint>().connectedBody = null;

                        objCopy_my2PortalHand_left = null;
                        grabObject = null;
                    }
                }
            }
        }
        else
        {

            if (myHand_left.GetComponent<FixedJoint>().connectedBody != null && portalHand_left.GetComponent<FixedJoint>().connectedBody == null)
            {
                // 1. create an copy
                grabObject = myHand_left.GetComponent<FixedJoint>().connectedBody.gameObject;
                objCopy_my2PortalHand_left = GameObject.Instantiate(grabObject);
                // 2. locate the copy obj to my hand
                Vector3 portalHand2Obj = grabObject.transform.position - myHand_left.transform.position;
                Vector3 objCopyPos = portalHand2Obj + portalHand_left.transform.position;
                objCopy_my2PortalHand_left.transform.position = objCopyPos;

                // // 3. remove collider and render(or just keep it) if it is not collided to the portal collider
                objCopy_my2PortalHand_left.GetComponent<BoxCollider>().enabled = true;
                Physics.IgnoreCollision(objCopy_my2PortalHand_left.GetComponent<BoxCollider>(), GameObject.Find("PortalCircleSurface").GetComponent<MeshCollider>());
                Physics.IgnoreCollision(grabObject.GetComponent<BoxCollider>(), GameObject.Find("PortalCircleSurface").GetComponent<MeshCollider>());

                // 4. add the object to my hand
                portalHand_left.GetComponent<FixedJoint>().connectedBody = objCopy_my2PortalHand_left.GetComponent<Rigidbody>();

            }
            else if (myHand_left.GetComponent<FixedJoint>().connectedBody == null && portalHand_left.GetComponent<FixedJoint>().connectedBody != null)
            {
                if (!myArm_left.GetComponent<ArmExtensionFromHeadToHand>().GetIsCollidingToPortal())
                {
                    portalHand_left.GetComponent<FixedJoint>().connectedBody = null;
                    portalHand_left.GetComponent<Hand>().RemoveContactInteractables(objCopy_my2PortalHand_left);

                    Destroy(objCopy_my2PortalHand_left);
                    objCopy_my2PortalHand_left = null;
                }
            }
        }
    }

    private void HandObjectManager_fromPortal2MyHand(GameObject myHand, GameObject myArm, GameObject portalHand, GameObject objCopy)
    {
        GameObject grabObject = null;

        if (!portalHand.GetComponent<Hand>().isActiveAndEnabled && objCopy != null)
        {

            if (!myArm.GetComponent<ArmExtensionFromHeadToHand>().GetIsCollidingToPortal() &&
                !myHand.GetComponent<MyPortalHandHandler>().GetIsCollidingToPortal() &&
                portalHand.GetComponent<FixedJoint>().connectedBody != null &&
                !objCopy.GetComponent<PortalObject>().GetIsCollidingToPortal()
                )
            {

                grabObject = portalHand.GetComponent<FixedJoint>().connectedBody.gameObject;
                portalHand.GetComponent<Hand>().RemoveContactInteractables(grabObject);

                Destroy(grabObject);
                portalHand.GetComponent<FixedJoint>().connectedBody = null;


                objCopy = null;
                grabObject = null;
            }
        }
        else
        {
            if (portalHand.GetComponent<FixedJoint>().connectedBody != null && myHand.GetComponent<FixedJoint>().connectedBody == null)
            {
                // 1. create an copy
                grabObject = portalHand.GetComponent<FixedJoint>().connectedBody.gameObject;
                if (grabObject.GetComponent<Interactable>().linkedObj != null)
                {
                    return;
                }
                objCopy = GameObject.Instantiate(grabObject);

                grabObject.GetComponent<Interactable>().linkedObj = objCopy.GetComponent<Interactable>();

                portalManager.ReparetToOriginRoom(objCopy, "Objects");

                // 2. locate the copy obj to my hand
                Vector3 portalHand2Obj = grabObject.transform.position - portalHand.transform.position;
                Vector3 objCopyPos = portalHand2Obj + myHand.transform.position;
                objCopy.transform.position = objCopyPos;


                // // 3. remove collider and render(or just keep it) if it is not collided to the portal collider
                objCopy.GetComponent<BoxCollider>().enabled = true;
                Physics.IgnoreCollision(objCopy.GetComponent<BoxCollider>(), GameObject.Find("PortalCircleSurface").GetComponent<MeshCollider>());

                // 4. add the object to my hand
                myHand.GetComponent<FixedJoint>().connectedBody = objCopy.GetComponent<Rigidbody>();

            }
            else if (portalHand.GetComponent<FixedJoint>().connectedBody == null && myHand.GetComponent<FixedJoint>().connectedBody != null)
            {
                // when the portal hand is beyond the portal
                if ((myHand.GetComponent<MyPortalHandHandler>().GetWasCollidingToPortal() && myArm.GetComponent<ArmExtensionFromHeadToHand>().GetIsCollidingToPortal()) || myHand.GetComponent<MyPortalHandHandler>().GetIsCollidingToPortal())
                {
                    myHand.GetComponent<FixedJoint>().connectedBody = null;
                    Destroy(objCopy);
                    objCopy = null;
                }
            }
        }

    }

    private void HandObjectManager_fromMy2PortalHand(GameObject myHand, GameObject myArm, GameObject portalHand, GameObject objCopy)
    {
        GameObject grabObject = null;

        if (!myHand.GetComponent<Hand>().isActiveAndEnabled)
        {
            if (myArm.GetComponent<ArmExtensionFromHeadToHand>().GetIsCollidingToPortal() &&
                myHand.GetComponent<FixedJoint>().connectedBody != null)
            {
                if (myHand.GetComponent<FixedJoint>().connectedBody.gameObject.GetComponent<PortalObject>() != null)
                {
                    if (!myHand.GetComponent<FixedJoint>().connectedBody.gameObject.GetComponent<PortalObject>().GetIsCollidingToPortal())
                    {

                        grabObject = myHand.GetComponent<FixedJoint>().connectedBody.gameObject;
                        myHand.GetComponent<Hand>().RemoveContactInteractables(grabObject);

                        Destroy(grabObject);
                        myHand.GetComponent<FixedJoint>().connectedBody = null;

                        objCopy = null;
                        grabObject = null;
                    }
                }
            }
            return;
        }
        if (myHand.GetComponent<FixedJoint>().connectedBody != null && portalHand.GetComponent<FixedJoint>().connectedBody == null)
        {
            // HERE
            grabObject = myHand.GetComponent<FixedJoint>().connectedBody.gameObject;

            if (grabObject.GetComponent<Interactable>().linkedObj != null)
            {
                return;
            }
            objCopy = GameObject.Instantiate(grabObject);

            grabObject.GetComponent<Interactable>().linkedObj = objCopy.GetComponent<Interactable>();

            // 2. locate the copy obj to my hand
            Vector3 portalHand2Obj = grabObject.transform.position - myHand.transform.position;
            Vector3 objCopyPos = portalHand2Obj + portalHand.transform.position;
            objCopy.transform.position = objCopyPos;

            // // 3. remove collider and render(or just keep it) if it is not collided to the portal collider
            objCopy.GetComponent<BoxCollider>().enabled = true;
            Physics.IgnoreCollision(objCopy.GetComponent<BoxCollider>(), GameObject.Find("PortalCircleSurface").GetComponent<MeshCollider>());
            Physics.IgnoreCollision(grabObject.GetComponent<BoxCollider>(), GameObject.Find("PortalCircleSurface").GetComponent<MeshCollider>());

            // 4. add the object to my hand
            portalHand.GetComponent<FixedJoint>().connectedBody = objCopy.GetComponent<Rigidbody>();

        }
        else if (myHand.GetComponent<FixedJoint>().connectedBody == null && portalHand.GetComponent<FixedJoint>().connectedBody != null)
        {
            if (!myArm.GetComponent<ArmExtensionFromHeadToHand>().GetIsCollidingToPortal())
            {
                portalHand.GetComponent<FixedJoint>().connectedBody = null;
                portalHand.GetComponent<Hand>().RemoveContactInteractables(objCopy);

                Destroy(objCopy);
                objCopy = null;
            }
        }
    }

    private void CleanCopy()
    {
        DestroyCopiedObjects();
    }

    private void CreateAndLinkInteractable(GameObject startHand, GameObject endHand, bool toOrigin)
    {
        GameObject grabObject = null;

        if (startHand.GetComponent<FixedJoint>().connectedBody == null)
        {
            return;
        }

        grabObject = startHand.GetComponent<FixedJoint>().connectedBody.gameObject;

        if (grabObject.GetComponent<Interactable>().linkedObj != null)
        {
            return;
        }
        objCopy = GameObject.Instantiate(grabObject);
        // Link objs together for easy reference
        grabObject.GetComponent<Interactable>().linkedObj = objCopy.GetComponent<Interactable>();
        objCopy.GetComponent<Interactable>().linkedObj = grabObject.GetComponent<Interactable>();

        if (toOrigin)
        {
            portalManager.ReparetToOriginRoom(objCopy, "Objects");
            //portalManager.TransformToOriginRoom(objCopy);
        }
        else
        {
            portalManager.ReparetToTargetRoom(objCopy, "Objects");
            portalManager.TransformToTargetRoom(objCopy);
        }

        // 2. locate the copy obj to my hand
        Vector3 endHand2Obj = grabObject.transform.position - startHand.transform.position;
        Vector3 objCopyPos = endHand2Obj + endHand.transform.position;
        objCopy.transform.position = objCopyPos;

        // // 3. remove collider and render(or just keep it) if it is not collided to the portal collider
        objCopy.GetComponent<BoxCollider>().enabled = true;
        Physics.IgnoreCollision(objCopy.GetComponent<BoxCollider>(), GameObject.Find("PortalCircleSurface").GetComponent<MeshCollider>());
        Physics.IgnoreCollision(grabObject.GetComponent<BoxCollider>(), GameObject.Find("PortalCircleSurface").GetComponent<MeshCollider>());

        // 4. add the object to my hand
        endHand.GetComponent<FixedJoint>().connectedBody = objCopy.GetComponent<Rigidbody>();
    }

    private void DropInteractable(GameObject startHand)
    {
        if (startHand.GetComponent<Hand>().IsGrabbing())
        {
            return;
        }
        if (startHand.GetComponent<FixedJoint>().connectedBody == null)
        {
            return;
        }
        if (objCopy == null)
        {
            return;
        }
        GameObject connectedOBJ = startHand.GetComponent<FixedJoint>().connectedBody.gameObject;
        if (connectedOBJ != null)
        {
            GameObject linked = connectedOBJ.GetComponent<Interactable>().linkedObj.gameObject;

            myHand_right.GetComponent<Hand>().RemoveContactInteractables(linked);
            portalHand_right.GetComponent<Hand>().RemoveContactInteractables(linked);
            portalHand_left.GetComponent<Hand>().RemoveContactInteractables(linked);
            myHand_left.GetComponent<Hand>().RemoveContactInteractables(linked);

            linked.GetComponent<Interactable>().ClearLinkedObject();
            Destroy(linked);

            objCopy = null;
            startHand.GetComponent<FixedJoint>().connectedBody = null;
        }
    }

    private void ProcressGrab()
    {
        if (myHand_right.GetComponent<Hand>().isActiveAndEnabled && myHand_right.GetComponent<Hand>().IsGrabbing())
        {
            CreateAndLinkInteractable(portalHand_right, myHand_right, true);
        }

        if (portalHand_left.GetComponent<Hand>().isActiveAndEnabled && portalHand_left.GetComponent<Hand>().IsGrabbing())
        {
            CreateAndLinkInteractable(myHand_left, portalHand_left, false);
        }

        DropInteractable(myHand_right);
        DropInteractable(portalHand_left);
    }



    //private void MoveBoxes(GameObject myHand, GameObject myArm, GameObject portalHand, GameObject objCopy)
    //{



    //    if (!(myHand.GetComponent<Hand>().IsGrabbing()) && myHand.GetComponent<FixedJoint>().connectedBody != null)
    //    { 
    //        myHand.GetComponent<Hand>().AddContactInteractables(myHand.GetComponent<FixedJoint>().connectedBody.gameObject);
    //        myHand.GetComponent<Hand>().Drop();
    //        myHand.GetComponent<FixedJoint>().connectedBody = null;
    //    }
    //    // If arm arm is not through, return
    //    if (!((myHand.GetComponent<MyPortalHandHandler>().GetWasCollidingToPortal() && myArm.GetComponent<ArmExtensionFromHeadToHand>().GetIsCollidingToPortal()) || myHand.GetComponent<MyPortalHandHandler>().GetIsCollidingToPortal()))
    //    {
    //        return;
    //    }
    //    GameObject grabObject = null;

    //    if (portalHand.GetComponent<FixedJoint>().connectedBody == null)
    //    {
    //        return;
    //    }


    //    grabObject = portalHand.GetComponent<FixedJoint>().connectedBody.gameObject;
    //    objCopy = GameObject.Instantiate(grabObject);

    //    portalManager.ReparetToOriginRoom(objCopy, "Objects");

    //    // 2. locate the copy obj to my hand
    //    Vector3 portalHand2Obj = grabObject.transform.position - portalHand.transform.position;
    //    Vector3 objCopyPos = portalHand2Obj + myHand.transform.position;
    //    objCopy.transform.position = objCopyPos;


    //    // // 3. remove collider and render(or just keep it) if it is not collided to the portal collider
    //    objCopy.GetComponent<BoxCollider>().enabled = true;
    //    Physics.IgnoreCollision(objCopy.GetComponent<BoxCollider>(), GameObject.Find("PortalCircleSurface").GetComponent<MeshCollider>());

    //    // 4. add the object to my hand
    //    myHand.GetComponent<FixedJoint>().connectedBody = objCopy.GetComponent<Rigidbody>();

    //    objCopy.GetComponent<Interactable>().linkedObj = grabObject.GetComponent<Interactable>();



    //    // if in the hand, return
    //    //if (portalHand.GetComponent<FixedJoint>().connectedBody && !myHand.GetComponent<FixedJoint>().connectedBody)
    //    //{
    //    //    MoveBoxToOrigin(myHand, portalHand);
    //    //}

    //}


    void Update(){
        if (myHand_left.GetComponent<FixedJoint>().connectedBody != null 
                && myHand_left.GetComponent<FixedJoint>().connectedBody.gameObject.GetComponent<PortalGrabCursor>() != null){
                return;
        }else if(myHand_right.GetComponent<FixedJoint>().connectedBody != null 
                && myHand_right.GetComponent<FixedJoint>().connectedBody.gameObject.GetComponent<PortalGrabCursor>() != null){
                return;
        }else{
            //RightHandObjectManager_fromMy2PortalHand();
            //LeftHandObjectManager_fromMy2PortalHand();

            //RightHandObjectManager_fromPortal2MyHand();
            //LeftHandObjectManager_fromPortal2MyHand();

            //MoveBoxes(myHand_left, myArm_left, portalHand_left, objCopy_my2PortalHand_left);
            //MoveBoxes(myHand_right, myArm_right, portalHand_right, objCopy_portal2MyHand_right);
            //MoveBoxes(myHand_left, myArm_left, portalHand_left, objCopy_my2PortalHand_left);
            //MoveBoxes(myHand_right, myArm_right, portalHand_right, objCopy_portal2MyHand_right);

            ProcressGrab();
            //HandObjectManager_fromMy2PortalHand(myHand_left, myArm_left, portalHand_left, objCopy_my2PortalHand_left);
            //HandObjectManager_fromMy2PortalHand(myHand_right, myArm_right, portalHand_right, objCopy_my2PortalHand_right);

            //HandObjectManager_fromPortal2MyHand(myHand_left, myArm_left, portalHand_left, objCopy_portal2MyHand_left);
            //HandObjectManager_fromPortal2MyHand(myHand_right, myArm_right, portalHand_right, objCopy_portal2MyHand_right);

        }
    }
}
