using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Assets.Scripts;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum SCENE { INTRO,T1,T2_OFFICE, T2_OUTSIDE};
public enum T2 { OFFICE, OUTSIDE};
public enum SHOT { JIM, BOSS, GUARD, NOTHING};
public enum SCENARIO_RESULT { TALK_DOWN, USER_KILL, SUSPECT_KILL, DISCHARGE};

public class ScenarioController : MonoBehaviour {

	//public objects
	public GameObject CC;
    public GameObject Gun;
	public GameObject suspect;
    public GameObject suspectGun;
	public GameObject boss;
	public GameObject Officer;
	public GameObject car;
	public GameObject guard;
	public GameObject[] T1waypoints;
	public GameObject[] T2Waypoints;
	public Light OutsideSun;
	public GameObject BGChars;
    public GameObject BuildingInside;
    public GameObject BuildingOutside;
    public GameObject OfficeLighting;
	public AudioClip[] IntroClips;
	public AudioClip[] TutorialClips;
	public AudioClip[] OfficeScriptClips;
	public AudioClip[] OutsideScriptClips;
    public AudioClip[] EndingClips;
	public AudioClip[] ExtraSFX;
	public GameObject drivingWaypoint;
	public GameObject drivingStrip;
    public GameObject officerT1Waypoint;
    public CanvasGroup results_cg;
    public Text results_text;
    public GameObject ResultsWaypoint;
	
	//private objects and vars
	private SuspectControllerFSM scFSM;
	private float timer;
    private Coroutine WaitShot;
	private Coroutine MoveCar;
	private Coroutine PlayIntroAudio;
	private Coroutine initT1;
	private Coroutine OfficerTut;
	private Coroutine initT2;
	private Coroutine escalateT2;
    private Coroutine T3Ending;
	private bool transitioning;
	private GrammarRecognizerScript GRS;
	private DialogueManager DM;
	private CameraMove CM;
	private AudioSource Audio1;
	private AudioSource Audio2;
	private SCENE currentScene;
	private SteamVR_PlayArea playArea;
	private Animator copAnim;
	private bool interrupt;
    private int[] shot;
    private SCENARIO_RESULT scenarioResult;
    private Animator JimAnim;
    private Animator GuardAnim;
    private Animator BossAnim;
    private AnimController_Jim JimAC;

	public bool Interrupt
	{
		set { interrupt = value; }
		get { return interrupt; }
	}

    public SCENARIO_RESULT ScenarioResult
    {
        get { return scenarioResult; }
    }

	private void Awake()
	{ 
		scFSM = suspect.GetComponent<SuspectControllerFSM>();
		scFSM.setScenarioController(this);
		timer = 0f;
        transitioning = false;
		GRS = CC.GetComponent<GrammarRecognizerScript>();
		DM = CC.GetComponent<DialogueManager>();
		CM = CC.GetComponent<CameraMove>();
		Audio1 = Officer.GetComponent<AudioSource>();
		Audio2 = car.GetComponent<AudioSource>();
		playArea = CC.GetComponentInChildren<SteamVR_PlayArea>();
		copAnim = Officer.GetComponent<Animator>();
        JimAnim = suspect.GetComponent<Animator>();
        GuardAnim = guard.GetComponent<Animator>();
        BossAnim = boss.GetComponent<Animator>();
        JimAC = suspect.GetComponent<AnimController_Jim>();
		currentScene = SCENE.INTRO;
		interrupt = false;
        shot = new int[4];
        suspectGun.SetActive(false);
        BuildingInside.SetActive(false);
        BuildingOutside.SetActive(false);
        results_cg.alpha = 0;
	}

    public void Start()
    {
        PlayCarIntro();
    }

    // Update is called once per frame
 //   void Update () {
	//	if (!transitioning && timer > 0)
	//	{
	//		fadetoClear(3);
	//		timer = 0;
	//	}
	//}

	/// <summary>
	/// play the scenario introduction
	/// </summary>
	public void PlayCarIntro()
	{
        CC.transform.Rotate(new Vector3(0, 90, 0), Space.Self);
        //float timer_ = 4f;

        ////wait a few second
        //while (timer_ >= 0)
        //{
        //    timer_ -= Time.deltaTime;
        //}

        fadetoClear(1f);
        MoveCar = StartCoroutine(CM.translateOverTime(drivingStrip, drivingWaypoint, 50));
		PlayIntroAudio = StartCoroutine(coDrivingIntro());
	}

