using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor;
using UnityEngine;

public class ButtonInteraction : MonoBehaviour {

    public enum buttonID { PAUSE, EXIT, SCENARIOS, RANGE, QUIT, BACK, START, STOP, RESET, SCENARIO, EXIT_SCENARIO, INSTRUCTIONS};

    public buttonID BID;
    public Color highlight = Color.green;
    public float popDepth = 0.1f;
    public bool scenario = false;
    private int scenarioID = -1;
    public GameObject MMContainerController;
    private bool buttonActive = false;

    public int ScenarioID
    {
        get { return scenarioID; }
    }

    private void Start()
    {
        if (scenario)
            scenarioID = MMContainerController.GetComponent<MainMenusController>().addScenario();
    }

    public void OnButton()
    {
        GetComponent<UnityEngine.UI.Image>().color = highlight;
        GetComponent<RectTransform>().transform.transform.Translate(0, 0, -popDepth);
        transform.localScale += new Vector3(0.05f, 0, 0.1f);
        buttonActive = true;
    }

    public void OffButton()
    {

        if (buttonActive)
        {
            GetComponent<UnityEngine.UI.Image>().color = Color.white;
            GetComponent<RectTransform>().transform.transform.Translate(0, 0, popDepth);
            transform.localScale -= new Vector3(0.05f, 0, 0.1f);
            buttonActive = false;
        }
    }

}