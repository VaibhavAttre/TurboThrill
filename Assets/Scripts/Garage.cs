using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Garage : MonoBehaviour
{

    public int maxCap = 5;
    public RawImage[] health, weapon, ammo, boost;
    public RawImage health1, health2, health3, health4, health5;
    public RawImage ammo1, ammo2, ammo3, ammo4, ammo5;
    public RawImage weapon1, weapon2, weapon3, weapon4, weapon5;
    public RawImage boost1, boost2, boost3, boost4, boost5;
    //public GameObject car;


    // Start is called before the first frame update
    void Start()
    {
        //car = GameObject.GetComponent<CarController>();    
        health = new RawImage[maxCap];
        weapon = new RawImage[maxCap];
        ammo = new RawImage[maxCap];
        boost = new RawImage[maxCap];
        health[0] = health1;
        health[1] = health2;
        health[2] = health3;
        health[3] = health4;
        health[4] = health5;
        ammo[0] = ammo1;
        ammo[1] = ammo2;
        ammo[2] = ammo3;
        ammo[3] = ammo4;
        ammo[4] = ammo5;
        weapon[0] = weapon1;
        weapon[1] = weapon2;
        weapon[2] = weapon3;
        weapon[3] = weapon4;
        weapon[4] = weapon5;
        boost[0] = boost1;
        boost[1] = boost2;
        boost[2] = boost3;
        boost[3] = boost4;
        boost[4] = boost5;
        setBar(health, CarData.getHealth());
        setBar(weapon, CarData.getWeapon());
        setBar(ammo, CarData.getAmmo());
        setBar(boost, CarData.getBoost());

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            reset(health, CarData.getHealth());
            reset(weapon, CarData.getWeapon());
            reset(ammo, CarData.getAmmo());
            reset(boost, CarData.getBoost());
            CarData.reset();
        }
    }

    public void upgradeHealth(){
        health[CarData.getHealth()].color = new Color(255, 255, 0);
        CarData.increaseHealth();
    }
    public void upgradeWepaon()
    {
        weapon[CarData.getWeapon()].color = new Color(255, 255, 0);
        CarData.increaseWeapon();
    }
    public void upgradeAmmo()
    {
        ammo[CarData.getAmmo()].color = new Color(255, 255, 0);
        CarData.increaseAmmo();
    }
    public void upgradeBoost()
    {
        boost[CarData.getBoost()].color = new Color(255, 255, 0);
        CarData.increaseBoost();
    }

    public void decreaseHealth()
    {
        health[CarData.getHealth()-1].color = new Color(255, 255, 255);
        CarData.decreaseHealth();
    }
    public void decreaseWepaon()
    {
        weapon[CarData.getWeapon() -1].color = new Color(255, 255, 255);
        CarData.decreaseWeapon();
    }
    public void decreaseAmmo()
    {
        ammo[CarData.getAmmo() -1].color = new Color(255, 255, 255);
        CarData.decreaseAmmo();
    }
    public void decreaseBoost()
    {
        boost[CarData.getBoost()-1].color = new Color(255, 255, 255);
        CarData.decreaseBoost();
    }
    public void setBar(RawImage[] array, int numLight)
    {
        //If number of bars lit up is less than it should be
        for(int i = 1; i <= numLight; i++)
        {
            if (array[i - 1] == null) return;
            array[i - 1].color = new Color(255, 255, 0);
        }
    }
    public void reset(RawImage[] array, int numLight)
    {
        for (int i = 1; i <= numLight; i++)
        {
            if (array[i - 1] != null) array[i - 1].color = new Color(255, 255, 255); ;
        }
    }
}
