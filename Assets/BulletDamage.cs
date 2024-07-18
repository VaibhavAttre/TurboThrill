using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDamage : MonoBehaviour
{
    public int damage;

    private void Update()
    {
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        Health health = collision.gameObject.GetComponent<Health>();
        if (health != null && collision.gameObject.tag != "Player")
        {
            health.TakeDamage(damage);
            Destroy(this.gameObject);
        }
    }
}
