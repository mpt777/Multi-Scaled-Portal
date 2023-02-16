using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class PortalInfo{
    public string portalStatus;
    public Vector3 portal_origin_pos;
    public Vector3 portal_target_pos;
   
    public Vector3 controlObj_pos;
    public Vector3 targetObj_pos;
    public float time_Ms;
    public string date;
    public string whichTaskType;
}

public class DBPortalTracker : MonoBehaviour
{
    public GameObject portalManager;
    public GameObject dockingTaskObj;
    public GameObject selectionTaskObj;
    public static DBPortalTracker instance;

    private PortalInfo portalInfo;

    private const string BASE_PATH = "Assets/ExperimentDataRepo";
    private string DBfolder_path;

    private int portalStateCounter = 0;

    private string getCurrentTime()
    {
        return Util.instance.GetTimeStamp();
    }

    private void Awake()
    {
        instance = this;
        CreateParticipantFolder(Experiment_Setting.instance.userID.ToString());
    }
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
    
 private PortalInfo returnNewPortalInfo()
    {
        PortalInfo portalInfo = new PortalInfo();

        portalInfo.date = GetCurrentDate();
        portalInfo.time_Ms = GetCurrentTimeInMS();
        portalInfo.whichTaskType = Util.instance.GetTrialTaskType();

        return portalInfo;
    }

    private void CreateParticipantFolder(string userId)
    {
        string trialTaskType = Util.instance.GetTrialTaskType();
        string trialExperimentStep = Util.instance.GetTrialExperimentStep();
        string experiment_info_path = "/" + trialExperimentStep + "/" + trialTaskType;
        userId = string.Format("{0}{1}", "user", userId);

        string path2ParticipantFolder = BASE_PATH + experiment_info_path + "/" + userId + "_portal_tracking_info";
        Debug.Log(path2ParticipantFolder);
        // 1. check whether user folder is or not
        if (!Directory.Exists(path2ParticipantFolder))
        {
            // 2. if not, create
            string guid = AssetDatabase.CreateFolder(BASE_PATH + experiment_info_path, userId+"_portal_tracking_info");
            DBfolder_path = AssetDatabase.GUIDToAssetPath(guid);
        }else{
            if(Experiment_Setting.instance.experiment_step == Experiment_Step.FULL){
                Debug.LogWarning("Please check the participant id. There is a duplication for this participant id");
                UnityEditor.EditorApplication.isPlaying = false;   
            }
        }
 

        DBfolder_path = path2ParticipantFolder;
    }

    // Update is called once per frame
    void Update()
    {
        if(Interaction_Setting.instance.cur_selection_type == Selection_Technique.PORTAL){
            // update the portal 
            string curTimeMin = GetCurrentDate_Minute();
            portalInfo = returnNewPortalInfo();

            if(portalManager.GetComponent<PortalManager>().GetIsPortalOpen()){
              
                portalInfo.portalStatus = "OPENING";
                portalInfo.portal_origin_pos = portalManager.GetComponent<PortalManager>().originPortal.transform.position;
                portalInfo.portal_target_pos = portalManager.GetComponent<PortalManager>().targetPortal.transform.position;
            }else{
                    // portal is in close state
                portalInfo.portalStatus = "CLOSING";
              
                portalInfo.portal_origin_pos = Vector3.zero;
                portalInfo.portal_target_pos = Vector3.zero;
            }

            GameObject controlObj;
            GameObject targetObj;
            
            if(Experiment_Setting.instance.task_type== Task_Type.SELECTION_TASK){
                // update target Object pos
                targetObj = selectionTaskObj.GetComponent<SelectionExperimentGenerator>().GetCurTarget();
                if(targetObj != null){
                    portalInfo.targetObj_pos = targetObj.transform.position;
                }
            }

            if(Experiment_Setting.instance.task_type== Task_Type.DOCKING_TASK){
                // update target and control Object pos
                controlObj = dockingTaskObj.GetComponent<InitDockingTask>().controlObject;
                targetObj= dockingTaskObj.GetComponent<InitDockingTask>().targetObject;

                if(controlObj != null){
                    portalInfo.controlObj_pos =  controlObj.transform.position;
                }

                if(targetObj!=null){
                    portalInfo.targetObj_pos =  targetObj.transform.position;
                }
                
                
            }

            //save

            SavePortalInfo(portalInfo, curTimeMin);
        }
    }

    private void SavePortalInfo(PortalInfo portalInfo, string timeStamp){

         string json = JsonUtility.ToJson(portalInfo, true);
        
        string file_path = DBfolder_path + "/"+timeStamp.Replace(" ", "_")+".json";


         using (StreamWriter sw = File.AppendText(file_path))
        {
            sw.WriteLine(json + ",");
        }	
    }
}
