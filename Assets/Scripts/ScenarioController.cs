using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SCENE { INTRO,T1,T2};
public enum T2 { OFFICE, OUTSIDE};

public class ScenarioController : MonoBehaviour {

    public GameObject CC;//Camera Container
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



    
    //
    //public GameObject securityGuard;
    //public GameObject partnerCop;
    //

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
        fadetoClear();
        Invoke("PlayCarIntro", 2);
	}
	
	// Update is called once per frame
	void Update () {
        if (!transitioning && timer > 0)
        {
            fadetoClear();
            timer = 0;
        }
	}

    public void PlayCarIntro()
    {
        GameObject[] toMove = { CC, Officer, car };
        MoveCar = StartCoroutine(CM.moveObjectsToPoint(toMove,drivingWaypoint));
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

    public void fadetoBlack()
    {
        SteamVR_Fade.View(Color.black, 2f);
    }

    public void fadetoClear()
    {
        SteamVR_Fade.View(Color.clear, 2f);
    }

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
                AudioOfficer.clip = IntroClips[i];
                AudioOfficer.Play();
                while(AudioOfficer.isPlaying)
                {
                    yield return 0;
                }
            }
        }

        fadetoBlack();
        initT1 = StartCoroutine(coInitT1());
    }

    private void startT1()
    {
        CM.Invoke("startMovement", 2.5f);

    }


    IEnumerator coInitT1()
    {
        transitioning = true;
        timer = 0f;
        StopCoroutine(MoveCar);

        while (transitioning)
        {
            timer += Time.deltaTime;
            if (timer > 2.5f)
            {
                transitioning = false;
                playArea.size = SteamVR_PlayArea.Size._400x300;
                CC.transform.position = new Vector3(8.62f, 0.5051446f, 1.23f);
                CC.transform.rotation = new Quaternion(0, 0, 0, 1);

                //move Officer to beside you
                Officer.transform.position = new Vector3(7f, 0.015f, .4f);
                Officer.transform.Rotate(Vector3.up, -15);
                //Officer.transform.Rotate(Vector3.right, -Officer.transform.rotation.x);
                Officer.transform.eulerAngles = new Vector3(0, Officer.transform.eulerAngles.y, 0);


                //turn off outside lighting
                OutsideSun.enabled = false;
                currentScene = SCENE.T1;


            }
            yield return 0;
        }

        OfficerTut = StartCoroutine(coTut());

    }


    IEnumerator coTut()
    {
        bool tutorial = true;
        float timer = 0;
        while (tutorial)
        {
            
            timer += Time.deltaTime;

            if (timer > 1f)
            {
                fadetoClear();
            }

            if (timer > 3f)
            {
                tutorial = false;
                copAnim.SetTrigger("T1Tut");
                for (int i = 0; i < TutorialClips.Length; i++)
                {
                    AudioOfficer.clip = TutorialClips[i];
                    AudioOfficer.Play();

                    if (i == 2)
                    {
                        CM.Invoke("startMovement",2f);
                        copAnim.SetTrigger("Walk");
                    }
                    while (AudioOfficer.isPlaying)
                    {
                        yield return 0;
                    }
                }

                while (Officer.transform.position.z < T1waypoints[0].transform.position.z+0.5)
                {
                    yield return 0;
                }

                copAnim.SetTrigger("Turn");
                Officer.transform.Rotate(Vector3.up, 25);
                Officer.transform.eulerAngles = new Vector3(0, Officer.transform.eulerAngles.y, 0);

                while (Officer.transform.position.x < T1waypoints[1].transform.position.x)
                {
                    yield return 0;
                }

                copAnim.SetTrigger("Stop");
                Officer.transform.position = new Vector3(Officer.transform.position.x, 0.015f, Officer.transform.position.z);
                Officer.transform.eulerAngles = new Vector3(0, Officer.transform.eulerAngles.y, 0);
            }
        }
    }



    IEnumerator coInitT2(T2 scene)
    {
        transitioning = true;
        timer = 0f;

        Vector3 ccPos;
        Vector3 suspectPos;
       

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
                    //move camera container to T2_WaypointOffice and rotate 45 degrees
                    ccPos = new Vector3(T2Waypoints[0].transform.position.x, CC.transform.position.y, T2Waypoints[0].transform.position.z);
                    CC.transform.SetPositionAndRotation(ccPos, new Quaternion(0, 45, 0, 1));
                    //transport Jimmy into office looking at the boss
                    suspectPos = new Vector3(T2Waypoints[1].transform.position.x, suspect.transform.position.y, T2Waypoints[1].transform.position.z);
                    
                    suspect.transform.position = suspectPos;
                    suspect.transform.LookAt(new Vector3(boss.transform.position.x, 0, boss.transform.position.z));
                    
                    //activate boss
                    //boss.GetComponent<Animator>().SetBool("START", true);
                    
                }
                yield return 0;
            }

        }
        else// scene == T2.OUTSIDE
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
                    transitioning = false;
                    OutsideSun.enabled = true;
                    //move camera container to T2_WaypointOutside and rotate 45 degrees
                    ccPos = new Vector3(T2Waypoints[2].transform.position.x, CC.transform.position.y, T2Waypoints[2].transform.position.z);
                    CC.transform.position = ccPos;
                    CC.transform.LookAt(new Vector3(car.transform.position.x, 0, car.transform.position.z));
                    //SetPositionAndRotation(ccPos, new Quaternion(0, 180, 0, 1));
                    //transport Jimmy outside looking at you
                    suspectPos = new Vector3(T2Waypoints[3].transform.position.x, suspect.transform.position.y, T2Waypoints[3].transform.position.z);

                    suspect.transform.position = suspectPos;
                    suspect.transform.LookAt(new Vector3(CC.transform.position.x, 0, CC.transform.position.z));
                }
                yield return 0;

            }
            
        }
       
    }
}
