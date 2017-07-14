using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Assets.Scripts;

public enum STATE { LOW_AGGRO, MED_AGGRO, HIGH_AGGRO};
public enum TRIGGER { INTRO, COPS };

public static class RangeConstants
{
    //grumble (muttering) index range constants
    public static int grumble1_Boss = 0;
    public static int grumble2_Company = 4;
    public static int grumble3_Cops = 8;
    public static int grumble4_BadGuy = 13;
    public static int grumble5_Stupid = 17;
    public static int grumble6_Unbelievable = 22;
    public static int grumble7_WhatDoing = 27;

    //Hey You index ranges
    public static int heyYou_A1 = 0;
    public static int heyYou_A2 = 1;
    public static int heyYou_A3 = 3;

    //Insult index range
    public static int insult_A1 = 0;
    public static int insult_A2 = 2;
    public static int insult_A3 = 4;

    //Name index range
    public static int name_A1 = 0;
    public static int name_A2 = 1;
    public static int name_A3 = 2;

    //Purpose index range
    public static int purpose_A1 = 0;
    public static int purpose_A2 = 1;
    public static int purpose_A3 = 3;

    //Resist index range
    public static int resist_A1 = 0;
    public static int resist_A2 = 3;
    public static int resist_A3 = 6;

    //StepOut index range
    public static int stepOut_A1 = 0;
    public static int stepOut_A2 = 2;
    public static int stepOut_A3 = 4;

    //TalkReason index range
    public static int talkReason_A1 = 0;
    public static int talkReason_A2 = 1;
    public static int talkReason_A3 = 3;

    //Approach index range
    public static int approach_A1 = 0;
    public static int approach_A2 = 2;
    public static int approach_A3 = 4;

    //Leave count
    public static int leave_count = 3;

    //LosPass count
    public static int losPass_count = 7;
}

public class SuspectControllerFSM : MonoBehaviour {

    private Coroutine co_audio;
    private Coroutine co_grumbles;
    private Coroutine co_wait;
    private Dictionary<STATE, StateBase> states;
    private Dictionary<string, string> responses;
    private StateBase currentState;
    private float aggroScore;
    private bool grumble;
    private bool losTest;
    private bool wait;

    public GameObject suspect;
    public AudioSource dialogueSource;
    public GameObject CC;

    /// <summary>
    /// LosTest property, for Line-Of-Sight (LOS) test pass or fail status
    /// </summary>
    public bool LosTest
    {
        get { return losTest; }
        set { losTest = value; }
    }

	// Use this for initialization
	void Awake () {

        aggroScore = 5;
        dialogueSource = this.gameObject.AddComponent<AudioSource>();
        initStates();
        states[STATE.MED_AGGRO].Enter(null);
        grumble = false;
        losTest = false;
        wait = false;
    }

    /// <summary>
    /// initialize all the states of the suspect FSM.
    /// </summary>
    private void initStates()
    {
        states = new Dictionary<STATE, StateBase>();
        states.Add(STATE.LOW_AGGRO, new StateLowAggro(this));
        states.Add(STATE.MED_AGGRO, new StateMedAggro(this));
        states.Add(STATE.HIGH_AGGRO, new StateHighAggro(this));
    }

    /// <summary>
    /// initialize the response dictionary.
    /// </summary>
    private void initResponses()
    {
        responses = new Dictionary<string, string>();

    }

    /// <summary>
    /// sets the current state
    /// </summary>
    /// <param name="state">the state to set as current</param>
    public void setCurrentState(StateBase state)
    {
        currentState = state;
    }

    /// <summary>
    /// retrieve a state
    /// </summary>
    /// <param name="state">the state to retrieve</param>
    /// <returns>the state specified</returns>
    public StateBase getState(STATE state)
    {
        return states[state];
    }

    /// <summary>
    /// get the current aggravation score for the suspect
    /// </summary>
    /// <returns>the current suspect aggravation score</returns>
    public float getAggroScore()
    {
        return aggroScore;
    }

    /// <summary>
    /// sets the aggravation score of the suspect
    /// </summary>
    /// <param name="newScore">the score to add-on</param>
    public void setAggroScore(float newScore)
    {
        //modify values slighty based on current state
        switch(currentState.getStateID())
        {
            case STATE.LOW_AGGRO:
                if (newScore > 0)
                    newScore *= 0.25f;
                else
                    newScore *= 0.75f;
                break;
            case STATE.MED_AGGRO:
                newScore *= 0.5f;
                break;
            case STATE.HIGH_AGGRO:
                if (newScore > 0)
                    newScore *= 0.75f;
                else
                    newScore *= 0.25f;
                break;
        }
        Debug.Log("Modified score: " + newScore);
        aggroScore += newScore;

        if (aggroScore > 10)
            aggroScore = 10;

        if (aggroScore < 0)
            aggroScore = 0;
    }

    /// <summary>
    /// starts a suspect FSM update
    /// </summary>
    /// <param name="newAggroScore">the new suspect aggravation score</param>
    /// <param name="semantics">the semantics of the users dialogue</param>
    public void UpdateFSM(float newAggroScore, List<string> semantics)
    {
        setAggroScore(newAggroScore);
        currentState.UpdateState(aggroScore, semantics);
    }

    /// <summary>
    /// selects and plays the suspects audio depending on response tags and the
    /// suspect audio clips given
    /// </summary>
    /// <param name="Tags">Tags retrieved from semantics</param>
    /// <param name="audioClips">list of clips based on tags</param>
    public void PlaySuspectAudio(List<string> Tags, List<AudioClip[]> audioClips)
    {
        grumble = false;

        if (co_grumbles != null)
            StopCoroutine(co_grumbles);

        co_audio = StartCoroutine(PlayAudio(Tags, audioClips));
    }

