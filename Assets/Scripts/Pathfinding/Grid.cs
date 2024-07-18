using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public bool displayGizmos;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    public LayerMask unwalkableMask;
    public LayerMask walkableMask;
    Node[,] grid;
    float nodeDiameter;
    int gridSizeX, gridSizeY;
    public TerrainType[] terrains;
    Dictionary<int, int> walkableRegionsDictionary = new Dictionary<int, int>();
    // Start is called before the first frame update
    void Awake()
    {
        nodeDiameter = nodeRadius*2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x/nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y/nodeDiameter);

        foreach (TerrainType region in terrains) {
            walkableMask.value |= region.mask.value;
            walkableRegionsDictionary.Add((int)Mathf.Log(region.mask.value, 2), region.penalty);
        }

        CreateGrid();
    }

    public int MaxSize {
        get {
            return gridSizeX * gridSizeY;
        }
    }
    public List<Node> GetNeighbors(Node node) {
        List<Node> neighbors = new List<Node>();
        for(int x = -1; x <= 1; x++) {
            for(int y = -1; y <= 1; y++) {
                if(x == 0 && y == 0) {
                    continue;
                } else {
                    int checkX = node.gridX + x;
                    int checkY = node.gridY + y;
                    if(checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY) {
                        neighbors.Add(grid[checkX,checkY]);
                    }
                }
            }
        }
        return neighbors;
    }

    void CreateGrid() {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right*gridWorldSize.x/2 - Vector3.forward * gridWorldSize.y/2;
        for (int x = 0; x < gridSizeX; x++) {
            for(int y = 0; y < gridSizeY; y++) {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward*(y*nodeDiameter+nodeRadius);
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));
                int penalty = 0;

                if(walkable) {
                    Ray ray = new Ray(worldPoint + Vector3.up * 50, Vector3.down);
                    RaycastHit hit;
                    if(Physics.Raycast(ray, out hit, 100, walkableMask)) {
                        walkableRegionsDictionary.TryGetValue(hit.collider.gameObject.layer, out penalty);
                    }
                }

                grid[x,y] = new Node(walkable, worldPoint, x, y, penalty);
            }
        }
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition) {
        float percentX = (worldPosition.x + gridWorldSize.x/2) / gridWorldSize.x;
        float percentY = (worldPosition.z + gridWorldSize.y/2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY-1)* percentY);
        return grid[x,y];
    }
    void OnDrawGizmos() {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));
    
        if(grid != null && displayGizmos) {
            foreach (Node node in grid) {
                Gizmos.color = (node.walkable)?Color.white : Color.red;
                Gizmos.DrawCube(node.worldPosition, Vector3.one * (nodeDiameter-.1f));
            }
        }
    }

    [System.Serializable]
    public class TerrainType {
        public LayerMask mask;
        public int penalty;
    }
}
