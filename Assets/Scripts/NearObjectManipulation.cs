using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class NearObjectManipulation : MonoBehaviour
{
    
    private FixedJoint m_Joint = null;
    public SteamVR_Action_Boolean doGrab;
    public SteamVR_Input_Sources handType;
    public GameObject referredHand;

    private Interactable m_CurrentInteractable = null;
    public List<Interactable> m_ContactInteractables = new List<Interactable>();
    private void Awake()
    {
        m_Joint = GetComponent<FixedJoint>();
    }

    // Update is called once per frame
    void Update()
    {
        // Down
        if (doGrab.GetStateDown(handType))
        {
            //Debug.Log(handType + " Trigger Down");
            Pickup();
        }

        //Up
        if (doGrab.GetStateUp(handType))
        {
            //Debug.Log(handType + " Trigger Up");
            Drop();
        }

    }

    public void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("MovableObject")) return;

        m_ContactInteractables.Add(other.gameObject.GetComponent<Interactable>());

    }

    public void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("MovableObject")) return;

        m_ContactInteractables.Remove(other.gameObject.GetComponent<Interactable>());
    }

    public void Pickup()
    {
        // Get nearest
        m_CurrentInteractable = GetNearestInteractable();

        // Null check
        if (!m_CurrentInteractable) return;

        //Debug.Log(m_CurrentInteractable.m_portalHand);
        //already held, check
        if (m_CurrentInteractable.m_portalHand)
        {
            m_CurrentInteractable.m_portalHand.Drop();
        }

        // position
        m_CurrentInteractable.transform.position = transform.position;

        //attach
        Rigidbody targetBody = m_CurrentInteractable.GetComponent<Rigidbody>();
        m_Joint.connectedBody = targetBody;

        //set active hand
        m_CurrentInteractable.m_portalHand = this;

    }

    public void Drop()
    {
        // Null check
        if (!m_CurrentInteractable) return;

        // apply velocity
        Rigidbody targetBody = m_CurrentInteractable.GetComponent<Rigidbody>();
        //targetBody.velocity = transform.GetComponent<Rigidbody>().velocity;
        targetBody.velocity = referredHand.GetComponent<SteamVR_Behaviour_Pose>().GetVelocity();
        
        targetBody.angularVelocity = referredHand.GetComponent<SteamVR_Behaviour_Pose>().GetAngularVelocity();

        // detach
        m_Joint.connectedBody = null;

        // clear
        m_CurrentInteractable.m_portalHand = null;
        m_CurrentInteractable = null;
    }

    private Interactable GetNearestInteractable()
    {
        Interactable nearest = null;

        float minDistance = float.MaxValue;
        float distance = 0.0f;

        foreach (Interactable interactable in m_ContactInteractables)
        {
            distance = (interactable.transform.position - transform.position).sqrMagnitude;

            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = interactable;
            }

        }

        return nearest;
    }


}
