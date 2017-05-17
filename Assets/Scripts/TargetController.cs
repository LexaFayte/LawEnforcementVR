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

    [SerializeField]
    private string[] keyWords;
    private KeywordRecognizer recognizer;

    private bool running = false;

    // Use this for initialization
    void Awake () {
        controller = controllerRight.GetComponent<SteamVR_TrackedController>();
        controller.MenuButtonClicked += MenuPressed;
        //trackedObj = controllerRight.GetComponent<SteamVR_TrackedObject>();

        //set up voice recognition
        keyWords = new string[4];
        keyWords[0] = "Start";
        keyWords[1] = "Stop";
        keyWords[2] = "Reset";
        keyWords[3] = "Exit";
        recognizer = new KeywordRecognizer(keyWords);
        recognizer.OnPhraseRecognized += onRecognition;
        recognizer.Start();

    }
	
    private void onRecognition(PhraseRecognizedEventArgs e)
    {
        switch(e.text)
        {
            case "Start":
                startTargetSequence();
                break;
            case "Stop":
                stopTargetSequence();
                break;
            case "Reset":
                resetTargetSequence();
                break;
            case "Exit":
                break;
        }
    }

    //start the targets
    private void startTargetSequence()
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
    private void stopTargetSequence()
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

    private void resetTargetSequence()
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
