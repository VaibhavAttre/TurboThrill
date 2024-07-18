using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using Cinemachine;
using UnityEngine.VFX;

public class CarController : MonoBehaviour
{
    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";

    private float horizontalInput;
    private float verticalInput;
    private float currentSteerAngle;
    private float currentBreakForce;
    private bool isBreaking;
    public VisualEffect visualEffect;
    public static float score;
    public static float time;

    public float maxXVelocity = 5;
    public float maxYVelocity = 5;


    [Header("General")]

    [SerializeField] private Rigidbody car;
    [SerializeField] private GameObject audioSource;
    [SerializeField] private AudioClip idleSound;
    [SerializeField] private AudioClip drivingSound;
    [SerializeField] private AudioClip driftingSound;
    [SerializeField] private float maxSteerAngle;
    [SerializeField] private float motorForce = 50f;
    [SerializeField] private float breakForce = 0f;
    //[SerializeField] private Rigidbody rb;
    [SerializeField] private GameObject trailRenderer;
    [SerializeField] private WheelCollider frontLeftWheelCollider;
    [SerializeField] private WheelCollider frontRightWheelCollider;
    [SerializeField] private WheelCollider backLeftWheelCollider;
    [SerializeField] private WheelCollider backRightWheelCollider;

    private Array wheels; 
    //haha would have been a good idea to implement huh *(pain)*

    [SerializeField] private Transform frontLeftWheelTransform;
    [SerializeField] private Transform frontRightWheelTransform;
    [SerializeField] private Transform backLeftWheelTransform;
    [SerializeField] private Transform backRightWheelTransform;

    [Header("Effects")]
    [SerializeField] private GameObject frontLeftWheelEffect;
    [SerializeField] private GameObject frontRightWheelEffect;
    [SerializeField] private GameObject backLeftWheelEffect;
    [SerializeField] private GameObject backRightWheelEffect;

    [Header("Game Background")]
    [SerializeField] private TMPro.TextMeshProUGUI scoreText;
    [SerializeField] private TMPro.TextMeshProUGUI timeText;

    [SerializeField] CinemachineVirtualCamera mainCamera;
    [SerializeField] CinemachineVirtualCamera adsCamera;

    public Vector3 com;
    public Rigidbody rb;

    private bool still = true;
    private bool moving;

    private void FixedUpdate()
    {
        GetInput();
        HandleMotor();
        HandleSteering();
        UpdateWheels();
        WheelEffects();
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = com;
    }

    private void OnEnable()
    {
        CameraSwitcher.Register(mainCamera);
        CameraSwitcher.Register(adsCamera);
        CameraSwitcher.SwitchCamera(mainCamera);
    }

    private void OnDisable()
    {
        CameraSwitcher.Unregister(mainCamera);
        CameraSwitcher.Unregister(adsCamera);
    }

    private void Update(){
        time += Time.deltaTime;
        scoreText.text = "Score: " + getScore();
        timeText.text = "Time: " + getMinute() + ":" + getSecond();

        if (Input.GetKeyDown(KeyCode.E))
        {
           if(CameraSwitcher.IsActiveCamera(adsCamera))
            {
                CameraSwitcher.SwitchCamera(mainCamera);
               // Debug.Log("main camera");
            } else if (CameraSwitcher.IsActiveCamera(mainCamera))
            {
                CameraSwitcher.SwitchCamera(adsCamera);
                //Debug.Log("ads camera");
            }
        }
        Vector3 velocityX = new Vector3(30f, 0f, 0f);
        Vector3 velocityY = new Vector3(0f, 0f, 30f);

        if(rb.velocity.magnitude > 15)
        {
            visualEffect.Play();
        } else
        {
            visualEffect.Stop();
        }
        //scoreText.text = "Score: " + getScore();
        //timeText.text = "Time: " + getMinute() + ":" + getSecond();
    }

    private void HandleMotor()
    {
        if (car.velocity.magnitude < 10) car.AddForce(car.rotation.normalized * Vector3.forward * 10000 * verticalInput);
        
        frontLeftWheelCollider.motorTorque = verticalInput * motorForce;
        frontRightWheelCollider.motorTorque = verticalInput * motorForce;
        //currentBreakForce = isBreaking ? breakForce : 0f;
    }

    //breaking is pretty useless tbh... just go backwards

