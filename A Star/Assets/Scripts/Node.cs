using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node>
{
    public bool walkable;
    public int gCost, hCost, gridX, gridY, movementPenalty;
    public Vector3 worldPosition;
    public Node parent;

    int heapIndex;

    public int FCost { get { return gCost + hCost; } }
    public int HeapIndex
    {
        get { return heapIndex; }
        set { heapIndex = value; }
    }

    public Node(bool walkable, Vector3 worldPosition, int gridX, int gridY, int movementPenalty)
    {
        this.walkable = walkable;
        this.worldPosition = worldPosition;
        this.gridX = gridX;
        this.gridY = gridY;
        this.movementPenalty = movementPenalty;
    }

    public int CompareTo(Node nodeToCompare)
    {
        int compare = FCost.CompareTo(nodeToCompare.FCost);

        if (compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }

        return -compare;
    }
}
