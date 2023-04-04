using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MyRealHand : MonoBehaviour
{

    public GameObject myPortalHand;
    public GameObject cameraRig;
    public SteamVR_Action_Boolean m_TeleportAction;
    private SteamVR_Behaviour_Pose m_Pose = null;
    // Start is called before the first frame update
    public void OnEnable()
    {
        m_Pose = GetComponent<SteamVR_Behaviour_Pose>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {        
        
        if (m_TeleportAction.GetStateUp(m_Pose.inputSource))
        {
            Teleport();
        }
        
    }

    public void UpdateMyPortalHand(GameObject? obj)
    {
        myPortalHand = obj;
    }

    private void Teleport()
    {
        Debug.Log("Here");
        Debug.Log(myPortalHand);
        Debug.Log(cameraRig);
        if (myPortalHand == null) { return; }
        MyPortalHand myPortalHand_myPortalHand = myPortalHand.GetComponent<MyPortalHand>();
        myPortalHand_myPortalHand.portalManager.ReparentToTargetRoom(cameraRig);
        myPortalHand_myPortalHand.portalManager.TransformToTargetRoom(cameraRig);
        myPortalHand_myPortalHand.portalManager.SwapRooms();


    }
}
