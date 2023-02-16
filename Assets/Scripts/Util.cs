using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
public class Util : MonoBehaviour
{
    public static Util instance;
    private bool m_IsTeleporting = false;
    private float m_FadeTime = 0.5f;

   
   public float GetTimeStampInMS(){
       return Time.time * 1000;
   }

    public string GetTimeStampInMinute()
    {
        return System.DateTime.Now.ToString("yyyy-MM-dd HH-mm");
    }

    public string GetTimeStamp()
    {
        return System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff tt");
    }

    private void Awake()
    {
        instance = this;
    }

    public string GetTrialHandInfo()
    {
        if (Experiment_Setting.instance.user_main_hand == Main_Hand.LEFT)
        {
            return "LEFT";
        }
        else if (Experiment_Setting.instance.user_main_hand == Main_Hand.RIGHT)
        {
            return "Right";
        }

        return "Right";
    }

    public string GetTrialSelectionType()
    {
        if (Interaction_Setting.instance.cur_selection_type == Selection_Technique.DIRECT)
        {
            return "DIRECT";
        }
        else

        if (Interaction_Setting.instance.cur_selection_type == Selection_Technique.GOGO)
        {
            return "LINEAR OFFSET";
            // return "GOGO";
        }
        else if (Interaction_Setting.instance.cur_selection_type == Selection_Technique.RAYCASTING)
        {
            return "RAYCASTING";
        }
        else if (Interaction_Setting.instance.cur_selection_type == Selection_Technique.PORTAL)
        {
            return "PORTAL";
        }

        return "DIRECT";
    }

    public string GetTrialTaskType()
    {
        if (Experiment_Setting.instance.task_type == Task_Type.SELECTION_TASK)
        {
            return "SELECTION_TASK";
        }
        else if (Experiment_Setting.instance.task_type == Task_Type.DOCKING_TASK)
        {
            return "DOCKING_TASK";
        }

        return "SELECTION TASK";
    }

    public string GetTrialExperimentStep()
    {
        if (Experiment_Setting.instance.experiment_step == Experiment_Step.TRAINING)
        {
            return "TRAINING";
        }
        else if (Experiment_Setting.instance.experiment_step == Experiment_Step.FULL)
        {
            return "FULL";
        }

        return "TRAINING";
    }

    public string GetTrialTargetDistance()
    {
        if (Interaction_Setting.instance.cur_target_distance == Target_Distance.FAR)
        {
            return "FAR";
        }
        else if (Interaction_Setting.instance.cur_target_distance == Target_Distance.MID)
        {
            return "MID";
        }
        else if (Interaction_Setting.instance.cur_target_distance == Target_Distance.NEAR)
        {
            return "NEAR";
        }

        return "NEAR";
    }
    
    public void endExperiment(){
        UnityEditor.EditorApplication.isPlaying = false;
    }

    public void returnUserOrigin(Vector3 OriginPos)
    {
        if (m_IsTeleporting)
            return;

        // Get camera rig, and head position
        Transform cameraRig = SteamVR_Render.Top().origin;
        Vector3 headPosition = SteamVR_Render.Top().head.position;

        // Figure out translation
        Vector3 groundPosition = new Vector3(headPosition.x, cameraRig.position.y, headPosition.z);
        Vector3 translateVector = OriginPos - groundPosition;

        // Move
        cameraRig.position = Vector3.zero;
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
    
}
