﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using UnityEngine.UI;

public class VoiceRecognizer_MainMenu : MonoBehaviour {

    [SerializeField]
    private string[] keyWords_MM;
    private KeywordRecognizer recognizer_MM;
    private bool scenarioMenu = false;

    public GameObject MM_Container;
    //audio HUD
    public Canvas AudioHUD;
    public Text HUDtext;

    private float FADE_LERP = 0.025f;
    private int FADE_NO = 4;

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
                StartCoroutine(FadeFlash(AudioHUD.GetComponent<CanvasGroup>(), AudioHUD));
                scenarioMenu = GetComponent<MainMenusController>().menuTransition(false);
                break;
            case "Range":
                //load the target range scene
                HUDtext.text = "Range";
                StartCoroutine(FadeFlash(AudioHUD.GetComponent<CanvasGroup>(), AudioHUD));
                GetComponent<MainMenusController>().Invoke("LoadRange", 1.5f);
                break;
            case "Quit":
                //Quit the appplication
                HUDtext.text = "Quit";
                StartCoroutine(FadeFlash(AudioHUD.GetComponent<CanvasGroup>(), AudioHUD));
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Invoke("Quit", 1.5f);
#endif
                break;
            case "Back":
                if(scenarioMenu)
                {
                    //transition back
                    HUDtext.text = "Back";
                    StartCoroutine(FadeFlash(AudioHUD.GetComponent<CanvasGroup>(), AudioHUD));
                    scenarioMenu = GetComponent<MainMenusController>().menuTransition(true);
                }
                break;
            case "Domestic Dispute":
                if(scenarioMenu)
                {
                    HUDtext.text = "Domestic Dispute";
                    StartCoroutine(FadeFlash(AudioHUD.GetComponent<CanvasGroup>(), AudioHUD));
                }
                break;
        }
    }

}
