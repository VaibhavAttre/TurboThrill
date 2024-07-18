using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class FollowCameraGun : MonoBehaviour
{
    public float lookSpeedX = 2.0f;
    public float lookSpeedY = 2.0f;
    public float upperLookLimit = 80.0f;
    public float lowerLookLimit = 80.0f;

    public Vector3 gunCamOffset;
    public Vector3 gunCamOffsetB;
    public Camera carCam;
    public Camera gunCam;
    public Camera currCam;
    public CinemachineVirtualCamera adsCam;

    //public CameraSwitch camSwitch;

    public GameObject obj;
    public GameObject turret;
    private float rotationY = 0;
    private float rotationX = 0;
    public float moveDuration = 1;
    private bool isTransitioning = false;
    public float transitionDuration = 1f;
    public float distanceBehindTurret = 4;

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void Update()
    {
        HandelMouseLook();
                
    }
    private void HandelMouseLook()
    {

        if (CameraSwitcher.IsActiveCamera(adsCam))
        { 
            float mouseX = Input.GetAxis("Mouse X") * lookSpeedX;
            float mouseY = Input.GetAxis("Mouse Y") * lookSpeedY;

            //Rotation in Y direction 
            rotationX -= mouseY;
            rotationX = Mathf.Clamp(rotationX, -upperLookLimit, lowerLookLimit);
            transform.Rotate(Vector3.up * mouseX);
            Quaternion objRotation = obj.transform.rotation;
            float newRotationX = rotationX + objRotation.eulerAngles.x;

            //Rotation in X direction
            rotationY += mouseX;
            float newRotationY = rotationY + objRotation.eulerAngles.y;

            //Apply rotation to turrent and camera
            Quaternion newRotation = Quaternion.Euler(newRotationX , newRotationY , objRotation.eulerAngles.z);
            transform.rotation = newRotation;
            turret.transform.rotation = Quaternion.Euler(newRotationX, newRotationY, objRotation.eulerAngles.z);

            //offsetting camera position as needed (decide later)
            Vector3 gunCamOffset = -transform.forward * distanceBehindTurret; 
            transform.position = turret.transform.position + gunCamOffset+ gunCamOffsetB;

        }
        else
        {
            Quaternion objRotation = obj.transform.rotation;
            turret.transform.rotation = Quaternion.Euler(objRotation.eulerAngles.x, objRotation.eulerAngles.y, objRotation.eulerAngles.z);
        }
        
    }

    IEnumerator TransitionCamera(Camera fromCamera, Camera toCamera)
    {
        isTransitioning = true;

        float elapsedTime = 0f;
        Vector3 startingPos = fromCamera.transform.position;
        Vector3 targetPos = toCamera.transform.position;


        while (elapsedTime < transitionDuration)
        {
            float t = elapsedTime / transitionDuration;
            fromCamera.transform.position = Vector3.Lerp(startingPos, targetPos, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Set the final position to avoid overshooting
        fromCamera.transform.position = targetPos;

        // Deactivate the fromCamera and set the toCamera as active
        fromCamera.gameObject.SetActive(false);
        toCamera.gameObject.SetActive(true);
        isTransitioning = false;
    }
}



