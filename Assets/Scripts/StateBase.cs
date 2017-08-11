using System.Collections.Generic;
using UnityEngine;


public abstract class StateBase {
    public AudioSource AS;
    public bool transitionA2;
    protected bool T2;
    protected SuspectControllerFSM scFSM;
    protected DialogueManager DM;
    protected AnimController_Jim JimAC;
    protected float aggroScore;
    protected STATE stateID;
    

    public abstract void UpdateState(float NewAggro, List<string> semantics);
    public abstract void Enter(List<string> semantics);
    public abstract void Exit();
    public abstract void selectAudio(string tag, AudioClip[] clips);
    public abstract STATE getStateID();
    public abstract void setT2(bool t2);
    public abstract void kill();
}
