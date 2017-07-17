﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using UnityEngine.UI;

public class GrammarRecognizerScript : MonoBehaviour {

    GrammarRecognizer grammarRec;
    public GameObject target;
    public Text feedbackUI;
    private float dialogueScore;

    private void Start()
    {
        grammarRec = new GrammarRecognizer(Application.streamingAssetsPath + "/XML/grammar_OfficeT1.xml", ConfidenceLevel.High);
        grammarRec.OnPhraseRecognized += OnGrammerRecognized;
        grammarRec.Start();       
        
        Debug.Log("grammar started\nStreaming asset path: " + grammarRec.GrammarFilePath);
    }

    void OnGrammerRecognized(PhraseRecognizedEventArgs args)
    {
        List<string> semantics = new List<string>();
        string tokens = "";
        dialogueScore = 0;

        Debug.Log("Phrase recognized: " + args.text);

        SemanticMeaning[] meanings = args.semanticMeanings;


        
        if(meanings.Length > 0)
        {
            for (int i = 0; i < meanings.Length; ++i)
            {
                Debug.Log("Semantic meaning(s)\nValue:" + meanings[i].values[0]);
                tokens += meanings[i].values[0];
            }

            SemanticsParser.parse(tokens, ref semantics);
            dialogueScore = this.GetComponent<DialogueManager>().evaluateDialogue(semantics);
            Debug.Log("Dialogue Score: " + dialogueScore);
            target.GetComponent<SuspectControllerFSM>().UpdateFSM(dialogueScore, semantics);
            float overallScore = target.GetComponent<SuspectControllerFSM>().getAggroScore();
            feedbackUI.text = args.text + "\nscore: " + dialogueScore + "\nCurrent Aggro: " + overallScore;
            
        }
    }
}
