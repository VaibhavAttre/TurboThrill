using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CarData
{
    public static int healthLevel, weaponLevel, ammoCount, boostLevel;
    //public int upgradeCost;
    public static int maxCap = 5;

    public static void increaseHealth()
    {
        if(healthLevel < maxCap) healthLevel++;
    }
    public static void decreaseHealth()
    {
        if (healthLevel > 0) healthLevel--;
    }
    public static int getHealth()
    {
        return Mathf.Min(healthLevel, maxCap);
    }
    public static void increaseWeapon()
    {
        if (weaponLevel < maxCap) weaponLevel++;
    }
    public static void decreaseWeapon()
    {
        if (weaponLevel > 0) weaponLevel--;
    }
    public static int getWeapon()
    {
        return Mathf.Min(weaponLevel, maxCap);
    }
    public static void increaseAmmo()
    {
        if (ammoCount < maxCap) ammoCount++;
    }
    public static void decreaseAmmo()
    {
        if (ammoCount > 0) ammoCount--;
    }
    public static int getAmmo()
    {
        return Mathf.Min(ammoCount, maxCap);
    }
    public static void increaseBoost()
    {
        if (boostLevel < maxCap) boostLevel++;
    }
    public static void decreaseBoost()
    {
        if (boostLevel > 0) boostLevel--;
    }
    public static int getBoost()
    {
        return Mathf.Min(boostLevel, maxCap);
    }
    public static void reset()
    {
        healthLevel = 0;
        weaponLevel = 0;
        ammoCount = 0;
        boostLevel = 0;

    }

}
