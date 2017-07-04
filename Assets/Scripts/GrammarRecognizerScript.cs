using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class GrammarRecognizerScript : MonoBehaviour {

    GrammarRecognizer grammarRec;

    private void Start()
    {
        grammarRec = new GrammarRecognizer(Application.streamingAssetsPath + "/XML/grammar_OfficeT1.xml");
        grammarRec.OnPhraseRecognized += OnGrammerRecognized;
        grammarRec.Start();       
        
        Debug.Log("grammar started\nStreaming asset path: " + grammarRec.GrammarFilePath);
    }

    void OnGrammerRecognized(PhraseRecognizedEventArgs args)
    {
        List<string> semantics = new List<string>();
        string tokens = "";
        float dialogueScore = 0;

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
        }

        //int a = 0;
    }
}