    /*
    private void ApplyBreaking()
    {
        car.AddForce(car.rotation.normalized * Vector3.forward * -100000);
        frontRightWheelCollider.motorTorque = currentBreakForce;
        frontLeftWheelCollider.motorTorque = currentBreakForce;
        backRightWheelCollider.brakeTorque = currentBreakForce;
        backLeftWheelCollider.brakeTorque = currentBreakForce;
    }
    */
    private void GetInput()
    {
        horizontalInput = Input.GetAxis(HORIZONTAL);
        verticalInput = Input.GetAxis(VERTICAL);
        if (verticalInput == 0f)
        {
            if (!still)
            {
                audioSource.GetComponent<AudioSource>().Stop();
            }
            still = true;   
            audioSource.GetComponent<AudioSource>().PlayOneShot(idleSound);
        }
        else if(this.gameObject.GetComponent<Rigidbody>().velocity.magnitude > 0 && verticalInput > 0f) {
            if(still)
            {
                audioSource.GetComponent<AudioSource>().Stop();
            }
            still = false;
               audioSource.GetComponent<AudioSource>().PlayOneShot(drivingSound);
        } else if (verticalInput < 0f)
        {
            if (still)
            {
                audioSource.GetComponent<AudioSource>().Stop();
            }
            still = false;
            audioSource.GetComponent<AudioSource>().PlayOneShot(driftingSound);
        }
        isBreaking = Input.GetKey(KeyCode.Space);
        if(Input.GetKey(KeyCode.F)){
            reset();
        }
        trailRenderer.SetActive(false);
        if (Input.GetKey(KeyCode.LeftShift) && transform.GetComponent<Boost>().canBoost)
        {
            trailRenderer.SetActive(true);
            transform.GetComponent<Boost>().UseBoost();
            transform.GetComponent<Boost>().timeSinceBoost = 0f;
            this.GetComponent<Rigidbody>().velocity += transform.forward * transform.GetComponent<Boost>().boostForce;
            

        } else
        {
            transform.GetComponent<Boost>().timeSinceBoost += Time.deltaTime;
        }

        if(Input.GetKey(KeyCode.LeftShift))
        {
            Camera.main.GetComponent<CameraShake>().Shake();
        }
    }

    private void HandleSteering()
    {
        currentSteerAngle = maxSteerAngle * horizontalInput;
        frontLeftWheelCollider.steerAngle = currentSteerAngle;
        frontRightWheelCollider.steerAngle = currentSteerAngle;
    }
    private void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheelTransform);
        UpdateSingleWheel(backLeftWheelCollider, backLeftWheelTransform);
        UpdateSingleWheel(backRightWheelCollider, backRightWheelTransform);
    }

    private void UpdateSingleWheel(WheelCollider WheelCollider, Transform WheelTransform)
    {
        Vector3 pos;
        Quaternion rot;
        WheelCollider.GetWorldPose(out pos, out rot);
        WheelTransform.rotation = rot;
        WheelTransform.position = pos;
    }

    void WheelEffects()
    {
//        Debug.Log(car.velocity.x);
        if (car.velocity.magnitude >= 10 && (Input.GetKey(KeyCode.S) || car.angularVelocity.magnitude >= 1.2))
        {
            backLeftWheelEffect.GetComponentInChildren<TrailRenderer>().emitting = true;
            backRightWheelEffect.GetComponentInChildren<TrailRenderer>().emitting = true;
            frontRightWheelCollider.motorTorque = 0;
            frontLeftWheelCollider.motorTorque = 0;
            //who needs arrays anyway
            //backLeftWheelEffect.GetComponentInChildren<ParticleSystem>().Emit(1);
            //backRightWheelEffect.GetComponentInChildren<ParticleSystem>().Emit(1);
            //frontLeftWheelEffect.GetComponentInChildren<ParticleSystem>().Emit(1);
            //frontRightWheelEffect.GetComponentInChildren<ParticleSystem>().Emit(1);
        }
        else
        {
            backLeftWheelEffect.GetComponentInChildren<TrailRenderer>().emitting = false;
            backRightWheelEffect.GetComponentInChildren<TrailRenderer>().emitting = false;
        }
    }

    public void incScore(float num){
        score += num;
    }
    public float getScore(){
        return score;
    }
    public static string getMinute(){
        return (int)(time/60) + "";
    }
    public static string getSecond(){
        string ans = "";
        int seconds = (int)(time % 60);
        if(seconds < 10){
            ans = "0";
        }
        ans += (int)(time % 60) + "";
        return ans;
    }
    public void OnCollisionEnter(Collision other){
        if(other.gameObject.tag == "Finish")
        {
            //Scoreboard here;
            Debug.Log("UDFHLJKesj");
            reset();
        }
    }
    public static void reset(){
        WriteTime();
        SceneManager.LoadScene(0);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        time = 0;
        score = 0;
    }
    public static void WriteTime()
   {
       string path = Application.persistentDataPath + "/Time.txt";
       //Write some text to the test.txt file
       StreamWriter writer = new StreamWriter(path, true);

       if(time != 0) writer.WriteLine(time);
       writer.Close();
       StreamReader reader = new StreamReader(path);
       //Print the text from the file
       Debug.Log(path.ToString());
       reader.Close();
    }
}
