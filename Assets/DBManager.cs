using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper);
    }

    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}

[System.Serializable]
public class TrialInfo
{   
    // user 
    public int userID;
    public int session;
    public int block;
    public int trial;
    public string dateTime;
    public string whichHand;
    public string whichSelectionTechnique;
    public string whichTaskType;
    public string whichExperimentStep;
    public string whichDistance;
    public float firstGrabTime;
    public float taskTimeStamps_start;
    public float taskTimeStamps_grab;
    public float taskTimeStamps_docking;


    public string taskTimeStamps_date_1;
    
    /// dependent variables to save
    public float firstGrabTime_docking;
    public float completionTime;
    public float completionAccuracy;

    
    public int numPortalOpenTrial;
    public int numTeleportToTarget;
    public int numTriggerAction;
}

public class DBManager : MonoBehaviour
{
    public static DBManager instance;
    private TrialInfo trialInfo;
    private List<TrialInfo> trials;
    private const string BASE_PATH = "Assets/ExperimentDataRepo";
    private string DBfolder_path;

    private string getCurrentTime()
    {
        return Util.instance.GetTimeStamp();
    }

    private void Awake()
    {
        instance = this;
    }

    private void createParticipantFolder(string userId)
    {
        string experiment_info_path = "/" + trialInfo.whichExperimentStep + "/" + trialInfo.whichTaskType;
        userId = string.Format("{0}{1}", "user", userId);

        string path2ParticipantFolder = BASE_PATH + experiment_info_path + "/" + userId;


        // 1. check whether user folder is or not
        if (!Directory.Exists(path2ParticipantFolder))
        {
            // 2. if not, create
            string guid = AssetDatabase.CreateFolder(BASE_PATH + experiment_info_path, userId);
            //Debug.Log(guid);
            DBfolder_path = AssetDatabase.GUIDToAssetPath(guid);
        }else{
            if(Experiment_Setting.instance.experiment_step == Experiment_Step.FULL){
                Debug.LogWarning("Please check the participant id. There is a duplication for this participant id");
                UnityEditor.EditorApplication.isPlaying = false;   
            }
        }
        

        DBfolder_path = BASE_PATH + experiment_info_path + "/" + userId;
    }

    private TrialInfo returnNewTrialInfo()
    {
        trialInfo = new TrialInfo();

        trialInfo.userID = Experiment_Setting.instance.userID;
        trialInfo.whichHand = Util.instance.GetTrialHandInfo();
        trialInfo.whichTaskType = Util.instance.GetTrialTaskType();
        trialInfo.whichExperimentStep = Util.instance.GetTrialExperimentStep();
        
        return trialInfo;
    }

    void Start()
    {
        trialInfo = new TrialInfo();
        trialInfo.userID = Experiment_Setting.instance.userID;
        
        trialInfo.whichHand = Util.instance.GetTrialHandInfo();
        trialInfo.whichTaskType = Util.instance.GetTrialTaskType();
        trialInfo.whichExperimentStep = Util.instance.GetTrialExperimentStep();

        trials = new List<TrialInfo>();
        createParticipantFolder(trialInfo.userID.ToString());
    }
    

    public void recordTrial(float firstGrabTime, float[] time, float[] accuracy, float[] taskTimeStamps, string[] taskTimeStamps_date){
   
    }

public void recordTrial_docking(float firstGrabTime, 
                                float time,
                                float accuracy, 
                                int wrongClick, 
                                float taskTimeStamps_start,
                                float taskTimeStamps_grab,
                                float taskTimeStamps_docking,
                                int numberTriggers)
    {       
        trialInfo.firstGrabTime_docking = firstGrabTime;
        trialInfo.completionTime = time;
        trialInfo.completionAccuracy = accuracy;
        trialInfo.numTriggerAction = numberTriggers;
        trialInfo.taskTimeStamps_start = taskTimeStamps_start;
        trialInfo.taskTimeStamps_grab = taskTimeStamps_grab;
        trialInfo.taskTimeStamps_docking = taskTimeStamps_docking;
     


        if(Interaction_Setting.instance.cur_selection_type == Selection_Technique.PORTAL){
            trialInfo.numPortalOpenTrial = PortalManager.instance.GetNumPortalOpen();
        }else if(Interaction_Setting.instance.cur_selection_type == Selection_Technique.DIRECT){
            trialInfo.numTeleportToTarget = TeleportCounter.instance.GetNumTeleport();
        }else{
            trialInfo.numPortalOpenTrial = 0;
        }

        trialInfo.dateTime = getCurrentTime();
        trialInfo.userID = Experiment_Setting.instance.userID;

        
        trialInfo.whichSelectionTechnique = Util.instance.GetTrialSelectionType();

        trialInfo.session = Interaction_Setting.instance.getCurSession()+1;
        trialInfo.block = Interaction_Setting.instance.getCurBlock()+1;
        trialInfo.trial = Interaction_Setting.instance.getCurTrial()+1;
        trialInfo.whichDistance = Util.instance.GetTrialTargetDistance();
        

        trials.Add(trialInfo);
        saveTrials(trialInfo);
        trialInfo = returnNewTrialInfo();
    }
    public void saveTrials(TrialInfo trialInfo)
    {
        // save the trials
        string jsonObj = JsonHelper.ToJson<TrialInfo>(trials.ToArray(), true);

        string filename = DBfolder_path + "/result.json";

        File.WriteAllText(filename, jsonObj);
    }

    public string returnValidFilename(string filename)
    {
        int idx = 0;

        while (true)
        {
            if (File.Exists(filename))
            {
                idx += 1;
            }
            else
            {
                filename += idx.ToString();
                break;
            }
        }

        return filename;
    }
}
