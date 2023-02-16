/*
 * Copyright (c) 2017 VR Stuff
 */

using UnityEngine;
using Valve.VR;

using Valve.VR.InteractionSystem;


public class ControllerGrabObject : MonoBehaviour
{
	private SteamVR_TrackedObject trackedObj;

	private GameObject collidingObject;
	private GameObject objectInHand;

	public SteamVR_Action_Boolean m_GrabAction = null;
	public SteamVR_Behaviour_Pose Controller;
	

	void Awake()
	{

	trackedObj = GetComponent<SteamVR_TrackedObject>();
	}	

	public void OnTriggerEnter(Collider other){
		SetCollidingObject(other);
	}

	public void OnTriggerStay(Collider other)
	{
		SetCollidingObject(other);
	}

	public void OnTriggerExit(Collider other)
	{
		if (!collidingObject)
		{
			return;
		}

		collidingObject = null;
	}

	private void SetCollidingObject(Collider col)
	{
		if (collidingObject || !col.GetComponent<Rigidbody>())
		{
			return;
		}

		collidingObject = col.gameObject;
	}

	void Update() {
		//if (Controller.GetHairTriggerDown())
		if (m_GrabAction.GetStateDown(Controller.inputSource))
		{
			if (collidingObject)
			{
				GrabObject();
			}
		}

		if (m_GrabAction.GetStateUp(Controller.inputSource))
		{
			if (objectInHand)
			{
				ReleaseObject();
			}
		}
	}

	private void GrabObject()
	{
		objectInHand = collidingObject;
		collidingObject = null;
		var joint = AddFixedJoint();
		joint.connectedBody = objectInHand.GetComponent<Rigidbody>();
	}

	private FixedJoint AddFixedJoint()
	{
		FixedJoint fx = gameObject.AddComponent<FixedJoint>();
		fx.breakForce = 20000;
		fx.breakTorque = 20000;
		return fx;
	}

	private void ReleaseObject()
	{
		if (GetComponent<FixedJoint>())
		{
		GetComponent<FixedJoint>().connectedBody = null;
		Destroy(GetComponent<FixedJoint>());

		objectInHand.GetComponent<Rigidbody>().velocity = Controller.GetVelocity();
		objectInHand.GetComponent<Rigidbody>().angularVelocity = Controller.GetAngularVelocity();

		}
		objectInHand = null;
	}
}
