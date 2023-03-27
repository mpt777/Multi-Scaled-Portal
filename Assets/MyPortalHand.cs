using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class MyPortalHand : MonoBehaviour
{
    public GameObject armObject;
    public GameObject realHandObject;
    public GameObject portalManager_obj;

    private GameObject portal_origin_centerPivot = null;
    private GameObject portal_target_centerPivot = null;

    [SerializeField]
    private GameObject teleportHand = null;
    private Camera VRCamera;
    private GameObject portalCamera;
    private PortalManager portalManager;

    private bool wasCollidingToPortal = false;
    private bool isCollidingToPortal = false;

    private float ogWidthMultiplier;

    private List<GameObject> collidingObjects = new();

    public bool IsColliding(GameObject gameObject)
    {
        return collidingObjects.Contains(gameObject);
    }

    public bool GetIsCollidingToPortal()
    {
        return isCollidingToPortal;
    }

    public bool GetWasCollidingToPortal()
    {
        return wasCollidingToPortal;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Portal")
        {
            isCollidingToPortal = true;
            wasCollidingToPortal = false;

            if (!collidingObjects.Contains(other.gameObject))
            {
                collidingObjects.Add(other.gameObject);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Portal")
        {
            isCollidingToPortal = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Portal")
        {
            isCollidingToPortal = false;
            wasCollidingToPortal = true;

            if (collidingObjects.Contains(other.gameObject))
            {
                collidingObjects.Remove(other.gameObject);
            }
        }
    }

    private bool isPortalHandActivate = false;

    public void Initialize(GameObject portalManager_obj, GameObject arm, GameObject realHand)
    {
        portalManager = portalManager_obj.GetComponent<PortalManager>();
        armObject = arm;
        realHandObject = realHand;

        teleportHand = gameObject;
        teleportHand.SetActive(true);
        teleportHand.GetComponent<Hand>().enabled = true;
        teleportHand.GetComponent<Collider>().enabled = false;
        teleportHand.GetComponent<MyLayHandler>().enabled = false;
        teleportHand.transform.GetChild(0).gameObject.SetActive(false);

        // camera
        VRCamera = portalManager.VRCamera;
        portalCamera = portalManager.portalCamera;

        // center pivots
        portal_origin_centerPivot = portalManager.portal_origin_centerPivot;
        portal_target_centerPivot = portalManager.portal_target_centerPivot;

        isPortalHandActivate = false;

        LineRenderer lineRenderer = teleportHand.GetComponent<LineRenderer>();
        //lineRenderer.SetWidth(lineRenderer.startWidth / portalManager.originToTargetTransform.x, lineRenderer.endWidth / portalManager.originToTargetTransform.x);
        ogWidthMultiplier = lineRenderer.widthMultiplier;

    }
    public void OnEnable()
    {
        //portalManager = portalManager_obj.GetComponent<PortalManager>();
        // set the attributes
        // hand
        //teleportHand = gameObject;
        //teleportHand.SetActive(true);
        //teleportHand.GetComponent<Hand>().enabled = true;
        //teleportHand.GetComponent<Collider>().enabled = false;
        //teleportHand.GetComponent<MyLayHandler>().enabled = false;
        //teleportHand.transform.GetChild(0).gameObject.SetActive(false);

        //// camera
        //VRCamera = portalManager.VRCamera;
        //portalCamera = portalManager.portalCamera;

        //// center pivots
        //portal_origin_centerPivot = portalManager.portal_origin_centerPivot;
        //portal_target_centerPivot = portalManager.portal_target_centerPivot;

        //isPortalHandActivate = false;
    }

    private void Update()
    {
        Vector3 curPos = realHandObject.transform.position;

        Vector3 chestPos = VRCamera.transform.position - new Vector3(0, 0.3f, 0);
        Vector3 camera2hand = curPos - chestPos;
        Vector3 camera2OrigincenterPivot = Vector3.Project(portal_origin_centerPivot.transform.position - chestPos, chestPos);

        GameObject portalCollider = portalManager.instanceOriginPortal.transform.Find("PortalCircle_origin/PortalCircleSurface").gameObject;
        bool isCollisionArm = armObject.GetComponent<ArmExtensionFromHeadToHand>().IsColliding(portalCollider); // GetIsCollidingToPortal();
        bool isCollisionHand = IsColliding(portalCollider); // GetIsCollidingToPortal();

        if (Experiment_Setting.instance.task_type == Task_Type.DOCKING_TASK)
        {
            if (isCollisionArm || (isCollisionHand && wasCollidingToPortal))
            {
                ShowPortalHand();

            }
            else
            {
                HidePortalHand();
            }

            if (isCollisionArm || (isCollisionHand && wasCollidingToPortal))
            {
                if (!isPortalHandActivate)
                {
                    OnEnablePortalHand();
                }
            }
            else
            {
                if (isPortalHandActivate)
                {
                    OnDisablePortalHand();
                }
            }
        }
        else if (Experiment_Setting.instance.task_type == Task_Type.SELECTION_TASK)
        {
            if (isCollisionArm || (isCollisionHand && wasCollidingToPortal))
            {
                ShowPortalHand();
                OnEnablePortalHand();

            }
            else
            {
                HidePortalHand();
                OnDisablePortalHand();
            }

        }
        updatePortalHand();
    }

    private bool isLookingSameDirection(Vector3 vecA, Vector3 vecB)
    {
        Vector3 projection = Vector3.Project(vecA, vecB);

        bool isMagnitudeCheck = false;
        bool isLookingSameDir = false;


        if (vecB.sqrMagnitude - 0.05f <= projection.sqrMagnitude)
            isMagnitudeCheck = true;

        float x_vecA = projection[0];
        float y_vecA = projection[1];
        float z_vecA = projection[2];

        float x_vecB = vecB[0];
        float y_vecB = vecB[1];
        float z_vecB = vecB[2];

        if (x_vecA / Mathf.Abs(x_vecA) == x_vecB / Mathf.Abs(x_vecB)
            && y_vecA / Mathf.Abs(y_vecA) == y_vecB / Mathf.Abs(y_vecB)
            && z_vecA / Mathf.Abs(z_vecA) == z_vecB / Mathf.Abs(z_vecB)
            )
            isLookingSameDir = true;

        return isMagnitudeCheck && isLookingSameDir;
    }

    private void ShowPortalHand()
    {
        if (teleportHand == null)
        {
            portalManager.SetPortalHands();
        }

        teleportHand.transform.GetChild(0).gameObject.SetActive(true);
    }

    private void HidePortalHand()
    {
        teleportHand.transform.GetChild(0).gameObject.SetActive(false);
    }

    private void OnEnablePortalHand()
    {
        if (teleportHand == null)
        {
            portalManager.SetPortalHands();
        }

        // uncover the real hand model        
        realHandObject.GetComponent<Hand>().enabled = false;
        realHandObject.GetComponent<MyLayHandler>().enabled = false;

        if (!PortalManager.instance.isAllowObjectTransportThroughPortal)
        {
            realHandObject.GetComponent<FixedJoint>().connectedBody = null;
        }

        // show the portal hand

        if (!teleportHand.activeSelf)
        {
            teleportHand.SetActive(true);
        }

        teleportHand.GetComponent<Hand>().enabled = true;
        teleportHand.GetComponent<Collider>().enabled = true;
        teleportHand.GetComponent<MyLayHandler>().portalManager = portalManager.gameObject;


        //if (realHandObject.transform.GetComponent<Custom_VR_Behaviour_Skeleton>().IsMainHand())
        //{
            // for the experiment block the function opening portal in portal
            if (PortalManager.instance.isAllowCreatingPortalInPortal)
            {
                teleportHand.GetComponent<MyLayHandler>().enabled = true;
                portalManager.ReparetToTargetRoom(teleportHand.GetComponent<MyLayHandler>().m_Pointer);

                LineRenderer lineRenderer = teleportHand.GetComponent<LineRenderer>();
                //lineRenderer.SetWidth(lineRenderer.startWidth / portalManager.originToTargetTransform.x, lineRenderer.endWidth / portalManager.originToTargetTransform.x);
                lineRenderer.widthMultiplier = ogWidthMultiplier;
                lineRenderer.widthMultiplier *= portalManager.originToTargetTransform.x;
            }
        //}

        isPortalHandActivate = true;
    }

    public Vector3 GetPortalHandPosition()
    {
        return teleportHand.transform.position;
    }

    private void updatePortalHand()
    {
        Matrix4x4 hand_to_world = realHandObject.transform.localToWorldMatrix;
        Matrix4x4 world_to_pivot = portal_origin_centerPivot.transform.worldToLocalMatrix;
        Matrix4x4 hand_to_pivot = world_to_pivot * hand_to_world;

        teleportHand.transform.localRotation = hand_to_pivot.GetRotation();
        teleportHand.transform.localPosition = hand_to_pivot.GetPosition();
    }

    private void OnDisablePortalHand()
    {

        teleportHand.GetComponent<Hand>().enabled = true;
        teleportHand.GetComponent<MyLayHandler>().enabled = false;
        teleportHand.GetComponent<Collider>().enabled = false;

        if (!PortalManager.instance.isAllowObjectTransportThroughPortal)
        {
            teleportHand.GetComponent<FixedJoint>().connectedBody = null;
        }
        isPortalHandActivate = false;
        realHandObject.GetComponent<Hand>().enabled = true;
        realHandObject.GetComponent<MyLayHandler>().enabled = true;
    }


    //return relative roation to VR camera 
    public Vector3 getAngleMovement(GameObject centerPivot)
    {
        Quaternion cameraRoatation = VRCamera.transform.rotation;
        Quaternion thisRotation = realHandObject.transform.rotation;

        Quaternion relativeRotation = Quaternion.Inverse(cameraRoatation) * thisRotation;

        return relativeRotation.eulerAngles;
    }

    public static Vector3 getRelativePosition(Vector3 origin, Vector3 position)
    {
        return origin - position;
    }

    public Vector3 getHandPosition_relaiveTo(GameObject centerPivot)
    {
        Vector3 relativePosition = new Vector3(-realHandObject.transform.position.z + centerPivot.transform.position.z, -realHandObject.transform.position.y + centerPivot.transform.position.y, +realHandObject.transform.position.x - centerPivot.transform.position.x);
        return relativePosition;
    }
}
