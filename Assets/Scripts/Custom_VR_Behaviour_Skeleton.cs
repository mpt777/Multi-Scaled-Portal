using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Valve.VR;

public class Custom_VR_Behaviour_Skeleton : SteamVR_Behaviour_Skeleton
{
    [Header("Interaction Setting")]
    [SerializeField]
    private Interaction_Setting interaction_setting;
    [SerializeField]
    private bool isPortalHand = false;
    private Selection_Technique cur_selection_type; // = interaction_setting.cur_selection_type;//GameObject.Find("Experiment_Setting").GetComponent<Interaction_Setting>().cur_selection_type;
    private GameObject targetObj;
    private Vector3 PreviousControllerPosition;

    private Vector3 virtualCursorPos;

    public bool IsPortalHand(){
        return isPortalHand;
    }

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        UpdateBehaviour();
    }

    public Vector3 GetVirtualCursorPos(){
        return virtualCursorPos;
    }

    public bool IsMainHand(){
        if (Experiment_Setting.instance.user_main_hand == Main_Hand.LEFT && inputSource == SteamVR_Input_Sources.LeftHand)
        {
            return true;
        }

        if (Experiment_Setting.instance.user_main_hand == Main_Hand.RIGHT && inputSource == SteamVR_Input_Sources.RightHand)
        {
            return true;
        }

        return false;
    }

    public void UpdateBehaviour()
    {
        if(!isPortalHand){
            gameObject.GetComponent<MyLayHandler>().enabled = false;
            gameObject.GetComponent<DockingTrialStopper>().enabled = false;
            gameObject.GetComponent<Hand>().enabled = true;
        }

        gameObject.GetComponent<MyLayHandler>().enabled = true;

        if (interaction_setting == null)
        {
            interaction_setting = Interaction_Setting.instance; // GameObject.Find("Experiment_Setting").GetComponent<Interaction_Setting>();
        }

        cur_selection_type = interaction_setting.cur_selection_type;

        if (isPortalHand)
        {
            if (Experiment_Setting.instance.user_main_hand == Main_Hand.LEFT && inputSource == SteamVR_Input_Sources.LeftHand)
            {
                gameObject.GetComponent<MyLayHandler>().enabled = true;
            }

            if (Experiment_Setting.instance.user_main_hand == Main_Hand.RIGHT && inputSource == SteamVR_Input_Sources.RightHand)
            {
                gameObject.GetComponent<MyLayHandler>().enabled = true;
            }
        }
        else
        {
            if (Experiment_Setting.instance.user_main_hand == Main_Hand.LEFT && inputSource == SteamVR_Input_Sources.LeftHand)
            {
                gameObject.GetComponent<MyLayHandler>().enabled = true;
            }
            else if(Experiment_Setting.instance.user_main_hand == Main_Hand.LEFT && inputSource == SteamVR_Input_Sources.RightHand)
            {
                if(Experiment_Setting.instance.task_type == Task_Type.DOCKING_TASK)
                {
                    gameObject.GetComponent<DockingTrialStopper>().enabled = true;
                }
            }

            if (Experiment_Setting.instance.user_main_hand == Main_Hand.RIGHT && inputSource == SteamVR_Input_Sources.RightHand)
            {
                gameObject.GetComponent<MyLayHandler>().enabled = true;
            }
            else if (Experiment_Setting.instance.user_main_hand == Main_Hand.RIGHT && inputSource == SteamVR_Input_Sources.LeftHand)
            {
                if (Experiment_Setting.instance.task_type == Task_Type.DOCKING_TASK)
                {
                    gameObject.GetComponent<DockingTrialStopper>().enabled = true;
                }
            }
        }
    }

    public Vector3 HOMERPosition(Vector3 targetPos, Vector3 originPos, Vector3 controllerPos, Vector3 bodyPos, float distance, Vector3 targetFirstPosition)
    {
        float dist = (Vector3.Distance(originPos, bodyPos));
        float distance2 = (Vector3.Distance(targetFirstPosition, bodyPos));
        float ratio = distance2/dist;

        Vector3 f = controllerPos - originPos;
        Vector3 f2 = Vector3.Normalize(targetFirstPosition - controllerPos);

        return new Vector3(
            targetFirstPosition.x + f.x * ratio ,
            targetFirstPosition.y + f.y * ratio ,
            targetFirstPosition.z + f.z * ratio 
        );
    }

    public Vector3 GoGoPosition(Vector3 controllerPos, Vector3 originPos, float coefficient, float offset)
    {
        float dist = Vector3.Distance(controllerPos, originPos);
        Vector3 uv = Vector3.Normalize(controllerPos - originPos);
        // Debug.Log("distance:" + dist);
        float len = dist;

        if (dist > offset)
        {
            // return (dist - offset) * coefficient * Vector3.Normalize(controllerPos - originPos);
            // 100 = (m -> cm)^2
            len  = dist + Mathf.Pow((dist - offset) ,2) * 100* coefficient;
        }

         return uv * len;
    }

    public Vector3 LinearOffsetPosition(Vector3 controllerPos, Vector3 bodyPos, Vector3 headPos, float coefficient)
    {   
        float dist = Vector3.Distance(controllerPos, bodyPos);
        Vector3 uv = Vector3.Normalize(controllerPos - bodyPos);

        return dist * coefficient * uv;
    }

    public Vector3 GetCurControllerPos(){
        Vector3 skeletonPosition = skeletonAction.GetLocalPosition();

        return this.transform.parent.TransformPoint(skeletonPosition);
    }

    public void CallPickUp(){
        transform.GetComponent<Hand>().Pickup();
    }

    protected override void UpdatePose()
    {
        if (skeletonAction == null)
            return;

        if (isPortalHand) return;

        Vector3 skeletonPosition = skeletonAction.GetLocalPosition();
        Quaternion skeletonRotation = skeletonAction.GetLocalRotation();

        if (origin == null || origin.tag == "MainCamera")
        {
            if (this.transform.parent != null)
            {
                switch (cur_selection_type)
                {
                    // Linear Offset
                    case Selection_Technique.GOGO:
                        skeletonPosition = this.transform.parent.TransformPoint(skeletonPosition);
                        Vector3 bodyPos = CameraUtil.instance.GetChestPos();
                        float LinearOffset = Experiment_Setting.instance.far_distance / (Experiment_Setting.instance.underArm_length_meter);
                        Vector3 newPos = LinearOffsetPosition(skeletonPosition, bodyPos, Camera.main.transform.position, LinearOffset);
                        
                        skeletonPosition = newPos + bodyPos; // + skeletonPosition;

                        skeletonRotation = this.transform.parent.rotation * skeletonRotation;
                        break;
                    case Selection_Technique.RAYCASTING:
                        ArrayList distanceFromHandToTarget = transform.GetComponent<MyLayHandler>().MoveHandToDistantObject();

                        if(distanceFromHandToTarget[0] == null){
                            if(targetObj!=null){
                                transform.GetComponent<Hand>().Drop();
                                transform.GetComponent<Hand>().RemoveContactInteractables(targetObj);
                                transform.GetComponent<Hand>().enabled = false;
                                targetObj = null;
                            }
                        }else{
                            
                            skeletonPosition = this.transform.parent.TransformPoint(skeletonPosition);
                            
                            bodyPos = CameraUtil.instance.GetChestPos();
                                                    
                            newPos = HOMERPosition(((GameObject)distanceFromHandToTarget[0]).transform.position, 
                                                    (Vector3)distanceFromHandToTarget[2],
                                                    skeletonPosition ,
                                                    bodyPos,
                                                     (float)distanceFromHandToTarget[1],
                                                     (Vector3)distanceFromHandToTarget[3]);

                            skeletonPosition = newPos;

                            if(!transform.GetComponent<Hand>().enabled){
                                targetObj = ((GameObject)distanceFromHandToTarget[0]);

                                transform.GetComponent<Hand>().enabled = true;
                                transform.GetComponent<Hand>().AddContactInteractables(((GameObject)distanceFromHandToTarget[0]));
                               
                                Invoke("CallPickUp", 0.1f);
                            }

                           
                            skeletonRotation = this.transform.parent.rotation * skeletonRotation;
                        }
                        break;
                    //case Selection_Technique.IN_MENU_STATE:
                    case Selection_Technique.PORTAL:
                    case Selection_Technique.DIRECT:
                    default:
                        skeletonPosition = this.transform.parent.TransformPoint(skeletonPosition);
                        skeletonRotation = this.transform.parent.rotation * skeletonRotation;
                        break;
                }

                virtualCursorPos = skeletonPosition;
            }
        }
        else
        {
            skeletonPosition = origin.TransformPoint(skeletonPosition);
            skeletonRotation = origin.rotation * skeletonRotation;
        }

        if (skeletonAction.poseChanged)
        {
            if (onTransformChanged != null)
                onTransformChanged.Invoke(this, inputSource);
            if (onTransformChangedEvent != null)
                onTransformChangedEvent.Invoke(this, inputSource);
        }

        this.transform.position = skeletonPosition;
        this.transform.rotation = skeletonRotation;

        if (onTransformUpdated != null)
            onTransformUpdated.Invoke(this, inputSource);
    }
}
