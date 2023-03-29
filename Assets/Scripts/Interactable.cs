using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Interactable : MonoBehaviour
{
    [HideInInspector]
    public Hand m_ActiveHand = null;
    [HideInInspector]
    public NearObjectManipulation m_portalHand = null;

    public Interactable linkedObj;
    public GameObject hand;

    public void ClearLinkedObject()
    {
        linkedObj.gameObject.GetComponent<Interactable>().linkedObj = null;
        linkedObj = null;
        hand = null;
    }

}
