using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class StateLowAggro : StateBase
{
    public StateLowAggro(SuspectControllerFSM SC)
    {
        transitionA2 = false;
        stateID = STATE.LOW_AGGRO;
        scFSM = SC;
        DM = scFSM.CC.GetComponent<DialogueManager>();
        AS = scFSM.dialogueSource;
        JimAC = scFSM.GetComponent<AnimController_Jim>();
    }

    public override void Enter(List<string> semantics)
    {
        scFSM.setCurrentState(this);
        aggroScore = scFSM.getAggroScore();
        //scFSM.Suspect.GetComponent<Renderer>().material.color = Color.yellow;
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
        
        if(aggroScore >= 7.5f)
        {
            scFSM.getState(STATE.HIGH_AGGRO).Enter(semantics);
        }
        else if(aggroScore >= 3.5f)
        {
            scFSM.getState(STATE.MED_AGGRO).Enter(semantics);
        }
        else
        {
            //get parsed semantics and respond with correct anims and audio
            response(semantics);
        }
    }


    public void response(List<string> semantics)
    {
        List<string> Tags = DM.semanticToAudio(semantics);
        List<AudioClip[]> audioClips = DM.getAudioClips(Tags);
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

        if (!T2)
        {
            switch (tag)
            {
                case "Name":
                    audioIndex = UnityEngine.Random.Range(RangeConstants.name_A1, RangeConstants.name_A2);
                    AS.clip = clips[audioIndex];
                    JimAC.triggerAnswer(aggroScore, tag, longclip);
                    break;
                case "HeyYou":
                    audioIndex = UnityEngine.Random.Range(RangeConstants.heyYou_A1, RangeConstants.heyYou_A2);
                    AS.clip = clips[audioIndex];
                    JimAC.triggerAnswer(aggroScore, tag, longclip);
                    break;
                case "Insult":
                    audioIndex = UnityEngine.Random.Range(RangeConstants.insult_A1, RangeConstants.insult_A2);
                    AS.clip = clips[audioIndex];
                    JimAC.triggerAnswer(aggroScore, tag, longclip);//
                    break;
                case "CalmDown":
                    audioIndex = UnityEngine.Random.Range(0, clips.Length);
                    AS.clip = clips[audioIndex];
                    JimAC.triggerAnswer(aggroScore, tag, longclip);
                    break;
                case "Question":
                    audioIndex = UnityEngine.Random.Range(0, clips.Length);
                    AS.clip = clips[audioIndex];
                    JimAC.triggerAnswer(aggroScore, tag, longclip);
                    break;
                case "TalkGun":
                    audioIndex = UnityEngine.Random.Range(0, clips.Length);
                    AS.clip = clips[audioIndex];
                    break;
                case "PointGun":
                    audioIndex = UnityEngine.Random.Range(0, clips.Length);
                    AS.clip = clips[audioIndex];
                    break;
                case "Resist":
                    audioIndex = UnityEngine.Random.Range(RangeConstants.resist_A1, RangeConstants.resist_A2);
                    AS.clip = clips[audioIndex];
                    JimAC.triggerAnswer(aggroScore, tag, longclip);
                    break;
                case "Purpose":
                    audioIndex = UnityEngine.Random.Range(RangeConstants.purpose_A1, RangeConstants.purpose_A2);
                    AS.clip = clips[audioIndex];
                    JimAC.triggerAnswer(aggroScore, tag, longclip);
                    break;
                case "LosPass":
                    audioIndex = UnityEngine.Random.Range(0, RangeConstants.losPass_count);
                    AS.clip = clips[audioIndex];
                    scFSM.LosTest = true;
                    if (RangeConstants.longClipLosPass.Contains(audioIndex))
                        longclip = true;
                    JimAC.triggerAnswer(aggroScore, tag, longclip);
                    break;
                case "StepOut":
                    audioIndex = UnityEngine.Random.Range(RangeConstants.stepOut_A1, RangeConstants.stepOut_A2);
                    AS.clip = clips[audioIndex];
                    JimAC.triggerAnswer(aggroScore, tag, longclip);
                    break;
                case "TalkReason":
                    audioIndex = UnityEngine.Random.Range(RangeConstants.talkReason_A1, RangeConstants.talkReason_A2);
                    AS.clip = clips[audioIndex];
                    JimAC.triggerAnswer(aggroScore, tag, longclip);
                    transitionA2 = true;
                    break;
                case "Talk":
                    audioIndex = UnityEngine.Random.Range(0, RangeConstants.talk_count);
                    AS.clip = clips[audioIndex];
                    JimAC.triggerAnswer(aggroScore, tag, longclip);
                    break;
                case "Approach":
                    audioIndex = UnityEngine.Random.Range(RangeConstants.approach_A1, RangeConstants.approach_A2);
                    AS.clip = clips[audioIndex];
                    JimAC.triggerAnswer(aggroScore, tag, longclip);
                    break;
                case "Remove":
                    audioIndex = UnityEngine.Random.Range(0, RangeConstants.remove_count);
                    AS.clip = clips[audioIndex];
                    JimAC.triggerAnswer(aggroScore, tag, longclip);
                    break;
                case "RemovePersist":
                    audioIndex = UnityEngine.Random.Range(0, RangeConstants.removePersist_count);
                    AS.clip = clips[audioIndex];
                    JimAC.triggerAnswer(aggroScore, tag, longclip);
                    break;
            }
        }
        else
        {
            switch(tag)
            {
                case "Temp":
                    break;
            }
        }
    }

    public override STATE getStateID()
    {
        return stateID;
    }

    public override void setT2(bool t2)
    {
        T2 = t2;
    }
}
