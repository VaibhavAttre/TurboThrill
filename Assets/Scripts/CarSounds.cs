using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSounds : MonoBehaviour
{
    [SerializeField] float minSpeed;
    [SerializeField] float maxSpeed;
    private float currentSpeed;

    private Rigidbody car;
    private AudioSource carAudio;

    [SerializeField] float minPitch;
    [SerializeField] float maxPitch;
    private float pitchFromCar;

    private void Start()
    {
        carAudio = GetComponent<AudioSource>();
        car = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        EngineSound();
    }

    void EngineSound()
    {
        currentSpeed = car.velocity.magnitude;
        pitchFromCar = car.velocity.magnitude / 50f;

        if (currentSpeed < minSpeed)
        {
            carAudio.pitch = minPitch;
        }

        if (currentSpeed > minSpeed && currentSpeed < maxSpeed) 
        {
            carAudio.pitch = minPitch * pitchFromCar;
        }

        if(currentSpeed > maxSpeed)
        {
            carAudio.pitch = maxPitch;
        }
    }
}
