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

[System.Serializable]
public class ResponseData
{

    public ResponseItem[] items;
}

[System.Serializable]
public class ResponseItem
{
    public string key;
    public string value;
}
