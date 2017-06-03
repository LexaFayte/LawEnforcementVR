using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using UnityEngine.UI;

public class VoiceRecognizer : MonoBehaviour {

    [SerializeField]
    private string[] keyWords;
    private KeywordRecognizer recognizer;

    //audio HUD
    public Canvas AudioHUD;
    public Text HUDtext;

    private float FADE_LERP = 0.025f;
    private int FADE_NO = 4;

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
    /// coroutine for fading in and out a UI canvas and it's elements
    /// </summary>
    /// <param name="cg">the canvas group of the canvas to fade</param>
    /// <returns></returns>
    IEnumerator FadeFlash(CanvasGroup cg, Canvas canvas)
    {
        for(int i = 0; i < FADE_NO; ++i)
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
            case "Start":
                if (!GetComponent<PauseController>().IsPaused)
                {
                   HUDtext.text = "Start";
                    StartCoroutine(FadeFlash(AudioHUD.GetComponent<CanvasGroup>(), AudioHUD));
                    GetComponent<TargetController>().Invoke("startTargetSequence", 1.5f);
                }
                break;
            case "Stop":
                if (!GetComponent<PauseController>().IsPaused)
                {
                    HUDtext.text = "Stop";
                    StartCoroutine(FadeFlash(AudioHUD.GetComponent<CanvasGroup>(), AudioHUD));
                    GetComponent<TargetController>().Invoke("stopTargetSequence", 1f);
                }
                break;
            case "Reset":
                if (!GetComponent<PauseController>().IsPaused)
                {
                    HUDtext.text = "Reset";
                    StartCoroutine(FadeFlash(AudioHUD.GetComponent<CanvasGroup>(), AudioHUD));
                    GetComponent<TargetController>().Invoke("resetTargetSequence",1f);
                }
                break;
            case "Pause":
                HUDtext.text = "Pause";
                StartCoroutine(FadeFlash(AudioHUD.GetComponent<CanvasGroup>(), AudioHUD));
                GetComponent<PauseController>().MenuCalled(true);
                break;
            case "Resume":
                HUDtext.text = "Resume";
                StartCoroutine(FadeFlash(AudioHUD.GetComponent<CanvasGroup>(), AudioHUD));
                GetComponent<PauseController>().MenuCalled(false);
                break;
            case "Exit":
                HUDtext.text = "Exit";
                StartCoroutine(FadeFlash(AudioHUD.GetComponent<CanvasGroup>(), AudioHUD));
                GetComponent<PauseController>().ExitCalled();          
                break;
        }
    }

}
