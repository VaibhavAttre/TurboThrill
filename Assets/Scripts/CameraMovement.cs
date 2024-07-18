using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public GameObject car;
    public float x;
    public float y = 10;
    public float z = -20;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = car.transform.position + new Vector3(x, y, z);
    }
}
