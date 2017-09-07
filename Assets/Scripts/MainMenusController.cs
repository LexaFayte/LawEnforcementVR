using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum MENU { MAIN, SCENARIO, INSTRUCTION };

public class MainMenusController : MonoBehaviour {
    //public fields
    public Transform MainCanvas;
    public Transform ScenarioCanvas;
    public GameObject controllerRight;
    public GameObject Button;
    public GameObject MenusContainer;

    //SteamVR
    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device device;
    private SteamVR_TrackedController controller;
    private SteamVR_FirstPersonController_MainMenu laser;
    private MenuAnimator MA;
    private MENU currentMenu;

    //scenario vars
    private List<int> scenarioIDs;
    private bool scenarioMenu = false;
    private float FADE_LERP = 0.015f;
    private int currentScenarioID;

    public int CurrectScenarioID
    {
        get { return currentScenarioID; }
        set { currentScenarioID = value; }
    }

    public MENU CurrentMenu
    {
        get { return currentMenu; }
    }

    private void Awake()
    {
        controller = controllerRight.GetComponent<SteamVR_TrackedController>();
        controller.TriggerUnclicked += triggerConfirm;
        SceneManager.sceneLoaded += sceneLoad;
        scenarioIDs = new List<int>();
        MA = MenusContainer.GetComponent<MenuAnimator>();
        Invoke("menuFade", 1f);
        currentScenarioID = -1;
        currentMenu = MENU.MAIN;
    }

    /// <summary>
    /// calls the coroutine to fade the menu in (called by invoke so it can be delayed)
    /// </summary>
    private void menuFade()
    {
        StartCoroutine(FadeIn(MainCanvas.GetComponent<CanvasGroup>()));
    }


    /// <summary>
    /// coroutine for fading a UI canvas and it's elements in
    /// </summary>
    /// <param name="cg">the canvas group of the canvas to fade</param>
    /// <returns></returns>
    IEnumerator FadeIn(CanvasGroup cg)
    {
        while(cg.alpha != 1)
        {
            cg.alpha += FADE_LERP;
            yield return 0;
        }
    }

    /// <summary>
    /// corouting for fading a UI canvas and it's elements out
    /// </summary>
    /// <param name="cg">the canvas group of the canvas to fade</param>
    /// <returns></returns>
    IEnumerator FadeOut(CanvasGroup cg)
    {
        while (cg.alpha != 1)
        {
            cg.alpha += FADE_LERP;
            yield return 0;
        }
    }

    /// <summary>
    /// things to do upon scene loading
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="lsm"></param>
    public void sceneLoad(Scene scene, LoadSceneMode lsm)
    {
        SteamVR_Fade.View(Color.clear, 2f);
    }

    /// <summary>
    /// sets the laser for the controller
    /// </summary>
    public void SetLaser()
    {
        laser = controllerRight.GetComponent<SteamVR_FirstPersonController_MainMenu>();
        laser.TogglePointer(true);
    }

    /// <summary>
    /// add scenario to scenario list
    /// </summary>
    /// <returns>the scenarioID</returns>
    public int addScenario()
    {
        scenarioIDs.Add(scenarioIDs.Count+2);
        return scenarioIDs.Count+1;
    }

    /// <summary>
    /// assign the button currently being targeted by the VR controller laser
    /// </summary>
    /// <param name="b">button being targeted; passed by reference</param>
    public void AssignButton(ref GameObject b)
    {
        Button = b;
    }

    /// <summary>
    /// sets the current menu and plays the animation for the menu transition
    /// </summary>
    /// <param name="menu">which menu to transition to</param>
    public void menuTransition(MENU menu)
    {
        switch(menu)
        {
            case MENU.MAIN:
                currentMenu = MENU.MAIN;
                MA.playTransToMainMenu();
                break;
            case MENU.SCENARIO:
                if (currentMenu == MENU.MAIN)
                {
                    currentMenu = MENU.SCENARIO;
                    MA.playTransToScenario();
                }
                else if(currentMenu == MENU.INSTRUCTION)
                {
                    currentMenu = MENU.SCENARIO;
                    MA.playTransToScenarioFromInstructions();
                }
                break;
            case MENU.INSTRUCTION:
                currentMenu = MENU.INSTRUCTION;
                MA.playTransToScenarioInstructions();
                break;
        }
    }

    /// <summary>
    /// event for trigger pull
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void triggerConfirm(object sender, ClickedEventArgs e)
    {
        if(Button != null)
        {
            switch(Button.GetComponent<ButtonInteraction>().BID)
            {
                case ButtonInteraction.buttonID.SCENARIOS:
                    //swap to scenarios menu
                    menuTransition(MENU.SCENARIO);
                    Button = null;
                    break;
                case ButtonInteraction.buttonID.SCENARIO:
                    currentScenarioID = Button.GetComponent<ButtonInteraction>().ScenarioID;
                    Button = null;
                    SteamVR_Fade.View(Color.black, 1.25f);
                    Invoke("LoadScenario", 1f);
                    break;
                case ButtonInteraction.buttonID.RANGE:
                    Button = null;
                    SteamVR_Fade.View(Color.black, 1.25f);
                    Invoke("LoadRange", 1f);
                    break;
                case ButtonInteraction.buttonID.QUIT:
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
                    break;
                case ButtonInteraction.buttonID.BACK:
                    if(currentMenu == MENU.SCENARIO)
                    {
                        menuTransition(MENU.MAIN);
                        Button = null;
                    }
                    else if(currentMenu == MENU.INSTRUCTION)
                    {
                        menuTransition(MENU.SCENARIO);
                        Button = null;
                    }
                    break;
                case ButtonInteraction.buttonID.INSTRUCTIONS:
                    menuTransition(MENU.INSTRUCTION);
                    Button = null;
                    break;
            }        
        }
    }

    /// <summary>
    /// loads into the Range
    /// </summary>
    public void LoadRange()
    {
        SceneManager.LoadScene(1);//the target range
    }

    /// <summary>
    /// loads into the selected scenario
    /// </summary>
    public void LoadScenario()
    {
        SceneManager.LoadScene(currentScenarioID);
    }
}
