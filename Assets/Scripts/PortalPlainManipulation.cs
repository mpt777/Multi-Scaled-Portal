using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalPlainManipulation : MonoBehaviour
{
    private Transform portalOriginParent;
    private Transform portalTargetParent;

  private GameObject portalOrigin;
  private GameObject portalTarget;

  private GameObject portalHand_left;
  private GameObject portalHand_right;

  private GameObject myHand_left;
  private GameObject myHand_right;

  private GameObject portalManipulationGrabPivot;
  private GameObject portalManipulationGrabPivot_target;


    private bool isOneHandGrab;
    private bool isTwoHandsGrab;


    public GameObject portalPlainGrabPoitner;
    private GameObject portalPlainGrabPointer_instance_left = null;
    private GameObject portalPlainGrabPointer_instance_right = null;

    private Vector3 myHand_left_last_position;
    private Vector3 myHand_right_last_position;

    private Vector3 myHand_left_last_rotation_inEuler;
    private Vector3 myHand_right_last_rotation_inEuler;

    // portal zooming technique 
    private bool isZoomingStart = false;
    private float prevDistance;


    public bool isGrabbingPortalPlane(){
        if(isOneHandGrab || isTwoHandsGrab) return true;

        return false;
    }

    public void OnEnable(){

        portalOriginParent = GameObject.FindWithTag("portalGate_origin").transform;
        portalTargetParent = GameObject.FindWithTag("portalGate_target").transform;

        portalHand_left =GameObject.FindWithTag("LeftHand_portal");
        portalHand_right = GameObject.FindWithTag("RightHand_portal");
        portalManipulationGrabPivot = GameObject.Find("grab_pivot");
        portalManipulationGrabPivot_target = GameObject.Find("grab_pivot_target");

        portalManipulationGrabPivot.transform.localPosition = new Vector3(0,0,0);
        portalManipulationGrabPivot_target.transform.localPosition = new Vector3(0,0,0);

        myHand_left = transform.GetComponent<PortalManager>().realHand_Left;
        myHand_right = transform.GetComponent<PortalManager>().realHand_Right;

        portalPlainGrabPointer_instance_left = transform.GetComponent<PortalManager>().portalPlaneCursor_left;
        portalPlainGrabPointer_instance_right = transform.GetComponent<PortalManager>().portalPlaneCursor_right;

        Physics.IgnoreCollision(portalPlainGrabPointer_instance_left.GetComponent<Collider>(), GameObject.Find("PortalCircleSurface").GetComponent<MeshCollider>());
        Physics.IgnoreCollision(portalPlainGrabPointer_instance_right.GetComponent<Collider>(), GameObject.Find("PortalCircleSurface").GetComponent<MeshCollider>());
    }

    public void DestoryCursorObject(){
        myHand_left.GetComponent<Hand>().RemoveContactInteractables(portalPlainGrabPointer_instance_left);
        myHand_right.GetComponent<Hand>().RemoveContactInteractables(portalPlainGrabPointer_instance_right);
    }

    private void ShowPortalPlainGrabPointer(){
        if(myHand_left.GetComponent<FixedJoint>().connectedBody == null){
            if(myHand_left.GetComponent<MyPortalHandHandler>().GetIsCollidingToPortal()){
                  portalPlainGrabPointer_instance_left.GetComponent<PortalGrabCursor>().ActivateCursor();
            }
        }

        if(myHand_right.GetComponent<FixedJoint>().connectedBody == null){
            if(myHand_right.GetComponent<MyPortalHandHandler>().GetIsCollidingToPortal()){         
                portalPlainGrabPointer_instance_right.GetComponent<PortalGrabCursor>().ActivateCursor();
            }
        }
    }

    private void HidePortalPlainGrabPointer(){
        if(!myHand_left.GetComponent<MyPortalHandHandler>().GetIsCollidingToPortal()){
            portalPlainGrabPointer_instance_left.GetComponent<PortalGrabCursor>().DeactivateCursor();
        }
        

        if(portalPlainGrabPointer_instance_right != null){
            if(!myHand_right.GetComponent<MyPortalHandHandler>().GetIsCollidingToPortal()){
                portalPlainGrabPointer_instance_right.GetComponent<PortalGrabCursor>().DeactivateCursor();
            }
        }
    }

    private void UpdatePortalPlainGrabCursorPos(){
        if(myHand_left.GetComponent<MyPortalHandHandler>().GetIsCollidingToPortal()){
            portalPlainGrabPointer_instance_left.transform.position = myHand_left.transform.Find("palmPos").transform.position;
            portalPlainGrabPointer_instance_left.transform.rotation = portalOrigin.transform.rotation;
        }
        
        if(myHand_right.GetComponent<MyPortalHandHandler>().GetIsCollidingToPortal()){
            portalPlainGrabPointer_instance_right.transform.position = myHand_right.transform.Find("palmPos").transform.position;
            portalPlainGrabPointer_instance_right.transform.rotation = portalOrigin.transform.rotation;
        }
    }

    public void ResetFixedJoint(){
        portalOrigin = GameObject.Find("PortalCircle_origin");
        portalTarget = GameObject.Find("PortalCircle_target");

        Destroy(portalPlainGrabPointer_instance_right.GetComponent<FixedJoint>());
        Destroy(portalPlainGrabPointer_instance_left.GetComponent<FixedJoint>());

        portalOrigin.transform.SetParent(portalOriginParent);
        portalTarget.transform.SetParent(portalTargetParent);
    }

    public void ManipulatePortalPlane(){
        portalOrigin = GameObject.Find("PortalCircle_origin");
        portalTarget = GameObject.Find("PortalCircle_target");

        if(portalPlainGrabPointer_instance_right.GetComponent<PortalGrabCursor>().GetIsGrabbed()){
            if(portalPlainGrabPointer_instance_right.GetComponent<FixedJoint>() == null ){
        
                portalPlainGrabPointer_instance_right.AddComponent<FixedJoint>(); 
                portalManipulationGrabPivot.transform.position = myHand_right.transform.position;
                portalManipulationGrabPivot_target.transform.position = portalTargetParent.position + (myHand_right.transform.position - portalOriginParent.position);

                portalOrigin.transform.SetParent(portalManipulationGrabPivot.transform);
                portalTarget.transform.SetParent(portalManipulationGrabPivot_target.transform);
            }else{                
                Vector3 offset =  myHand_right.transform.position - myHand_left_last_position;

                portalManipulationGrabPivot.transform.position =  myHand_right.transform.position;
                portalManipulationGrabPivot.transform.rotation = portalPlainGrabPointer_instance_right.transform.rotation;

                portalManipulationGrabPivot_target.transform.position = portalTargetParent.position + (myHand_right.transform.position - portalOriginParent.position);
                portalManipulationGrabPivot_target.transform.rotation = portalPlainGrabPointer_instance_right.transform.rotation;
            }
        }

        if(portalPlainGrabPointer_instance_left.GetComponent<PortalGrabCursor>().GetIsGrabbed()){

        }
    }


    public void ZoomPortalPlane(){
        if(!isZoomingStart){
            prevDistance = Vector3.Distance(portalHand_left.transform.position, portalHand_right.transform.position);
            portalTarget.transform.SetParent(portalManipulationGrabPivot_target.transform);

            isZoomingStart = true;
        }else{
            float curDistance = Vector3.Distance(portalHand_left.transform.position, portalHand_right.transform.position);

            float dif = curDistance - prevDistance;
           
            if(Mathf.Abs(dif) < 0.01){
                // give a threshold
                return;
            }

            portalOrigin = GameObject.Find("PortalCircle_origin");
            portalTarget = GameObject.Find("PortalCircle_target");
            
            Vector3 portalTargetPos = portalTarget.transform.position;
            Vector3 portalOriginPos = portalOrigin.transform.position;

            if(dif > 0){
                // zoom out
                // == move portal camera backward    
                portalTargetParent.position = portalTargetParent.position + dif * Vector3.Normalize(portalOriginPos - portalTargetPos);
            }else{
                // zoom in
                // == move portal camera forward
                portalTargetParent.position = portalTargetParent.position + dif * Vector3.Normalize(portalOriginPos - portalTargetPos);
            }

            prevDistance = curDistance;
        }
    }


    void Update()
    {
        ShowPortalPlainGrabPointer();
        HidePortalPlainGrabPointer();
        
       
        if(portalPlainGrabPointer_instance_right.GetComponent<PortalGrabCursor>().GetIsGrabbed()
        && portalPlainGrabPointer_instance_left.GetComponent<PortalGrabCursor>().GetIsGrabbed()
        ){
            isTwoHandsGrab = true;
            isOneHandGrab = false;
        }else if(portalPlainGrabPointer_instance_right.GetComponent<PortalGrabCursor>().GetIsGrabbed()
        || portalPlainGrabPointer_instance_left.GetComponent<PortalGrabCursor>().GetIsGrabbed()
        ){
            isTwoHandsGrab = false;
            isOneHandGrab = true;
            
        }else{
            isTwoHandsGrab = false;
            isOneHandGrab = false;
            isZoomingStart = false;
        }

        if(isOneHandGrab && !isZoomingStart){
            ManipulatePortalPlane();
        }

        if(isTwoHandsGrab){
            ZoomPortalPlane();
        }

        if(!isOneHandGrab && !isTwoHandsGrab){
            ResetFixedJoint();
            UpdatePortalPlainGrabCursorPos();
        }
    }
}
