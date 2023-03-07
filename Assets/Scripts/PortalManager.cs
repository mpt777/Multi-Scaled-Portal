using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using Valve.VR;
using cakeslice;

public class PortalManager : MonoBehaviour
{
    public static PortalManager instance;

    public GameObject originPortal;
    public GameObject targetPortal;

    public GameObject originRoom;
    public GameObject targetRoom;

    [SerializeField]
    public Portal_Navigation_Technique portalTechnique;

    [SerializeField]
    public Portal_Rotation_Technique rotationTechnique;

    public GameObject rotationController;


    private GameObject targetPortalInstance;

    private GameObject instanceOriginPortal;

    private bool isPortalOpen = false;

    private int numPortalOpen = 0;

    public bool GetIsPortalOpen(){
        return isPortalOpen;
    }

    public bool isAllowObjectTransportThroughPortal;
    public bool isAllowPortalPlainManipulation;
    public bool isAllowCreatingPortalInPortal;

    [Header("Real Env")]
    public Camera VRCamera;
    public GameObject realHand_Left;
    public GameObject realHand_Right;
    public GameObject realArm_Left;
    public GameObject realArm_Right;

    [Header("Portal Env")]
    public GameObject portalHand_Left;
    public GameObject portalHand_Right;
    public GameObject portal_origin_centerPivot = null;
    public GameObject portal_target_centerPivot = null;

    public GameObject portalPlaneCursor_left = null;
    public GameObject portalPlaneCursor_right = null;
    

    private Vector3 camera2OriginPortalPositionOffset = new Vector3(0,0,0);


    [SerializeField]
    public GameObject portalCamera;

    public void Awake()
     {
         instance = this;
        
     }

    public void StopPortalInteraction()
    {
        realHand_Left = GameObject.FindWithTag("LeftHand_real");
        realHand_Right = GameObject.FindWithTag("RightHand_real");

        realHand_Left.GetComponent<MyPortalHandHandler>().enabled = false;
        realHand_Right.GetComponent<MyPortalHandHandler>().enabled = false;
    }

    private void StartPortalInteraction()
    {
        // set real hand
        realHand_Left = GameObject.FindWithTag("LeftHand_real");
        realHand_Right = GameObject.FindWithTag("RightHand_real");

        // set HMD camera
        VRCamera = Camera.main;

        GameObject portal_origin = GameObject.FindWithTag("portalGate_origin");

        if (portal_origin == null) return;
        // set the attributes
        // hand
        portalHand_Left = GameObject.FindWithTag("LeftHand_portal");
        portalHand_Right = GameObject.FindWithTag("RightHand_portal");

        portalPlaneCursor_left = GameObject.Find("PortalPlainGrabPointer_left");
        portalPlaneCursor_right = GameObject.Find("PortalPlainGrabPointer_right");
        portalPlaneCursor_left.GetComponent<PortalGrabCursor>().DeactivateCursor();
        portalPlaneCursor_right.GetComponent<PortalGrabCursor>().DeactivateCursor();

        if(isAllowObjectTransportThroughPortal){
            transform.GetComponent<PortalObjectThrough>().enabled = true;
        }

        if(isAllowPortalPlainManipulation){
            transform.GetComponent<PortalPlainManipulation>().enabled = true;
        }

        // portal camera gameobject
        portalCamera = GameObject.FindWithTag("PortalCamera");

        // center pivots
        //Debug.Log(portalCamera.GetComponent<Camera>().fieldOfView);
        //portalCamera.GetComponent<Camera>().fieldOfView = 30f;
        //Debug.Log(portalCamera.GetComponent<Camera>().fieldOfView);
        portal_origin_centerPivot = GameObject.FindWithTag("portalGate_origin").transform.Find("PortalCircle_origin/Portal_Center_Pivot").gameObject;
        portal_target_centerPivot = GameObject.FindWithTag("portalGate_target").transform.Find("PortalCircle_target/Portal_Center_Pivot").gameObject;

        realHand_Left.GetComponent<MyPortalHandHandler>().enabled = true;
        realHand_Right.GetComponent<MyPortalHandHandler>().enabled = true;

        // set outline effect to the portalCamera
        Destroy(VRCamera.gameObject.GetComponent<OutlineEffect>());
        Invoke("AddOutlineEffectToPortalCamera", 0.1f);
    }

