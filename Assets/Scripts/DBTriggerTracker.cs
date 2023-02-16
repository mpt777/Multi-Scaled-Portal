using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using System.IO;

public class TriggerInfo{
    public string date;
    public float time_Ms;
}

public class DBTriggerTracker : MonoBehaviour
{

    public TriggerInfo t_info;
    public static DBTriggerTracker instance;
    private const string BASE_PATH = "Assets/ExperimentDataRepo";
    private string DBfolder_path;

    void Awake(){
        instance = this;
        CreateParticipantFolder(Experiment_Setting.instance.userID.ToString());
    }

    private void CreateParticipantFolder(string userId)
    {
        string trialTaskType = Util.instance.GetTrialTaskType();
        string trialExperimentStep = Util.instance.GetTrialExperimentStep();
        string experiment_info_path = "/" + trialExperimentStep + "/" + trialTaskType;
        userId = string.Format("{0}{1}", "user", userId);

        string path2ParticipantFolder = BASE_PATH + experiment_info_path + "/" + userId + "_trigger_info";
        // 1. check whether user folder is or not
        if (!Directory.Exists(path2ParticipantFolder))
        {
            // 2. if not, create
            string guid = AssetDatabase.CreateFolder(BASE_PATH + experiment_info_path, userId+"_trigger_info");
            DBfolder_path = AssetDatabase.GUIDToAssetPath(guid);
        }else{
            if(Experiment_Setting.instance.experiment_step == Experiment_Step.FULL){
                Debug.LogWarning("Please check the participant id. There is a duplication for this participant id");
                UnityEditor.EditorApplication.isPlaying = false;   
            }
        }
 

        DBfolder_path = path2ParticipantFolder;
    }

    private TriggerInfo returnNewTriggerInfo()
    {
        TriggerInfo triggerInfo = new TriggerInfo();

        triggerInfo.date = Util.instance.GetTimeStamp();
        triggerInfo.time_Ms = Util.instance.GetTimeStampInMS();

        return triggerInfo;
    }

    public void SaveTriggerInfo(){
        t_info = returnNewTriggerInfo();

        string json = JsonUtility.ToJson(t_info, true);
        
        string file_path = DBfolder_path + "/triggerTracker.json";

        using (StreamWriter sw = File.AppendText(file_path))
        {
            sw.WriteLine(json + ",");
        }	
    }
}
