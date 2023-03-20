using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MyLayHandler : MonoBehaviour
{
    [Header("Raycasting Interaction")]

    public MenuInputModule m_MenuInputModule;
    public Material m_TeleportMaterial;
    public Material m_HandMaterial;

    public GameObject m_Pointer;
    public GameObject m_PointerGameObject;
    public SteamVR_Action_Boolean m_TeleportAction;
    public SteamVR_Action_Boolean m_GrabAction;

    private SteamVR_Behaviour_Pose m_Pose = null;

    private bool m_HasPosition = false;
    private bool m_IsTeleporting = false;
    private float m_FadeTime = 0.5f;
    
    // lineRenderer
    private LineRenderer m_LineRenderer = null;

    // grab
    private Vector3 PreviousHandPosition;
    private FixedJoint m_Joint = null;
    private GameObject m_GrabObject = null;
    private Vector3 m_GrabObject_FirstPos;

    [Header("Teleport Line Renderer Variable")]
    public float speed;
    public Vector3 velocity;
    public float angle;
    public int resolution;
    float g ;
    float radianAngle;

    [Header("Interaction Setting")]
    [SerializeField]
    private Interaction_Setting interaction_setting;
    private Selection_Technique cur_selection_type; 
    private Task_Type cur_task;
    public GameObject portalManager;

    private bool isRayCastingSelectObj = false;
    private float distanceFromHandToObj = 0.0f;
    private Vector3 handPositionAtHOMERStart;

    public void OnDisable()
    {
        
        m_Pointer.SetActive(false);
        m_LineRenderer.enabled = false;
    }

    void OnEnable()
    {
        m_Pose = GetComponent<SteamVR_Behaviour_Pose>();
        m_Joint = GetComponent<FixedJoint>();
        PreviousHandPosition = transform.position;

        if(m_Pointer == null){
            m_Pointer = Instantiate(m_PointerGameObject, Vector3.zero, Quaternion.identity);
        }

        if (interaction_setting == null)
        {
            interaction_setting = Interaction_Setting.instance; 
        }

        cur_selection_type = interaction_setting.cur_selection_type;
        cur_task = Experiment_Setting.instance.task_type;

        // direct selection is only allowed to the "Direct"
        if (cur_selection_type == Selection_Technique.DIRECT ||
            cur_selection_type == Selection_Technique.GOGO ||
            cur_selection_type == Selection_Technique.PORTAL
            )
        {
            gameObject.GetComponent<Hand>().enabled = true;

        }
        else
        {
            gameObject.GetComponent<Hand>().enabled = false;

        }

        m_LineRenderer = GetComponent<LineRenderer>();
    }

    private void UpdateLineRendererLength()
    {
        m_LineRenderer.SetVertexCount(2);

        m_LineRenderer.SetPosition(0, transform.position);
        m_LineRenderer.SetPosition(1, m_Pointer.transform.position);
    }

    private void UpdateLineRendererTeleport()
    {
        // temp
        Vector3 temp = new Vector3(transform.position.x, 0, transform.position.z);
        
        resolution = 100;
        speed = 10f;
        velocity = speed * transform.forward;
        
        m_LineRenderer.SetVertexCount(resolution +1);

        Vector3[] lineRendererArcPosition = ProjectileCurve.instance.CalculateArcArray(velocity, resolution, transform);
        Vector3[] validLineRendererArcPosition = new Vector3[resolution+1];

        for(int i=0; i< lineRendererArcPosition.Length; i++){

            Ray ray = new Ray(transform.position, lineRendererArcPosition[i] - transform.position);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit, Vector3.Distance(transform.position,lineRendererArcPosition[i]), ~(1<<8)/* mousepointer layer*/))
            {
                
                if(hit.transform.gameObject.name.Contains("Wall")){
                    for(int j=i; j< lineRendererArcPosition.Length; j++){
                         validLineRendererArcPosition[j] = lineRendererArcPosition[i-1];
                     }

                    break;
                }else{
                    
                }
            }

            validLineRendererArcPosition[i] = lineRendererArcPosition[i];
        }

        m_LineRenderer.SetPositions(validLineRendererArcPosition);
        ProjectileCurve.instance.UpdateTeleportTargetPos(validLineRendererArcPosition[validLineRendererArcPosition.Length-1], validLineRendererArcPosition[validLineRendererArcPosition.Length-2]);
    }

    private void LineRendererReset()
    {
        m_LineRenderer.SetVertexCount(2);
    }

    void Update_Lay_DirectMode()
    {
        UpdateLineRendererTeleport();

        // Teleport
        if (m_GrabAction.GetStateUp(m_Pose.inputSource))
        {
            TryTeleport();
        }
            
        PreviousHandPosition = transform.position;
    }

    void Update_Lay_LaycastingMode()
    {
        
        //Pointer
        m_HasPosition = UpdatePointer();
        m_Pointer.SetActive(m_HasPosition);

        if (m_HasPosition)
            UpdateLineRendererLength();

        // Grab Down
        if (m_GrabAction.GetStateDown(m_Pose.inputSource))
        {
            if(cur_task == Task_Type.DOCKING_TASK)
            {
                // move the hand object to the object
                // 1. create a copy
                // 2. hide the user hand
                // 3. move the copy to the object
                //Debug.Log("raycasting");
                Pickup_LaycastingMode();
                DBTriggerTracker.instance.SaveTriggerInfo();
            }else if(cur_task == Task_Type.SELECTION_TASK)
            {
                // do nothing
                  if (m_GrabObject.GetComponent<SelectionTargetController>() != null){

                    bool isTarget = m_GrabObject.GetComponent<SelectionTargetController>().getIsTarget();

                    if (isTarget) 
                    {
                        m_GrabObject.GetComponent<SelectionTargetController>().setIsClicked(true);
                    }
                }

                if( m_GrabObject.GetComponent<StartNewTrial>()!=null){
                    // when object is ReadyObject
                    m_GrabObject.GetComponent<StartNewTrial>().TriggerStartNewTrial();
                }

                if( m_GrabObject.GetComponent<SelectionCenterController>() != null){
                    m_GrabObject.GetComponent<SelectionCenterController>().setIsWaiting(false);
                }
            }
        }

        // Grab Up
        // if (m_TeleportAction.GetState(m_Pose.inputSource) && m_GrabAction.GetStateUp(m_Pose.inputSource))
        if (m_GrabAction.GetStateUp(m_Pose.inputSource))
        {
            //Debug.Log(m_Pose.inputSource + " Trigger Up");
            if (cur_task == Task_Type.DOCKING_TASK)
            {
                // move back the copy hand
                // 1. move back the copy hand to the user
                // 2. remove the copy
                // 3. show the user hand again
                // Debug.Log("drop laycasting");
                
                Drop_LaycastingMode();
            }
            else if (cur_task == Task_Type.SELECTION_TASK)
            {
               
            }
        }

        PreviousHandPosition = transform.position;
    }

    void Update_Lay_MenuSelectionMode()
    {
        //Pointer
        m_HasPosition = UpdatePointer();
        m_Pointer.SetActive(m_HasPosition);

        if (m_HasPosition)
            UpdateLineRendererLength();

        PreviousHandPosition = transform.position;
    }

    void Update_Lay_PortalSelectionMode()
    {
        //Pointer
        m_HasPosition = UpdatePointer();
        m_Pointer.SetActive(m_HasPosition);

        if (m_HasPosition)
            UpdateLineRendererLength();

        // Grab Down
        if (m_GrabAction.GetStateDown(m_Pose.inputSource))
        {
        }

        // Grab Up
        if (m_GrabAction.GetStateUp(m_Pose.inputSource) )
        {
            // open portal
            PortalManager.instance.OpenPortal( m_Pointer.transform.position, 
                                                transform.position,
                                                m_HasPosition, 
                                                gameObject.GetComponent<Custom_VR_Behaviour_Skeleton>().IsPortalHand(),
                                                m_GrabObject
                                                );
        }
    }

    void Update_Lay()
    {
        switch (cur_selection_type)
        {
            case Selection_Technique.DIRECT:
                Update_Lay_DirectMode();
                break;
            case Selection_Technique.GOGO:
                // do nothing
                break;
            case Selection_Technique.RAYCASTING:
                Update_Lay_LaycastingMode();
                break;
            case Selection_Technique.IN_MENU_STATE:
                // do nothing yet
                break;
            case Selection_Technique.PORTAL:
                Update_Lay_PortalSelectionMode();
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(cur_selection_type == Selection_Technique.RAYCASTING){
            // drop the object
            if (cur_task == Task_Type.DOCKING_TASK)
            {
                if(isRayCastingSelectObj){
                    // don't draw lay;
                    // make lay follow the hand or remove lay
                    m_Pointer.SetActive(false);
                    m_LineRenderer.enabled = false;

                     if (m_GrabAction.GetStateUp(m_Pose.inputSource)){
                         Drop_LaycastingMode();
                     }
                }else{
                    m_Pointer.SetActive(true);
                    m_LineRenderer.enabled = true;
                    Update_Lay();
                }
            }
            else if (cur_task == Task_Type.SELECTION_TASK)
            {
                m_Pointer.SetActive(true);
                m_LineRenderer.enabled = true;
                Update_Lay();
            }
        }else {
            if (
                (cur_selection_type == Selection_Technique.DIRECT ||
                cur_selection_type == Selection_Technique.PORTAL)
                && m_LineRenderer!=null
                && m_TeleportAction.GetState(m_Pose.inputSource))
            {
                m_Pointer.SetActive(true);
                m_LineRenderer.enabled = true;

                Update_Lay();
            }
            else
            {
                if(!transform.GetComponent<Custom_VR_Behaviour_Skeleton>().IsPortalHand() ){
                    ProjectileCurve.instance.HideTeleportTarget();
                    turnLayOff();
                }else{
                    turnLayOff();

                }
            }
        }
    }

    public ArrayList MoveHandToDistantObject(){
        ArrayList arr = new ArrayList();

        if(isRayCastingSelectObj){

            arr.Add(m_GrabObject);
            arr.Add(distanceFromHandToObj);
            arr.Add(handPositionAtHOMERStart);
            arr.Add(m_GrabObject_FirstPos);
            
            return arr;
        }else{
            arr.Add(null);
        }
        
        return arr;
    }

    public void turnLayOn(){
        m_Pointer.SetActive(true);
        m_LineRenderer.enabled = true;
    }

    public void turnLayOff(){

        LineRendererReset();
        m_Pointer.SetActive(false);
        m_LineRenderer.enabled = false;
    }

    public void Pickup_LaycastingMode()
    {
        if(m_GrabObject!= null){
            if(m_GrabObject.GetComponent<StartNewTrial>() != null){
                m_GrabObject.GetComponent<StartNewTrial>().TriggerStartNewTrial();
            }

            if( m_GrabObject.GetComponent<SelectionCenterController>() != null){
                    m_GrabObject.GetComponent<SelectionCenterController>().setIsWaiting(false);
            }

            if(m_GrabObject.GetComponent<DockingTaskObject>() != null){
                isRayCastingSelectObj = true;
                Vector3 bodyPos = CameraUtil.instance.GetChestPos();
                m_GrabObject.GetComponent<DockingTaskObject>().setIsClicked(true);


                Vector3 targetCenter = m_GrabObject.GetComponent<DockingTaskObject>().getCenterCoordinate();
                distanceFromHandToObj = Vector3.Distance(targetCenter, bodyPos);
                handPositionAtHOMERStart = transform.GetComponent<Custom_VR_Behaviour_Skeleton>().GetCurControllerPos();
                m_GrabObject_FirstPos = m_Pointer.transform.position;

                turnLayOff();
            }
        }
    }

    public void Drop_LaycastingMode()
    {
        isRayCastingSelectObj = false;
        distanceFromHandToObj = 0.0f;

        turnLayOn();
        // detach
        m_Joint.connectedBody = null;

        // clear
        m_GrabObject = null;
    }

    private void TryTeleport()
    {
    
        // Get camera rig, and head position
        Transform cameraRig = SteamVR_Render.Top().origin;
        Vector3 headPosition = SteamVR_Render.Top().head.position;

        // Figure out translation
        Vector3 groundPosition = new Vector3(headPosition.x, cameraRig.position.y, headPosition.z);
        Vector3 translateVector =  ProjectileCurve.instance.GetTeleportTargetPos()- groundPosition;
       
        // Move
        StartCoroutine(MoveRig(cameraRig, translateVector));
        TeleportCounter.instance.IncrementNumTeleport();
    }

    private IEnumerator MoveRig(Transform cameraRig, Vector3 translation)
    {
        // Flag 
        m_IsTeleporting = true;

        //Fade to black
        SteamVR_Fade.Start(Color.black, m_FadeTime, true);

        // Apply translation
        yield return new WaitForSeconds(m_FadeTime);
        cameraRig.position = new Vector3(cameraRig.position.x + translation.x, cameraRig.position.y, cameraRig.position.z + translation.z);
       
        //Fade to clear
        SteamVR_Fade.Start(Color.clear, m_FadeTime, true);

        // De-flag
        m_IsTeleporting = false;

    }

    private bool UpdatePointer()
    {
        // Ray from the controller
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if(m_GrabObject!=null && m_GrabObject.GetComponent<SelectionTargetController>()!=null){
            m_GrabObject.GetComponent<SelectionTargetController>().TurnTargetIsNotContacting();
        }


        // If it's a hit
        if(Physics.Raycast(ray, out hit, Mathf.Infinity, ~(1<<8)/* mousepointer layer*/))
        {
            if (hit.transform.tag == "MovableObject")
            {
                m_Pointer.GetComponent<MeshRenderer>().material = m_HandMaterial;
                m_GrabObject = hit.transform.gameObject;
            }else if (hit.transform.tag == "SelectionTarget")
            {
                m_Pointer.GetComponent<MeshRenderer>().material = m_HandMaterial;
                m_GrabObject = hit.transform.gameObject;
                m_GrabObject.GetComponent<SelectionTargetController>().TurnTargetIsContacting();
            }
            else if (hit.transform.tag == "ReadyObject")
            {
                m_Pointer.GetComponent<MeshRenderer>().material = m_HandMaterial;
                m_GrabObject = hit.transform.gameObject;
            }else if (hit.transform.tag == "PortalWall")
            {
                m_Pointer.GetComponent<MeshRenderer>().material = m_HandMaterial;
                m_GrabObject = hit.transform.gameObject;
            }
            else{
                m_Pointer.GetComponent<MeshRenderer>().material = m_TeleportMaterial;
                m_GrabObject = hit.transform.gameObject;
            }

            m_Pointer.transform.position = hit.point;
            return true;
        }

        // If not a hit
        return false;
    }

    private Vector3 CalculateEnd()
    {
        return new Vector3(0, 0, 0);
    }
}
