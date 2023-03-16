using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class MyPortalHandHandler : MonoBehaviour
{
    public GameObject armObject;

    private GameObject portal_origin_centerPivot = null;
    private GameObject portal_target_centerPivot = null;

    [SerializeField]
    private GameObject teleportHand = null;
    private Camera VRCamera;
    private GameObject portalCamera;
    public PortalManager portalManager;

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
            wasCollidingToPortal = false;
            isCollidingToPortal = true;
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

    private bool isPortalHandActivate = false;

    public void OnEnable()
    {
        // set the attributes
        // hand
        if (gameObject.tag == "RightHand_real")
        {
            teleportHand = portalManager.portalHand_Right;
            teleportHand.SetActive(true);
            teleportHand.GetComponent<Hand>().enabled = true;
            teleportHand.GetComponent<Collider>().enabled = false;
            teleportHand.GetComponent<MyLayHandler>().enabled = false;
            teleportHand.transform.GetChild(0).gameObject.SetActive(false);
        }
        else if (gameObject.tag == "LeftHand_real")
        {
            teleportHand = portalManager.portalHand_Left;
            teleportHand.SetActive(true);
            teleportHand.GetComponent<Hand>().enabled = true;
            teleportHand.GetComponent<Collider>().enabled = false;
            teleportHand.GetComponent<MyLayHandler>().enabled = false;
            teleportHand.transform.GetChild(0).gameObject.SetActive(false);
        }

        // camera
        VRCamera = portalManager.VRCamera;
        portalCamera = portalManager.portalCamera;

        // center pivots
        portal_origin_centerPivot = portalManager.portal_origin_centerPivot;
        portal_target_centerPivot = portalManager.portal_target_centerPivot;

        isPortalHandActivate = false;
    }

    private void Update()
    {
        Vector3 curPos = gameObject.transform.position;

        Vector3 chestPos = VRCamera.transform.position - new Vector3(0, 0.3f, 0);
        Vector3 camera2hand = curPos - chestPos;
        Vector3 camera2OrigincenterPivot =  Vector3.Project(portal_origin_centerPivot.transform.position - chestPos, chestPos);

        bool isCollision = armObject.GetComponent<ArmExtensionFromHeadToHand>().GetIsCollidingToPortal();
        

        if(Experiment_Setting.instance.task_type == Task_Type.DOCKING_TASK){
            if(isCollidingToPortal || (isCollision && wasCollidingToPortal)){
                ShowPortalHand();
                
            }else{
                HidePortalHand();
            }

            if (isCollision)
            {
                if (!isPortalHandActivate)
                {
                    OnEnablePortalHand();
                }
            }
            else
            {
                if (isPortalHandActivate)
                {
                    OnDisablePortalHand();
                }
            }
        }else if(Experiment_Setting.instance.task_type == Task_Type.SELECTION_TASK){
            if(isCollidingToPortal || (isCollision && wasCollidingToPortal)){
                ShowPortalHand();
                OnEnablePortalHand();
              
            }else{
                HidePortalHand();
                OnDisablePortalHand();
            }

        }
        updatePortalHand();
    }

    private bool isLookingSameDirection(Vector3 vecA, Vector3 vecB)
    {
        Vector3 projection = Vector3.Project(vecA, vecB);

        bool isMagnitudeCheck = false;
        bool isLookingSameDir = false;


        if(vecB.sqrMagnitude - 0.05f <= projection.sqrMagnitude )
            isMagnitudeCheck = true;

        float x_vecA = projection[0];
        float y_vecA = projection[1];
        float z_vecA = projection[2];

        float x_vecB = vecB[0];
        float y_vecB = vecB[1];
        float z_vecB = vecB[2];

        if (x_vecA / Mathf.Abs(x_vecA) == x_vecB / Mathf.Abs(x_vecB)
            && y_vecA / Mathf.Abs(y_vecA) == y_vecB / Mathf.Abs(y_vecB)
            && z_vecA / Mathf.Abs(z_vecA) == z_vecB / Mathf.Abs(z_vecB)
            )
            isLookingSameDir = true;

        return isMagnitudeCheck && isLookingSameDir;
    }

    private void ShowPortalHand(){
        if (teleportHand == null)
        {
            portalManager.SetPortalHands();
        }

        teleportHand.transform.GetChild(0).gameObject.SetActive(true);
    }

    private void HidePortalHand(){
        teleportHand.transform.GetChild(0).gameObject.SetActive(false);
    }

    private void OnEnablePortalHand()
    {
        if (teleportHand == null)
        {
            portalManager.SetPortalHands();
        }

        // uncover the real hand model        
        gameObject.GetComponent<Hand>().enabled = false;
        gameObject.GetComponent<MyLayHandler>().enabled = false;

        if(!PortalManager.instance.isAllowObjectTransportThroughPortal){
            gameObject.GetComponent<FixedJoint>().connectedBody = null;
        }

        // show the portal hand
        
        if(!teleportHand.activeSelf){
            teleportHand.SetActive(true);
        }

        teleportHand.GetComponent<Hand>().enabled = true;
        teleportHand.GetComponent<Collider>().enabled = true;
        teleportHand.GetComponent<MyLayHandler>().portalManager = portalManager.gameObject;


        if (transform.GetComponent<Custom_VR_Behaviour_Skeleton>().IsMainHand()){
            // for the experiment block the function opening portal in portal
            if (PortalManager.instance.isAllowCreatingPortalInPortal)
            {
                teleportHand.GetComponent<MyLayHandler>().enabled = true;
                portalManager.ReparetToTargetRoom(teleportHand.GetComponent<MyLayHandler>().m_Pointer);

                LineRenderer lineRenderer = teleportHand.GetComponent<LineRenderer>();
                //lineRenderer.SetWidth(lineRenderer.startWidth / portalManager.originToTargetTransform.x, lineRenderer.endWidth / portalManager.originToTargetTransform.x);
                lineRenderer.widthMultiplier *= portalManager.originToTargetTransform.x;
            }
        }

        isPortalHandActivate = true;
    }

    public Vector3 GetPortalHandPosition(){
        return teleportHand.transform.position;
    }

    private void updatePortalHand()
    {
        Matrix4x4 hand_to_world = transform.localToWorldMatrix;
        Matrix4x4 world_to_pivot = portal_origin_centerPivot.transform.worldToLocalMatrix;
        Matrix4x4 hand_to_pivot = world_to_pivot * hand_to_world;

        teleportHand.transform.localRotation = hand_to_pivot.GetRotation();
        teleportHand.transform.localPosition = hand_to_pivot.GetPosition();
    }

    private void OnDisablePortalHand()
    {

        teleportHand.GetComponent<Hand>().enabled = true;
        teleportHand.GetComponent<MyLayHandler>().enabled = false;
        teleportHand.GetComponent<Collider>().enabled = false;

        if(!PortalManager.instance.isAllowObjectTransportThroughPortal){
            teleportHand.GetComponent<FixedJoint>().connectedBody = null;  
        }
        isPortalHandActivate = false;
        gameObject.GetComponent<Hand>().enabled = true;
        gameObject.GetComponent<MyLayHandler>().enabled = true;
    }


    //return relative roation to VR camera 
    public Vector3 getAngleMovement(GameObject centerPivot)
    {
        Quaternion cameraRoatation = VRCamera.transform.rotation;
        Quaternion thisRotation = transform.rotation;

        Quaternion relativeRotation = Quaternion.Inverse(cameraRoatation) * thisRotation;

        return relativeRotation.eulerAngles;
    }

    public static Vector3 getRelativePosition(Vector3 origin, Vector3 position)
    {
        return origin - position;
    }

    public Vector3 getHandPosition_relaiveTo(GameObject centerPivot)
    {
        Vector3 relativePosition = new Vector3(-transform.position.z + centerPivot.transform.position.z, -transform.position.y + centerPivot.transform.position.y, +transform.position.x - centerPivot.transform.position.x);
        return relativePosition;
    }
}
