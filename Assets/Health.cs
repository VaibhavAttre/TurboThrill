using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public GameObject healthBar;
    private int health;
    public Slider slider;
    public GameObject effect;
    void Start()
    {
        slider.value = health;
        health = 100;
        slider.maxValue = 100;
        if(this.transform.gameObject.tag != "Player")
        {
            healthBar.SetActive(false);
            
        }
        
    }

    void Update()
    {   
        slider.value = health;
        if (this.transform.gameObject.tag != "Player" && healthBar.activeSelf == true)
        {
            healthBar.transform.LookAt(GameObject.FindGameObjectWithTag("Player").transform);
        }
        if (effect != null && health > 0)
        {
            effect.GetComponent<ParticleSystem>().Stop();
        }
    }


    public int GetHealth()
    {
        return health; 
    }

    public void TakeDamage(int damageAmount)
    {
        healthBar.SetActive(true);
        health -= damageAmount;
        if (health <= 0 && this.transform.gameObject.tag != "Player")
        {
            StartCoroutine(PlayDestroyEffect());
        }
    }

    IEnumerator PlayDestroyEffect()
    {
        if(effect != null)
        {
            effect.GetComponent<ParticleSystem>().Play();
            yield return new WaitForSeconds(effect.GetComponent<ParticleSystem>().main.duration);
        }
        Destroy(this.gameObject);
    }

    public void Heal(int heal)
    {
        if(health + heal <= slider.maxValue)
        {
            health += heal;
        } 
    }
}
