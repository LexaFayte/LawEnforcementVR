using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonInteraction : MonoBehaviour {

    private void OnCollisionEnter(Collision collision)
    {
        GetComponent<UnityEngine.UI.Image>().color = Color.red;
    }

    private void OnTriggerEnter(Collider other)
    {
        GetComponent<UnityEngine.UI.Image>().color = Color.red;
    }
}
