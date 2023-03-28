﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalObjectThrough : MonoBehaviour
{

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

    private List<GameObject> allHands;

    public void OnEnable() {
        //portalHand_left = GameObject.FindWithTag("LeftHand_portal");
        //portalHand_right = GameObject.FindWithTag("RightHand_portal");

        portalManager = transform.GetComponent<PortalManager>();

        portalHand_left = portalManager.portalHand_Left;
        portalHand_right = portalManager.portalHand_Right;

        myHand_left = transform.GetComponent<PortalManager>().realHand_Left;
        myHand_right = transform.GetComponent<PortalManager>().realHand_Right;
        myArm_left = transform.GetComponent<PortalManager>().realArm_Left;
        myArm_right = transform.GetComponent<PortalManager>().realArm_Right;


        allHands = new List<GameObject>() { myHand_right, portalHand_right, portalHand_left, myHand_left };

        // set the boolean for is grabbing
    }

    public void DestroyCopiedObjects()
    {
        if (objCopy_portal2MyHand_right != null)
        {
            myHand_right.GetComponent<Hand>().RemoveContactInteractables(objCopy_portal2MyHand_right);
        }

        if (objCopy_portal2MyHand_left != null)
        {
            myHand_left.GetComponent<Hand>().RemoveContactInteractables(objCopy_portal2MyHand_left);
        }
        if (objCopy_my2PortalHand_right != null)
        {
            portalHand_right.GetComponent<Hand>().RemoveContactInteractables(objCopy_my2PortalHand_right);
        }

        if (objCopy_my2PortalHand_left != null)
        {
            portalHand_left.GetComponent<Hand>().RemoveContactInteractables(objCopy_my2PortalHand_left);
        }

        Destroy(objCopy_portal2MyHand_right);
        Destroy(objCopy_portal2MyHand_left);
        Destroy(objCopy_my2PortalHand_right);
        Destroy(objCopy_my2PortalHand_left);
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
                //Physics.IgnoreCollision(objCopy.GetComponent<BoxCollider>(), GameObject.Find("PortalCircleSurface").GetComponent<MeshCollider>());

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
            //Physics.IgnoreCollision(objCopy.GetComponent<BoxCollider>(), GameObject.Find("PortalCircleSurface").GetComponent<MeshCollider>());
            //Physics.IgnoreCollision(grabObject.GetComponent<BoxCollider>(), GameObject.Find("PortalCircleSurface").GetComponent<MeshCollider>());

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

        if (startHand == null || endHand == null)
        {
            return;
        }

        if (!startHand.GetComponent<Hand>().isActiveAndEnabled || !startHand.GetComponent<Hand>().IsGrabbing())
        {
            return;
        }

        if (startHand.GetComponent<FixedJoint>().connectedBody == null)
        {
            return;
        }

        if (endHand.GetComponent<FixedJoint>().connectedBody != null)
        {
            return;
        }

        grabObject = startHand.GetComponent<FixedJoint>().connectedBody.gameObject;

        if (grabObject.GetComponent<Interactable>().linkedObj != null)
        {
            return;
        }
        InstanceAndAttach(grabObject, startHand, endHand, toOrigin);

    }

    private void InstanceAndAttach(GameObject grabObject, GameObject startHand, GameObject endHand, bool toOrigin)
    {
        objCopy = GameObject.Instantiate(grabObject);
        // Link objs together for easy reference
        grabObject.GetComponent<Interactable>().linkedObj = objCopy.GetComponent<Interactable>();
        objCopy.GetComponent<Interactable>().linkedObj = grabObject.GetComponent<Interactable>();

        Vector3 room2RoomScale = new Vector3(1, 1, 1);
        if (toOrigin)
        {
            portalManager.ReparetToOriginRoom(objCopy, "Objects");
            //portalManager.TransformToOriginRoom(objCopy);
            room2RoomScale = portalManager.targetToOriginTransform;
        }
        else
        {
            portalManager.ReparetToTargetRoom(objCopy, "Objects");
            room2RoomScale = portalManager.originToTargetTransform;
        }

        // 2. locate the copy obj to my hand
        Vector3 endHand2Obj = Vector3.Scale((grabObject.transform.position - startHand.transform.position), room2RoomScale);
        Vector3 objCopyPos = endHand2Obj + endHand.transform.position;
        objCopy.transform.position = objCopyPos;

        // // 3. remove collider and render(or just keep it) if it is not collided to the portal collider
        objCopy.GetComponent<BoxCollider>().enabled = true;
        //Physics.IgnoreCollision(objCopy.GetComponent<BoxCollider>(), GameObject.Find("PortalCircleSurface").GetComponent<MeshCollider>());
        //Physics.IgnoreCollision(grabObject.GetComponent<BoxCollider>(), GameObject.Find("PortalCircleSurface").GetComponent<MeshCollider>());

        // 4. add the object to my hand
        endHand.GetComponent<FixedJoint>().connectedBody = objCopy.GetComponent<Rigidbody>();
    }

    private void RemoveInteractables(List<GameObject> hands, GameObject linked)
    {
        for (int i = 0; i < hands.Count; i++)
        {
            hands[i].GetComponent<Hand>().RemoveContactInteractables(linked);
        }
    }
    private void AddInteractables(List<GameObject> hands, GameObject linked)
    {
        for (int i = 0; i < hands.Count; i++)
        {
            hands[i].GetComponent<Hand>().AddContactInteractables(linked);
        }
    }

    private void DropInteractable(GameObject myHand, GameObject portalHand)
    {
        if (myHand.GetComponent<Hand>().IsGrabbing() || portalHand == null)
        {
            return;
        }
        if (myHand.GetComponent<FixedJoint>().connectedBody == null && portalHand.GetComponent<FixedJoint>().connectedBody == null)
        {
            return;
        }
        if (portalHand.GetComponent<MyPortalHand>().GetIsPortalHandActivate()) 
        // if the portalhand is active, drop in the portal
        { 
            DropInHand(myHand, portalHand);
        }
        // if not colliding, box in current room
        else
        {
            DropInHand(portalHand, myHand);
        }
    }

    private void DropInHand(GameObject dropHand, GameObject keepHand)
    {
        GameObject dropObject = null;
        GameObject keepObject = null;

        if (dropHand.GetComponent<FixedJoint>().connectedBody != null)
        {
            dropObject = dropHand.GetComponent<FixedJoint>().connectedBody.gameObject;
            keepObject = dropHand.GetComponent<FixedJoint>().connectedBody.gameObject.GetComponent<Interactable>().linkedObj.gameObject;
        }
        else if (keepHand.GetComponent<FixedJoint>().connectedBody != null)
        {
            dropObject = keepHand.GetComponent<FixedJoint>().connectedBody.gameObject.GetComponent<Interactable>().linkedObj.gameObject;
            keepObject = keepHand.GetComponent<FixedJoint>().connectedBody.gameObject;
        }
        if (dropObject == null)
        {
            Debug.Log("STOP THE PRESSES!");
        }
        if (dropObject != null)
        {
            // Remove the box from all hands to ensure no null interactables
            
            RemoveInteractables(allHands, dropObject);
            dropObject.GetComponent<Interactable>().ClearLinkedObject();

            Destroy(dropObject);

            objCopy = null;
            dropHand.GetComponent<FixedJoint>().connectedBody = null;
            keepHand.GetComponent<FixedJoint>().connectedBody = null;
        }
    }

    private void MoveInteractableToNewPortal(GameObject realHand, GameObject portalHand)
    {
        if (!realHand.GetComponent<Hand>().IsGrabbing())
        {
            return;
        }
        if (realHand.GetComponent<FixedJoint>().connectedBody == null)
        {
            return;
        }
        if (realHand.GetComponent<MyRealHand>().myPortalHand == null)
        {
            return;
        }

        GameObject grabObject = realHand.GetComponent<FixedJoint>().connectedBody.gameObject;
        GameObject linkedObject = grabObject.GetComponent<Interactable>().linkedObj.gameObject;


        if (realHand.GetComponent<MyRealHand>().myPortalHand == null)
        {
            return;
        }
        if (realHand.GetComponent<MyRealHand>().myPortalHand.GetComponent<FixedJoint>().connectedBody != null)
        {
            return;
        }


        Vector3 room2RoomScale = new Vector3(1, 1, 1);
        //if (toOrigin)
        //{
        //    portalManager.ReparetToOriginRoom(objCopy, "Objects");
        //    //portalManager.TransformToOriginRoom(objCopy);
        //    room2RoomScale = portalManager.targetToOriginTransform;
        //}
        //else
        //{
        //    portalManager.ReparetToTargetRoom(objCopy, "Objects");
        //    room2RoomScale = portalManager.originToTargetTransform;
        //}

        // 2. locate the copy obj to my hand
        Vector3 endHand2Obj = Vector3.Scale((grabObject.transform.position - realHand.transform.position), room2RoomScale);
        Vector3 objCopyPos = endHand2Obj + portalHand.transform.position;
        linkedObject.transform.position = objCopyPos;

        // // 3. remove collider and render(or just keep it) if it is not collided to the portal collider
        linkedObject.GetComponent<BoxCollider>().enabled = true;
        //Physics.IgnoreCollision(objCopy.GetComponent<BoxCollider>(), GameObject.Find("PortalCircleSurface").GetComponent<MeshCollider>());
        //Physics.IgnoreCollision(grabObject.GetComponent<BoxCollider>(), GameObject.Find("PortalCircleSurface").GetComponent<MeshCollider>());

        // 4. add the object to my hand
        portalHand.GetComponent<FixedJoint>().connectedBody = linkedObject.GetComponent<Rigidbody>();

    }

    private void ProcressGrab()
    {
        //CreateAndLinkInteractable(portalHand_right, myHand_right, true);
        //CreateAndLinkInteractable(portalHand_left, myHand_left, true);
        //CreateAndLinkInteractable(myHand_left, portalHand_left, false);
        //CreateAndLinkInteractable(myHand_right, portalHand_right, false);

        GameObject right_portalHand_right = myHand_right.GetComponent<MyRealHand>().myPortalHand;
        GameObject left_portalHand_left = myHand_left.GetComponent<MyRealHand>().myPortalHand;

        CreateAndLinkInteractable(right_portalHand_right, myHand_right, true);
        CreateAndLinkInteractable(left_portalHand_left, myHand_left, true);
        CreateAndLinkInteractable(myHand_left, left_portalHand_left, false);
        CreateAndLinkInteractable(myHand_right, right_portalHand_right, false);

        //MoveInteractableToNewPortal(myHand_left, left_portalHand_left);
        //MoveInteractableToNewPortal(myHand_right, right_portalHand_right);

        DropInteractable(myHand_right, right_portalHand_right);
        DropInteractable(myHand_left, left_portalHand_left); 
    }

    void Update(){
        if (myHand_left.GetComponent<FixedJoint>().connectedBody != null 
                && myHand_left.GetComponent<FixedJoint>().connectedBody.gameObject.GetComponent<PortalGrabCursor>() != null){
                return;
        }else if(myHand_right.GetComponent<FixedJoint>().connectedBody != null 
                && myHand_right.GetComponent<FixedJoint>().connectedBody.gameObject.GetComponent<PortalGrabCursor>() != null){
                return;
        }else{
            ProcressGrab();
        }
    }
}
