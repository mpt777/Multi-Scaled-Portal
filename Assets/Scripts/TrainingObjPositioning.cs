using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingObjPositioning : MonoBehaviour
{
    public Target_Distance dist;

    void Start()
    {
        if(dist == Target_Distance.NEAR){
            Vector3 tempPos = Vector3.Normalize(transform.position) * Experiment_Setting.instance.near_distance;
            transform.position = new Vector3(tempPos.x, CameraUtil.instance.GetChestPos().y,tempPos.z);
        }

        if(dist == Target_Distance.MID){
            Vector3 tempPos = Vector3.Normalize(transform.position)* Experiment_Setting.instance.middle_distance;
            transform.position = new Vector3(tempPos.x, CameraUtil.instance.GetChestPos().y,tempPos.z);
        }

        if(dist == Target_Distance.FAR){
            Vector3 tempPos = Vector3.Normalize(transform.position) * Experiment_Setting.instance.far_distance;
            transform.position = new Vector3(tempPos.x, CameraUtil.instance.GetChestPos().y,tempPos.z);
        }

    }

}
