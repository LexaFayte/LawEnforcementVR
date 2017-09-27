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

    /// <summary>
    /// plays the transition animation;
    /// transitions from the scenario menu to the scenario instructions screen
    /// </summary>
    public void playTransToScenarioInstructions()
    {
        anims.Play("TransToInstructions");
    }

    /// <summary>
    /// plays the transition animation;
    /// transitions from the scenario instructions screen to the scenario menu
    /// </summary>
    public void playTransToScenarioFromInstructions()
    {
        anims.Play("TransToScenarioFromInstruction");
    }
}
