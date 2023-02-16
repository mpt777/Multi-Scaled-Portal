using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;
using UnityEditor;
using System.IO;

[System.Serializable]
public class VR_HardWareInfo
{   
    
    // user 
    public Vector3 hmd_pos;
    
    public Vector3 controller_right_pos;
    public Vector3 controller_left_pos;

    
    public Vector3 portal_hand_right_pos;
    public Vector3 portal_hand_left_pos;
    public Vector3 real_hand_right_pos;
    public Vector3 real_hand_left_pos;
    public float time_Ms;
    public string date;
}



public class DBHardWareTracker : MonoBehaviour
{
    public static DBHardWareTracker instance;
    private VR_HardWareInfo hardwareInfo;
    private const string BASE_PATH = "Assets/ExperimentDataRepo";
    private string DBfolder_path;

    private string timeStamp = null;
    public GameObject hmd_cam;

    public GameObject controller_right;
    public GameObject controller_left;
    private string GetCurrentDate()
    {
        return Util.instance.GetTimeStamp();
    }
    private string GetCurrentDate_Minute()
    {
        return Util.instance.GetTimeStampInMinute();
    }
    
    private float GetCurrentTimeInMS()
    {
        return Util.instance.GetTimeStampInMS();
    }
    
    private void CreateParticipantFolder(string userId)
    {
        string trialTaskType = Util.instance.GetTrialTaskType();
        string trialExperimentStep = Util.instance.GetTrialExperimentStep();
        string experiment_info_path = "/" + trialExperimentStep + "/" + trialTaskType;
        userId = string.Format("{0}{1}", "user", userId);

        string path2ParticipantFolder = BASE_PATH + experiment_info_path + "/" + userId + "_hw_tracking_info";
        Debug.Log(path2ParticipantFolder);
        // 1. check whether user folder is or not
        if (!Directory.Exists(path2ParticipantFolder))
        {
            // 2. if not, create
            string guid = AssetDatabase.CreateFolder(BASE_PATH + experiment_info_path, userId+"_hw_tracking_info");
            DBfolder_path = AssetDatabase.GUIDToAssetPath(guid);
        }else{
            if(Experiment_Setting.instance.experiment_step == Experiment_Step.FULL){
                Debug.LogWarning("Please check the participant id. There is a duplication for this participant id");
                UnityEditor.EditorApplication.isPlaying = false;   
            }
        }
 

        DBfolder_path = path2ParticipantFolder;
    }

    private void Awake()
    {
        instance = this;
        Debug.Log(Experiment_Setting.instance.userID.ToString());
        CreateParticipantFolder(Experiment_Setting.instance.userID.ToString());
    }

    private VR_HardWareInfo returnNewHWInfo()
    {
        VR_HardWareInfo hardwareInfo = new VR_HardWareInfo();

        hardwareInfo.date = GetCurrentDate();
        hardwareInfo.time_Ms = GetCurrentTimeInMS();
       
        return hardwareInfo;
    }

    void Update(){

        string curTimeMin = GetCurrentDate_Minute();

        VR_HardWareInfo  hwTracker = returnNewHWInfo();

        hwTracker.hmd_pos = hmd_cam.transform.position;
        hwTracker.controller_right_pos = controller_right.transform.position;
        hwTracker.controller_left_pos = controller_left.transform.position;
        
        hwTracker.real_hand_right_pos = controller_right.GetComponent<Custom_VR_Behaviour_Skeleton>().GetVirtualCursorPos();
        hwTracker.real_hand_left_pos = controller_left.GetComponent<Custom_VR_Behaviour_Skeleton>().GetVirtualCursorPos();
        
        // search whether the portal hand exist
        if(controller_right.GetComponent<MyPortalHandHandler>().enabled){
            hwTracker.portal_hand_right_pos = controller_right.GetComponent<MyPortalHandHandler>().GetPortalHandPosition();
        }else{
            hwTracker.portal_hand_right_pos = Vector3.zero;
        }

        if(controller_left.GetComponent<MyPortalHandHandler>().enabled){
            hwTracker.portal_hand_left_pos = controller_left.GetComponent<MyPortalHandHandler>().GetPortalHandPosition();
        }else{
            hwTracker.portal_hand_left_pos = Vector3.zero;
        }

        SaveHWInfo(hwTracker, curTimeMin);
    }

     public void SaveHWInfo(VR_HardWareInfo hwInfo, string timeStamp)
    {
        // save the trials
     
        string json = JsonUtility.ToJson(hwInfo, true);
        
        string file_path = DBfolder_path + "/"+timeStamp.Replace(" ", "_")+".json";

        if(!File.Exists(file_path)){
            
        }else{
            // File.Delete(file_path);
        }

        using (StreamWriter sw = File.AppendText(file_path))
        {
            sw.WriteLine(json + ",");
        }	
    }
}
