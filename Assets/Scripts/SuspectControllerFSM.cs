using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Assets.Scripts;

public enum STATE { LOW_AGGRO, MED_AGGRO, HIGH_AGGRO};
public enum TRIGGER { INTRO, COPS };

public class SuspectControllerFSM : MonoBehaviour {

	private Coroutine co_audio;
	private Coroutine co_grumbles;
	private Coroutine co_wait;
	private Dictionary<STATE, StateBase> states;
	private StateBase currentState;
	private float aggroScore;
	private float defuseScore;
	private bool defused;
	private bool grumble;
	private bool losTest;
	private bool wait;
	private bool copsAnim;
	private ScenarioController SC;
	private DialogueManager DM;
	private GameObject suspect;
	private AnimController_Jim JimAC;
	private bool tier2;
    private bool kill;
	private bool finish;

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

	public GameObject Suspect
	{
		get { return suspect; }
	}

	public bool Tier2
	{
		get { return tier2; }
	}

	public bool Defused
	{
		get { return defused; }
		set { defused = value; }
	}

    public bool Kill
    {
        get { return kill; }
    }

	public bool Finish
	{
		get { return finish; }
	}

	// Use this for initialization
	void Awake () {
		suspect = gameObject;
		aggroScore = 5;
		defuseScore = 10;
		initStates();
		states[STATE.MED_AGGRO].Enter(null);
		grumble = false;
		losTest = false;
		wait = false;
		copsAnim = false;
		tier2 = false;
		defused = false;
		finish = false;
		DM = CC.GetComponent<DialogueManager>();
		JimAC = suspect.GetComponent<AnimController_Jim>();
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

	public void setScenarioController(ScenarioController sc)
	{
		SC = sc;
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
	/// set the defuse score
	/// </summary>
	/// <param name="newScore"></param>
	public void setDefuseScore(float newScore)
	{
		defuseScore += newScore;

		if(defuseScore <= 5)
		{
			defused = true;
		}
	}

	/// <summary>
	/// get the current defuse score
	/// </summary>
	/// <returns>the defuse score</returns>
	public float getDefuseScore()
	{
		return defuseScore;
	}

	/// <summary>
	/// starts a suspect FSM update
	/// </summary>
	/// <param name="newScore">the new suspect aggravation score</param>
	/// <param name="semantics">the semantics of the users dialogue</param>
	public void UpdateFSM(float newScore, List<string> semantics)
	{
		setAggroScore(newScore);
		currentState.UpdateState(aggroScore, semantics);
	}

	public void UpdateT2Suspect(float newDefuseScore, List<string> semantics)
	{
		setDefuseScore(newDefuseScore);
		SC.Interrupt = true;

		string Tag = "";

		Tag = DM.semanticToAudio(semantics);

		AudioClip[] audioClips = DM.getSingleClips(Tag);

		PlaySuspectAudio(Tag, audioClips);
	}

	/// <summary>
	/// selects and plays the suspects audio depending on response tags and the
	/// suspect audio clips given
	/// </summary>
	/// <param name="Tags">Tags retrieved from semantics</param>
	/// <param name="audioClips">list of clips based on tags</param>
	public void PlaySuspectAudio(string Tag, AudioClip[] audioClips)
	{
		grumble = false;

		if (co_grumbles != null)
			StopCoroutine(co_grumbles);
		if (!tier2)
			co_audio = StartCoroutine(PlayAudio(Tag, audioClips));
		else
			co_audio = StartCoroutine(PlayDefuseResponse(Tag, audioClips));
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
				//trigger animation
				JimAC.triggerIntroRant();
				//trigger audio
				co_audio = StartCoroutine(PlaySingleAudio(DM.getSingleClips("Intro"), false));
				break;
			case TRIGGER.COPS:
				//trigger animation
				copsAnim = true;
				JimAC.triggerIntroCops();
				//trigger audio
				co_audio = StartCoroutine(PlaySingleAudio(DM.getSingleClips("CopsHere"), true));
				break;
		}
		
	}

	/// <summary>
	/// transition to the second tier of the scenario
	/// </summary>
	public void StartT2()
	{
		tier2 = true;
		//check for losTest true or false (true = pass; false = fail)
		if (currentState.getStateID() == STATE.HIGH_AGGRO)
		{
			//transition to T2 Boss Office

			SC.initializeT2(T2.OFFICE);
			Debug.Log("Starting T2 Boss Office");
		}
		else
		{
			//transition to T2 Outside

			SC.initializeT2(T2.OUTSIDE);
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

	/// <summary>
	/// set each state into T2
	/// </summary>
	public void setStatesT2()
	{
		foreach(KeyValuePair<STATE, StateBase> s in states)
		{
			s.Value.setT2(true);
		}
	}

	/// <summary>
	/// triggers the suspect to kill in a T2 scene
	/// </summary>
	public void triggerKill()
	{
        if (tier2)
        {
            kill = true;
            currentState.kill();
        }
	}

	//coroutines

	/// <summary>
	/// plays a current states audio clip response
	/// </summary>
	/// <param name="Tags">Tag for response</param>
	/// <param name="audioClips">all audio clips for response</param>
	/// <returns>nothing</returns>
	IEnumerator PlayAudio(string Tag, AudioClip[] audioClips)
	{
		currentState.selectAudio(Tag, audioClips);
		currentState.AS.volume = 1f;
		currentState.AS.Play();

		while (currentState.AS.isPlaying)
		{
			yield return 0;
		}

		if (currentState.transitionA2)
		{
			startCoWait(3.5f);
		}
		else
			StartCoGrumbles();
	}

	IEnumerator PlayDefuseResponse(string Tag, AudioClip[] audioClips)
	{
		switch(Tag)
		{
			case "AssureReprimand":
				dialogueSource.clip = audioClips[UnityEngine.Random.Range(0, RangeConstants.assureReprimand_count)];
				break;
			case "Confide":
				dialogueSource.clip = audioClips[UnityEngine.Random.Range(0, RangeConstants.confide_count)];
				//finish scenario
				finish = true;
				break;
			case "Dismiss":
				dialogueSource.clip = audioClips[UnityEngine.Random.Range(0, RangeConstants.dismiss_count)];
				break;
			case "Focus":
				dialogueSource.clip = audioClips[UnityEngine.Random.Range(0, RangeConstants.focus_count)];
				break;
			case "Title":
				dialogueSource.clip = audioClips[UnityEngine.Random.Range(0, RangeConstants.title_count)];
				break;
			case "CalmDown":
				dialogueSource.clip = audioClips[UnityEngine.Random.Range(0, RangeConstants.calmDown_count)];
				break;
			case "Resist":
				dialogueSource.clip = audioClips[UnityEngine.Random.Range(0, RangeConstants.resist_count)];
				break;
			case "Purpose":
				dialogueSource.clip = audioClips[UnityEngine.Random.Range(0, RangeConstants.purpose_count)];
				break;

		}

		dialogueSource.volume = 1f;
		dialogueSource.Play();

		while (dialogueSource.isPlaying)
		{
			yield return 0;
		}

		if (finish)
		{
			SC.startT3();
		}
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

		while (dialogueSource.isPlaying)
		{
			yield return 0;
		}

		if (copsAnim)
		{
			suspect.GetComponent<AnimController_Jim>().stopRant();
			copsAnim = false;
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
