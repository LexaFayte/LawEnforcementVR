using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseController : MonoBehaviour {

    public Transform PauseCanvas;
    public GameObject controllerRight;
    public GameObject GunOBJ;
    public GameObject ViveControllerModel;

    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device device;
    private SteamVR_TrackedController controller;

    private bool paused = false;

    public bool IsPaused
    {
        get { return paused; }
    }

    private void Awake()
    {
        controller = controllerRight.GetComponent<SteamVR_TrackedController>();
        controller.MenuButtonClicked += MenuPressed;
        Invoke("controllerRenderOff", 0.25f);
        
    }

    private void controllerRenderOff()
    {
        ViveControllerModel.gameObject.SetActive(false);
    }

    //deal with menu button press
    private void MenuPressed(object sender, ClickedEventArgs e)
    {
        if(PauseCanvas.gameObject.activeInHierarchy == false)
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

        }
    }

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
            }
        }
    }
}
