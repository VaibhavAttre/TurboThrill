using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombat : State
{
    private GameObject player;
    private float time;
    private float fireRate = 3;
    private int bulletCount = 5;
    public EnemyCombat(FSM fsm) : base("Combat", fsm)
    {
    }

    public override void Enter()
    {
        player = GameObject.Find("Player Car Variant");
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        time += Time.deltaTime;
        Vector3 directionToPlayer = player.transform.position - fsm.transform.position;
        directionToPlayer.y = 0; // Ignore height difference
        if (fireRate < time)
        {
            time = 0;
            bulletCount--;
            fireBullet(fsm.transform.position+fsm.transform.up*3,new Quaternion(), player.transform.position - (fsm.transform.position + fsm.transform.up * 3));
        }
        if (bulletCount <= 0 && time >= 8)
        {
            bulletCount = 5;
            time = 0;
        }

       
        Vector3 movementDirection = Vector3.Cross(fsm.transform.up, directionToPlayer.normalized);
        fsm.transform.position += movementDirection.normalized*0.5f*(1f/directionToPlayer.magnitude);
        Quaternion targetRotation = Quaternion.LookRotation(movementDirection);
        fsm.transform.rotation = Quaternion.Slerp(fsm.transform.rotation, targetRotation, 50 * Time.deltaTime);
        RaycastHit hit;
        if (Physics.Raycast(fsm.transform.position, -fsm.transform.up, out hit, 5f))
        {
            Vector3 forward = Vector3.Cross(fsm.transform.right, hit.normal);
            Quaternion avoidanceRotation = Quaternion.LookRotation(forward, hit.normal);
            fsm.transform.rotation = Quaternion.Slerp(fsm.transform.rotation, avoidanceRotation, 50 * Time.deltaTime);
        }
     /*   if (Physics.Raycast(fsm.transform.position, fsm.transform.forward, out hit, 5f))
        {
            Vector3 forward = Vector3.Cross(fsm.transform.forward, fsm.transform.up);
            Quaternion avoidanceRotation = Quaternion.LookRotation(forward);
            fsm.transform.rotation = Quaternion.Slerp(fsm.transform.rotation, avoidanceRotation, 50 * Time.deltaTime);
        }*/



        if ((player.transform.position - fsm.transform.position).magnitude > 50)
        {
            fsm.changeState(new EnemyScouting(this.fsm));
        }
        else if((player.transform.position - fsm.transform.position).magnitude > 30)
        {
            fsm.changeState(new EnemySusAroused(this.fsm));
        }
        base.Update();
    }

    private void fireBullet(Vector3 p, Quaternion r, Vector3 t)
    {
        GameObject bullet = fsm.instantiate1(fsm.g, p, r);

        // Get the bullet's Rigidbody component
        Rigidbody rb = bullet.GetComponent<Rigidbody>();

        System.Random rand = new System.Random();
        float spreadX = rand.Next(-2, 2);
        float spreadY = rand.Next(-2, 2);

        // Calculate the spread direction
        Vector3 direction = t;
        direction = Quaternion.Euler(0, 0, 0) * direction;

        // Apply force to the bullet in the spread direction
        rb.velocity = direction * 5f;
    }
}
