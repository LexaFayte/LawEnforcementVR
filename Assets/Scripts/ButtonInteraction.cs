using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ButtonInteraction : MonoBehaviour {

    public enum buttonID { PAUSE, EXIT, SCENARIOS, RANGE, QUIT, BACK};

    public buttonID BID;
    public Color highlight = Color.green;
    public bool scenario = false;
    private int scenarioID = -1;
    public GameObject MMContainerController;

    private void Start()
    {
        if (scenario)
            scenarioID = MMContainerController.GetComponent<MainMenusController>().addScenario();
    }

    public void OnButton()
    {
        GetComponent<UnityEngine.UI.Image>().color = highlight;
        GetComponent<RectTransform>().transform.transform.Translate(0, 0, -0.1f);
        transform.localScale += new Vector3(0.05f, 0, 0.1f);
    } 

    public void OffButton()
    {
        GetComponent<UnityEngine.UI.Image>().color = Color.white;
        GetComponent<RectTransform>().transform.transform.Translate(0, 0, 0.1f);
        transform.localScale -= new Vector3(0.05f, 0, 0.1f);
    }

}