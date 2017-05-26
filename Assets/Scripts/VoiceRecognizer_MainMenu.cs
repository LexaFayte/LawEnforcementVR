using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class VoiceRecognizer_MainMenu : MonoBehaviour {

    [SerializeField]
    private string[] keyWords_MM;
    private KeywordRecognizer recognizer_MM;

    private void Awake()
    {
        //set up voice recognition
        keyWords_MM = new string[4];
        keyWords_MM[0] = "Scenarios";
        keyWords_MM[1] = "Range";
        keyWords_MM[2] = "Quit";
        keyWords_MM[3] = "case one";
        recognizer_MM = new KeywordRecognizer(keyWords_MM);
        recognizer_MM.OnPhraseRecognized += onRecognition;
        recognizer_MM.Start();
    }

    private void onRecognition(PhraseRecognizedEventArgs e)
    {

        switch (e.text)
        {
            case "Scenarios":
                //scroll to the scenarios menu
                break;
            case "Range":
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
