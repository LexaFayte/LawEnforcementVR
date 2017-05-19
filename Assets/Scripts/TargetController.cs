using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class TargetController : MonoBehaviour {

    public GameObject [] Targets;
    public GameObject controllerRight;

    //private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device device;
    private SteamVR_TrackedController controller;

    private bool running = false;

    // Use this for initialization
    void Awake () {
        controller = controllerRight.GetComponent<SteamVR_TrackedController>();
        controller.MenuButtonClicked += MenuPressed;
    }

    //start the targets
    public void startTargetSequence()
    {
        if (!running)
        {
            for (int i = 0; i < Targets.Length; ++i)
            {
                Targets[i].gameObject.GetComponent<TargetMove>().Invoke("Move", i * 1.2f);
            }
            running = true;
        }
    }

    //stop the targets
    public void stopTargetSequence()
    {
        if (running)
        {
            for (int i = 0; i < Targets.Length; ++i)
            {
                Targets[i].gameObject.GetComponent<TargetMove>().CancelInvoke();
                Targets[i].gameObject.GetComponent<TargetMove>().Invoke("StopMove", 0.2f);
            }
            running = false;
        }
    }

    public void resetTargetSequence()
    {

        for (int i = 0; i < Targets.Length; ++i)
        {
            Targets[i].gameObject.GetComponent<TargetMove>().Invoke("Reset", 0f);
        }
    }

    private void MenuPressed(object sender, ClickedEventArgs e)
    {
        //bring up options menu
    }
}
