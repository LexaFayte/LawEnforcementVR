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
    //private Coroutine coDown;
    //private Coroutine coDownHit;
    //private Coroutine Display;

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

    /// <summary>
    /// coroutine for moving the target up and down
    /// </summary>
    /// <param name="start">start position</param>
    /// <param name="finish">end position</param>
    /// <param name="time">how long to lerp from start pos to end pos</param>
    /// <returns></returns>
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

    /// <summary>
    /// coroutine for moving the target forward for display when finished shooting
    /// </summary>
    /// <param name="start">start position</param>
    /// <param name="finish">end position</param>
    /// <param name="time">how long to lerp from start pos to end pos</param>
    /// <returns></returns>
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

    /// <summary>
    /// coroutine for resetting the target and removing all bullet marks
    /// </summary>
    /// <param name="start">start position</param>
    /// <param name="finish">end position</param>
    /// <param name="time">how long to lerp from start pos to end pos</param>
    /// <returns></returns>
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

    /// <summary>
    /// sets the move flag to true
    /// </summary>
    public void Move()
    {
        move = true;
    }

    /// <summary>
    /// sets the move flag to flase and stops all target motion
    /// </summary>
    public void StopMove()
    {
        move = false;
        StopAllCoroutines();
        moving = false;
        stop = true;
    }

    /// <summary>
    /// resets the target
    /// </summary>
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
                StartCoroutine(moveTarget(transform.position, finishDown, LERP_TIME / 3));
                up = false;

                timer = 0f;
            }

            if(timer >= TIME_LIMIT)//if the timer expires
            {
                if (up)//if moving up
                    coUp = StartCoroutine(moveTarget(transform.position, finishUp, LERP_TIME));
                else//else moving down
                    StartCoroutine(moveTarget(transform.position, finishDown, LERP_TIME));

                timer = 0f;//reset timer
            }

        }
        else
        {
            if(stop)
            {
                StartCoroutine(DisplayTargets(transform.position, stopPos, LERP_TIME / 3));
            }

            if(reset)
            {
                StartCoroutine(ResetTargets(transform.position, finishDown, LERP_TIME));
            }
        }
	}
}
