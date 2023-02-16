using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
// add the UXF namespace
// using UXF;

public class SelectionExperimentGenerator : MonoBehaviour
{
    public GameObject targetContainer;
    public GameObject centerObj;

    private float firstGrabTime;

    private float[] selectionTimeStamps;
    private string[] selectionTimeStamps_date;
    private int selectionTimeStampIdx = 0;
    private float[] selectionTimes;
    private bool[] isTargetSelected;
    private int[] selectionOrder;
    private float[] selectionCounts;

    private GameObject[] targets;
    private int curIndex;
    private GameObject curTarget;
    private GameObject mainHand;

    private float trialTimer = 0.0f;
    private int trialCount = 0;

    

    private bool isInTrial = false;

    public void waitSelectionTask()
    {
        centerObj.GetComponent<SelectionCenterController>().setIsWaiting(true);
    }

    public void runSelectionTask()
    {
        centerObj.GetComponent<SelectionCenterController>().setIsWaiting(false);
        StartTrial();
    }

    private void ResetTimer()
    {
        trialTimer = 0.0f;
    }

    private void recordTrialResult()
    {
        // call DB save function();
       
        DBManager.instance.recordTrial(firstGrabTime, selectionTimes, selectionCounts, selectionTimeStamps, selectionTimeStamps_date);
    }

    private void initIsTargetSelected(){
        isTargetSelected = new bool[targets.Length];

        for(int idx=0;idx<targets.Length; idx++){
            isTargetSelected[idx] = false;
        }
    }

    private void initTimeStamps(){
        int tempLength = targetContainer.GetComponent<InitSelectionTask>().GetFittsLawObjects().Length + 2;

        selectionTimeStamps = new float[tempLength];
        selectionTimeStamps_date = new string[tempLength];
        selectionTimeStampIdx = 0;

        for(int idx=0;idx<tempLength; idx++){
            selectionTimeStamps[idx] = 0.0f;
            selectionTimeStamps_date[idx] = "";

        }
    }

    private void initselectionTimes(){
        selectionTimes = new float[targets.Length];
        

        for(int idx=0;idx<targets.Length; idx++){
            selectionTimes[idx] = 0.0f;
        }

        
    }

    private void initselectionCounts(){
        selectionCounts = new float[targets.Length];

        for(int idx=0;idx<targets.Length; idx++){
            selectionCounts[idx] = 0;
        }
    }


    private void OnEnable()
    {
        SetTargetLocation();
        if(!targetContainer.GetComponent<InitSelectionTask>().enabled){
            targetContainer.GetComponent<InitSelectionTask>().enabled = true;
        }

        waitSelectionTask();

        if(Experiment_Setting.instance.user_main_hand == Main_Hand.RIGHT){
            mainHand =  GameObject.FindGameObjectsWithTag("RightHand_real")[0];
        }

        if(Experiment_Setting.instance.user_main_hand == Main_Hand.LEFT){
            mainHand =  GameObject.FindGameObjectsWithTag("LeftHand_real")[0];
        }
    }

    private void SetTargetLocation()
    {
        float posX = 0.0f;
        float posY = SteamVR_Render.Top().head.position.y; //Camera.main.transform.position.y / 2;
        float posZ =  0.0f; //SteamVR_Render.Top().head.position.z;
        Target_Distance dist = Interaction_Setting.instance.cur_target_distance;

        switch (dist)
        {
            case Target_Distance.FAR:
                posX = -1 * Experiment_Setting.instance.far_distance;

                transform.localPosition = new Vector3(posX, posY, posZ);
                break;

            case Target_Distance.MID:
                posX = -1 * Experiment_Setting.instance.middle_distance;

                transform.localPosition = new Vector3(posX, posY, posZ);
                break;

            case Target_Distance.NEAR:
                posX = -1 * Experiment_Setting.instance.near_distance; 

                transform.localPosition = new Vector3(posX, posY, posZ);
                break;

            case Target_Distance.ARM_REACH:
                posX = -1 * Experiment_Setting.instance.arm_reach_distance; 

                transform.localPosition = new Vector3(posX, posY, posZ);
                break;
        }
    }

