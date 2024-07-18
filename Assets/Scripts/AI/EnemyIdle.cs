using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class EnemyIdle : State
{
    float time;
    private GameObject player;
    private float initRot;
    private float maxRot;
    public EnemyIdle(FSM fsm) : base("Idle", fsm)
    {
    }

    public override void Enter()
    {
        initRot = 0;
        player = GameObject.Find("Player Car Variant");
        Debug.Log("enter idle");
        System.Random rnd = new System.Random();
        float maxRot = rnd.Next(1, 120);
        base.Enter();
    }

    public override void Exit()
    {
        Debug.Log("exit idle");
        base.Exit();
    }

    public override void Update()
    {
        fsm.transform.Rotate(0, 30 * Time.deltaTime, 0);
        initRot += 30 * Time.deltaTime;
        Vector3 dir = fsm.transform.position - player.transform.forward;
        if (dir.magnitude < 90) {
            float angle = Vector3.Angle(dir, player.transform.forward);
            if (angle < 50 && (player.transform.position-fsm.transform.position).magnitude<20)
            {
                fsm.changeState(new EnemyCombat(fsm));
            }
        }
        if(initRot >= maxRot)
        {
            //maxRot = 0;
            //System.Random r = new System.Random();
            //if (r.Next(0, 2) > 1)
            //{
                fsm.changeState(new EnemyScouting(fsm));
          //  }
        /*    else
            {
                System.Random rnd = new System.Random();
                float maxRot = rnd.Next(1, 120);
            }*/
        }
        base.Update();
    }
}
