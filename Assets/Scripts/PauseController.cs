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
    

    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device device;
    private SteamVR_TrackedController controller;
    private SteamVR_FirstPersonController laser;

    private bool paused = false;

    public bool IsPaused
    {
        get { return paused; }
    }

    private void Awake()
    {
        controller = controllerRight.GetComponent<SteamVR_TrackedController>();
        laser = controllerRight.GetComponent<SteamVR_FirstPersonController>();
        controller.MenuButtonClicked += MenuPressed;
        controller.TriggerUnclicked += triggerConfirm;
        Invoke("controllerRenderOff", 0.25f);
        
    }

    /// <summary>
    /// turn the rendering for the Vive controller off
    /// (this model is only needed in the pause menu)
    /// </summary>
    private void controllerRenderOff()
    {
        ViveControllerModel.gameObject.SetActive(false);
    }


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
                    LoadMainMenu();
                    break;
            }        
        }
    }

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

            GunOBJ.GetComponent<MeshRenderer>().enabled = false;
            foreach (Transform child in GunOBJ.transform)
            {
                child.GetComponent<MeshRenderer>().enabled = false;
            }

            ViveControllerModel.gameObject.SetActive(true);
            laser.TogglePointer(true);

        }
        else
        {
            PauseCanvas.gameObject.SetActive(false);
            Time.timeScale = 1;
            paused = false;

            GunOBJ.GetComponent<MeshRenderer>().enabled = true;
            foreach (Transform child in GunOBJ.transform)
            {
                child.GetComponent<MeshRenderer>().enabled = true;
            }

            ViveControllerModel.gameObject.SetActive(false);
            laser.TogglePointer(false);
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

                GunOBJ.GetComponent<MeshRenderer>().enabled = false;
                foreach (Transform child in GunOBJ.transform)
                {
                    child.GetComponent<MeshRenderer>().enabled = false;
                }

                ViveControllerModel.gameObject.SetActive(true);
                laser.TogglePointer(true);
            }
        }
        else
        {
            if (PauseCanvas.gameObject.activeInHierarchy == true)
            {
                PauseCanvas.gameObject.SetActive(false);
                Time.timeScale = 1;
                paused = false;

                GunOBJ.GetComponent<MeshRenderer>().enabled = true;
                foreach (Transform child in GunOBJ.transform)
                {
                    child.GetComponent<MeshRenderer>().enabled = true;
                }

                ViveControllerModel.gameObject.SetActive(false);
                laser.TogglePointer(false);
            }
        }
    }
}
