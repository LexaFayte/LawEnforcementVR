using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DialogueManager : MonoBehaviour {

    private Dictionary<string, float> dialogueText;
    private Dictionary<string, AudioClip[]> suspectDialogue;
    private Dictionary<string, string> responseMap;

    // Use this for initialization
    void Awake () {
        loadDialogueText("Office_T1.json");
        loadResponses("Office_T1_Audio.json");
        //loadSuspectDialogue();
        loadResponsesAudio("Office_T1_ClipData.json");
	}
	
    /// <summary>
    /// loads the dialogue aggravation score information
    /// from a JSON file and into a dictionary data structure
    /// </summary>
    /// <param name="filename">The name of the JSON file</param>
	public void loadDialogueText(string filename)
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

            Debug.Log("Json dialogue loaded");
            //int a = 0;
        }
    }

    /// <summary>
    /// loads the dialogue response tags information
    /// from a JSON file and into a dictionary data structure
    /// </summary>
    /// <param name="filename">The name of the JSON file</param>
    public void loadResponses(string filename)
    {
        responseMap = new Dictionary<string, string>();
        string filepath = Path.Combine(Application.streamingAssetsPath, filename);

        if (File.Exists(filepath))
        {
            string jsonData = File.ReadAllText(filepath);
            ResponseData data = JsonUtility.FromJson<ResponseData>(jsonData);

            for (int i = 0; i < data.items.Length; ++i)
            {
                responseMap.Add(data.items[i].key, data.items[i].value);
            }

            Debug.Log("Json responses loaded");
        }
    }


    /// <summary>
    /// loads the audio clips from information stored in a JSON file,
    /// and puts it into a dictionary data structure
    /// </summary>
    /// <param name="filename">The name of the JSON file</param>
    public void loadResponsesAudio(string filename)
    {

        suspectDialogue = new Dictionary<string, AudioClip[]>();
        string filepath = Path.Combine(Application.streamingAssetsPath, filename);

        if (File.Exists(filepath))
        {
            string jsonData = File.ReadAllText(filepath);
            ResponseData data = JsonUtility.FromJson<ResponseData>(jsonData);

            for (int i = 0; i < data.items.Length; ++i)
            {
                suspectDialogue.Add(data.items[i].key, Resources.LoadAll<AudioClip>(data.items[i].value));
            }

            Debug.Log("Json responses loaded");
        }
    }

    /// <summary>
    /// evaluates the spoken dialogues semantics and returns
    /// an aggravation score
    /// </summary>
    /// <param name="semantics"></param>
    /// <returns></returns>
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

    /// <summary>
    /// loads all suspect response audio clips into a List data structure
    /// </summary>
    public void loadSuspectDialogue()
    {
        suspectDialogue = new Dictionary<string, AudioClip[]>();

        suspectDialogue.Add("Intro", Resources.LoadAll<AudioClip>("Dialogue/Intro"));
        suspectDialogue.Add("Name", Resources.LoadAll<AudioClip>("Dialogue/Name"));
        suspectDialogue.Add("CopsHere", Resources.LoadAll<AudioClip>("Dialogue/CopsHere"));
        suspectDialogue.Add("HeyYou", Resources.LoadAll<AudioClip>("Dialogue/HeyYou"));
        suspectDialogue.Add("Insult", Resources.LoadAll<AudioClip>("Dialogue/Insult"));
        suspectDialogue.Add("CalmDown", Resources.LoadAll<AudioClip>("Dialogue/CalmDown"));
        suspectDialogue.Add("Question", Resources.LoadAll<AudioClip>("Dialogue/Question"));
        suspectDialogue.Add("TalkGun", Resources.LoadAll<AudioClip>("Dialogue/TalkGun"));
        suspectDialogue.Add("PointGun", Resources.LoadAll<AudioClip>("Dialogue/PointGun"));
        suspectDialogue.Add("Leave", Resources.LoadAll<AudioClip>("Dialogue/Leave"));
        suspectDialogue.Add("Resist", Resources.LoadAll<AudioClip>("Dialogue/Resist"));
    }

    /// <summary>
    /// translate semantics into audio clip tags
    /// </summary>
    /// <param name="s">list of tags</param>
    /// <returns>a list of audio clip tags</returns>
    public List<string> semanticToAudio(List<string> s)
    {
        List<string> responseTags = new List<string>();

        

        for (int i = 0; i < s.Count; i++)
        {

            if (i + 1 < s.Count)
            {
                if (responseMap[s[i]] == "StepOut" && responseMap[s[i + 1]] == "TalkReason")
                    ++i;
                else if (responseMap[s[i]] == "HeyYou" && (responseMap[s[i + 1]] == "CalmDown" || responseMap[s[i + 1]] == "Insult"))
                    ++i;
                else if (responseMap[s[i]] == "Resist" && responseMap[s[i + 1]] == "Insult")
                {
                    responseTags.Add(responseMap[s[i]]);
                    ++i;
                    continue;
                }
            }
            
            
            if (responseMap[s[i]] != "NONE")
                responseTags.Add(responseMap[s[i]]);
            
        }



        return responseTags;
    }

    /// <summary>
    /// find all audio clips given the input tags
    /// </summary>
    /// <param name="tags">list of audio clip tags</param>
    /// <returns> list of audio clips </returns>
    public List<AudioClip[]> getAudioClips(List<string> tags)
    {
        List<AudioClip[]> audioClips = new List<AudioClip[]>();

        for (int i = 0; i < tags.Count; i++)
        {
            audioClips.Add(suspectDialogue[tags[i]]);
        }

        return audioClips;
    }

    /// <summary>
    /// find a single set of audio clips given the input tag
    /// </summary>
    /// <param name="Tag">the audio clip tag</param>
    /// <returns>array of audio clips</returns>
    public AudioClip[] getSingleClips(string Tag)
    {
        return suspectDialogue[Tag];
    }


}
