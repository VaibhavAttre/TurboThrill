using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Camera playerCamera;
    public float speed = 6f;
    public float lookSpeeds = 2f;
    public float lookXLimit = 45f;
    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    private CharacterController charControl;
    private bool canMove = true;
    // Start is called before the first frame update
    void Start()
    {
        charControl = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        float curSpeedX = canMove ? speed * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? speed * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right*curSpeedY);

        if(Input.GetButton("Jump")) {
            moveDirection.y = 10f;
        }
        
        if(Input.GetKey(KeyCode.LeftControl)) {
            moveDirection.y = -10f;
        }

        charControl.Move(moveDirection * Time.deltaTime);

        if (canMove) {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeeds;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeeds, 0);
        }
    }
}
