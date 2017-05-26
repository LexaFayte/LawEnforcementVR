using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonInteraction : MonoBehaviour {

    public enum buttonID { PAUSE, EXIT, SCENARIOS, RANGE, QUIT};

    public buttonID BID;

    public void OnButton()
    {
        GetComponent<UnityEngine.UI.Image>().color = Color.green;
        GetComponent<RectTransform>().transform.transform.Translate(0, 0, -0.1f);
        transform.localScale += new Vector3(0.1f, 0, 0.1f);
    } 

    public void OffButton()
    {
        GetComponent<UnityEngine.UI.Image>().color = Color.white;
        GetComponent<RectTransform>().transform.transform.Translate(0, 0, 0.1f);
        transform.localScale -= new Vector3(0.1f, 0, 0.1f);
    }
}
