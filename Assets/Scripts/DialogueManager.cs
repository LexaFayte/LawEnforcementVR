﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DialogueManager : MonoBehaviour {

    private Dictionary<string, float> dialogueText;
    private Dictionary<string, AudioClip[]> suspectDialogue;
    private Dictionary<string, string> responseMap;
    private bool t2;

    //properties
    public bool T2
    {
        get { return t2; }
        set { t2 = value; }
    }

    // Use this for initialization
    void Awake () {
        loadDialogueText("Office_T1.json");
        loadResponses("Office_T1_Audio.json");
        loadResponsesAudio("Office_T1_ClipData.json");
	}
	
    public void initT2Dialogue()
    {
        unloadResponseAudio();
        loadDialogueText("Office_T2.json");
        loadResponses("Office_T2_Audio.json");
        loadResponsesAudio("Office_T2_ClipData.json");
        //Resources.UnloadUnusedAssets();
        t2 = true;
    }

    /// <summary>
    /// loads the dialogue aggravation score information
    /// from a JSON file and into a dictionary data structure
    /// </summary>
    /// <param name="filename">The name of the JSON file</param>
	public void loadDialogueText(string filename)
    {
        if(dialogueText == null)
            dialogueText = new Dictionary<string, float>();
        else
            dialogueText.Clear();

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
        if (responseMap == null)
            responseMap = new Dictionary<string, string>();
        else
            responseMap.Clear();

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
    /// unloads the current audio for responses
    /// </summary>
    private void unloadResponseAudio()
    {
        foreach(KeyValuePair<string, AudioClip[]> clips in suspectDialogue)
        {

            for (int i = 0; i < clips.Value.Length;++i)
            {
                //Resources.UnloadAsset(clips.Value[i]);
                clips.Value[i].UnloadAudioData();
            }
        }
    }

    /// <summary>
    /// loads the audio clips from information stored in a JSON file,
    /// and puts it into a dictionary data structure
    /// </summary>
    /// <param name="filename">The name of the JSON file</param>
    public void loadResponsesAudio(string filename)
    {
        if (suspectDialogue == null)
            suspectDialogue = new Dictionary<string, AudioClip[]>();
        else
            suspectDialogue.Clear();

        string filepath = Path.Combine(Application.streamingAssetsPath, filename);

        if (File.Exists(filepath))
        {
            string jsonData = File.ReadAllText(filepath);
            ResponseData data = JsonUtility.FromJson<ResponseData>(jsonData);

            for (int i = 0; i < data.items.Length; ++i)
            {
                suspectDialogue.Add(data.items[i].key, Resources.LoadAll<AudioClip>(data.items[i].value));
            }

            Debug.Log("Json responses audio loaded");
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
                if (!t2)
                {
                    if (responseMap[s[i]] == "StepOut" && responseMap[s[i + 1]] == "TalkReason")
                        ++i;
                    else if (responseMap[s[i]] == "HeyYou" && (responseMap[s[i + 1]] == "CalmDown" || responseMap[s[i + 1]] == "Insult"))
                        ++i;
                    else if (responseMap[s[i]] == "RemovePersist" && responseMap[s[i + 1]] == "Remove")
                    {
                        responseTags.Add(responseMap[s[i]]);
                        ++i;
                        continue;
                    }
                    else if (responseMap[s[i]] == "Resist" && responseMap[s[i + 1]] == "Insult")
                    {
                        responseTags.Add(responseMap[s[i]]);
                        ++i;
                        continue;
                    }
                    else if(responseMap[s[i]] == "Talk" && responseMap[s[i+1]] == "TalkReason")
                    {
                        i++;
                    }

                }
                else
                {

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
