using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseController : MonoBehaviour {

    public Transform PauseCanvas;
    public GameObject controllerRight;
    public GameObject GunOBJ;
    public GameObject ViveControllerModel;
    public GameObject Button;
    public UnityEngine.UI.Text statusText;
    
    //SteamVR
    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device device;
    private SteamVR_TrackedController controller;
    private SteamVR_FirstPersonController laser;
    private TargetController TC;

    private bool paused = false;
    private bool controllerToggle = false;
    private bool load = false;
    float timer = 0f;

    //properties
    public bool IsPaused
    {
        get { return paused; }
    }

    public bool ViveController
    {
        get { return controllerToggle; }
    }


    //initialization
    private void Awake()
    {
       
        controller = controllerRight.GetComponent<SteamVR_TrackedController>();
        laser = controllerRight.GetComponent<SteamVR_FirstPersonController>();
        controller.MenuButtonClicked += MenuPressed;
        controller.TriggerUnclicked += triggerConfirm;
        controller.PadClicked += PadPressed;
        controller.PadUnclicked += PadReleased;
        Invoke("controllerRenderOff", 0.25f);
        SceneManager.sceneLoaded += sceneLoad;
        TC = GetComponent<TargetController>();
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
    /// turn the rendering for the Vive controller off
    /// (this model is only needed in the pause menu)
    /// </summary>
    private void controllerRenderOff()
    {
        ViveControllerModel.gameObject.SetActive(false);
    }

    /// <summary>
    /// event for trigger being pulled
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void triggerConfirm(object sender, ClickedEventArgs e)
    {
        if(Button != null)
        {
            switch(Button.GetComponent<ButtonInteraction>().BID)
            {
                case ButtonInteraction.buttonID.PAUSE:
                    PauseAndResume();
                    Button = null;
                    break;
                case ButtonInteraction.buttonID.EXIT:
                    SteamVR_Fade.View(Color.black, 1.25f);
                    Time.timeScale = 1;
                    Invoke("LoadMainMenu", 1.5f);
                    break;
                case ButtonInteraction.buttonID.START:
                    statusText.text = "Targets started";
                    TC.startTargetSequence();
                    break;
                case ButtonInteraction.buttonID.STOP:
                    TC.stopTargetSequence();
                    statusText.text = "Targets stopped";
                    break;
                case ButtonInteraction.buttonID.RESET:
                    if (!TC.IsRunning)
                    {
                        TC.resetTargetSequence();
                        statusText.text = "Targets reset";
                    }
                    break;
            }        
        }
    }

    public void PadPressed(object sender, ClickedEventArgs e)
    {
        if (!paused)
        {
            controllerToggle = true;
            ToggleViveController(true);
        }
    }

    public void PadReleased(object sender, ClickedEventArgs e)
    {
        if (!paused)
        {
            controllerToggle = false;
            ToggleViveController(false);
            if (Button != null)
                Button.GetComponent<ButtonInteraction>().OffButton();
        }
    }   

    /// <summary>
    /// Exit voice command triggered, load the main menu
    /// </summary>
    public void ExitCalled()
    {
        SteamVR_Fade.View(Color.black, 1.25f);
        Time.timeScale = 1;
        Invoke("LoadMainMenu", 1.5f);
    }

    /// <summary>
    /// loads into the main menu
    /// </summary>
    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
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
    /// swaps the active controller model and functionality; Vive controller model, and Gun model
    /// </summary>
    /// <param name="active">the active state of the vive controller model</param>
    private void ToggleViveController(bool active)
    {

        GunOBJ.GetComponent<MeshRenderer>().enabled = !active;
        foreach (Transform child in GunOBJ.transform)
        {
            child.GetComponent<MeshRenderer>().enabled = !active;
        }
        
        laser.TogglePointer(active);
        ViveControllerModel.gameObject.SetActive(active);
    }

    /// <summary>
    /// controller specific.
    /// Toggles the pause menu.
    /// </summary>
    private void PauseAndResume()
    {
        if (PauseCanvas.gameObject.activeInHierarchy == false)
        {
            PauseCanvas.gameObject.SetActive(true);
            Time.timeScale = 0;
            paused = true;

            ToggleViveController(true);

        }
        else
        {
            PauseCanvas.gameObject.SetActive(false);
            Time.timeScale = 1;
            paused = false;

            ToggleViveController(false);
        }
    }


    /// <summary>
    /// deal with menu button press
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MenuPressed(object sender, ClickedEventArgs e)
    {
        PauseAndResume();
    }

    /// <summary>
    /// Toggle menu when called by Voice Command
    /// </summary>
    /// <param name="activate">boolean representing if menu should be activated.</param>
    public void MenuCalled(bool activate)
    {

        if (activate)
        {
            if (PauseCanvas.gameObject.activeInHierarchy == false)
            {
                PauseCanvas.gameObject.SetActive(true);
                Time.timeScale = 0;
                paused = true;

                ToggleViveController(true);
            }
        }
        else
        {
            if (PauseCanvas.gameObject.activeInHierarchy == true)
            {
                PauseCanvas.gameObject.SetActive(false);
                Time.timeScale = 1;
                paused = false;

                ToggleViveController(false);
            }
        }
    }
}
