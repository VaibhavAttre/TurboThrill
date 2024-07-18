using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RotateCarInGarage : MonoBehaviour
{
    // Start is called before the first frame update


    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 0.25f, 0);
    }

}

