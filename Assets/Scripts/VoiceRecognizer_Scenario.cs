using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using UnityEngine.UI;

public class VoiceRecognizer_Scenario : MonoBehaviour {


    [SerializeField]
    private string[] keyWords_S;
    private KeywordRecognizer recognizer_S;
    private CanvasGroup AudioCG;

    public Canvas AudioHUD;
    public Text HUDtext;

    private float FADE_LERP = 0.025f;
    private int FADE_NO = 4;


    private void Awake()
    {
        AudioCG = AudioHUD.GetComponent<CanvasGroup>();

        //setup keywords

        keyWords_S = new string[1];
        keyWords_S[0] = "continue";
        recognizer_S = new KeywordRecognizer(keyWords_S);
        recognizer_S.OnPhraseRecognized += onRecognition;
    }

    /// <summary>
    /// start the keyword recognition system
    /// </summary>
    public void startKeyWordRecognition()
    {
        recognizer_S.Start();
    }

    /// <summary>
    /// stop and dispose of the keyword recognition system
    /// </summary>
    public void stopKeyWordRecognition()
    {
        recognizer_S.Stop();
        recognizer_S.Dispose();
    }

    /// <summary>
    /// event for when a phrase or word is recognized
    /// </summary>
    /// <param name="e"></param>
    private void onRecognition(PhraseRecognizedEventArgs e)
    {
        if(e.text == "continue")
        {
            HUDtext.text = "continue";
            StartCoroutine(FadeFlash(AudioCG, AudioHUD));
            GetComponent<ScenarioController>().ExitScenario = true;
        }
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

}
