using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySusAroused : State
{
    private Vector3[] path;
    private int currPathID;
    private GameObject player;
    public EnemySusAroused(FSM fsm) : base("Enemy sus aroused", fsm)
    {
        fsm = fsm;
    }

    // Start is called before the first frame update
    public override void Enter()
    {
        player = GameObject.Find("Player Car Variant");
        Debug.Log("enter SUS");
        Vector3 end =  player.transform.position;
        //PathFindStatic.init(fsm.GetComponent<Grid>());
        //path = PathFindStatic.Path(fsm.transform.position, end);

    }

    public override void Exit()

    {
        Debug.Log("exit SUS");
        base.Exit();
    }

    // Update is called once per frame
    public override void Update()
    {
        /*if (currPathID >= path.Length)
        {
            fsm.changeState(new EnemyIdle(fsm));
        }
        float distance = Vector3.Distance(path[currPathID], fsm.transform.position);
        fsm.transform.position = Vector3.MoveTowards(fsm.transform.position, path[currPathID], Time.deltaTime);

        Vector3 direction = path[currPathID] - fsm.transform.position;

        if (direction != Vector3.zero)
        {
            direction.z = 0;
            direction = direction.normalized;
            var rotation = Quaternion.LookRotation(direction);
            fsm.transform.rotation = Quaternion.Slerp(fsm.transform.rotation, rotation, 50 * Time.deltaTime);
        }
        else
        {
            currPathID++;
        }
        if (direction.magnitude < 30)
        {
              fsm.changeState(new EnemyCombat(fsm));
            
        }
        if(direction.magnitude > 50)
        {
            fsm.changeState(new EnemyScouting(fsm));
        }*/
        Vector3 directionToGoal = player.transform.position - fsm.transform.position;
        //directionToGoal.y = 0;

        fsm.transform.position += directionToGoal.normalized * 3f * Time.deltaTime;
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

        if (directionToGoal.magnitude < 30)
        {
            fsm.changeState(new EnemyCombat(fsm));

        }
        if (directionToGoal.magnitude > 50)
        {
            fsm.changeState(new EnemyScouting(fsm));
        }
        base.Update();
    }
}
