using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(SteamVR_TrackedObject))]
public class PickUpParent : MonoBehaviour {


    SteamVR_TrackedObject trackedObj;
    SteamVR_Controller.Device device;
    public Transform sphere;


    void Awake ()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();		
	}
	
	//for physics
	void FixedUpdate ()
    {
        device = SteamVR_Controller.Input((int)trackedObj.index);

        if (device.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad))
        {
            Debug.Log("you have activated 'Touchpad' on the trigger");
            sphere.transform.position = new Vector3(-.919f, 0.22f, -0.243f);
            sphere.GetComponent<Rigidbody>().velocity = Vector3.zero;
            sphere.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        }

    }

    private void OnTriggerStay(Collider col)
    {
        Debug.Log("You have collided with " + col.name + " and acitvated on TriggerStay");

        if(device.GetTouch(SteamVR_Controller.ButtonMask.Trigger))
        {
            col.attachedRigidbody.isKinematic = true;
            col.gameObject.transform.SetParent(gameObject.transform);
        }

        if (device.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger))
        {
            col.gameObject.transform.SetParent(null);
            col.attachedRigidbody.isKinematic = false;

            tossObject(col.attachedRigidbody);
        }
    }


    void tossObject(Rigidbody rb)
    {
        Transform origin = trackedObj.origin ? trackedObj.origin : trackedObj.transform.parent;
        if (origin != null)
        {
            rb.velocity = origin.TransformVector(device.velocity);
            rb.angularVelocity = origin.TransformVector(device.angularVelocity);
        }
        else
        {
            rb.velocity = device.velocity;
            rb.angularVelocity = device.angularVelocity;
        }
    }

}
