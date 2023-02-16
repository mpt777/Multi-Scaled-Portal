using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using UnityEngine.UI;

public enum Main_Hand
{
    RIGHT,
    LEFT
}

public enum Task_Type
{
    SELECTION_TASK,
    DOCKING_TASK
}


public enum Experiment_Step
{
    TRAINING, FULL
}

public class Experiment_Setting : MonoBehaviour
{
    public static Experiment_Setting instance;

    [Header("Participant Manager")]
    public int userID;
    public Main_Hand user_main_hand;
    [Tooltip("Please note that the length Unit in Unity is m (meter).")]
    public float underArm_length_meter;
    // public float participantHeight ;

    [Header("Task Manager")]
    public Experiment_Step experiment_step;
    public Task_Type task_type;

    [Tooltip("It should be even number ex.12")]
    public int maxNumTrial;
    [Tooltip("It is equal to the number of distance (mid, far, very Far)")]
    public int maxNumBlock;
    [Tooltip("equal to the number of techniques")]
    public int maxNumSession;

    [Header("Selection Task Manager")]
    public int selectionTrialsPerBlock;

    [Header("Docking Task Manager")]
    public int dockingTrialsPerBlock;

    [Header("Distance Manager")]
    public float reach_offset_for_selectionTask;

    public float arm_reach_distance;
    public float near_distance;
    public float middle_distance;
    public float far_distance;

    public GameObject RangeObject_near;
    public GameObject RangeObject_middle;
    public GameObject RangeObject_far;

    public GameObject originPosition;

    public void SetMaxNumBlock(int numBlock){
        maxNumBlock = numBlock;
    }

    private void Awake()
    {
        instance = this;    

        if(task_type == Task_Type.DOCKING_TASK)
        {
            // GOGO, Direct, Portal Manipulation
            maxNumSession = DockingTaskOrderManager.instance.techniques.Length;
            // mid. far, very far
            maxNumBlock = DockingTaskOrderManager.instance.session1_blocks.Length;
            maxNumTrial = dockingTrialsPerBlock;
        }
        
        if(task_type == Task_Type.SELECTION_TASK)
        {
            // GOGO, Direct, Portal Manipulation
            maxNumSession = SelectionTaskOrderManager.instance.techniques.Length;
            // mid. far, very far
            maxNumBlock = SelectionTaskOrderManager.instance.session1_blocks.Length;
            maxNumTrial = selectionTrialsPerBlock;
        }

        if(underArm_length_meter == 0){
            Debug.LogWarning("You did't specify the user's arm reach (in m). Please set the user's arm reach.");
            UnityEditor.EditorApplication.isPlaying = false;  
        }

        if(reach_offset_for_selectionTask == 0 ){
            reach_offset_for_selectionTask = underArm_length_meter * 0.25f;
        }

        arm_reach_distance = underArm_length_meter;

        //if(near_distance == 0){
        //    near_distance = underArm_length_meter * 3;
        //    RangeObject_near.transform.localScale = new Vector3(near_distance, 1, near_distance);
        //}else{
        //    RangeObject_near.transform.localScale = new Vector3(near_distance, 1, near_distance);
        //}

        //if(middle_distance == 0){
        //    middle_distance = underArm_length_meter * 6;
        //    RangeObject_middle.transform.localScale = new Vector3(middle_distance, 1, middle_distance);
        //}else{
        //    RangeObject_middle.transform.localScale = new Vector3(middle_distance, 1, middle_distance);
        //}

        //if(far_distance == 0){
        //    far_distance = underArm_length_meter * 12;
        //    RangeObject_far.transform.localScale = new Vector3(far_distance, 1, far_distance);
        //}else{
        //    RangeObject_far.transform.localScale = new Vector3(far_distance, 1, far_distance);
        //}
    }
}
