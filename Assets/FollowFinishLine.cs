using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class FollowFinishLine : MonoBehaviour
{

    public Transform MinimapCam;
    public float MinimapSize;
    Vector3 TempV3;

    void Update()
    {
        TempV3 = transform.parent.transform.position;
        TempV3.y = transform.position.y;
        transform.position = TempV3;
    }

    void LateUpdate()
    {
        Vector3 centerPosition = MinimapCam.transform.localPosition;
        centerPosition.y -= 0.5f;
        float Distance = Vector3.Distance(transform.position, centerPosition);
        if (Distance > MinimapSize)
        {
            Vector3 fromOriginToObject = transform.position - centerPosition;
            fromOriginToObject *= MinimapSize / Distance;
            transform.position = centerPosition + fromOriginToObject;
        }
        transform.position = new Vector3(transform.position.x, 20, transform.position.z);
    }
}
