﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimController_Jim : MonoBehaviour {

    private Animator animator;
    private Transform jimTransform;
    private float timer;
    private float initiateQuirk;
    public GameObject CC;

    private void Awake()
    {
        animator = gameObject.GetComponent<Animator>();
        jimTransform = gameObject.transform;
        timer = 0f;
        initiateQuirk = 3.5f;
    }

    private void Update()
    {

        timer += Time.deltaTime;

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("breathing_idle"))
        {
            if (timer >= initiateQuirk)
            {
                recenter();
                animator.SetInteger("QuirkID", UnityEngine.Random.Range(0, 3));
                animator.SetBool("Quirk", true);
                timer = 0f;
                initiateQuirk = UnityEngine.Random.Range(2f, 3.5f);
            }
        }
        else
        {
            if (timer > 1f)
            {
                animator.SetBool("Quirk", false);
                timer = -1.5f;
            }
        }


    }

    private void recenter()
    {
        Vector3 target = new Vector3(CC.transform.position.x, 0, CC.transform.position.z);
        jimTransform.LookAt(target);
    }

    void OnAnimatorIK()
    {
        // set the speed of our animator to the public variable 'animSpeed'
        animator.SetLookAtWeight(0.05f);                    // set the Look At Weight - amount to use look at IK vs using the head's animation
    }

    /// <summary>
    /// trigger animation for the intro rant
    /// </summary>
    public void triggerIntroRant()
    {
        animator.SetTrigger("Rant");
        timer = 0;
    }

    /// <summary>
    /// trigger animation for the turing towards the cops
    /// </summary>
    public void triggerIntroCops()
    {
        animator.SetTrigger("Cops");
    }

    /// <summary>
    /// rant is over (stop the animation)
    /// </summary>
    public void stopRant()
    {
        animator.SetTrigger("Rant Over");
    }

    public void triggerAnswer(float aggro, string tag, bool longClip)
    {
        if (longClip)
        {
            animator.SetBool("LongClip", true);
        }
        else
        {
            animator.SetBool("LongClip", false);
        }

        animator.SetFloat("AggroLevel", aggro);

        switch (tag)
        {
            case "Name":
                animator.SetTrigger("Name");
                break;
            case "Question":
                animator.SetTrigger("Question");
                break;
            case "Insult":
                animator.SetTrigger("Insult");
                break;
            case "Resist":
                animator.SetTrigger("Resist");
                break;
            case "Approach":
                animator.SetTrigger("Approach");
                break;
            case "Purpose":
                animator.SetTrigger("Purpose");
                break;
            case "CalmDown":
                animator.SetTrigger("CalmDown");
                break;
            case "HeyYou":
                animator.SetTrigger("HeyYou");
                break;
            case "Talk":
                animator.SetTrigger("Talk");
                break;
            case "StepOut":
                animator.SetTrigger("StepOut");
                break;
            case "Remove":
                animator.SetTrigger("Remove");
                break;
            case "RemovePersist":
                animator.SetTrigger("RemovePersist");
                break;
            case "LosPass":
                animator.SetTrigger("LosPass");
                break;
            case "TalkReason":
                animator.SetTrigger("TalkReason");
                break;
            case "Leave":
                animator.SetTrigger("Leave");
                break;
        }
    }
}
