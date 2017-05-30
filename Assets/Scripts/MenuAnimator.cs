using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuAnimator : MonoBehaviour {
    public Animation anims;

    /// <summary>
    /// plays the transition animation;
    /// transition from the main menu to the scenario menu
    /// </summary>
    public void playTransToScenario()
    {
        anims.Play("TransToScenarios");
    }

    /// <summary>
    /// plays the transition animation;
    /// transitions from the scenario menu to the main menu
    /// </summary>
    public void playTransToMainMenu()
    {
        anims.Play("TransToMainMenu");
    }
}