	/// <summary>
	/// set up T2
	/// </summary>
	/// <param name="scene">enum indicating which T2 scene to set up</param>
	public void initializeT2(T2 scene)
	{
		initT2 = StartCoroutine(coInitT2(scene));
	}

	/// <summary>
	/// fade the VR View to black
	/// </summary>
	public void fadetoBlack(float t)
	{
		SteamVR_Fade.View(Color.black, t);
	}

	/// <summary>
	/// fade the VR View to clear
	/// </summary>
	public void fadetoClear(float t)
	{
		SteamVR_Fade.View(Color.clear, t);
	}

    /// <summary>
    /// logic to determine what happens when the user has fired the gun.
    /// </summary>
    /// <param name="targetTag">tag belonging to the object the user shot</param>
    public void shotsFired(string targetTag)
    {
        switch(targetTag)
        {
            case "Jim":
                if(!scFSM.Tier2)
                {
                    shot[(int)SHOT.JIM]++;
                    if (WaitShot != null)
                        StopCoroutine(WaitShot);
                    //wtf? don't shoot him
                    scenarioResult = SCENARIO_RESULT.USER_KILL;
                    StartCoroutine(scenarioEndingKill());
                }
                else
                {
                    shot[(int)SHOT.JIM]++;
                    if (WaitShot != null)
                        StopCoroutine(WaitShot);
                    
                    scenarioResult = SCENARIO_RESULT.USER_KILL;
                    StartCoroutine(scenarioEndingKill());
                    
                }
                break;
            case "Boss":
                shot[(int)SHOT.BOSS]++;
                if (WaitShot != null)
                    StopCoroutine(WaitShot);

                scenarioResult = SCENARIO_RESULT.USER_KILL;
                StartCoroutine(scenarioEndingKill());
                //wtf? don't shoot him
                break;
            case "SecuirtyGuard":
                shot[(int)SHOT.GUARD]++;
                if (WaitShot != null)
                    StopCoroutine(WaitShot);

                scenarioResult = SCENARIO_RESULT.USER_KILL;
                StartCoroutine(scenarioEndingKill());
                //wtf? don't shoot him
                break;
            default:
                //DO NOT DISCHARGE YOUR WEAPON!
                shot[(int)SHOT.NOTHING]++;
                if(WaitShot == null)
                    WaitShot = StartCoroutine(wait(2f));
                break;
        }
    }

    /// <summary>
    /// wait to see if more shots are fired; user accuracy may not be 100%, it's possible
    /// for the first shot or two to miss.
    /// If the only shots fired are misses, then the intent was not to shoot a person, and
    /// therefore classified as illegally discharging your weapon (an automatic fail)
	/// </summary>
	/// <returns></returns>
	IEnumerator wait(float t)
    {
        float timer = t;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return 0;
        }

