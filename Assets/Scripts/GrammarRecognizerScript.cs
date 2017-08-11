using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using UnityEngine.UI;
using System.Linq;

public class GrammarRecognizerScript : MonoBehaviour {

    GrammarRecognizer grammarRec;
    public GameObject target;
    public Text feedbackUI;
    private float dialogueScore;
    private DialogueManager DM;
    private SuspectControllerFSM scFSM;

    private void Start()
    {
        DM = GetComponent<DialogueManager>();
        scFSM = target.GetComponent<SuspectControllerFSM>();
        grammarRec = new GrammarRecognizer(Application.streamingAssetsPath + "/XML/grammar_OfficeT1.xml");
        grammarRec.OnPhraseRecognized += OnGrammerRecognized;
        grammarRec.Start();       
        
        Debug.Log("grammar started\nStreaming asset path: " + grammarRec.GrammarFilePath);
    }

    /// <summary>
    /// When grammar from the grXML file was recognized, this event is triggered
    /// </summary>
    /// <param name="args">arguments of the grammar recognized</param>
    void OnGrammerRecognized(PhraseRecognizedEventArgs args)
    {
        List<string> semantics = new List<string>();
        string tokens = "";
        dialogueScore = 0;

        Debug.Log("Phrase recognized: " + args.text);

        SemanticMeaning[] meanings = args.semanticMeanings;



        if (meanings.Length > 0)
        {
            for (int i = 0; i < meanings.Length; ++i)
            {
                Debug.Log("Semantic meaning(s)\nValue:" + meanings[i].values[0]);
                tokens += meanings[i].values[0];
            }

            //Tier 1 of scenario
            if (!scFSM.Tier2)
            {

                SemanticsParser.parse(tokens, ref semantics);
                dialogueScore = DM.evaluateDialogue(semantics);
                Debug.Log("Dialogue Score: " + dialogueScore);
                scFSM.UpdateFSM(dialogueScore, semantics);
                float overallScore = scFSM.getAggroScore();
                feedbackUI.text = args.text + "\nscore: " + dialogueScore + "\nCurrent Aggro: " + overallScore;

            }
            else //Tier 2 of scenario
            {
                SemanticsParser.parseNonDistinct(tokens, ref semantics);
                dialogueScore = DM.evaluateDialogue(semantics);
                semantics = semantics.Distinct().ToList();
                scFSM.UpdateT2Suspect(dialogueScore, semantics);
            }
        }
    }

    /// <summary>
    /// on swapping to T2 of the scenario, stop current Grammar Recognizer,
    /// dispose of current resources for it (grXML), assign T2 grXML file,
    /// and start the Grammar Recognizer.
    /// </summary>
    public void initGrammarT2()
    {
        grammarRec.Stop();
        grammarRec.Dispose();
        grammarRec = new GrammarRecognizer(Application.streamingAssetsPath + "/XML/grammar_OfficeT2.xml");
        grammarRec.OnPhraseRecognized += OnGrammerRecognized;
        grammarRec.Start();

        Debug.Log("grammar T2 started\nStreaming asset path: " + grammarRec.GrammarFilePath);
    }

    /// <summary>
    /// on exiting the scenario, stop the Grammar Recognizer and dispose of resources.
    /// </summary>
    public void exitScenario()
    {
        grammarRec.Stop();
        grammarRec.Dispose();
    }
}
