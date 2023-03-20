using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Valve.VR;

public class Hand : MonoBehaviour
{
    public SteamVR_Action_Boolean m_GrabAction = null;
    private SteamVR_Behaviour_Pose m_Pose = null;
    private FixedJoint m_Joint = null;

    private Interactable m_CurrentInteractable = null;
    public List<Interactable> m_ContactInteractables = new List<Interactable>();
    private float grabbaleDistance = 0.5f;

    private Interaction_Setting interaction_setting;
    private Task_Type cur_task;

    private bool hasItemInHand = false;

    private void OnEnable()
    {
        if (interaction_setting == null)
        {
            interaction_setting = Interaction_Setting.instance;  //GameObject.Find("Experiment_Setting").GetComponent<Interaction_Setting>();
        }
        m_ContactInteractables.Clear();
        cur_task = Experiment_Setting.instance.task_type;

        m_Pose = GetComponent<SteamVR_Behaviour_Pose>();
        m_Joint = GetComponent<FixedJoint>();
    }

    //set and get for hasItemInhand

    public bool IsGrabbing(){
        return m_GrabAction.GetState(m_Pose.inputSource);
    }

    public bool IsGrabTriggered(){
        return m_GrabAction.GetStateDown(m_Pose.inputSource);
    }

    public bool IsGrabReleased(){
        return m_GrabAction.GetStateUp(m_Pose.inputSource);
    }

    private void GrabActionForCommon()
    {
        // Down
        if (m_GrabAction.GetStateDown(m_Pose.inputSource))
        {
            Pickup();
        }

        //Up
        if (m_GrabAction.GetStateUp(m_Pose.inputSource))
        {
            Drop();
        }
    }

    public void GrabActionForDockingTask(bool giveAShort = false)
    {
        if (m_GrabAction.GetStateDown(m_Pose.inputSource) || giveAShort)
        {
           
            // set the DockingTaskObject isClicked true

            // Get nearest
            m_CurrentInteractable = GetNearestInteractable();

            // Null check
            if (!m_CurrentInteractable) return;

            

            
            if( m_CurrentInteractable.GetComponent<DockingTaskObject>() != null)
            {
                // check is the object having DockingTaskObject script
                bool isTarget = m_CurrentInteractable.GetComponent<DockingTaskObject>().getIsTarget();

                if (isTarget)
                {
                    // do nothing
                }
                else
                {
                    // it means the current Object is the object the participant should control 
                    m_CurrentInteractable.GetComponent<DockingTaskObject>().setIsClicked(true);
                    // pickup
                    
                    Pickup();
                    if (DBTriggerTracker.instance != null)
                    {
                        DBTriggerTracker.instance.SaveTriggerInfo();
                    }
                }
            }else{
                Pickup();
            }

            if( m_CurrentInteractable.GetComponent<StartNewTrial>() != null){
                m_CurrentInteractable.GetComponent<StartNewTrial>().TriggerStartNewTrial();
            }

            if( m_CurrentInteractable.GetComponent<SelectionCenterController>() != null){
                m_CurrentInteractable.GetComponent<SelectionCenterController>().setIsWaiting(false);
            }
        }

        //Up
        if (m_GrabAction.GetStateUp(m_Pose.inputSource))
        {
            m_CurrentInteractable = GetNearestInteractable();
            DockingDrop();
        }
    }

    private void GrabActionForSelectionTask()
    {
        // Down
        if (m_GrabAction.GetStateDown(m_Pose.inputSource))
        {
            // Get nearest
            m_CurrentInteractable = GetNearestInteractable();

            // Null check
            if (!m_CurrentInteractable) return;

            if (m_CurrentInteractable.GetComponent<SelectionTargetController>() != null){
                // when object is selectable object

                bool isTarget = m_CurrentInteractable.GetComponent<SelectionTargetController>().getIsTarget();

                if (isTarget)
                {
                    m_CurrentInteractable.GetComponent<SelectionTargetController>().setIsClicked(true);
                }
            }
            
            if( m_CurrentInteractable.GetComponent<StartNewTrial>()!=null){
                // when object is ReadyObject
                m_CurrentInteractable.GetComponent<StartNewTrial>().TriggerStartNewTrial();
            }

            if( m_CurrentInteractable.GetComponent<SelectionCenterController>() != null){
                m_CurrentInteractable.GetComponent<SelectionCenterController>().setIsWaiting(false);
            }
        }
    }

    public void ClearContactInteractables(){
        m_ContactInteractables.Clear();
    }

    // Update is called once per frame
    void Update()
    {
        if(cur_task == Task_Type.SELECTION_TASK)
        {
            GrabActionForSelectionTask();


        }else if(cur_task == Task_Type.DOCKING_TASK)
        {
            GrabActionForDockingTask();
        }
        else
        {
            // default
            GrabActionForCommon();
        }
    }

    public void AddContactInteractables(GameObject obj){
        if (obj != null)
        {
            m_ContactInteractables.Add(obj.GetComponent<Interactable>());
        }
    }

