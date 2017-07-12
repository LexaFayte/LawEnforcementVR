using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateHighAggro : StateBase
{
    public StateHighAggro(SuspectControllerFSM SC)
    {
        transitionA2 = false;
        stateID = STATE.HIGH_AGGRO;
        scFSM = SC;
        AS = scFSM.suspect.GetComponent<SuspectControllerFSM>().dialogueSource;
    }


    public override void Enter(List<string> semantics)
    {
        scFSM.suspect.GetComponent<SuspectControllerFSM>().setCurrentState(this);
        aggroScore = scFSM.suspect.GetComponent<SuspectControllerFSM>().getAggroScore();
        scFSM.suspect.GetComponent<Renderer>().material.color = Color.red;
        if (semantics != null)
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

        if (aggroScore <= 3.4f)
        {
            scFSM.suspect.GetComponent<SuspectControllerFSM>().getState(STATE.LOW_AGGRO).Enter(semantics);
        }
        else if (aggroScore <= 7.4f)
        {
            scFSM.suspect.GetComponent<SuspectControllerFSM>().getState(STATE.MED_AGGRO).Enter(semantics);
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
        switch (tag)
        {
            case "Name":
                audioIndex = UnityEngine.Random.Range(RangeConstants.name_A3, clips.Length);
                AS.clip = clips[audioIndex];
                break;
            case "HeyYou":
                audioIndex = UnityEngine.Random.Range(RangeConstants.heyYou_A3, clips.Length);
                AS.clip = clips[audioIndex];
                break;
            case "Insult":
                audioIndex = UnityEngine.Random.Range(RangeConstants.insult_A3, clips.Length);
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
                transitionA2 = true;
                break;
            case "Resist":
                if (aggroScore < 10)
                {
                    audioIndex = UnityEngine.Random.Range(RangeConstants.resist_A3, clips.Length);
                    AS.clip = clips[audioIndex];
                }
                else
                {
                    audioIndex = UnityEngine.Random.Range(0, RangeConstants.leave_count);
                    AS.clip = scFSM.CC.GetComponent<DialogueManager>().getSingleClips("Leave")[audioIndex];
                    transitionA2 = true;
                }
                break;
            case "StepOut":
                audioIndex = UnityEngine.Random.Range(RangeConstants.stepOut_A3, clips.Length);
                AS.clip = clips[audioIndex];
                break;
            case "TalkReason":
                audioIndex = UnityEngine.Random.Range(RangeConstants.talkReason_A3, clips.Length);
                AS.clip = clips[audioIndex];
                break;

        }
    }

    public override STATE getStateID()
    {
        return stateID;
    }
}
