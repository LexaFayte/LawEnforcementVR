using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DialogueManager : MonoBehaviour {

    private Dictionary<string, float> dialogueText;
	// Use this for initialization
	void Awake () {
        LoadDialogueText("Office_T1.json");
	}
	
	public void LoadDialogueText(string filename)
    {
        dialogueText = new Dictionary<string, float>();
        string filepath = Path.Combine(Application.streamingAssetsPath, filename);

        if(File.Exists(filepath))
        {
            string jsonData = File.ReadAllText(filepath);
            DialogueData data = JsonUtility.FromJson<DialogueData>(jsonData);

            for (int i = 0; i < data.items.Length; ++i)
            {
                dialogueText.Add(data.items[i].key, data.items[i].value);
            }

            Debug.Log("Json loaded");
            //int a = 0;
        }
    }

    public float evaluateDialogue(List<string> semantics)
    {
        float result = 0;

        if (semantics.Count > 0)
        {
            for (int i = 0; i < semantics.Count; ++i)
            {
                result += dialogueText[semantics[i]];
            }
        }
        return result;
    }
}
