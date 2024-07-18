using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Floater : MonoBehaviour
{
    public Rigidbody rb;
    public float depthBeforeSubmerge = 1f;
    public float displacementAmount= 3f;

    void FixedUpdate()
    {
        if(rb.transform.position.y < 5f)
        {
            //float displacementMultiplier = Mathf.Clamp01(-transform.position.y/depthBeforeSubmerge) * displacementAmount;
            float displacementMultiplier = Mathf.Clamp01((5f - rb.transform.position.y) / depthBeforeSubmerge) * displacementAmount;

            rb.AddForce(new Vector3(0f, Mathf.Abs(Physics.gravity.y)  * displacementMultiplier, 0f), ForceMode.Acceleration);
        }
    }
}
