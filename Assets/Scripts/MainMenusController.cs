using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenusController : MonoBehaviour {
    public Transform MainCanvas;
    public Transform SceneCanvas;
    public GameObject controllerRight;
    public GameObject Button;
    public GameObject ViveControllerModel;

    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device device;
    private SteamVR_TrackedController controller;
    private SteamVR_FirstPersonController_MainMenu laser;


    private bool once;

    private void Awake()
    {
        controller = controllerRight.GetComponent<SteamVR_TrackedController>();
        laser = controllerRight.GetComponent<SteamVR_FirstPersonController_MainMenu>();
        controller.TriggerUnclicked += triggerConfirm;
    }

    /// <summary>
    /// assign the button currently being targeted by the VR controller laser
    /// </summary>
    /// <param name="b">button being targeted; passed by reference</param>
    public void AssignButton(ref GameObject b)
    {
        Button = b;
    }

    public void triggerConfirm(object sender, ClickedEventArgs e)
    {
        if(Button != null)
        {
            switch(Button.GetComponent<ButtonInteraction>().BID)
            {
                case ButtonInteraction.buttonID.SCENARIOS:
                    //swap to scenarios menu
                    Button = null;
                    break;
                case ButtonInteraction.buttonID.RANGE:
                    Button = null;
                    break;
                case ButtonInteraction.buttonID.QUIT:
                    Button = null;
                    break;
            }        
        }
    }

    public void LoadRange()
    {

    }
}