    public void RemoveContactInteractables(GameObject obj){
        while (m_ContactInteractables.Contains(obj.GetComponent<Interactable>()))
        {
            m_ContactInteractables.Remove(obj.GetComponent<Interactable>());
        }
    }


    public void OnTriggerEnter(Collider other)
    {
        if (!(other.gameObject.CompareTag("MovableObject") || other.gameObject.CompareTag("SelectionTarget") || other.gameObject.CompareTag("ReadyObject") || other.gameObject.CompareTag("PortalPlaneCursor"))){
            return;
        }

        m_ContactInteractables.Add(other.gameObject.GetComponent<Interactable>());

        foreach(Interactable interactable in m_ContactInteractables)
        {            
            if(interactable == other.gameObject.GetComponent<Interactable>())
                continue;

            if(interactable == null){
                
                RemoveContactInteractables(interactable.gameObject);
            }else{
                if(interactable.GetComponent<SelectionTargetController>()!=null){
                    interactable.GetComponent<SelectionTargetController>().TurnTargetIsNotContacting();
                }
            }
        }

        Interactable nearest = GetNearestInteractable();

        if(nearest!=null && nearest.GetComponent<SelectionTargetController>()!=null){
            nearest.GetComponent<SelectionTargetController>().TurnTargetIsContacting();
        }
    }

    public void OnTriggerStay(Collider other){
        if (!(other.gameObject.CompareTag("MovableObject") || other.gameObject.CompareTag("SelectionTarget") || other.gameObject.CompareTag("ReadyObject") || other.gameObject.CompareTag("PortalPlaneCursor"))) {
            return;
        }

        bool isInContactableList = false;

        foreach(Interactable interactable in m_ContactInteractables)
        {
            if(interactable == other.gameObject.GetComponent<Interactable>()){
                isInContactableList = true;
                break;
            }
        }

        if(!isInContactableList){
            m_ContactInteractables.Add(other.gameObject.GetComponent<Interactable>());
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (!(other.gameObject.CompareTag("MovableObject") || other.gameObject.CompareTag("SelectionTarget")|| other.gameObject.CompareTag("ReadyObject") || other.gameObject.CompareTag("PortalPlaneCursor"))) return;

        m_ContactInteractables.Remove(other.gameObject.GetComponent<Interactable>());

        if(other.gameObject.CompareTag("SelectionTarget")){
            other.gameObject.GetComponent<SelectionTargetController>().TurnTargetIsNotContacting();
        }
        
        Interactable nearest = GetNearestInteractable();

        if(nearest!=null && nearest.GetComponent<SelectionTargetController>()!=null){
            nearest.GetComponent<SelectionTargetController>().TurnTargetIsContacting();
        }

    }

    public void Pickup()
    {
        // Get nearest
        m_CurrentInteractable = GetNearestInteractable();


        // Null check
        if (!m_CurrentInteractable) return;

       

        if(m_CurrentInteractable.GetComponent<PortalGrabCursor>() != null){
            m_CurrentInteractable.GetComponent<PortalGrabCursor>().SetIsGrabbed(true);
        }

        // position
        //m_CurrentInteractable.transform.position = transform.position;
        //attach
        Rigidbody targetBody = m_CurrentInteractable.GetComponent<Rigidbody>();
       

        m_Joint.connectedBody = targetBody;
        //set active hand
        m_CurrentInteractable.m_ActiveHand = this;
    }

    public void DockingDrop()
    {
        // Null check
        if (!m_CurrentInteractable) return;


        if(m_CurrentInteractable.gameObject.GetComponent<PortalGrabCursor>()!=null){
            m_CurrentInteractable.gameObject.GetComponent<PortalGrabCursor>().SetIsGrabbed(false);
        }

        Rigidbody targetBody = m_CurrentInteractable.GetComponent<Rigidbody>();
        targetBody.velocity = new Vector3(0,0,0);
        targetBody.angularVelocity = new Vector3(0,0,0);

        // detach
        m_Joint.connectedBody = null;

        // clear
        m_CurrentInteractable.m_ActiveHand = null;
        m_CurrentInteractable = null;
    }

    public void Drop()
    {
        // Null check
        if (!m_CurrentInteractable) return;

        // apply velocity
        Rigidbody targetBody = m_CurrentInteractable.GetComponent<Rigidbody>();

        // detach
        m_Joint.connectedBody = null;

        // clear
        m_CurrentInteractable.m_ActiveHand = null;
        m_CurrentInteractable = null;
    }

    private Interactable GetNearestInteractable() {
        Interactable nearest = null;

        float minDistance = grabbaleDistance;
        float distance = 0.0f;

        foreach(Interactable interactable in m_ContactInteractables)
        {
            distance = Vector3.Distance(interactable.transform.position, transform.position);

            if(distance < minDistance)
            {
                minDistance = distance;
                nearest = interactable;
            }
        }

        return nearest;
    }
}