    /// <summary>
    /// start the coroutine for random grumble lines
    /// </summary>
    private void StartCoGrumbles()
    {
        grumble = true;
        co_grumbles = StartCoroutine(RandomGrumbles());

        if (co_audio != null)
            StopCoroutine(co_audio);
    }

    public void triggerHit(TRIGGER t)
    {
        if (co_audio != null)
            StopCoroutine(co_audio);

        switch(t)
        {
            case TRIGGER.INTRO:
                co_audio = StartCoroutine(PlaySingleAudio(CC.GetComponent<DialogueManager>().getSingleClips("Intro"), false));
                break;
            case TRIGGER.COPS:
                co_audio = StartCoroutine(PlaySingleAudio(CC.GetComponent<DialogueManager>().getSingleClips("CopsHere"), true));
                break;
        }
        
    }

    /// <summary>
    /// transition to the second tier of the scenario
    /// </summary>
    public void StartT2()
    {
        Color c = Color.grey;
        //check for losTest true or false (true = pass; false = fail)
        if (currentState.getStateID() == STATE.HIGH_AGGRO)
        {
            //transition to T2 Boss Office
            if (losTest)
                c = Color.magenta;
            else
                c = Color.black;

            suspect.GetComponent<Renderer>().material.color = c;
            Debug.Log("Starting T2 Boss Office");
        }
        else
        {
            //transition to T2 Outside
            if (losTest)
                c = Color.cyan;
            else
                c = Color.white;
            suspect.GetComponent<Renderer>().material.color = c;
            Debug.Log("Starting T2 Outside");
        }

    }

    /// <summary>
    /// starts the co routine to wait;
    /// this gives the user the opportunity to respond to the Line-Of-Sight (LOS) test
    /// </summary>
    private void startCoWait(float time)
    {
        if(!wait)
        {
            wait = true;
            co_wait = StartCoroutine(waitForLosTestResponse(time));
        }
    }

    //coroutines

    /// <summary>
    /// plays a current states audio clip response
    /// </summary>
    /// <param name="Tags">Tags for response</param>
    /// <param name="audioClips">all audio clips for response</param>
    /// <returns>nothing</returns>
    IEnumerator PlayAudio(List<string> Tags, List<AudioClip[]> audioClips)
    {
        string lastTag = "";
        float delay = 0;
        for (int i = 0; i < Tags.Count; i++)
        {
            if (Tags[i] != lastTag)
            {
                lastTag = Tags[i];
                currentState.selectAudio(Tags[i], audioClips[i]);
                currentState.AS.volume = 1f;
                currentState.AS.PlayDelayed(delay);
                delay += 0.5f;
                while (currentState.AS.isPlaying)
                {
                    yield return 0;
                }
            }
        }

        if (currentState.transitionA2)
        {
            if (currentState.getStateID() != STATE.HIGH_AGGRO)
                startCoWait(3.5f);
            else
                startCoWait(2.0f);
        }
        else
            StartCoGrumbles();
    }

    /// <summary>
    /// play an audio clip from a single array source
    /// </summary>
    /// <param name="audioClips">array of clips to randomly choose from</param>
    /// <returns>void</returns>
    IEnumerator PlaySingleAudio(AudioClip[] audioClips, bool grumble)
    {
        dialogueSource.clip = audioClips[UnityEngine.Random.Range(0, audioClips.Length)];
        dialogueSource.volume = 1f;
        dialogueSource.Play();
        while(dialogueSource.isPlaying)
        {
            yield return 0;
        }

        if (grumble)
            StartCoGrumbles();
    }

    /// <summary>
    /// have a random grumbling/muttering clip play
    /// </summary>
    /// <returns></returns>
    IEnumerator RandomGrumbles()
    {
        float delay = 3.5f;
        float timer = 0;

        //choose random grumble, tag: "Grumbles"
        List<AudioClip> grumbles = Helper.YatesShuffle(CC.GetComponent<DialogueManager>().getSingleClips("Grumbles"));
        dialogueSource.clip = grumbles.Last<AudioClip>();
        dialogueSource.volume = 0.5f;
        timer = delay;
        if(grumble)
            dialogueSource.PlayDelayed(1.5f);

        while (grumble)//while grumbling is needed
        {
            timer -= Time.deltaTime;//reduce timer

            if (timer <= 0)//times up
            {
                if (!dialogueSource.isPlaying)//make sure clip isn't playing
                {
                    grumbles.RemoveAt(grumbles.Count - 1);//remove last
                    if(grumbles.Count > 0)
                        dialogueSource.clip = grumbles.Last<AudioClip>();//assign next clip
                    else
                    {
                        grumbles = Helper.YatesShuffle(CC.GetComponent<DialogueManager>().getSingleClips("Grumbles"));
                        dialogueSource.clip = grumbles.Last<AudioClip>();//assign next clip
                    }

                    dialogueSource.Play();
                    timer = UnityEngine.Random.Range(3.5f, 4.5f);
                }
            }

            yield return 0;
        }
    }

    /// <summary>
    /// user is given time to respond to the Line-Of-Sight (LOS) test;
    /// when time is up, the transition to the second tier of the scenario begins.
    /// </summary>
    /// <returns></returns>
    IEnumerator waitForLosTestResponse(float t)
    {
        float timer = t;

        while(timer > 0)
        {
            timer -= Time.deltaTime;
            yield return 0;
        }

        StartT2();
    }

}
