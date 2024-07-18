using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;

    private void Awake()
    {
        if(instance = null)
        {
            instance = this;
        } else if (instance != this)
        {
            Debug.Log("exists");
            Destroy(this);
        }
    }
}
