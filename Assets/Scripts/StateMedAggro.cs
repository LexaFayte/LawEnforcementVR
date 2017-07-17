﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMedAggro : StateBase
{


    public StateMedAggro(SuspectControllerFSM SC)
    {
        transitionA2 = false;
        stateID = STATE.MED_AGGRO;
        scFSM = SC;
        AS = scFSM.suspect.GetComponent<SuspectControllerFSM>().dialogueSource;

    }

    public override void Enter(List<string> semantics)
    {
        scFSM.suspect.GetComponent<SuspectControllerFSM>().setCurrentState(this);
        aggroScore = scFSM.suspect.GetComponent<SuspectControllerFSM>().getAggroScore();
        scFSM.suspect.GetComponent<Renderer>().material.color = Color.yellow;
        if(semantics != null)
            response(semantics);
    }

    public override void Exit()
    {
        //do stuff before exiting(resetting things? Is this needed?)
    }

    public override void UpdateState(float NewAggro, List<string> semantics)
    {
        //do update stuff
        aggroScore = NewAggro;

        if (aggroScore >= 7.5f)
        {
            scFSM.suspect.GetComponent<SuspectControllerFSM>().getState(STATE.HIGH_AGGRO).Enter(semantics);
        }
        else if (aggroScore <= 3.4f)
        {
            scFSM.suspect.GetComponent<SuspectControllerFSM>().getState(STATE.LOW_AGGRO).Enter(semantics);
        }
        else
        {
            //get parsed semantics and respond with correct anims and audio
            response(semantics);

        }
    }

    public void response(List<string> semantics)
    {
        List<string> Tags = scFSM.CC.GetComponent<DialogueManager>().semanticToAudio(semantics);
        List<AudioClip[]> audioClips = scFSM.CC.GetComponent<DialogueManager>().getAudioClips(Tags);
        switch (Tags.Count)
        {
            case 1:
                scFSM.PlaySuspectAudio(Tags, audioClips);
                break;
            case 2:
                scFSM.PlaySuspectAudio(Tags, audioClips);
                break;
            case 3:
                scFSM.PlaySuspectAudio(Tags, audioClips);
                break;
            case 4:
                scFSM.PlaySuspectAudio(Tags, audioClips);
                break;

        }
    }

    public override void selectAudio(string tag, AudioClip[] clips)
    {
        int audioIndex = 0;
        switch(tag)
        {
            case "Intro":
                audioIndex = UnityEngine.Random.Range(0, clips.Length);
                AS.clip = clips[audioIndex];
                break;
            case "Name":
                audioIndex = UnityEngine.Random.Range(RangeConstants.name_A2, RangeConstants.name_A3);
                AS.clip = clips[audioIndex];
                break;
            case "CopsHere":
                audioIndex = UnityEngine.Random.Range(0, clips.Length);
                AS.clip = clips[audioIndex];
                break;
            case "HeyYou":
                audioIndex = UnityEngine.Random.Range(RangeConstants.heyYou_A2, RangeConstants.heyYou_A3);
                AS.clip = clips[audioIndex];
                break;
            case "Insult":
                audioIndex = UnityEngine.Random.Range(RangeConstants.insult_A2, RangeConstants.insult_A3);
                AS.clip = clips[audioIndex];
                break;
            case "CalmDown":
                audioIndex = UnityEngine.Random.Range(0, clips.Length);
                AS.clip = clips[audioIndex];
                break;
            case "Question":
                audioIndex = UnityEngine.Random.Range(0, clips.Length);
                AS.clip = clips[audioIndex];
                break;
            case "TalkGun":
                audioIndex = UnityEngine.Random.Range(0, clips.Length);
                AS.clip = clips[audioIndex];
                break;
            case "PointGun":
                audioIndex = UnityEngine.Random.Range(0, clips.Length);
                AS.clip = clips[audioIndex];
                break;
            case "Leave":
                audioIndex = UnityEngine.Random.Range(0, clips.Length);
                AS.clip = clips[audioIndex];
                break;
            case "Resist":
                audioIndex = UnityEngine.Random.Range(RangeConstants.resist_A2, RangeConstants.resist_A3);
                AS.clip = clips[audioIndex];
                break;
            case "Purpose":
                audioIndex = UnityEngine.Random.Range(RangeConstants.purpose_A2, RangeConstants.purpose_A3);
                AS.clip = clips[audioIndex];
                break;
            case "StepOut":
                audioIndex = UnityEngine.Random.Range(RangeConstants.stepOut_A2, RangeConstants.stepOut_A3);
                AS.clip = clips[audioIndex];
                break;
            case "LosPass":
                audioIndex = UnityEngine.Random.Range(0, RangeConstants.losPass_count);
                AS.clip = clips[audioIndex];
                scFSM.LosTest = true;
                break;
            case "TalkReason":
                if (aggroScore <= 3.75)
                {
                    audioIndex = UnityEngine.Random.Range(RangeConstants.talkReason_A2, RangeConstants.talkReason_A3);
                    AS.clip = clips[audioIndex];
                    transitionA2 = true;
                }
                else
                {
                    audioIndex = UnityEngine.Random.Range(RangeConstants.resist_A2+1, RangeConstants.resist_A3);
                    AS.clip = scFSM.CC.GetComponent<DialogueManager>().getSingleClips("Resist")[audioIndex];
                }
                break;
            case "Talk":
                audioIndex = UnityEngine.Random.Range(0, RangeConstants.talk_count);
                AS.clip = clips[audioIndex];
                break;
            case "Approach":
                audioIndex = UnityEngine.Random.Range(RangeConstants.approach_A2, RangeConstants.approach_A3);
                AS.clip = clips[audioIndex];
                break;
            case "Remove":
                audioIndex = UnityEngine.Random.Range(0, RangeConstants.remove_count);
                AS.clip = clips[audioIndex];
                break;
            case "RemovePersist":
                audioIndex = UnityEngine.Random.Range(0, RangeConstants.removePersist_count);
                AS.clip = clips[audioIndex];
                break;

        }
    }

    public override STATE getStateID()
    {
        return stateID;
    }
}
