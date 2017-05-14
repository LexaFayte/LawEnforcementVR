using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[RequireComponent(typeof(SteamVR_TrackedObject))]
public class ParentFixedJoint : MonoBehaviour {


    public Transform sphere;
    public Rigidbody rigidBodyAttachPoint;

    FixedJoint fixedJoint;
    SteamVR_TrackedObject trackedObj;
    SteamVR_Controller.Device device;

    void Awake ()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        
    }


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
        if (fixedJoint == null && device.GetTouch(SteamVR_Controller.ButtonMask.Trigger))
        {
            fixedJoint = col.gameObject.AddComponent<FixedJoint>();
            fixedJoint.connectedBody = rigidBodyAttachPoint;
        }
        else if(fixedJoint != null && device.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger))
        {
            GameObject go = fixedJoint.gameObject;
            Rigidbody rigidbody = go.GetComponent<Rigidbody>();
            Object.Destroy(fixedJoint);
            fixedJoint = null;

            tossObject(rigidbody);
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
