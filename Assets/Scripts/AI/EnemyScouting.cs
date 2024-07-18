using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = System.Random;

public class EnemyScouting : State
{
    private GameObject player;
    private Rigidbody rb;
    private Vector3 end;
    public EnemyScouting(FSM fsm) : base("Scouting", fsm)
    {
    }

    public override void Enter()
    {
        player = GameObject.Find("Player Car Variant");
        Random r = new System.Random();
        Debug.Log("enter scouting");
        end = new Vector3((int)(fsm.transform.position.x + r.Next(0, 20)),10,(int)( fsm.transform.position.z + r.Next(0, 20)));
        Vector3 direction = Vector3.up;
        //rb = fsm.GetComponent<Rigidbody>();  
     }

    public override void Exit()

    {
        Debug.Log("exit scouting");
        base.Exit();
    }

    public override void Update()
    {


        /*Vector2 moveDir = findValidMove();

        rb.AddForce(moveDir.normalized*1000);*/
        //rb.AddForce(fsm.transform.up* 100);
        Vector3 directionToGoal = end - fsm.transform.position;
        //directionToGoal.y = 0;

        fsm.transform.position += directionToGoal.normalized*3f*Time.deltaTime;
        Quaternion targetRotation = Quaternion.LookRotation(directionToGoal);
        fsm.transform.rotation = Quaternion.Slerp(fsm.transform.rotation, targetRotation, 50 * Time.deltaTime);
        RaycastHit hit;
        if (Physics.Raycast(fsm.transform.position, -fsm.transform.up, out hit, 5f))
        {
            Vector3 forward = Vector3.Cross(fsm.transform.right, hit.normal);
            Quaternion avoidanceRotation = Quaternion.LookRotation(forward, hit.normal);
            fsm.transform.rotation = Quaternion.Slerp(fsm.transform.rotation, avoidanceRotation, 50 * Time.deltaTime);
        }
        if (Physics.Raycast(fsm.transform.position, fsm.transform.forward, out hit, 5f))
        {
            Vector3 forward = Vector3.Cross(fsm.transform.forward, fsm.transform.up);
            Quaternion avoidanceRotation = Quaternion.LookRotation(forward);
            fsm.transform.rotation = Quaternion.Slerp(fsm.transform.rotation, avoidanceRotation, 50 * Time.deltaTime);
        }

        if (Vector3.Distance(fsm.transform.position,player.transform.position) < 40)
        {
            float angle = Vector3.Angle(fsm.transform.forward, player.transform.forward);
            if (angle < 50 && (player.transform.position - fsm.transform.position).magnitude < 20)
            {
               fsm.changeState(new EnemyCombat(fsm));
            }
        }
        if (directionToGoal.magnitude < 1)
        {
            fsm.changeState(new EnemyIdle(this.fsm));
        }

        base.Update();
    }

    public Vector3 findValidMove()
    {
        Vector3 o;
        RaycastHit hit;
        Vector3 direction = (end - fsm.transform.position).normalized;
        if (Physics.Raycast(fsm.transform.position, direction, out hit, 2) )
        {
            // Calculate a new direction to avoid the obstacle
            Vector3 avoidDirection = Vector3.Reflect(direction, hit.normal);
            avoidDirection.y = 15; // Keep the same height
            direction = avoidDirection.normalized;
        }

       /* // Move the object
        transform.position += direction * speed * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(direction);*/

        return direction;
    }
}
