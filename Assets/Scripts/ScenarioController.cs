using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SCENE { INTRO,T1,T2};
public enum T2 { OFFICE, OUTSIDE};

public class ScenarioController : MonoBehaviour {

    //public objects
    public GameObject CC;
    public GameObject suspect;
    public GameObject boss;
    public GameObject Officer;
    public GameObject car;
    public GameObject[] T1waypoints;
    public GameObject[] T2Waypoints;
    public Light OutsideSun;
    public GameObject BGChars;
    public AudioClip[] IntroClips;
    public AudioClip[] TutorialClips;
    public GameObject drivingWaypoint;
    public GameObject drivingStrip;
    
    //private objects and vars
    private SuspectControllerFSM scFSM;
    private float timer;
    private Coroutine MoveCar;
    private Coroutine PlayIntroAudio;
    private Coroutine initT1;
    private Coroutine OfficerTut;
    private Coroutine initT2;
    private bool transitioning;
    private GrammarRecognizerScript GRS;
    private DialogueManager DM;
    private CameraMove CM;
    private AudioSource AudioOfficer;
    private AudioSource AudioCar;
    private SCENE currentScene;
    private SteamVR_PlayArea playArea;
    private Animator copAnim;

    private void Awake()
    { 
        scFSM = suspect.GetComponent<SuspectControllerFSM>();
        scFSM.setScenarioController(this);
        timer = 0f;
        transitioning = false;
        GRS = CC.GetComponent<GrammarRecognizerScript>();
        DM = CC.GetComponent<DialogueManager>();
        CM = CC.GetComponent<CameraMove>();
        AudioOfficer = Officer.GetComponent<AudioSource>();
        AudioCar = car.GetComponent<AudioSource>();
        playArea = CC.GetComponentInChildren<SteamVR_PlayArea>();
        copAnim = Officer.GetComponent<Animator>();
        currentScene = SCENE.INTRO;
    }

    // Use this for initialization
    void Start () {
        fadetoClear(2.5f);
        Invoke("PlayCarIntro", 2);
	}
	
	// Update is called once per frame
	void Update () {
        if (!transitioning && timer > 0)
        {
            fadetoClear(3);
            timer = 0;
        }
	}

    /// <summary>
    /// play the scenario introduction
    /// </summary>
    public void PlayCarIntro()
    {
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
                AudioCar.clip = IntroClips[i];
                AudioCar.Play();
                while(AudioCar.isPlaying)
                {
                    yield return 0;
                }
            }
            else
            {
                copAnim.SetTrigger("Radio");
                AudioOfficer.clip = IntroClips[i];
                AudioOfficer.Play();
                while(AudioOfficer.isPlaying)
                {
                    yield return 0;
                }
            }
        }

        fadetoBlack(2);
        initT1 = StartCoroutine(coInitT1());
    }

    /// <summary>
    /// start Tier 1 in of the scenario (will start it after 2.5s)
    /// </summary>
    private void startT1()
    {
        CM.Invoke("startMovement", 3f);
    }

    /// <summary>
    /// coroutine for initializing Tier 1 in the scenario
    /// </summary>
    /// <returns></returns>
    IEnumerator coInitT1()
    {
        transitioning = true;
        timer = 0f;
        StopCoroutine(MoveCar);
        drivingStrip.SetActive(false);

        while (transitioning)
        {
            timer += Time.deltaTime;
            if (timer > 2f)
            {
                transitioning = false;
                //move Officer to beside you
                Officer.transform.position = new Vector3(7f, 0.015f, 2f);
                Officer.transform.Rotate(Vector3.up, 25);
                Officer.transform.eulerAngles = new Vector3(0, Officer.transform.eulerAngles.y, 0);

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

        OfficerTut = StartCoroutine(coTut());

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
                    AudioOfficer.clip = TutorialClips[i];
                    AudioOfficer.Play();

                    if (i == 2)
                    {
                        CM.Invoke("startMovement", 3.5f);
                       
                    }
                    while (AudioOfficer.isPlaying)
                    {
                        yield return 0;
                    }
                }


                copAnim.SetTrigger("Walk");
                Officer.transform.Rotate(Vector3.up, -40);
                Officer.transform.eulerAngles = new Vector3(0, Officer.transform.eulerAngles.y, 0);

                //while (Officer.transform.position.z < T1waypoints[0].transform.position.z + 0.5)
                //{
                //    yield return 0;
                //}

                //copAnim.SetTrigger("Turn");
                //Officer.transform.Rotate(Vector3.up, 25);
                //Officer.transform.eulerAngles = new Vector3(0, Officer.transform.eulerAngles.y, 0);

                //while (Officer.transform.position.x < T1waypoints[1].transform.position.x - 2)
                //{
                //    yield return 0;
                //}

                //copAnim.SetTrigger("Stop");
                //Officer.transform.position = new Vector3(Officer.transform.position.x, 0.015f, Officer.transform.position.z);
                //Officer.transform.eulerAngles = new Vector3(0, Officer.transform.eulerAngles.y, 0);
            }
        }
    }


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
           
            SteamVR_Fade.View(Color.black, 2f);
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
                    
                    //camera
                    ccPos = new Vector3(T2Waypoints[0].transform.position.x, CC.transform.position.y, T2Waypoints[0].transform.position.z);
                    CC.transform.SetPositionAndRotation(ccPos, new Quaternion(0, 45, 0, 1));
                    
                    //Jim (Suspect)
                    suspectPos = new Vector3(T2Waypoints[1].transform.position.x, suspect.transform.position.y, T2Waypoints[1].transform.position.z);
                    suspect.transform.position = suspectPos;
                    suspect.transform.LookAt(new Vector3(boss.transform.position.x, 0, boss.transform.position.z));

                    //Officer
                    officerPos = new Vector3(T2Waypoints[2].transform.position.x, Officer.transform.position.y, T2Waypoints[2].transform.position.z);
                    Officer.transform.position = officerPos;
                    Officer.transform.LookAt(new Vector3(suspect.transform.position.x, 0, suspect.transform.position.z));

                    
                    //activate boss
                    //boss.GetComponent<Animator>().SetBool("START", true);
                    
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

            SteamVR_Fade.View(Color.black, 2f);
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
                    OutsideSun.enabled = true;
                    
                    //Jim (Suspect)
                    suspectPos = new Vector3(T2Waypoints[4].transform.position.x, suspect.transform.position.y, T2Waypoints[4].transform.position.z);
                    suspect.transform.position = suspectPos;
                    suspect.transform.LookAt(new Vector3(CC.transform.position.x, 0, CC.transform.position.z));

                    //Camera
                    ccPos = new Vector3(T2Waypoints[3].transform.position.x, CC.transform.position.y, T2Waypoints[3].transform.position.z);
                    CC.transform.position = ccPos;
                    CC.transform.LookAt(new Vector3(suspect.transform.position.x, 0, suspect.transform.position.z));

                    //Officer
                    officerPos = new Vector3(T2Waypoints[5].transform.position.x, Officer.transform.position.y, T2Waypoints[5].transform.position.z);
                    Officer.transform.position = officerPos;
                    Officer.transform.LookAt(new Vector3(suspect.transform.position.x, 0, suspect.transform.position.z));
                }
                yield return 0;

            }
            
        }
    }
}
