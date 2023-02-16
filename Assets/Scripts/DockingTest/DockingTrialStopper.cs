using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class DockingTrialStopper : MonoBehaviour
{
    private SteamVR_Behaviour_Pose m_Pose = null;

    public SteamVR_Action_Boolean m_TeleportAction;
    public SteamVR_Action_Boolean m_GrabAction;

    public int wrongClick = 0;

    private void OnEnable()
    {
        m_Pose = GetComponent<SteamVR_Behaviour_Pose>();

    }
    // Update is called once per frame
    void Update()
    {
       
        if (DockingExperimentGenerator.instance != null &&  DockingExperimentGenerator.instance.getIsInTrial())
        {
            if (m_GrabAction.GetStateDown(m_Pose.inputSource))
            {
               
                DockingExperimentGenerator.instance.setStopstopTriggerCalledTrue();
                
            }
        }
    }
}
