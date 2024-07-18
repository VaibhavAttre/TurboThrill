using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM : MonoBehaviour
{
    private State state;
    [SerializeField] public GameObject g;
    void Start()
    {
        state = getInitState();

        
    }

    public virtual State getInitState()
    {
        State s = new EnemyIdle(this);
        s.Enter();
        return s;
    }

    // Update is called once per frame
    void Update()
    {
        //Console.WriteLine(state.name);
        if (state != null)
        {
            state.Update();
        }
    }
    public void changeState(State newState)
    {
        state.Exit();
        state = newState;
        state.Enter();
    }
    public GameObject instantiate1(GameObject g, Vector3 p, Quaternion r)
    {
        return Instantiate(g, p, r);
    }

}
