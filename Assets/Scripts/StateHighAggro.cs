using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Assets.Scripts;

public class StateHighAggro : StateBase
{
    public StateHighAggro(SuspectControllerFSM SC)
    {
        transitionA2 = false;
        stateID = STATE.HIGH_AGGRO;
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

        if (aggroScore <= 3.4f)
        {
            scFSM.getState(STATE.LOW_AGGRO).Enter(semantics);
        }
        else if (aggroScore <= 7.4f)
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
        string Tag = "";

        Tag = DM.semanticToAudio(semantics);

        AudioClip[] audioClips = DM.getSingleClips(Tag);

        scFSM.PlaySuspectAudio(Tag, audioClips);
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
                    audioIndex = UnityEngine.Random.Range(RangeConstants.name_A3, clips.Length);
                    AS.clip = clips[audioIndex];
                    JimAC.triggerAnswer(aggroScore, tag, longclip);
                    break;
                case "HeyYou":
                    if (aggroScore < 10)
                    {
                        audioIndex = UnityEngine.Random.Range(RangeConstants.heyYou_A3, clips.Length);
                        AS.clip = clips[audioIndex];
                        JimAC.triggerAnswer(aggroScore, tag, longclip);
                    }
                    else
                    {
                        audioIndex = UnityEngine.Random.Range(0, RangeConstants.leave_count);
                        AS.clip = DM.getSingleClips("Leave")[audioIndex];

                        if (RangeConstants.longClipLeave.Contains(audioIndex))
                            longclip = true;

                        JimAC.triggerAnswer(aggroScore, "Leave", longclip);
                        transitionA2 = true;
                    }
                    break;
                case "Insult":
                    if (aggroScore < 10)
                    {
                        audioIndex = UnityEngine.Random.Range(RangeConstants.insult_A3, clips.Length);
                        AS.clip = clips[audioIndex];
                        JimAC.triggerAnswer(aggroScore, tag, longclip);
                    }
                    else
                    {
                        audioIndex = UnityEngine.Random.Range(0, RangeConstants.leave_count);
                        AS.clip = DM.getSingleClips("Leave")[audioIndex];

                        if (RangeConstants.longClipLeave.Contains(audioIndex))
                            longclip = true;

                        JimAC.triggerAnswer(aggroScore, "Leave", longclip);
                        transitionA2 = true;
                    }
                    break;
                case "CalmDown":
                    if (aggroScore < 10)
                    {
                        audioIndex = UnityEngine.Random.Range(0, clips.Length);
                        AS.clip = clips[audioIndex];
                        JimAC.triggerAnswer(aggroScore, tag, longclip);
                    }
                    else
                    {
                        audioIndex = UnityEngine.Random.Range(0, RangeConstants.leave_count);
                        AS.clip = DM.getSingleClips("Leave")[audioIndex];

                        if (RangeConstants.longClipLeave.Contains(audioIndex))
                            longclip = true;

                        JimAC.triggerAnswer(aggroScore, "Leave", longclip);
                        transitionA2 = true;
                    }
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
                        if (RangeConstants.longClipResist.Contains(audioIndex))
                            longclip = true;

                        JimAC.triggerAnswer(aggroScore, tag, longclip);
                    }
                    else
                    {
                        audioIndex = UnityEngine.Random.Range(0, RangeConstants.leave_count);
                        AS.clip = DM.getSingleClips("Leave")[audioIndex];

                        if (RangeConstants.longClipLeave.Contains(audioIndex))
                            longclip = true;

                        JimAC.triggerAnswer(aggroScore, "Leave", longclip);
                        transitionA2 = true;
                    }
                    break;
                case "StepOut":
                    audioIndex = UnityEngine.Random.Range(RangeConstants.stepOut_A3, clips.Length);
                    AS.clip = clips[audioIndex];
                    JimAC.triggerAnswer(aggroScore, tag, longclip);
                    break;
                case "TalkReason":
                    audioIndex = UnityEngine.Random.Range(RangeConstants.talkReason_A3, clips.Length);
                    AS.clip = clips[audioIndex];
                    JimAC.triggerAnswer(aggroScore, tag, longclip);
                    break;
                case "Talk":
                    audioIndex = UnityEngine.Random.Range(0, RangeConstants.talk_count);
                    AS.clip = clips[audioIndex];
                    JimAC.triggerAnswer(aggroScore, tag, longclip);
                    break;
                case "Purpose":
                    audioIndex = UnityEngine.Random.Range(RangeConstants.purpose_A3, clips.Length);
                    AS.clip = clips[audioIndex];
                    JimAC.triggerAnswer(aggroScore, tag, longclip);
                    break;
                case "Approach":
                    audioIndex = UnityEngine.Random.Range(RangeConstants.approach_A3, clips.Length);
                    AS.clip = clips[audioIndex];
                    JimAC.triggerAnswer(aggroScore, tag, longclip);
                    break;
                case "Remove":
                    if (aggroScore < 10)
                    {
                        audioIndex = UnityEngine.Random.Range(0, RangeConstants.remove_count);
                        AS.clip = clips[audioIndex];
                        JimAC.triggerAnswer(aggroScore, tag, longclip);
                    }
                    else
                    {
                        audioIndex = UnityEngine.Random.Range(0, RangeConstants.leave_count);
                        AS.clip = DM.getSingleClips("Leave")[audioIndex];

                        if (RangeConstants.longClipLeave.Contains(audioIndex))
                            longclip = true;

                        JimAC.triggerAnswer(aggroScore, "Leave", longclip);

                        transitionA2 = true;
                    }
                    break;
                case "RemovePersist":
                    if (aggroScore < 10)
                    {
                        audioIndex = UnityEngine.Random.Range(0, RangeConstants.removePersist_count);
                        AS.clip = clips[audioIndex];
                        JimAC.triggerAnswer(aggroScore, tag, longclip);
                    }
                    else
                    {
                        audioIndex = UnityEngine.Random.Range(0, RangeConstants.leave_count);
                        AS.clip = DM.getSingleClips("Leave")[audioIndex];

                        if (RangeConstants.longClipLeave.Contains(audioIndex))
                            longclip = true;

                        JimAC.triggerAnswer(aggroScore, "Leave", longclip);
                        transitionA2 = true;
                    }
                    break;

            }
        }
        //else//T2
        //{
        //    switch (tag)
        //    {
        //        case "Temp":
        //            AS.clip = clips[audioIndex];
        //            //JimAC.triggerAnswer(aggroScore, tag, longclip);
        //            break;
        //    }
        //}
    }

    public override STATE getStateID()
    {
        return stateID;
    }

    public override void setT2(bool t2)
    {
        T2 = t2;
    }

    public override void kill()
    {
        //Debug.Log("SUSPECT KILLS");
    }
}