        if(shot[(int)SHOT.NOTHING] == shot.Sum())
        {
            //user only shot at nothing; illegal discharge of weapon
            scenarioResult = SCENARIO_RESULT.DISCHARGE;
            StartCoroutine(scenarioEndingKill());
        }
    }


    //
    //---------------------------------------->>TIER 1 SCENARIO SEQUENCE<<----------------------------------------
    //

    /// <summary>
    /// coroutine for scenario introduction;
    /// driving and speaking to dispatch
    /// </summary>
    /// <returns></returns>
    IEnumerator coDrivingIntro()
	{
		//loop through all intro clips while moving car forward
		
		for (int i = 0; i < IntroClips.Length; i++)
		{
			if(i%2 == 0)
			{
				Audio2.clip = IntroClips[i];
				Audio2.Play();
				while(Audio2.isPlaying)
				{
					yield return 0;
				}
			}
			else
			{
				copAnim.SetTrigger("Radio");
				Audio1.clip = IntroClips[i];
				Audio1.Play();
				while(Audio1.isPlaying)
				{
					yield return 0;
				}
			}
		}

		fadetoBlack(1);
        BuildingInside.SetActive(true);
        OfficeLighting.SetActive(true);
		initT1 = StartCoroutine(coInitT1());
	}

	/// <summary>
	/// start Tier 1 in of the scenario (will start it after 2.5s)
	/// </summary>
	private void startT1()
	{
        Gun.GetComponent<GunController>().setScenarioController(this);
        fadetoClear(1.5f);
		CM.Invoke("startMovement", 2f);
	}

	/// <summary>
	/// coroutine for initializing Tier 1 in the scenario
	/// </summary>
	/// <returns></returns>
	IEnumerator coInitT1()
	{
		transitioning = true;
		timer = 0f;
		

		while (transitioning)
		{
			timer += Time.deltaTime;
			if (timer > 2f)
			{
                StopCoroutine(MoveCar);
                drivingStrip.SetActive(false);
                transitioning = false;

                //move Officer to beside you
                Officer.transform.position = new Vector3(officerT1Waypoint.transform.position.x, Officer.transform.position.y, officerT1Waypoint.transform.position.z);
				Officer.transform.eulerAngles = new Vector3(0, Officer.transform.eulerAngles.y, 0);
                Officer.transform.LookAt(new Vector3(suspect.transform.position.x, Officer.transform.position.y, suspect.transform.position.z));
                copAnim.SetTrigger("T1Tut");
                copAnim.SetLayerWeight(1, 0);
                copAnim.SetLayerWeight(2, 0);

                playArea.size = SteamVR_PlayArea.Size._400x300;
				CC.transform.position = new Vector3(8.62f, 0.5051446f, 1.23f);
				CC.transform.rotation = new Quaternion(0, 0, 0, 1);

				//car
				car.transform.position = new Vector3(191.52f, 0.1f, -22.1f);
				car.transform.Rotate(0, 0, -52.957f);

				//turn off outside lighting
				OutsideSun.enabled = false;
				currentScene = SCENE.T1;


			}
			yield return 0;
		}

        //OfficerTut = StartCoroutine(coTut());
        startT1();

	}


	/// <summary>
	/// coroutine for partner officers introduction ("tutorial") for the user
	/// </summary>
	/// <returns></returns>
	IEnumerator coTut()
	{
		bool tutorial = true;
		float timer = 0;
		while (tutorial)
		{

			timer += Time.deltaTime;

			if (timer > 1f)
			{
				copAnim.SetTrigger("T1Tut");
				fadetoClear(3);                
			}

			if (timer > 3.5f)
			{
				tutorial = false;

                for (int i = 0; i < TutorialClips.Length; i++)
                {
                    Audio1.clip = TutorialClips[i];
                    Audio1.Play();

                    if (i == 2)
                    {
                        CM.Invoke("startMovement", 3.5f);

                    }
                    while (Audio1.isPlaying)
                    {
                        yield return 0;
                    }
                }

                Gun.GetComponent<GunController>().setScenarioController(this);
			}
		}
	}

    //
    //---------------------------------------->>TIER 2 SCENARIO SEQUENCE<<----------------------------------------
    //

    /// <summary>
    /// initializing Office Scenario Tier 2
    /// </summary>
    /// <param name="scene">the T2 scene (Outside or Office)</param>
    /// <returns></returns>
    IEnumerator coInitT2(T2 scene)
	{
		transitioning = true;
		timer = 0f;

		Vector3 ccPos;
		Vector3 suspectPos;
		Vector3 officerPos;
	   

		if (scene == T2.OFFICE)
		{
		   
			SteamVR_Fade.View(Color.black, 1f);
			while (transitioning)
			{
				timer += Time.deltaTime;
				if (timer > 2.5f)
				{
					DM.initT2Dialogue();
					GRS.initGrammarT2();
					scFSM.setStatesT2();
					BGChars.SetActive(false);
					transitioning = false;
                    boss.SetActive(true);
					
					//camera
					ccPos = new Vector3(T2Waypoints[0].transform.position.x, CC.transform.position.y, T2Waypoints[0].transform.position.z);
					CC.transform.SetPositionAndRotation(ccPos, new Quaternion(0, 45, 0, 1));
					
					//Jim (Suspect)
					suspectPos = new Vector3(T2Waypoints[1].transform.position.x, suspect.transform.position.y, T2Waypoints[1].transform.position.z);
					suspect.transform.position = suspectPos;
					suspect.transform.LookAt(new Vector3(boss.transform.position.x, suspect.transform.position.y, boss.transform.position.z));

					//Officer
					officerPos = new Vector3(T2Waypoints[2].transform.position.x, Officer.transform.position.y, T2Waypoints[2].transform.position.z);
					Officer.transform.position = officerPos;
					Officer.transform.LookAt(new Vector3(suspect.transform.position.x, Officer.transform.position.y, suspect.transform.position.z));

                    currentScene = SCENE.T2_OFFICE;

					if (escalateT2 != null)
						StopCoroutine(escalateT2);

					fadetoClear(1f);
					escalateT2 = StartCoroutine(coOfficeSituation());
					
				}
				yield return 0;
			}

		}
		else if(scene == T2.OUTSIDE)
		{
		   
			while(timer < 3.5)
			{
				timer += Time.deltaTime;
				yield return 0;
			}
			timer = 0f;

			SteamVR_Fade.View(Color.black, 1f);
			while (transitioning)
			{
				timer += Time.deltaTime;
				if (timer > 2.5f)
				{
					DM.initT2Dialogue();
					GRS.initGrammarT2();
					scFSM.setStatesT2();
					BGChars.SetActive(false);
					transitioning = false;
                    BuildingOutside.SetActive(true);
                    OutsideSun.enabled = true;
					guard.SetActive(true);
					

					//positions

					//Jim (Suspect)
					suspectPos = new Vector3(T2Waypoints[4].transform.position.x, suspect.transform.position.y, T2Waypoints[4].transform.position.z);
					suspect.transform.position = suspectPos;
 
					//Camera
					ccPos = new Vector3(T2Waypoints[3].transform.position.x, CC.transform.position.y, T2Waypoints[3].transform.position.z);
					CC.transform.position = ccPos;

					//Officer
					officerPos = new Vector3(T2Waypoints[5].transform.position.x, Officer.transform.position.y, T2Waypoints[5].transform.position.z);
					Officer.transform.position = officerPos;

					//Guard
					
					
					//rotations

					//Jim (Suspect)
					suspect.transform.LookAt(new Vector3(CC.transform.position.x, suspect.transform.position.y, CC.transform.position.z));

					//Camera
					CC.transform.LookAt(new Vector3(suspect.transform.position.x, CC.transform.position.y, suspect.transform.position.z));

					//Officer
					Officer.transform.LookAt(new Vector3(suspect.transform.position.x, Officer.transform.position.y, suspect.transform.position.z));

					//Guard
					guard.transform.LookAt(new Vector3(suspect.transform.position.x, guard.transform.position.y, suspect.transform.position.z));

                    currentScene = SCENE.T2_OUTSIDE;

					if (escalateT2 != null)
						StopCoroutine(escalateT2);

					
                    interrupt = false;
                    OfficeLighting.SetActive(false);
                    BuildingInside.SetActive(false);
                    fadetoClear(1f);
                    escalateT2 = StartCoroutine(coOutsideSituation());
				}
				yield return 0;

			}
			
		}
	}

	/// <summary>
	/// coroutine for T2 Office situation;
	/// scripted sequence where boss and suspect exchange words.
	/// </summary>
	/// <returns></returns>
	IEnumerator coOfficeSituation()
	{
        int interruptCount = 0;
        Audio1 = suspect.GetComponent<AudioSource>();
		Audio2 = boss.GetComponent<AudioSource>();
        JimAnim.SetBool("Office", true);
        float coTimer = 0f;

        //=============================
        //FIRST DEFUSE SITUATION
        //=============================
        for (int i = 0; i < RangeConstants.office_firstDefuse; i++)
		{
            //JIM has one line that is back to back (special case)
            if (i == RangeConstants.office_firstDefuse - 1)
            {
                Audio1.clip = OfficeScriptClips[i];
                Audio1.Play();
                JimAnim.SetInteger("Scripted Index", i);
                JimAnim.SetTrigger("PlayScripted");
                while (Audio1.isPlaying)
                {
                    if (interrupt)
                    {
                        interruptCount++;
                        goto interrupted;
                    }
                    else
                        yield return 0;
                }

                scFSM.triggerKill();
                suspectGun.SetActive(true);
                while (coTimer < 2.5f)
                {
                    coTimer += Time.deltaTime;
                    yield return 0;
                }

                Audio1.clip = ExtraSFX[RangeConstants.gunShot_index];
                Audio1.Play();

                while (Audio1.isPlaying)
                    yield return 0;

                scFSM.Grumble = false;
                scenarioResult = SCENARIO_RESULT.SUSPECT_KILL;
                T3Ending = StartCoroutine(scenarioEndingKill());
                break;
            }

            if (i % 2 == 0)//JIM
			{
				Audio1.clip = OfficeScriptClips[i];
				Audio1.Play();
                JimAnim.SetInteger("Scripted Index", i);
                JimAnim.SetTrigger("PlayScripted");
                while (Audio1.isPlaying)
				{
                    if (interrupt)
                    {
                        interruptCount++;
                        goto interrupted;
                    }
                    else
                        yield return 0;
                }
            }
			else//BOSS
			{
                Audio2.clip = OfficeScriptClips[i];
                Audio2.Play();
                BossAnim.SetInteger("Scripted Index", i);
                BossAnim.SetTrigger("PlayScripted");
                while (Audio2.isPlaying)
                {
                    if (interrupt)
                    {
                        interruptCount++;
                        goto interrupted;
                    }
                    else
                        yield return 0;
                }
                
			}
		}

        interrupted:
        JimAC.turnToCamera();
        JimAnim.SetTrigger("interrupt");

        //let lines finish
        while (Audio1.isPlaying)
        {
            yield return 0;
        }

        while (Audio2.isPlaying)
        {
            yield return 0;
        }

        //interrupt = false;
        //coTimer = 0f;

        //=============================
        //SECOND DEFUSE SITUATION
        //=============================
        //if (interruptCount > 0)
        //{
        //    while (coTimer < 2f)
        //    {
        //        coTimer += Time.deltaTime;
        //        yield return 0;
        //    }

        //    scFSM.setDefuseScore(2);

        //    for (int j = RangeConstants.office_firstDefuse; j < RangeConstants.office_secondDefuse; ++j)
        //    {
        //        if (interrupt)
        //        {
        //            JimAC.turnToCamera();
        //            JimAnim.SetTrigger("interrupt");
        //            interruptCount++;
        //            break;
        //        }

        //        if (j % 2 == 0)//BOSS
        //        {
        //            Audio2.clip = OfficeScriptClips[j];
        //            Audio2.Play();
        //            //BossAnim.SetInteger("Scripted Index", i);
        //            //BossAnim.SetTrigger("PlayScripted");
        //            if (j == RangeConstants.office_firstDefuse)
        //                JimAC.turnToBoss();

        //            while (Audio2.isPlaying)
        //            {
        //                yield return 0;
        //            }
        //        }
        //        else//JIM
        //        {

        //            Audio1.clip = OfficeScriptClips[j];
        //            Audio1.Play();
        //            JimAnim.SetInteger("Scripted Index", j);
        //            JimAnim.SetTrigger("PlayScripted");
        //            while (Audio1.isPlaying)
        //            {
        //                yield return 0;
        //            }

        //            if (j == RangeConstants.office_firstDefuse - 1)
        //            {
        //                suspectGun.SetActive(true);
        //                while (coTimer < 2.5f)
        //                {
        //                    coTimer += Time.deltaTime;
        //                    yield return 0;
        //                }

        //                Audio1.clip = ExtraSFX[RangeConstants.gunShot_index];
        //                Audio1.Play();
        //                scFSM.Grumble = false;
        //                break;
        //            }

        //        }

        //    }

        //    interrupt = false;
        //}

        //went too far; trigger suspect shooting boss
        //if(!scFSM.Defused)
        //    scFSM.triggerKill();
    }

	/// <summary>
	/// coroutine for T2 Outside situation;
	/// scripted sequence where security guard and suspect exchange words.
	/// </summary>
	/// <returns></returns>
	IEnumerator coOutsideSituation()
	{
        int interruptCount = 0;
		Audio1 = suspect.GetComponent<AudioSource>();
		Audio2 = guard.GetComponent<AudioSource>();
        JimAnim.SetBool("Outside", true);
		//animate guard to drop stuff (play dropping stuff audio)
		Audio2.clip = ExtraSFX[RangeConstants.dropStuff_index];
		Audio2.Play();
		float coTimer = 0f;

        JimAnim.SetInteger("Scripted Index", 0);
        JimAnim.SetTrigger("PlayScripted");

        //=============================
        //FIRST DEFUSE SITUATION
        //=============================
        for (int i = 0; i < RangeConstants.outside_firstDefuse; i++)
		{
			if (i % 2 == 0)//JIM
			{
				Audio1.clip = OutsideScriptClips[i];
				Audio1.Play();

                if (i > 0)
                {
                    JimAnim.SetInteger("Scripted Index", i);
                    JimAnim.SetTrigger("PlayScripted");
                }

                while (Audio1.isPlaying)
				{
                    if(interrupt)
                    {
                        interruptCount++;
                        goto interrupted;
                    }
                    else
					    yield return 0;
				}

                if(i == RangeConstants.outside_firstDefuse-1)
                {
                    scFSM.triggerKill();
                    suspectGun.SetActive(true);
                    
                    while (coTimer < 1.25f)
                    {
                        coTimer += Time.deltaTime;
                        yield return 0;
                    }

                    Audio1.clip = ExtraSFX[RangeConstants.gunShot_index];
                    Audio1.Play();

                    while (Audio1.isPlaying)
                        yield return 0;
                    //Audio1.PlayOneShot(ExtraSFX[RangeConstants.gunShot_index]);
                    scFSM.Grumble = false;
                    scenarioResult = SCENARIO_RESULT.SUSPECT_KILL;

                    T3Ending = StartCoroutine(scenarioEndingKill());
                    break;
                }
			}
			else//GUARD
			{
				Audio2.clip = OutsideScriptClips[i];
				Audio2.Play();
                GuardAnim.SetInteger("Scripted Index", i);
                GuardAnim.SetTrigger("PlayScripted");
                while (Audio2.isPlaying)
				{
                    if (interrupt)
                    {
                        interruptCount++;
                        goto interrupted;
                    }
                    else
                        yield return 0;
				}
			}
		}

        interrupted:
        JimAC.turnToCamera();
        JimAnim.SetTrigger("interrupt");

        //let lines finish
        while (Audio1.isPlaying)
        {
            yield return 0;
        }

        while(Audio2.isPlaying)
        {
            yield return 0;
        }
        
        //reset the interrupt variable
        interrupt = false;
        coTimer = 0f;

        //=============================
        //SECOND DEFUSE SITUATION
        //=============================
        if (interruptCount > 0)
        {
            while (coTimer < 2f)
            {
                coTimer += Time.deltaTime;
                yield return 0;
            }

            scFSM.setDefuseScore(2);

            for (int j = RangeConstants.outside_firstDefuse; j < RangeConstants.outside_secondDefuse; ++j)
            {
                if (j % 2 == 0)//JIM
                {
                    Audio1.clip = OutsideScriptClips[j];
                    Audio1.Play();
                    JimAnim.SetInteger("Scripted Index", j);
                    JimAnim.SetTrigger("PlayScripted");
                    while (Audio1.isPlaying)
                    {

                        if (interrupt)
                            goto interrupted2;
                        else
                            yield return 0;
                    }

                    if (j == RangeConstants.outside_firstDefuse - 1)
                    {
                        scFSM.triggerKill();
                        suspectGun.SetActive(true);

                        while (coTimer < 2f)
                        {
                            coTimer += Time.deltaTime;
                            yield return 0;
                        }

                        Audio1.clip = ExtraSFX[RangeConstants.gunShot_index];
                        Audio1.Play();

                        while (Audio1.isPlaying)
                            yield return 0;

                        scFSM.Grumble = false;
                        scenarioResult = SCENARIO_RESULT.SUSPECT_KILL;
                        T3Ending = StartCoroutine(scenarioEndingKill());
                        break;
                    }
                }
                else//GUARD
                {
                    Audio2.clip = OutsideScriptClips[j];
                    Audio2.Play();
                    GuardAnim.SetInteger("Scripted Index", j);
                    GuardAnim.SetTrigger("PlayScripted");
                    while (Audio2.isPlaying)
                    {

                        if (interrupt)
                            goto interrupted2;
                        else
                            yield return 0;
                    }
                }

            }
            
            interrupted2:
            JimAC.turnToCamera();
            JimAnim.SetTrigger("interrupt");

            //let lines finish
            while (Audio1.isPlaying)
            {
                yield return 0;
            }

            while (Audio2.isPlaying)
            {
                yield return 0;
            }

            interrupt = false;
        }

        //went too far; trigger suspect shooting guard
        //if (!scFSM.Defused)
			//scFSM.triggerKill();
	}

    //
    //---------------------------------------->>TIER 3 SCENARIO SEQUENCE<<----------------------------------------
    //

    /// <summary>
    /// starts the final portion of the scenario; the ending
    /// </summary>
    public void startT3()
    {
        if(T3Ending == null)
            T3Ending = StartCoroutine(scenarioEndingTalk());
    }


    IEnumerator scenarioEndingTalk()
    {
        float timer_ = 2;

        //stop the grammar recognizer
        GRS.scenarioCleanUp();

        Audio1 = suspect.GetComponent<AudioSource>();
        Audio2 = Officer.GetComponent<AudioSource>();

        timer_ = 2f;
        //wait a few second
        while (timer_ > 0)
        {
            timer_ -= Time.deltaTime;
            yield return 0;
        }

        for (int i = 0; i < EndingClips.Length; i++)
        {
            if (i % 2 == 0)
            {
                Audio1.clip = EndingClips[i];
                Audio1.Play();
                JimAnim.SetInteger("EndIndex", i);
                JimAnim.SetTrigger("PlayScriptedEnd");
                while (Audio1.isPlaying)
                {
                    yield return 0;
                }
            }
            else
            {
                Audio2.clip = EndingClips[i];
                Audio2.Play();
                copAnim.SetInteger("EndIndex", i);
                copAnim.SetTrigger("PlayScriptedEnd");
            }
        }

        timer_ = 2;
        //wait a few second
        while (timer_ > 0)
        {
            timer_ -= Time.deltaTime;
            yield return 0;
        }

        fadetoBlack(1f);
        //to the ending screen (tells the user their outcome)

        timer_ = 2;
        //wait a few second
        while (timer_ > 0)
        {
            timer_ -= Time.deltaTime;
            yield return 0;
        }

        //move camera (user) position
        CC.transform.position = new Vector3(ResultsWaypoint.transform.position.x, CC.transform.position.y, ResultsWaypoint.transform.position.z);
        CC.transform.rotation = new Quaternion(0, 0, 0, 1);


        if(currentScene == SCENE.T2_OUTSIDE)
        {
            results_text.text = "Result: PASS\n\nThe situation was resolved successfully; the suspect was isolated and controlled without the need to use force.\n\n\nYou will be returned to the Main Menu shortly.";//PASS
        }
        else if (currentScene == SCENE.T2_OFFICE)
        {
            results_text.text = "Result: PASS\n\nThe situation was resolved successfully, but not optimally; the situation escalated unnecessarily into a confrontation with between the suspect and his boss.\n\n\nYou will be returned to the Main Menu shortly.";//PASS
        }

        //fade to clear
        fadetoClear(1f);

        //show results screen (outcome; pass/fail; reason for pass/fail; accuracy?)
        results_cg.alpha = 1;

        //Count Down
        timer_ = 8;

        //wait a few second
        while (timer_ > 0)
        {
            timer_ -= Time.deltaTime;
            if(timer_ <= 3f)
                fadetoBlack(1f);
            yield return 0;
        }
                
        //return back to Main Menu
        SceneManager.LoadScene(0);
    }


    IEnumerator scenarioEndingKill()
    {
        float timer_ = 2;
        //stop all audio
        Audio1.Stop();
        Audio2.Stop();
        suspect.GetComponent<AudioSource>().Stop();

        //stop all co-routines except this one
        if (escalateT2 != null)
            StopCoroutine(escalateT2);
        if (WaitShot != null)
            StopCoroutine(WaitShot);
        scFSM.stopCoroutines();        

        //stop the scenario (freeze all animations, except the gun)
        JimAnim.enabled = false;
        copAnim.enabled = false;
        if(scFSM.Tier2)
        {
            if (boss.activeSelf == true)
                BossAnim.enabled = false;
            else if (guard.activeSelf == true)
                GuardAnim.enabled = false;
        }

        //stop the grammar recognizer
        GRS.scenarioCleanUp();

        //wait a few second
        while (timer_ > 0)
        {
            timer_ -= Time.deltaTime;
            yield return 0;
        }

        //fade to black
        fadetoBlack(1f);

        timer_ = 2;
        //wait a few second
        while (timer_ > 0)
        {
            timer_ -= Time.deltaTime;
            yield return 0;
        }

        //move camera (user) position
        CC.transform.position = new Vector3(ResultsWaypoint.transform.position.x, CC.transform.position.y, ResultsWaypoint.transform.position.z);
        CC.transform.rotation = new Quaternion(0, 0, 0, 1);

        //calculate results
        results_text.text = calculateFinalResult();

        //fade to clear
        fadetoClear(1f);
        
        //show results screen (outcome; pass/fail; reason for pass/fail; accuracy?)
        results_cg.alpha = 1;


        //Count Down
        timer_ = 8;

        //wait a few second
        while (timer_ > 0)
        {
            timer_ -= Time.deltaTime;
            if (timer_ <= 3f)
                fadetoBlack(1f);
            yield return 0;
        }

        //return back to Main Menu
        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// calculates the result of the scenario (what to display to the user)
    /// </summary>
    /// <returns>string containing the result of the scenario</returns>
    public string calculateFinalResult()
    {
        string result = "";
        //what scene is it?
        if (currentScene == SCENE.INTRO)
        {
            result = "Result: FAIL\n\nThere was no threat; discharge of a firearm without reasonable grounds for believing it necessary is illegal.\n\n\nYou will be returned to the Main Menu shortly.";//FAIL
        }
        else if (currentScene == SCENE.T1)
        {
            checkVictimT1:
            SHOT victim = (SHOT)Enumerable.Range(0, shot.Count()).Aggregate((a, b) => (shot[a] > shot[b]) ? a : b);

            //account for bad accuracy (user may have missed more shots than they hit)
            if (victim == SHOT.NOTHING)
            {
                for (int i = 0; i < shot.Count()-1; i++)
                {
                    if(shot[i] > 0)
                    {
                        shot[(int)SHOT.NOTHING] = 0;
                        goto checkVictimT1;
                    }
                }

                result = "Result: FAIL\n\nThere was no threat; discharge of a firearm without reasonable grounds for believing it necessary is illegal.\n\n\nYou will be returned to the Main Menu shortly.";//FAIL
            }
            else if (victim == SHOT.JIM)
            {
                result = "Result: FAIL\n\nThe suspect was not an immediate danger to anyone's life.\n\n\nYou will be returned to the Main Menu shortly.";//FAIL
            }
            
        }
        else
        {
            //T2; who did the shooting?
            if (scFSM.Kill)
            {
                //suspect did the killing
                if(currentScene == SCENE.T2_OUTSIDE)
                    result = "Result: FAIL\n\nThe situation was not defused resulting in the suspect shooting the security guard.";//FAIL
                else
                    result = "Result: FAIL\n\nThe situation was not defused resulting in the suspect shooting the boss.";//FAIL
            }
            else
            {
                if (scenarioResult == SCENARIO_RESULT.DISCHARGE)
                {
                    //Illegal discharge of weapon
                    result = "Result: FAIL\n\nThere was no threat; discharge of a firearm without reasonable grounds for believing it necessary is illegal.\n\n\nYou will be returned to the Main Menu shortly.";//FAIL
                }
                else
                {   
                    //check accuracy
                    //user did the shooting and it was a person; who was shot?
                    SHOT victim = (SHOT)Enumerable.Range(0, shot.Count()).Aggregate((a, b) => (shot[a] > shot[b]) ? a : b);
                    checkVictim:
                    switch (victim)
                    {
                        case SHOT.NOTHING:
                            shot[(int)SHOT.NOTHING] = 0;
                            victim = (SHOT)Enumerable.Range(0, shot.Count()).Aggregate((a, b) => (shot[a] > shot[b]) ? a : b);
                            goto checkVictim;
                        case SHOT.JIM:
                            //was kill triggered?
                            if(scFSM.Kill)
                            {
                                result = "Result: PASS\n\nThe suspect was an immediate danger.\n\n\nYou will be returned to the Main Menu shortly.";
                            }
                            else
                            {
                                result = "Result: FAIL\n\nThe suspect was not an immediate danger to anyone's life.\n\n\nYou will be returned to the Main Menu shortly.";//FAIL
                            }
                            break;
                        case SHOT.GUARD:
                            //no reason for this
                            result = "Result: FAIL\n\nThe guard was not a threat.\n\n\nYou will be returned to the Main Menu shortly.";
                            break;
                        case SHOT.BOSS:
                            //no reason for this
                            result = "Result: FAIL\n\nThe boss was not a threat.\n\n\nYou will be returned to the Main Menu shortly.";
                            break;
                    }
                }
            }
        }

        return result;
    }
}
