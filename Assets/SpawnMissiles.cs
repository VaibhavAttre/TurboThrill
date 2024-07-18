using System.Collections;
using System.Collections.Generic;
using Tarodev;
using UnityEngine;

public class SpawnMissiles : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject missiles;
    [SerializeField] private int radius = 25;
     // Update is called once per frame
    void Update()
    {
        Vector3 carLocation = GameObject.FindGameObjectWithTag("Player").transform.position;
        
        if(NumMissiles.countOfMissiles < 10) {
            Debug.Log(NumMissiles.countOfMissiles);
            Vector3 randomDirection = Random.insideUnitSphere;
            randomDirection.y = 50; // Ensure the position is on the same plane as the car
            Vector3 randomPosition = transform.position + randomDirection * radius;
            GameObject missile = Instantiate(missiles, randomPosition, Quaternion.identity);
            NumMissiles.countOfMissiles++;          
        }
    }
}

public static class NumMissiles
{
    public static int countOfMissiles = 0;
}