    private void AddOutlineEffectToMainCamera(){
        VRCamera.gameObject.AddComponent<OutlineEffect>();
        VRCamera.gameObject.GetComponent<OutlineEffect>().sourceCamera = VRCamera;
        VRCamera.gameObject.GetComponent<OutlineEffect>().lineThickness = 6.0f;
        VRCamera.gameObject.GetComponent<OutlineEffect>().lineIntensity = 10.0f;
        VRCamera.gameObject.GetComponent<OutlineEffect>().fillAmount = 0.0f;
        VRCamera.gameObject.GetComponent<OutlineEffect>().additiveRendering = true;
        VRCamera.gameObject.GetComponent<OutlineEffect>().cornerOutlines = true;
        VRCamera.gameObject.GetComponent<OutlineEffect>().scaleWithScreenSize = true;
    }

    private void AddOutlineEffectToPortalCamera(){
        portalCamera.AddComponent<OutlineEffect>();
        portalCamera.GetComponent<OutlineEffect>().sourceCamera = portalCamera.GetComponent<Camera>();
        portalCamera.GetComponent<OutlineEffect>().lineThickness = 6.0f;
        portalCamera.GetComponent<OutlineEffect>().lineIntensity = 10.0f;
        portalCamera.GetComponent<OutlineEffect>().fillAmount = 0.0f;
        portalCamera.GetComponent<OutlineEffect>().additiveRendering = true;
        portalCamera.GetComponent<OutlineEffect>().scaleWithScreenSize = false;
    }

    public void SetPortalHands()
    {
        portalHand_Left = GameObject.FindWithTag("LeftHand_portal");
        portalHand_Right = GameObject.FindWithTag("RightHand_portal");
    }

    private void DestroyPortalObjects(){
        if(isAllowObjectTransportThroughPortal){
            transform.GetComponent<PortalObjectThrough>().DestroyCopiedObjects();
        }

        if(isAllowPortalPlainManipulation){
            transform.GetComponent<PortalPlainManipulation>().DestoryCursorObject();
        }
        
        
    }

    public void ClosePortal()
    {
        Debug.Log("Close");
        Destroy(portalCamera.GetComponent<OutlineEffect>());
        DestroyPortalObjects();
        Invoke("AddOutlineEffectToMainCamera", 0.1f);
        
        if(isAllowObjectTransportThroughPortal){
            transform.GetComponent<PortalObjectThrough>().enabled = false;
        }

        if(isAllowPortalPlainManipulation){
            transform.GetComponent<PortalPlainManipulation>().enabled = false;
        }
        
        GameObject oldPortal_origin = GameObject.FindWithTag("portalGate_origin");
        GameObject oldPortal_target = GameObject.FindWithTag("portalGate_target");

        camera2OriginPortalPositionOffset = new Vector3(0,0,0);

        if (oldPortal_origin != null)
        {
            Destroy(oldPortal_origin);
            Destroy(oldPortal_target);
        }

        Selection_Technique cur_selection_type = Interaction_Setting.instance.cur_selection_type;

        if (cur_selection_type == Selection_Technique.DIRECT ||
           cur_selection_type == Selection_Technique.GOGO ||
           cur_selection_type == Selection_Technique.PORTAL
           ) { 
            if(realHand_Left.GetComponent<Custom_VR_Behaviour_Skeleton>().IsMainHand()){
                realHand_Left.GetComponent<Hand>().enabled = true;
                realHand_Left.GetComponent<MyLayHandler>().enabled = true;
            }

            if(realHand_Right.GetComponent<Custom_VR_Behaviour_Skeleton>().IsMainHand()){
                realHand_Right.GetComponent<Hand>().enabled = true;
                realHand_Right.GetComponent<MyLayHandler>().enabled = true;
            }
        }

        isPortalOpen = false;
        StopPortalInteraction();
    }

