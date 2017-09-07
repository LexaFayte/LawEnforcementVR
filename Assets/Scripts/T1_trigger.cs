using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class T1_trigger : MonoBehaviour {

    public GameObject suspect;
    public TRIGGER t;

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Triggered!");
        suspect.GetComponent<SuspectControllerFSM>().triggerHit(t);
        Invoke("deactivate", 0.5f);
    }

    private void deactivate()
    {
        gameObject.SetActive(false);
    }
}
