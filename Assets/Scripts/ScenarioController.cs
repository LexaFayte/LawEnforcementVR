using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum T2 { OFFICE, OUTSIDE};

public class ScenarioController : MonoBehaviour {

    public GameObject CC;//Camera Container
    public GameObject suspect;
    public GameObject boss;
    public GameObject[] T2Waypoints;

    private SuspectControllerFSM scFSM;
    private float timer;
    private Coroutine initT2;
    private bool transitioning;

    
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
    }
    // Use this for initialization
    void Start () {
	    	
	}
	
	// Update is called once per frame
	void Update () {
        if (!transitioning && timer > 0)
        {
            fadetoClear();
            timer = 0;
        }
	}

    public void initializeT2(T2 scene)
    {
        initT2 = StartCoroutine(coInitT2(scene));
    }

    public void fadetoClear()
    {
        SteamVR_Fade.View(Color.clear, 2f);
    }

    IEnumerator coInitT2(T2 scene)
    {
        transitioning = true;
        timer = 0f;

        Vector3 ccPos;
        Vector3 suspectPos;
        SteamVR_Fade.View(Color.black, 2f);

        if (scene == T2.OFFICE)
        {
           
            
            while (transitioning)
            {
                timer += Time.deltaTime;
                if (timer > 2.5f)
                {
                    transitioning = false;
                    //move camera container to T2_WaypointOffice and rotate 45 degrees
                    ccPos = new Vector3(T2Waypoints[0].transform.position.x, CC.transform.position.y, T2Waypoints[0].transform.position.z);
                    CC.transform.SetPositionAndRotation(ccPos, new Quaternion(0, 45, 0, 1));
                    //transport Jimmy into office looking at the boss
                    suspectPos = new Vector3(T2Waypoints[1].transform.position.x, suspect.transform.position.y, T2Waypoints[1].transform.position.z);
                    
                    suspect.transform.position = suspectPos;
                    suspect.transform.LookAt(new Vector3(boss.transform.position.x, 0, boss.transform.position.z));
                    //suspect.transform.rotation = Quaternion.LookRotation(boss.transform.position - suspect.transform.position);
                    
                    //activate boss
                    //boss.GetComponent<Animator>().SetBool("START", true);
                    
                }
                yield return 0;
            }

        }
        else// scene == T2.OUTSIDE
        {
            while (transitioning)
            {
                timer += Time.deltaTime;
                if (timer > 2.5f)
                {
                    //move camera container to T2_WaypointOffice and rotate 45 degrees
                    //ccPos = new Vector3(T2Waypoints[0].transform.position.x, CC.transform.position.y, T2Waypoints[0].transform.position.z);
                    //CC.transform.SetPositionAndRotation(ccPos, new Quaternion(0, 45, 0, 1));
                    //transport Jimmy into office looking at the boss
                    //suspectPos = new Vector3(T2Waypoints[1].transform.position.x, suspect.transform.position.y, T2Waypoints[1].transform.position.z);
                    //suspect.transform.SetPositionAndRotation(suspectPos, Quaternion.LookRotation(boss.transform.position - suspect.transform.position));
                    //activate boss
                    //boss.GetComponent<Animator>().SetBool("START", true);
                    transitioning = false;
                }
                yield return 0;

            }
            
        }
       
    }
}
