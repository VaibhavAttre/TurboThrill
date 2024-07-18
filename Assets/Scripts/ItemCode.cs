using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCode : MonoBehaviour
{
    public GameObject car;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = car.transform.position + new Vector3(Random.Range(-10, 10), 0, Random.Range(15, 30));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void OnTriggerEnter(Collider other)
    {
        transform.position = transform.position + new Vector3(Random.Range(-10, 10), 0, Random.Range(15,25));
        /*if(gameObject.CompareTag("Speed")){
            car.GetComponent<CarPhysics>().changeSpeed(2.0f);
        }
        else if(gameObject.CompareTag("Coin")){
            car.GetComponent<CarPhysics>().incScore(1.0f);
        }*/
    }
}
