using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    private void Awake()
    {
        controller = controllerRight.GetComponent<SteamVR_TrackedController>();
        controller.TriggerUnclicked += triggerConfirm;
    }

    public void SetLaser()
    {
        laser = controllerRight.GetComponent<SteamVR_FirstPersonController_MainMenu>();
        laser.TogglePointer(true);
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
                    LoadRange();
                    break;
                case ButtonInteraction.buttonID.QUIT:
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
                    break;
            }        
        }
    }

    public void LoadRange()
    {
        SceneManager.LoadScene(1);//the target range
    }
}
