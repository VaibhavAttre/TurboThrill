using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public bool enable = false;
    public AnimationCurve curve;
    public float duration = 1f;
    public void Shake()
    {
    
        StartCoroutine(Shaking());
        
    }

    IEnumerator Shaking()
    {
        
        Vector3 start = transform.position;
        float elapsed = 0f;

        while(elapsed < duration)
        {
            Debug.Log("HIII");
            elapsed += Time.deltaTime;
            float strength = curve.Evaluate(elapsed/duration);
            transform.position = start + Random.insideUnitSphere * strength;
            yield return null;
        }
        transform.position = start;
    }
}
