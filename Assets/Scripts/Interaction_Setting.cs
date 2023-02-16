using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using UnityEngine.UI;

public enum Selection_Technique
{
    //NONE,
    DIRECT,
    GOGO,
    RAYCASTING,
    PORTAL,
    //TELEPORTED_HAND,
    IN_MENU_STATE
}

public enum Target_Distance
{
    ARM_REACH, NEAR, MID, FAR
}

public class Interaction_Setting : MonoBehaviour
{
    public static Interaction_Setting instance;

    
    [Header("Task Progress")]
    [SerializeField]
    private int curSession = 0;
    [SerializeField]
    private int curBlock = 0;
    [SerializeField]
    private int curTrial = 0;
    [SerializeField]
    private Selection_Technique m_selection_type;
    [SerializeField]
    private Target_Distance m_target_distance;

    private Target_Distance[] trial_target_distances;
    private Selection_Technique[] trial_techniques;

    [Header("Objects")]
    public GameObject leftHandController;
    public GameObject rightHandController;
    public GameObject OriginObject;

    public Selection_Technique cur_selection_type { 
        get { return m_selection_type; } 
        set { m_selection_type = value; } 
    }

    public Target_Distance cur_target_distance {
        get { return m_target_distance; }
        set { m_target_distance = value; }
    }
    
    private void Awake()
    {
        instance = this;
    }

    public int getCurSession(){
        return curSession;
    }

    public int getCurBlock()
    {
        return curBlock;
    }

    public int getCurTrial()
    {
        return curTrial;
    }

    public void setNextTrial()
    {

        if(!UpdatePanel.instance.inforBox.activeSelf){
            UpdatePanel.instance.inforBox.SetActive(true);
        }
        
        // add 1 to trial
        curTrial += 1;
        UpdateTrialPanel();
       
    }

    public void setNextBlock()
    {
        curBlock += 1;
        curTrial = 0;

        if(curBlock == Experiment_Setting.instance.maxNumBlock){
            setNextSession();
            return;
        }

        m_target_distance = trial_target_distances[curBlock];

        UpdateTrialPanel();
        UpdateHandInteraction();
    }

    private Target_Distance[] GetNextDistances(){
        if(Experiment_Setting.instance.task_type == Task_Type.SELECTION_TASK){
            if(curSession == 0){
                Experiment_Setting.instance.SetMaxNumBlock(SelectionTaskOrderManager.instance.session1_blocks.Length);
                return SelectionTaskOrderManager.instance.session1_blocks;
            }

            if(curSession == 1){
                Experiment_Setting.instance.SetMaxNumBlock(SelectionTaskOrderManager.instance.session2_blocks.Length);

                return SelectionTaskOrderManager.instance.session2_blocks;
            }

            
        }

        if(Experiment_Setting.instance.task_type == Task_Type.DOCKING_TASK){
            if(curSession == 0){
                Experiment_Setting.instance.SetMaxNumBlock(SelectionTaskOrderManager.instance.session1_blocks.Length);
                return DockingTaskOrderManager.instance.session1_blocks;
            }

            if(curSession == 1){
                Experiment_Setting.instance.SetMaxNumBlock(SelectionTaskOrderManager.instance.session2_blocks.Length);
                return DockingTaskOrderManager.instance.session2_blocks;
            }

        }

        return null;
    }

    public void setNextSession(){
        curSession +=1;
        curBlock = 0;
        curTrial = 0;

        if(curSession == Experiment_Setting.instance.maxNumSession){
            Util.instance.endExperiment();
        }

        m_selection_type = trial_techniques[curSession];

        trial_target_distances = GetNextDistances();
        
        m_target_distance = trial_target_distances[curBlock];
        

        UpdateTrialPanel();
        UpdateHandInteraction();
    }

    private void Start()
    {
        initInteractionSetting();
    }

    private Selection_Technique[] GetTrialTechniques(){
         if(Experiment_Setting.instance.task_type == Task_Type.SELECTION_TASK){
                return SelectionTaskOrderManager.instance.techniques;
        }

        if(Experiment_Setting.instance.task_type == Task_Type.DOCKING_TASK){
                return DockingTaskOrderManager.instance.techniques;
        }

        return null;
    }

    public void initInteractionSetting()
    {
        trial_techniques = GetTrialTechniques();
        trial_target_distances = GetNextDistances();
        
        // target Distance and technique should be counter-balanced.
        m_target_distance = trial_target_distances[curBlock];
        m_selection_type = trial_techniques[curSession];

        UpdateTrialPanel();
        UpdateHandInteraction();
    }

    private void ExperimentTaskOrderSetup(){
        trial_target_distances = SelectionTaskOrderManager.instance.session1_blocks;
        
        // target Distance and technique should be counter-balanced.
        m_target_distance = trial_target_distances[curBlock];
        m_selection_type = trial_techniques[curSession];

        UpdateTrialPanel();
        UpdateHandInteraction();
    }

    private void Update()
    {
        if(curTrial == Experiment_Setting.instance.maxNumTrial)
        {
            setNextBlock();
        }
    }

    private void UpdateTrialPanel(){
        UpdatePanel.instance.DoTaskTextUpdate(Util.instance.GetTrialTaskType());
        UpdatePanel.instance.DoTechniqueTextUpdate(Util.instance.GetTrialSelectionType());
        UpdatePanel.instance.DoTrialTextUpdate(curTrial, Experiment_Setting.instance.maxNumTrial);
        UpdatePanel.instance.DoBlockTextUpdate(curBlock, Experiment_Setting.instance.maxNumBlock);
    }

    private void UpdateHandInteraction(){
        leftHandController.GetComponent<Custom_VR_Behaviour_Skeleton>().UpdateBehaviour();
        rightHandController.GetComponent<Custom_VR_Behaviour_Skeleton>().UpdateBehaviour();
    }
}

