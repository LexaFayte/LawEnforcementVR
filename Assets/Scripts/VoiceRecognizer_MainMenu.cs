using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class VoiceRecognizer_MainMenu : MonoBehaviour {

    [SerializeField]
    private string[] keyWords_MM;
    private KeywordRecognizer recognizer_MM;
    private bool scenarioMenu = false;

    public GameObject MM_Container;

    private void Awake()
    {
        //set up voice recognition
        keyWords_MM = new string[5];
        keyWords_MM[0] = "Scenarios";
        keyWords_MM[1] = "Range";
        keyWords_MM[2] = "Quit";
        keyWords_MM[3] = "Back";
        keyWords_MM[4] = "Domestic Dispute";
        recognizer_MM = new KeywordRecognizer(keyWords_MM);
        recognizer_MM.OnPhraseRecognized += onRecognition;
        recognizer_MM.Start();
    }

    /// <summary>
    /// event for when a phrase or word is recognized
    /// </summary>
    /// <param name="e"></param>
    private void onRecognition(PhraseRecognizedEventArgs e)
    {

        switch (e.text)
        {
            case "Scenarios":
                //swap to scenarios menu
                scenarioMenu = GetComponent<MainMenusController>().menuTransition(false);
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
            case "Back":
                if(scenarioMenu)
                {
                    //transition back
                    scenarioMenu = GetComponent<MainMenusController>().menuTransition(true);
                }
                break;
            case "Domestic Dispute":
                if(scenarioMenu)
                {

                }
                break;
        }
    }

}
