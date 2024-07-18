using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshBake : MonoBehaviour
{

    public List<GameObject> surfaces = new List<GameObject>();
    //public Transform[] objectsToRotate;

    // Use this for initialization
    void Update()
    {


        for (int i = 0; i < surfaces.Count; i++)
        {
            surfaces[i].GetComponent<NavMeshSurface>().BuildNavMesh();
        }
    }

}