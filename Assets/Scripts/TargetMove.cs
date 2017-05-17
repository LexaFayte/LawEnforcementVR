using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetMove : MonoBehaviour {

    //vars
    private bool move = false;
    private const float TIME_LIMIT = 3f;
    private const float LERP_TIME = 1.2f;
    private bool wasHit = false;
    private float timer = 3f;
    private bool up = true;
    private bool moving = false;
    private bool stop = false;
    private bool reset = false;

    private Vector3 finishUp;
    private Vector3 finishDown;

    public Vector3 stopPos;

    private Coroutine coUp;
    private Coroutine coDown;
    private Coroutine coDownHit;
    private Coroutine Display;

    private void Awake()
    {
        finishDown = transform.position;
        finishUp = transform.position + (Vector3.up * 2.2f);
    }

    //properties
    public bool Hit
    {
        get { return wasHit; }
        set { wasHit = value; }
    }


    //coroutines

    //move target up and down
    IEnumerator moveTarget(Vector3 start, Vector3 finish, float time)
    {
        if (!moving)
        {
            moving = true;
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / time;
                transform.position = Vector3.Lerp(start, finish, t);

                yield return 0;
            }
            moving = false;
            if (!up)
                wasHit = false;

            up = !up;
        }
    }

    //move target forward for display after finished shooting
    IEnumerator DisplayTargets(Vector3 start, Vector3 finish, float time)
    {
        if (!moving)
        {
            moving = true;
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / time;
                transform.position = Vector3.Lerp(start, finish, t);

                yield return 0;
            }
            moving = false;
            stop = false;
        }
    }

    IEnumerator ResetTargets(Vector3 start, Vector3 finish, float time)
    {
        if (!moving)
        {
            moving = true;
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / time;
                transform.position = Vector3.Lerp(start, finish, t);

                yield return 0;
            }
            moving = false;
            reset = false;
            up = true;
            timer = 3f;
            wasHit = false;
            foreach (Transform child in this.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }



    //functions
    public void Move()
    {
        move = true;
    }

    public void StopMove()
    {
        move = false;
        StopAllCoroutines();
        moving = false;
        stop = true;
    }

    public void Reset()
    {
        move = false;
        StopAllCoroutines();
        moving = false;
        reset = true;
    }


	// Update is called once per frame
	void Update () {
		
        if(move)
        {
            timer += Time.deltaTime;//add time to timer

            if(wasHit)
            {
                StopCoroutine(coUp);
                moving = false;
                coDownHit = StartCoroutine(moveTarget(transform.position, finishDown, LERP_TIME / 3));
                up = false;

                timer = 0f;
            }

            if(timer >= TIME_LIMIT)//if the timer expires
            {
                if (up)//if moving up
                    coUp = StartCoroutine(moveTarget(transform.position, finishUp, LERP_TIME));
                else//else moving down
                    coDown = StartCoroutine(moveTarget(transform.position, finishDown, LERP_TIME));

                timer = 0f;//reset timer
            }

        }
        else
        {
            if(stop)
            {
                Display = StartCoroutine(DisplayTargets(transform.position, stopPos, LERP_TIME / 3));
            }

            if(reset)
            {
                coDown = StartCoroutine(ResetTargets(transform.position, finishDown, LERP_TIME));
            }
        }
	}
}
