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
public enum Portal_Rotation_Technique
{
    STATIONARY,
    JOYSTICK,
    BUTTONTOGGLE,
    ROTATION
}


public class Navigation : MonoBehaviour
{

    public PortalManager portalManager;
    public SteamVR_Action_Vector2 LeftJoystick;
    public SteamVR_Action_Vector2 RightJoyStick;

    public SteamVR_Action_Vector2 RotateStick;
    public SteamVR_Action_Boolean CenterClick;
    public Portal_Navigation_Technique portalTechnique;
    public Portal_Rotation_Technique rotationTechnique;

    public GameObject rotationController;

    private bool canRotate = true;
    private bool canMove = true;
    private bool canControllerRotate = true;
    private Rigidbody portalBody;

    public float speed = 1f;
    public float rotationSpeed = 20;
    private float fallingSpeed = -9.81f;
    private Vector3 transformScaleToRelative = new Vector3(1, 1, 1);

    private void Start()
    {
        portalBody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (portalTechnique == Portal_Navigation_Technique.JOYSTICK)
        {
            canMove = true;
        }

        if (rotationTechnique == Portal_Rotation_Technique.JOYSTICK)
        {
            canRotate = true;
            RotateStick = RightJoyStick;
        }
        if (rotationTechnique == Portal_Rotation_Technique.ROTATION)
        {
            canRotate = false;
            RotateByController();
        }
        if (rotationTechnique == Portal_Rotation_Technique.BUTTONTOGGLE)
        {
            RotateStick = LeftJoystick;
            if (CenterClick.stateDown)
            {
                canRotate = !canRotate;
            }
            canMove = !canRotate;
        }

        if (canMove)
        {
            MoveByJoystick();
        }
        if (canRotate)
        {
            RotateByJoystick();
        }

    }
    public void SetTransformScaleToRelative(Vector3 t)
    {
        transformScaleToRelative = t;
        Debug.Log(transformScaleToRelative);
    }

    private void MoveByJoystick()
    {
        if (LeftJoystick.axis.magnitude > 0.1f)
        {
            Vector3 inputDirection = new Vector3(LeftJoystick.axis.x, 0, LeftJoystick.axis.y) * -1;

            Quaternion portalYaw = Quaternion.Euler(0, transform.eulerAngles.y, 0);

            Debug.Log(transformScaleToRelative);
            Vector3 direction = Vector3.Scale(portalYaw * inputDirection, transformScaleToRelative);

            portalBody.MovePosition(transform.position + direction * Time.deltaTime * speed);

        }
    }
    private void RotateByJoystick()
    {
        if (RotateStick.axis.magnitude > 0.3f && RotateStick.axis.y < 0.4f)
        {
            transform.RotateAround(transform.position, Vector3.up, rotationSpeed * RotateStick.axis.x * Time.deltaTime);
        }
    }

    private void RotateByController()
    {

        float degUp = Vector3.Angle(portalManager.rotationController.transform.forward, Vector3.up) + 25;


        if (Math.Abs(degUp - 90) > 15)
        {
            float offsetUp = degUp - 90;
            transform.RotateAround(transform.position, Vector3.back, rotationSpeed * (offsetUp / 90) * Time.deltaTime);
        }
        float degRight = Vector3.Angle(portalManager.rotationController.transform.forward, portalManager.originPortal.transform.forward);

        if (Math.Abs(degRight-90) > 15)
        {
            float offsetRight = degRight - 90;
            transform.RotateAround(transform.position, Vector3.up, rotationSpeed * (offsetRight / 90) * Time.deltaTime);
        }
    }
}
