using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node>
{
    public bool walkable;
    public Vector3 worldPosition;
    public int gCost;
    public int hCost;
    public int gridX;
    public int gridY;
    public Node parent;
    public int penalty;
    int heapIndex;
    public Node (bool walkable, Vector3 worldPosition, int gridX, int gridY, int penalty) {
        this.walkable = walkable;
        this.worldPosition = worldPosition;
        this.gridX = gridX;
        this.gridY = gridY;
        this.penalty = penalty;
    }

    public int fCost {
        get {
            return gCost + hCost;
        }
    }

    public int HeapIndex {
        get {
            return heapIndex;
        }
        set {
            heapIndex = value;
        }
    }

    public int CompareTo(Node compare) {
        int comp = fCost.CompareTo(compare.fCost);
        if(comp == 0) {
            comp = hCost.CompareTo(compare.hCost);
        }
        return -comp;
    }
}
