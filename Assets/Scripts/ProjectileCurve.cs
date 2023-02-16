using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileCurve : MonoBehaviour
{
    public static ProjectileCurve instance; 
    public GameObject teleport_target_obj;

    public Material mr_teleport_target_success;
    public Material mr_teleport_target_fail;
    float g;
    float radianAngle;
    // Start is called before the first frame update

    private void Awake() {
        instance = this;
        g = Mathf.Abs(Physics2D.gravity.y);
    }

    public Vector3 GetTeleportTargetPos(){
        return teleport_target_obj.transform.position;
    }

    public void HideTeleportTarget(){
        teleport_target_obj.SetActive(false);
    }

    public void UpdateTeleportTargetPos(Vector3 pos1, Vector3 pos2){
        if(!teleport_target_obj.activeSelf){
            teleport_target_obj.SetActive(true);
        }

        if(pos1 == pos2){
            teleport_target_obj.transform.position = pos1;
            
            teleport_target_obj.GetComponent<Renderer>().material = mr_teleport_target_fail;

            Vector3 groundPos = new Vector3(pos1[0], 0, pos1[2]);
            float angle = Vector3.Angle(groundPos, transform.forward);

            if(angle < 45 && angle > -45){
                teleport_target_obj.transform.eulerAngles = new Vector3(0, 90, 90);
            }else if(angle > 135 && angle < 225){
                teleport_target_obj.transform.eulerAngles = new Vector3(0, 90, 90);
            }else{
                teleport_target_obj.transform.eulerAngles = new Vector3(0, 0, 90);
            }

            

        }else{
            teleport_target_obj.transform.position = pos1;
            teleport_target_obj.transform.eulerAngles = new Vector3(0,0, 0);
             teleport_target_obj.GetComponent<Renderer>().material = mr_teleport_target_success;
        }
        

    }

    public Vector3[] CalculateArcArray(Vector3 velocity, int resolution, Transform startPoint){
        Vector3[] positions = new Vector3[resolution+1];

        // float flightDuration = (2 * velocity.y) / g;
        float flightDuration = (velocity.y + Mathf.Sqrt(velocity.y*velocity.y + 2*g*startPoint.position.y))/ g;
        float stepTime = flightDuration / resolution;

        // float maxDistance =  (velocity)*Mathf.Cos(radianAngle)/g* (velocity * Mathf.Sin(radianAngle) + Mathf.Sqrt(velocity*velocity*Mathf.Sin(radianAngle)+2*g*startPoint.position.y));

        for(int i=0; i<resolution+1; i++){
            
            float stepTimePassed = (float) i * stepTime;
            
            Vector3 movementVector = new Vector3(
                velocity.x*stepTimePassed,
                velocity.y*stepTimePassed - 0.5f * g*stepTimePassed*stepTimePassed,
                velocity.z*stepTimePassed
            );

            positions[i] = movementVector + startPoint.position;
        }

        return positions;
    }


    // calcaulate height, distance
     public Vector3 CalculateArcPoint(float t, float maxDistance, float velocity, Transform startPoint, Vector3 angles){
        // need to multiply cos(theta) relative to y axis rotation
        float x = startPoint.position.x + (t * maxDistance) ;        
        // y and z has bug.
        float y = startPoint.position.y + x * Mathf.Tan(radianAngle) - ((g*x*x)/(2 * (velocity * Mathf.Cos(radianAngle))*(velocity * Mathf.Cos(radianAngle))));
        float z = startPoint.position.z + (t * maxDistance) ;
        return new Vector3(x, y, z) ;
    }
   
}