    public void Update()
    {
        if (isInTrial)
        {
            // update the independent values
            trialTimer += Time.deltaTime;

            if(mainHand.GetComponent<Hand>().IsGrabTriggered()){
                selectionCounts[curIndex]++;
            }

            // 3.wait until a participant select the target
            if (curTarget.GetComponent<SelectionTargetController>().getIsClicked())
            {
                selectionTimeStamps[selectionTimeStampIdx] = Util.instance.GetTimeStampInMS();
                selectionTimeStamps_date[selectionTimeStampIdx++] = Util.instance.GetTimeStamp();
                CallNextSelection();
            }
        }
        else
        {
            //1. check whether the center is waiting user or not
            bool isCenterWaiting = centerObj.GetComponent<SelectionCenterController>().getIsWaiting();

            if (isCenterWaiting)
            {
                // record the first grab
                trialTimer += Time.deltaTime;

                if(selectionTimeStampIdx == 0){
                    initTimeStamps();

                    selectionTimeStamps[selectionTimeStampIdx] = Util.instance.GetTimeStampInMS();
                    selectionTimeStamps_date[selectionTimeStampIdx++] = Util.instance.GetTimeStamp();
                }
            }
            else
            {
                //2. if center obj is not waiting start trial
                firstGrabTime = trialTimer;
                
                StartTrial();
                selectionTimeStamps[selectionTimeStampIdx] = Util.instance.GetTimeStampInMS();
                selectionTimeStamps_date[selectionTimeStampIdx++] = Util.instance.GetTimeStamp();
            }
        }
    }

    public void StartTrial()
    {        
        selectionOrder = targetContainer.GetComponent<InitSelectionTask>().GetSelectionOrder();
        targets = targetContainer.GetComponent<InitSelectionTask>().GetFittsLawObjects();
        initIsTargetSelected();
        initselectionCounts();
        initselectionTimes();

        curIndex = 0;
        curTarget = targets[selectionOrder[curIndex]];
        curTarget.GetComponent<SelectionTargetController>().setIsTarget(true);
        isInTrial = true;

        // start Timer
        ResetTimer();
    }

    private void CallNextSelection(){
        // update the isTargetSelected and timer
        isTargetSelected[curIndex] = true;
        selectionTimes[curIndex] = trialTimer;
        
        curTarget.GetComponent<SelectionTargetController>().setIsTarget(false);
        
        curIndex++;

        if(curIndex > targets.Length-1){
            StopTrial();
            return;
        }

        curTarget = targets[selectionOrder[curIndex]];
        curTarget.GetComponent<SelectionTargetController>().setIsTarget(true);

        ResetTimer();
    }

    public GameObject GetCurTarget(){
        return curTarget;
    }

    public void StopTrial()
    {
        curTarget = null;
        isInTrial = false;

        recordTrialResult();
        
        firstGrabTime  = 0.0f;
        selectionTimeStampIdx =0;
       
        // // if curTechnique is portal
        // // close the portal
        if(Interaction_Setting.instance.cur_selection_type == Selection_Technique.PORTAL){
            GameObject.Find("teleported_vr_glove_right").GetComponent<Hand>().ClearContactInteractables();
            GameObject.Find("teleported_vr_glove_left").GetComponent<Hand>().ClearContactInteractables();
            PortalManager.instance.ResetNumPortalOpen();
            PortalManager.instance.ClosePortal();
        }
        GameObject.Find("vr_glove_right").GetComponent<Hand>().ClearContactInteractables();
        GameObject.Find("vr_glove_left").GetComponent<Hand>().ClearContactInteractables();
       
        gameObject.SetActive(false);
         // set next trial
        Interaction_Setting.instance.setNextTrial();
    }
}
