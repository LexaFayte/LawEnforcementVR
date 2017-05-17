using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class TargetController : MonoBehaviour {

    public GameObject [] Targets;
    public GameObject controllerRight;

    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device device;
    private SteamVR_TrackedController controller;

    [SerializeField]
    private string[] keyWords;
    private KeywordRecognizer recognizer;

    // Use this for initialization
    void Awake () {
        controller = controllerRight.GetComponent<SteamVR_TrackedController>();
        controller.MenuButtonClicked += MenuPressed;
        trackedObj = controllerRight.GetComponent<SteamVR_TrackedObject>();

        keyWords = new string[1];
        keyWords[0] = "Stop";
        recognizer = new KeywordRecognizer(keyWords);
        recognizer.OnPhraseRecognized += onRecognition;
        recognizer.Start();

    }
	
    private void onRecognition(PhraseRecognizedEventArgs e)
    {
        if(e.text == keyWords[0])
        {
            Targets[0].transform.Translate(new Vector3(0,0, -5), Space.World);
        }
    }




    private void MenuPressed(object sender, ClickedEventArgs e)
    {
        //bring up options menu
    }
}
