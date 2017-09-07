using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using UnityEngine.UI;

public class VoiceRecognizer_MainMenu : MonoBehaviour {

    [SerializeField]
    private string[] keyWords_MM;
    private KeywordRecognizer recognizer_MM;
    private MainMenusController MMC;
    private CanvasGroup AudioCG;

    public GameObject MM_Container;
    //audio HUD
    public Canvas AudioHUD;
    public Text HUDtext;

    private float FADE_LERP = 0.025f;
    private int FADE_NO = 4;

    private void Awake()
    {
        MMC = GetComponent<MainMenusController>();
        AudioCG = AudioHUD.GetComponent<CanvasGroup>();
        //set up voice recognition
        keyWords_MM = new string[6];
        keyWords_MM[0] = "Scenarios";
        keyWords_MM[1] = "Range";
        keyWords_MM[2] = "Quit";
        keyWords_MM[3] = "Back";
        keyWords_MM[4] = "Trespass";
        keyWords_MM[5] = "Instructions";
        recognizer_MM = new KeywordRecognizer(keyWords_MM);
        recognizer_MM.OnPhraseRecognized += onRecognition;
        recognizer_MM.Start();
    }


    /// <summary>
    /// coroutine for fading in and out a UI canvas and it's elements
    /// </summary>
    /// <param name="cg">the canvas group of the canvas to fade</param>
    /// <returns></returns>
    IEnumerator FadeFlash(CanvasGroup cg, Canvas canvas)
    {
        for (int i = 0; i < FADE_NO; ++i)
        {
            if (i % 2 == 0)
            {
                while (cg.alpha != 1)
                {
                    cg.alpha += FADE_LERP;//change FADE_LERP
                    yield return 0;
                }
            }
            else
            {
                while (cg.alpha != 0)
                {
                    cg.alpha -= FADE_LERP;//change FADE_LERP
                    yield return 0;
                }
            }
        }
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
                HUDtext.text = "Scenarios";
                StartCoroutine(FadeFlash(AudioCG, AudioHUD));
                MMC.menuTransition(MENU.SCENARIO);
                break;
            case "Range":
                //load the target range scene
                HUDtext.text = "Range";
                StartCoroutine(FadeFlash(AudioCG, AudioHUD));
                MMC.Invoke("LoadRange", 1.5f);
                break;
            case "Quit":
                //Quit the application
                HUDtext.text = "Quit";
                StartCoroutine(FadeFlash(AudioCG, AudioHUD));
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
                break;
            case "Back":
                if(MMC.CurrentMenu == MENU.SCENARIO)
                {
                    //transition back to the main menu
                    HUDtext.text = "Back";
                    StartCoroutine(FadeFlash(AudioCG, AudioHUD));
                    MMC.menuTransition(MENU.MAIN);
                }
                else if(MMC.CurrentMenu == MENU.INSTRUCTION)
                {
                    //transition back to the scenario menu
                    HUDtext.text = "Back";
                    StartCoroutine(FadeFlash(AudioCG, AudioHUD));
                    MMC.menuTransition(MENU.SCENARIO);
                }
                break;
            case "Trespass":
                if(MMC.CurrentMenu == MENU.SCENARIO)
                {
                    HUDtext.text = "Trespass";
                    StartCoroutine(FadeFlash(AudioCG, AudioHUD));
                    MMC.CurrectScenarioID = 2;
                    MMC.Invoke("LoadScenario", 1.5f);
                }
                break;
            case "Instructions":
                if(MMC.CurrentMenu == MENU.SCENARIO)
                {
                    //transition back to the scenario menu
                    HUDtext.text = "Instructions";
                    StartCoroutine(FadeFlash(AudioCG, AudioHUD));
                    MMC.menuTransition(MENU.INSTRUCTION);
                }
                break;
        }
    }

}
