using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraUtil : MonoBehaviour
{
    public Transform rightShoulder;
    public Transform leftShoulder;

    public GameObject chest;

   public static CameraUtil instance;

   public void Awake(){
       instance = this;
   }

   public Vector3 GetChestPos(){
        return (rightShoulder.position + leftShoulder.position)/2;
   }

   private void Update() {
    chest.transform.position = GetChestPos();    
   }
}
