using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GazeManager : MonoBehaviour {
    private float sightLength = 50f;
    public Text feedbackUI;
    // Update is called once per frame
    void FixedUpdate () {
        RaycastHit hit;
        Ray rayDir = new Ray(transform.position, transform.forward);

        if(Physics.Raycast(rayDir,out hit,sightLength))
        {
            
            if(hit.collider.GetType() == typeof(BoxCollider) && hit.collider.tag == "Jim")
            {
                feedbackUI.text = "\nLooking at Jim";
            }
            else
            {
                feedbackUI.text = "\nLooking at nothing";
            }
        }
	}
}
