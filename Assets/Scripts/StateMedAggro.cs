using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class StateMedAggro : StateBase
{


    public StateMedAggro(SuspectControllerFSM SC)
    {
        transitionA2 = false;
        stateID = STATE.MED_AGGRO;
        scFSM = SC;
        AS = scFSM.Suspect.GetComponent<SuspectControllerFSM>().dialogueSource;

    }

    public override void Enter(List<string> semantics)
    {
        scFSM.Suspect.GetComponent<SuspectControllerFSM>().setCurrentState(this);
        aggroScore = scFSM.Suspect.GetComponent<SuspectControllerFSM>().getAggroScore();
        //scFSM.Suspect.GetComponent<Renderer>().material.color = Color.yellow;
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
            scFSM.Suspect.GetComponent<SuspectControllerFSM>().getState(STATE.HIGH_AGGRO).Enter(semantics);
        }
        else if (aggroScore <= 3.4f)
        {
            scFSM.Suspect.GetComponent<SuspectControllerFSM>().getState(STATE.LOW_AGGRO).Enter(semantics);
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
        bool longclip = false;
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
                scFSM.Suspect.GetComponent<AnimController_Jim>().triggerAnswer(aggroScore, tag, longclip);
                break;
            case "CopsHere":
                audioIndex = UnityEngine.Random.Range(0, clips.Length);
                AS.clip = clips[audioIndex];
                break;
            case "HeyYou":
                audioIndex = UnityEngine.Random.Range(RangeConstants.heyYou_A2, RangeConstants.heyYou_A3);
                AS.clip = clips[audioIndex];
                scFSM.Suspect.GetComponent<AnimController_Jim>().triggerAnswer(aggroScore, tag, longclip);
                break;
            case "Insult":
                audioIndex = UnityEngine.Random.Range(RangeConstants.insult_A2, RangeConstants.insult_A3);
                AS.clip = clips[audioIndex];
                if (RangeConstants.longClipResist.Contains(audioIndex))
                    longclip = true;

                scFSM.Suspect.GetComponent<AnimController_Jim>().triggerAnswer(aggroScore, tag, longclip);//
                break;
            case "CalmDown":
                audioIndex = UnityEngine.Random.Range(0, clips.Length);
                AS.clip = clips[audioIndex];
                scFSM.Suspect.GetComponent<AnimController_Jim>().triggerAnswer(aggroScore, tag, longclip);
                break;
            case "Question":
                audioIndex = UnityEngine.Random.Range(0, clips.Length);
                AS.clip = clips[audioIndex];
                scFSM.Suspect.GetComponent<AnimController_Jim>().triggerAnswer(aggroScore, tag, longclip);
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
                if (RangeConstants.longClipResist.Contains(audioIndex))
                    longclip = true;
                scFSM.Suspect.GetComponent<AnimController_Jim>().triggerAnswer(aggroScore, tag, longclip);
                break;
            case "Purpose":
                if (aggroScore < 5)
                    audioIndex = 1;
                else
                    audioIndex = 2;

                AS.clip = clips[audioIndex];
                if (RangeConstants.longClipPurpose.Contains(audioIndex))
                    longclip = true;
                scFSM.Suspect.GetComponent<AnimController_Jim>().triggerAnswer(aggroScore, tag, longclip);
                break;
            case "StepOut":
                audioIndex = UnityEngine.Random.Range(RangeConstants.stepOut_A2, RangeConstants.stepOut_A3);
                AS.clip = clips[audioIndex];
                scFSM.Suspect.GetComponent<AnimController_Jim>().triggerAnswer(aggroScore, tag, longclip);
                break;
            case "LosPass":
                audioIndex = UnityEngine.Random.Range(0, RangeConstants.losPass_count);
                AS.clip = clips[audioIndex];
                scFSM.LosTest = true;
                if (RangeConstants.longClipLosPass.Contains(audioIndex))
                    longclip = true;
                scFSM.Suspect.GetComponent<AnimController_Jim>().triggerAnswer(aggroScore, tag, longclip);
                break;
            case "TalkReason":
                if (aggroScore < 5)
                {
                    audioIndex = UnityEngine.Random.Range(RangeConstants.talkReason_A2, RangeConstants.talkReason_A3);
                    AS.clip = clips[audioIndex];
                    scFSM.Suspect.GetComponent<AnimController_Jim>().triggerAnswer(aggroScore, tag, longclip);
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
                scFSM.Suspect.GetComponent<AnimController_Jim>().triggerAnswer(aggroScore, tag, longclip);
                break;
            case "Approach":
                audioIndex = UnityEngine.Random.Range(RangeConstants.approach_A2, RangeConstants.approach_A3);
                AS.clip = clips[audioIndex];
                scFSM.Suspect.GetComponent<AnimController_Jim>().triggerAnswer(aggroScore, tag, longclip);
                break;
            case "Remove":
                audioIndex = UnityEngine.Random.Range(0, RangeConstants.remove_count);
                AS.clip = clips[audioIndex];
                scFSM.Suspect.GetComponent<AnimController_Jim>().triggerAnswer(aggroScore, tag, longclip);
                break;
            case "RemovePersist":
                audioIndex = UnityEngine.Random.Range(0, RangeConstants.removePersist_count);
                AS.clip = clips[audioIndex];
                scFSM.Suspect.GetComponent<AnimController_Jim>().triggerAnswer(aggroScore, tag, longclip);
                break;

        }
    }

    public override STATE getStateID()
    {
        return stateID;
    }
}
