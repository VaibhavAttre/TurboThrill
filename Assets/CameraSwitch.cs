using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    public Camera camCar;
    public Camera camGun;
    public bool gunCamActive = false;
    // Update is called once per frame

    public CameraSwitch(Camera camCar, Camera camGun)
    {
        this.camCar = camCar;
        this.camGun = camGun;
    }

    public void GetCameraInput()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            camCar.gameObject.SetActive(false);
            camGun.gameObject.SetActive(true);
        } else if (Input.GetKeyDown(KeyCode.E))
        {
            camCar.gameObject.SetActive(true);
            camGun.gameObject.SetActive(false);
        }

    }
}
