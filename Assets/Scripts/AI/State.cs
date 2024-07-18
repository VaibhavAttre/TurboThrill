using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State
{
    public string name;
    public FSM fsm;
    public State(string name, FSM fsm)
    {
        this.name = name;
        this.fsm = fsm;

    }
    // Start is called before the first frame update
    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Update() { }
}
