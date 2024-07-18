using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using JetBrains.Annotations;
using System.Linq;
using System;
using UnityEngine.Assertions.Must;
public class Pathfinding : MonoBehaviour
{
    Grid grid;
    PathfindingManager manager;
    void Awake() {
        manager = GetComponent<PathfindingManager>();
        grid = GetComponent<Grid>();
    }
    public void StartFindPath(Vector3 start, Vector3 end) {
        StartCoroutine(FindPath(start, end));
    }
    IEnumerator FindPath(Vector3 startPos, Vector3 targetPos) {

        Vector3[] waypoints = new Vector3[0];
        bool success = false;

        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        if (startNode.walkable && targetNode.walkable) {
            Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
            HashSet<Node> closeSet = new HashSet<Node>();
            openSet.Add(startNode);
            while(openSet.Count > 0) {
                Node curr = openSet.RemoveFirst();
                closeSet.Add(curr);
                if(curr == targetNode) {
                    success = true;
                    break;
                }

                foreach (Node neighbor in grid.GetNeighbors(curr)) {
                    if(!neighbor.walkable || closeSet.Contains(neighbor)) {
                        continue;
                    }
                    
                    int mvmtCostToNeighbor = curr.gCost + GetDistance(curr, neighbor) + neighbor.penalty;
                    if(mvmtCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor)) {
                        neighbor.gCost = mvmtCostToNeighbor;
                        neighbor.hCost = GetDistance(neighbor, targetNode);
                        neighbor.parent = curr;

                        if(!openSet.Contains(neighbor)) {
                            openSet.Add(neighbor);
                        } else {
                            openSet.UpdateItem(neighbor);
                        }
                    }
                }
            }
        }
        yield return null;
        if(success) {
            waypoints = RetracePath(startNode, targetNode);
        }
        manager.FinishedProcessingPath(waypoints, success);
    }
    
    Vector3[] RetracePath (Node startNode, Node endNode) {
        List<Node> path = new List<Node>();
        Node curr = endNode;

        while(curr != startNode) {
            path.Add(curr);
            curr = curr.parent;
        }
        Vector3[] waypoints = SimplifyPath(path);
        Array.Reverse(waypoints);
        return waypoints;
    }

    Vector3[] SimplifyPath(List<Node> path) {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;

        for(int i = 1; i < path.Count; i++) {
            Vector2 directionNew = new Vector2(path[i-1].gridX - path[i].gridX, path[i-1].gridY - path[i].gridY);
            if(directionNew != directionOld) {
                waypoints.Add(path[i].worldPosition);
            }
            directionOld = directionNew;
        }
        return waypoints.ToArray();
    }
    int GetDistance(Node A, Node B) {
        int distX = Mathf.Abs(A.gridX - B.gridX);
        int distY = Mathf.Abs(A.gridY - B.gridY);

        if(distX > distY) {
            return 14*distY+10*(distX - distY);
        } return 14*distX + 10*(distY - distX);
    }
}
