using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class DockingExperimentGenerator : MonoBehaviour
{
    public static DockingExperimentGenerator instance;
  
    private int numberOfTriggerAction;
    private int wrongClick;
    private float firstGrabTime;

    private float dockingTimeStamps_start;
    private float dockingTimeStamps_grab;
    private float dockingTimeStamps_docking;

    private float dockingTimeStamps_1;

    private string dockingTimeStamps_date_1;
    private int dockingTimeStampIdx;
    private float dockingTimes;
    private float dockingErrorRates;


    private float trialTimer;
    private int curIdx = 0;
    public int totalDockingTrials;
    private bool stopTriggerCalled = false;
    private bool isInTrial;

    public GameObject OriginPos;
    public GameObject taskContainer;
    public GameObject controlObject;

    public bool returnOriginAfterTrial;

    public void IncreaseNumberOfTrigger(){
        numberOfTriggerAction+=1;
    }

    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        initTrial();
    }

    private void SetTargetDistance(){
        Target_Distance dist = Interaction_Setting.instance.cur_target_distance;

        transform.position = OriginPos.transform.position;

        float randomRotY = Random.Range(30.0f, 60.0f);

        float posX = 0.0f;
        // approximate chest position
        float posY = SteamVR_Render.Top().head.position.y - 0.3f;
        float posZ = 0.0f; 

        switch (dist)
        {
            case Target_Distance.FAR:
                posX = -1 * Experiment_Setting.instance.far_distance;
                transform.localPosition = new Vector3(posX, posY, posZ);
                break;

            case Target_Distance.MID:
                posX =  -1 * Experiment_Setting.instance.middle_distance;
                transform.localPosition = new Vector3(posX, posY, posZ);
                break;

            case Target_Distance.NEAR:
                posX =  -1 * Experiment_Setting.instance.near_distance;
                transform.localPosition = new Vector3(posX , posY, posZ);
                break;
            case Target_Distance.ARM_REACH:
                posX = -1 * Experiment_Setting.instance.arm_reach_distance; 

                transform.localPosition = new Vector3(posX, posY, posZ);
                break;
        }
    }
    
    private void initDockingTimes(){
       

        dockingTimes = 0.0f;
        firstGrabTime = 0.0f;
        dockingTimeStamps_1  = 0.0f;
        dockingTimeStamps_date_1  ="";

        dockingTimeStampIdx = 0;

    }

    private void initDockingErrorRates(){

        dockingErrorRates = 0.0f;
        wrongClick = 0;
        numberOfTriggerAction = 0;
    }

    public void initTrial()
    {
        curIdx = 0;
       
        isInTrial = false;
        stopTriggerCalled = false;
        
        initDockingTimes();
        initDockingErrorRates();

        ResetTimer();
        SetTargetDistance();
        dockingTimeStamps_start = Util.instance.GetTimeStampInMS();
    }

    public void CallNextDocking(){
        dockingTimes = trialTimer;
        dockingErrorRates = taskContainer.GetComponent<InitDockingTask>().CalculateAccuracy();
        controlObject.GetComponent<DockingTaskObject>().setIsClicked(false);

        recordTrialResult();
        dockingTimeStampIdx = 0;
        curIdx++;

        if(curIdx > totalDockingTrials-1){
            StopTrial();
            return;
        }

        if(Interaction_Setting.instance.cur_selection_type == Selection_Technique.PORTAL){
            PortalManager.instance.ClosePortal();
        }

        if(Interaction_Setting.instance.cur_selection_type == Selection_Technique.DIRECT){
           Util.instance.returnUserOrigin(new Vector3(0,0,0));
        }

        isInTrial = false;
        stopTriggerCalled = false;

        taskContainer.SetActive(false);
        Invoke("TrialRestart", 0.3f);
        ResetTimer();
    }

    private void TrialRestart(){
        taskContainer.SetActive(true);
    }

    public void StopTrial()
    {
        isInTrial = false;

        if(Interaction_Setting.instance.cur_selection_type == Selection_Technique.PORTAL){
            PortalManager.instance.ResetNumPortalOpen();
            PortalManager.instance.ClosePortal();
        }

        if(Interaction_Setting.instance.cur_selection_type == Selection_Technique.DIRECT){
            TeleportCounter.instance.ResetNumTeleport();
            
        }

        // obj_startNewTrial.SetActive(true);
        if (returnOriginAfterTrial){
           Util.instance.returnUserOrigin(new Vector3(0,0,0)); 
        }

        if(DockingTaskOrderManager.instance.DoSeeThroughDockingTask){
            GameObject.FindWithTag("PortalWall").SetActive(false);
        }
        
        gameObject.SetActive(false);
        // set next trial
        Interaction_Setting.instance.setNextTrial();
    }
    
    void Update()
    {
        if (isInTrial)
        {
            // // update the independent values
            trialTimer += Time.deltaTime;
            

            // // 3.wait until a participant select the other side button click 
            // // & target and control objects are collided
            if (stopTriggerCalled)
            {
                dockingTimeStamps_docking = Util.instance.GetTimeStampInMS();
             
                CallNextDocking();
            }
        }
        else
        {
            //1. check whether the participant is starting to control the control object to target
            bool isClicked = controlObject.GetComponent<DockingTaskObject>().getIsClicked();
            // Debug.Log(isClicked);
            if (isClicked)
            {
                firstGrabTime = trialTimer;


                dockingTimeStamps_grab = Util.instance.GetTimeStampInMS();

                ResetTimer();
                isInTrial = true;
            }
            else
            {
                trialTimer += Time.deltaTime;
            }
        }
    }


    private void ResetTimer()
    {   
        trialTimer = 0.0f;
    }

    private void recordTrialResult()
    {
        // record trialTime and trialAccuracy

        // save
        DBManager.instance.recordTrial_docking(firstGrabTime, 
                                                dockingTimes, 
                                                dockingErrorRates, 
                                                wrongClick, 
                                                dockingTimeStamps_start,
                                                dockingTimeStamps_grab,
                                                dockingTimeStamps_docking,
                                                numberOfTriggerAction
                                                );        
    }

    public bool getIsInTrial()
    {
        return isInTrial;
    }

    public void setIsInTrialTrue()
    {
        isInTrial = true;
    }

    public void setStopstopTriggerCalledTrue()
    {
        if(controlObject.GetComponent<DockingTaskObject>().getIsSatisfyOffset()){
            stopTriggerCalled = true;
        }else{
            wrongClick+=1;
        }
    }
}
