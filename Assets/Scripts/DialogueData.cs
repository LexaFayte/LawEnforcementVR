using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueData {

    public DialogueItem[] items;
}

[System.Serializable]
public class DialogueItem
{
    public string key;
    public float value;
}