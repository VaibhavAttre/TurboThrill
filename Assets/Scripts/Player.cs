using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
   // public float minX = 
    // Start is called before the first frame update
    Grid grid;
    public float minX; // Minimum X value
    public float maxX;  // Maximum X value
    public float minY; // Minimum Y value
    public float maxY;  // Maximum Y value

    void Start()
    {
        grid = GetComponent<Grid>();
        minX = -grid.gridWorldSize.x / 2;
        maxX = grid.gridWorldSize.x / 2;
        minY = -grid.gridWorldSize.y/2;
        maxY = grid.gridWorldSize.y/2;

        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);

        transform.position = new Vector3(randomX, 1, randomY);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
