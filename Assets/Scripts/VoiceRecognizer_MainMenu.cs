using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class VoiceRecognizer_MainMenu : MonoBehaviour {

    [SerializeField]
    private string[] keyWords;
    private KeywordRecognizer recognizer;

    private void Awake()
    {
        //set up voice recognition
        keyWords = new string[4];
        keyWords[0] = "Scenarios";
        keyWords[1] = "Target range";
        keyWords[2] = "Quit";
        keyWords[3] = "case one";
        recognizer = new KeywordRecognizer(keyWords);
        recognizer.OnPhraseRecognized += onRecognition;
        recognizer.Start();
    }

    private void onRecognition(PhraseRecognizedEventArgs e)
    {

        switch (e.text)
        {
            case "Scenarios":
                //scroll to the scenarios menu
                break;
            case "Target range":
                //load the target range scene
                GetComponent<MainMenusController>().LoadRange();
                break;
            case "Quit":
                //Quit the appplication
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
                break;
            case "case one":
                break;
        }
    }

}