    public void UpdatePortalPosition(){
        Debug.Log("update");
        if(camera2OriginPortalPositionOffset == new Vector3(0,0,0)) return;


        Vector3 originalPosition = instanceOriginPortal.transform.position;
        Vector3 pivot = SteamVR_Render.Top().head.position;

        Vector3 offsetVector = originalPosition - pivot;

        offsetVector = Quaternion.Euler(SteamVR_Render.Top().head.rotation.eulerAngles) * offsetVector;

        Vector3 desiredRotation = offsetVector + SteamVR_Render.Top().head.position;

        instanceOriginPortal.transform.position = desiredRotation;
    }

    public void Update(){
        if(isPortalOpen){
        }
    }

    IEnumerator WaitASecond(Vector3 m_PointerPos, Vector3 handPos, bool m_HasPosition, bool isCalledFromPortalHand, GameObject hitObj)
    {
        //Print the time of when the function is first called.

        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(0.1f);
        OpenPortal(m_PointerPos, handPos, m_HasPosition, false, hitObj);
        
    }

    public void TransformPortal() {
        GameObject origin_portal = GameObject.FindWithTag("portalGate_origin");
        GameObject target_portal = GameObject.FindWithTag("portalGate_target");
        GameObject pointer = GameObject.FindWithTag("mousePointer");
        Navigation navigationScript = target_portal.GetComponent<Navigation>();

        navigationScript.portalManager = this;

        navigationScript.portalTechnique = portalTechnique;
        navigationScript.rotationTechnique = rotationTechnique;
        navigationScript.rotationController = rotationController;

        if (originRoom != null & targetRoom != null)
        {
            //reparent
            if (origin_portal.transform.parent == null)
            {
                originRoom.GetComponent<Room>().ReparentObject(origin_portal);
                originRoom.GetComponent<Room>().ReparentObject(pointer);
            }

            if (target_portal.transform.parent == null)
            {
                targetRoom.GetComponent<Room>().ReparentObject(target_portal);

                //transform obj
                originRoom.GetComponent<Room>().TransformObjTo(targetRoom.GetComponent<Room>(), target_portal);

                //set navigation
                Transform targetRoom_relative_transform = originRoom.GetComponent<Room>().TransformToRelative(targetRoom.transform);

                navigationScript.SetTransformScaleToRelative(targetRoom_relative_transform.localScale);
            }
        }
        
    }

    
    public void OpenPortal(Vector3 m_PointerPos, Vector3 handPos, bool m_HasPosition, bool isCalledFromPortalHand, GameObject hitObj)
    {
        // Check for valid position
        if (!m_HasPosition)
            return ;
        if(hitObj == null){
            if (isPortalOpen && !isCalledFromPortalHand)
            {
                ClosePortal();
                return ;
            }else if(isPortalOpen && isCalledFromPortalHand){
                ClosePortal();
                StartCoroutine(WaitASecond(m_PointerPos, handPos, m_HasPosition, false, null));
                return ;
            }

                Vector3 targetPosition = m_PointerPos;
                float targetPortalParameter = Experiment_Setting.instance.underArm_length_meter *0.25f;

                // create portal target
                Vector3 targetPortalPosition = targetPosition - Vector3.Normalize(targetPosition - handPos) * targetPortalParameter;
                Vector3 portalTargetPos = new Vector3(targetPortalPosition.x, targetPortalPosition.y, targetPortalPosition.z);

                Quaternion rotationOrigin2Target = Quaternion.LookRotation( handPos - portalTargetPos, Vector3.up);

                targetPortalInstance = Instantiate(targetPortal, portalTargetPos, rotationOrigin2Target);
                

                // open Portal in front of the user

                float originPortalParameter = Experiment_Setting.instance.underArm_length_meter *0.5f;
                // set the position and rotation offset for updating original portal's position
                camera2OriginPortalPositionOffset = SteamVR_Render.Top().head.transform.forward * originPortalParameter;
                Vector3 originPortalPosition = SteamVR_Render.Top().head.position + camera2OriginPortalPositionOffset;
                Vector3 portalOriginPos = new Vector3(originPortalPosition.x, CameraUtil.instance.GetChestPos().y, originPortalPosition.z);

                instanceOriginPortal = Instantiate(originPortal, portalOriginPos, rotationOrigin2Target);
                
                isPortalOpen = true;
                numPortalOpen++;
                StartPortalInteraction();
        }else{
            // Check if there is already created portal
            // if they are, remove old

            if (isPortalOpen && !isCalledFromPortalHand)
            {
                ClosePortal();
                return ;
            }else if(isPortalOpen && isCalledFromPortalHand){
                ClosePortal();
                StartCoroutine(WaitASecond(m_PointerPos, handPos, m_HasPosition, false, hitObj));
                return;
            }

            if(hitObj.tag == "PortalWall"){
            
                Vector3 targetPosition = m_PointerPos;

                Bounds hitObjBound= hitObj.GetComponent<MeshRenderer>().bounds;
             
                float boundSizeX = hitObjBound.size.x;
                // create portal target
                Vector3 targetPortalPosition = targetPosition + new Vector3(-1, 0, 0) * boundSizeX + new Vector3(-0.25f, 0, 0)  ;
                Vector3 portalTargetPos = new Vector3(targetPortalPosition.x, targetPortalPosition.y, targetPortalPosition.z);

                // Quaternion rotationOrigin2Target = Quaternion.LookRotation( SteamVR_Render.Top().head.position - portalTargetPos, Vector3.up);
                Quaternion rotationOrigin2Target = Quaternion.LookRotation( handPos - portalTargetPos, Vector3.up);

                targetPortalInstance = Instantiate(targetPortal, portalTargetPos, rotationOrigin2Target);
                

                // open Portal in front of the user

                float originPortalParameter = Experiment_Setting.instance.underArm_length_meter *0.5f;
                // set the position and rotation offset for updating original portal's position
                // camera2OriginPortalPositionOffset = Vector3.Normalize(targetPosition - SteamVR_Render.Top().head.position) * originPortalParameter;
                camera2OriginPortalPositionOffset = SteamVR_Render.Top().head.transform.forward * originPortalParameter;
                Vector3 originPortalPosition = SteamVR_Render.Top().head.position + camera2OriginPortalPositionOffset;
                Vector3 portalOriginPos = new Vector3(originPortalPosition.x, CameraUtil.instance.GetChestPos().y, originPortalPosition.z);

                instanceOriginPortal = Instantiate(originPortal, portalOriginPos, rotationOrigin2Target);

                isPortalOpen = true;
                numPortalOpen++;
                StartPortalInteraction();
            }else{
                
                Vector3 targetPosition = m_PointerPos;
                float targetPortalParameter = Experiment_Setting.instance.underArm_length_meter *0.25f;

                // create portal target
                Vector3 targetPortalPosition = targetPosition - Vector3.Normalize(targetPosition - handPos) * targetPortalParameter;
                Vector3 portalTargetPos = new Vector3(targetPortalPosition.x, targetPortalPosition.y, targetPortalPosition.z);

                Quaternion rotationOrigin2Target = Quaternion.LookRotation( handPos - portalTargetPos, Vector3.up);

                targetPortalInstance = Instantiate(targetPortal, portalTargetPos, rotationOrigin2Target);
                

                ////// open Portal in front of the user

                float originPortalParameter = Experiment_Setting.instance.underArm_length_meter *0.5f;
                // set the position and rotation offset for updating original portal's position
                camera2OriginPortalPositionOffset = SteamVR_Render.Top().head.transform.forward * originPortalParameter;
                Vector3 originPortalPosition = SteamVR_Render.Top().head.position + camera2OriginPortalPositionOffset;
                Vector3 portalOriginPos = new Vector3(originPortalPosition.x, CameraUtil.instance.GetChestPos().y, originPortalPosition.z);

                instanceOriginPortal = Instantiate(originPortal, portalOriginPos, rotationOrigin2Target);
                      
                isPortalOpen = true;
                numPortalOpen++;
                StartPortalInteraction();
            }
        }




        TransformPortal();
    }

    public int GetNumPortalOpen(){
        return numPortalOpen;
    }

    public void ResetNumPortalOpen(){
        numPortalOpen = 0;
    }

    public GameObject GetTargetPortal(){

        return targetPortalInstance;
    }
}
