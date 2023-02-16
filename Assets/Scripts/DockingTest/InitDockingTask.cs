using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;

public enum DockingObjectLocation
{
    FRONT_TOP_LEFT,
    FRONT_TOP_RIGHT,
    BACK_TOP_LEFT,
    BACK_TOP_RIGHT,
    FRONT_BOTTOM_LEFT,
    FRONT_BOTTOM_RIGHT,
    BACK_BOTTOM_LEFT,
    BACK_BOTTOM_RIGHT,
}

public enum DockingObjectRotation
{
    NO_ROTATION,
    X_ROTATION,
    Y_ROTATION,
    Z_ROTATION,
    X_Y_ROTATION,
    X_Z_ROTATION,
    Y_Z_ROTATION,
    X_Y_Z_ROTATION
}


public class InitDockingTask : MonoBehaviour
{
    public const int numTrialsInBlock = 8;

    public Dictionary<int, DockingObjectLocation> dockingObjectLocationDict;
    public Dictionary<int, DockingObjectRotation> dockingObjectRotationDict;

    [Header("GameObject")]
    
    public GameObject targetObject;
    public GameObject controlObject;

    public GameObject portalSeeThroughWall;

    private Vector3[] targetObjectVertices;
    private Vector3[] controlObjectVertices;

    [Header("offset")]
    public float dockingSuccessOffset;
    public bool giveDifferentOrientation;

    public int wrongClick = 0;
    private void OnEnable(){
        controlObject.transform.localPosition = Vector3.zero;
        targetObject.transform.localPosition = GetRandomPosition();

        controlObject.GetComponent<DockingTaskObject>().InitVertices();
        targetObject.GetComponent<DockingTaskObject>().InitVertices();
        // InitVertices

        if(giveDifferentOrientation){
            controlObject.transform.rotation = GetRandomRotation();
            targetObject.transform.rotation = GetRandomRotation();
        }else{
            Quaternion tempRot = GetRandomRotation();
            controlObject.transform.rotation = tempRot;
            targetObject.transform.rotation = tempRot;
        }


        if(DockingTaskOrderManager.instance.DoSeeThroughDockingTask){
            portalSeeThroughWall.SetActive(true);

            Bounds wallObjBound= portalSeeThroughWall.GetComponent<MeshRenderer>().bounds;
            float boundSizeX = wallObjBound.size.x;
            float xPosition = - 3 + Experiment_Setting.instance.underArm_length_meter;
            portalSeeThroughWall.transform.position = new Vector3(xPosition, 1, 0);
        }
    }

    private void Update(){
        float errorRate = CalculateAccuracy();

            if(errorRate < dockingSuccessOffset){
                controlObject.GetComponent<Outline>().eraseRenderer = false;
                controlObject.GetComponent<DockingTaskObject>().setIsSatisfyOffset(true);
            }else{
                controlObject.GetComponent<Outline>().eraseRenderer = true;
                controlObject.GetComponent<DockingTaskObject>().setIsSatisfyOffset(false);
            }
    }

    public float CalculateAccuracy()
    {
        controlObjectVertices = controlObject.GetComponent<DockingTaskObject>().GetVertices();
        targetObjectVertices = targetObject.GetComponent<DockingTaskObject>().GetVertices(); 

        float distanceA = Vector3.Distance(targetObjectVertices[0], controlObjectVertices[0]);
        float distanceB = Vector3.Distance(targetObjectVertices[1], controlObjectVertices[1]);
        float distanceC = Vector3.Distance(targetObjectVertices[2], controlObjectVertices[2]);
        float distanceD = Vector3.Distance(targetObjectVertices[3], controlObjectVertices[3]);

        float rms = Mathf.Sqrt((Mathf.Pow(distanceA, 2) + Mathf.Pow(distanceB, 2) + Mathf.Pow(distanceC, 2) + Mathf.Pow(distanceD, 2))/4);

        return rms;
    }

    public Vector3 GetRandomPosition(){
        Vector3 tempPos = Vector3.Normalize(Random.insideUnitSphere) * Experiment_Setting.instance.reach_offset_for_selectionTask;

        return tempPos;
    }

    public Quaternion GetRandomRotation(){
        Vector3 tempRot = new Vector3(Random.Range(0.0f, 180.0f), Random.Range(0.0f, 180.0f), Random.Range(0.0f, 180.0f));

        return Quaternion.Euler(tempRot);
    }
}
