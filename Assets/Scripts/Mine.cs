using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour
{

    // Start is called before the first frame update
    [SerializeField] private float distance;
    [SerializeField] private float explosionMag;
    private GameObject player;

    void Start()
    {
        // Find the GameObject with the name "P"
        player = GameObject.Find("Player Car Variant");
        
    }

    // Update is called once per frame
    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        
        if (distanceToPlayer <= distance)
        {
            Vector3 directionToPlayer = player.transform.position - transform.position;
            directionToPlayer.Normalize();
            Rigidbody rb = player.GetComponent<Rigidbody>();
            rb.velocity = new Vector3(rb.velocity.x - 5,rb.velocity.y - 5, rb.velocity.z-5);
            rb.GetComponent<Rigidbody>().AddForce(directionToPlayer*explosionMag+Vector3.up*80000,ForceMode.Force);
            Destroy(this.gameObject);
        }
    }
}
