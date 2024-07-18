    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boost : MonoBehaviour
{
    public GameObject boostBar;
    public float consumptionRate;
    public float rechargeRate;
    public float restTime;
    private float boostAmount;
    public Slider slider;
    public float boostForce;
    public float timeSinceBoost = 0f;
    public bool canBoost = true;
    private bool recharging = false;
    void Start()
    {
        boostAmount = 100f;
        slider.value = boostAmount;
        slider.maxValue = boostAmount;
    }

    // Update is called once per frame
    void Update()
    {
        if (boostAmount <= 0f || timeSinceBoost > restTime)
        {
            canBoost = false;
         //   recharging = true;
        }
        if(!canBoost)
        {
            if(boostAmount <= slider.maxValue)
            {
                Recharge();
            }
            else
            {
                //recharging = false;
                canBoost = true;
            }
        }
        slider.value = boostAmount;
    }

    public void UseBoost()
    {
        if (canBoost)
        {
            boostAmount -= consumptionRate;
        }
    }

    private void Recharge()
    {
        boostAmount += rechargeRate;
    }
}
