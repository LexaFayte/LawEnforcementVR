using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class VoiceRecognizer : MonoBehaviour {

    [SerializeField]
    private string[] keyWords;
    private KeywordRecognizer recognizer;

    private void Awake()
    {
        //set up voice recognition
        keyWords = new string[6];
        keyWords[0] = "Start";
        keyWords[1] = "Stop";
        keyWords[2] = "Reset";
        keyWords[3] = "Pause";
        keyWords[4] = "Resume";
        keyWords[5] = "Exit";
        recognizer = new KeywordRecognizer(keyWords);
        recognizer.OnPhraseRecognized += onRecognition;
        recognizer.Start();
    }

    /// <summary>
    /// event for when a phrase or word is recognized
    /// </summary>
    /// <param name="e"></param>
    private void onRecognition(PhraseRecognizedEventArgs e)
    {

        switch (e.text)
        {
            case "Start":
                if(!GetComponent<PauseController>().IsPaused)
                    GetComponent<TargetController>().startTargetSequence();
                break;
            case "Stop":
                if (!GetComponent<PauseController>().IsPaused)
                    GetComponent<TargetController>().stopTargetSequence();
                break;
            case "Reset":
                if (!GetComponent<PauseController>().IsPaused)
                    GetComponent<TargetController>().resetTargetSequence();
                break;
            case "Pause":
                GetComponent<PauseController>().MenuCalled(true);
                break;
            case "Resume":
                GetComponent<PauseController>().MenuCalled(false);
                break;
            case "Exit":
                GetComponent<PauseController>().ExitCalled();          
                break;
        }
    }

}
