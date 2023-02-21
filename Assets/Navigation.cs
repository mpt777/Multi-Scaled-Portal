using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using Valve.VR;

public enum Portal_Navigation_Technique
{
    STATIONARY,
    JOYSTICK

}

public class Navigation : MonoBehaviour
{
    public SteamVR_Action_Vector2 touchpadInput;
    public Portal_Navigation_Technique portalTechnique;
    private Rigidbody portalBody;

    public float speed = 1;
    private float fallingSpeed = -9.81f;

    private void Start()
    {
        portalBody = GetComponent<Rigidbody>();

        //Fixes the offset from roomscale when everything starts up
        //Valve.VR.OpenVR.Chaperone.ResetZeroPose(ETrackingUniverseOrigin.TrackingUniverseStanding);
        Debug.Log("LOL");

    }

    void FixedUpdate()
    {
        if (portalTechnique == Portal_Navigation_Technique.JOYSTICK)
        {
            MoveByJoystick();
        }
    }

    private void MoveByJoystick()
    {
        if (touchpadInput.axis.magnitude > 0.1f)
        {
            //Vector3 direction = Player.instance.hmdTransform.TransformDirection(new Vector3(touchpadInput.axis.x, 0, touchpadInput.axis.y));
            Vector3 inputDirection = new Vector3(touchpadInput.axis.x, 0, touchpadInput.axis.y);

            Quaternion portalYaw = Quaternion.Inverse(Quaternion.Euler(0, transform.eulerAngles.y, 0));

            Vector3 direction = portalYaw * inputDirection;

            portalBody.MovePosition(transform.position + direction * Time.fixedDeltaTime * speed);
            //portalBody.MovePosition(Vector3.up * Time.fixedDeltaTime * fallingSpeed);

            //characterController.Move(speed * Time.deltaTime * Vector3.ProjectOnPlane(direction, Vector3.up) - new Vector3(0, 9.81f,0)*Time.deltaTime);
        }
    }
}
