using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PathRequestManager : MonoBehaviour
{
    Pathfinding pathfinding;
    Queue<PathResult> results = new Queue<PathResult>();

    static PathRequestManager instance;

    private void Awake()
    {
        instance = this;
        pathfinding = GetComponent<Pathfinding>();
    }

    private void Update()
    {
        if (results.Count > 0)
        {
            int itemsInQueue = results.Count;

            lock (results)
            {
                for (int i=0; i<itemsInQueue; i++)
                {
                    PathResult result = results.Dequeue();
                    result.callback(result.path, result.success);
                }
            }
        }
    }

    public void FinishProcessingPath(PathResult result)
    {
        lock (results)
        {
            results.Enqueue(result);
        }
    }

    public static void RequestPath(PathRequest request)
    {
        ThreadStart threadStart = delegate
        {
            instance.pathfinding.FindPath(request, instance.FinishProcessingPath);
        };

        threadStart.Invoke();
    }
}

public struct PathRequest
{
    public Vector3 pathStart, pathEnd;
    public Action<Vector3[], bool> callback;

    public PathRequest(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback)
    {
        this.pathStart = pathStart;
        this.pathEnd = pathEnd;
        this.callback = callback;
    }
}

public struct PathResult
{
    public bool success;
    public Vector3[] path;
    public Action<Vector3[], bool> callback;

    public PathResult(Vector3[] path, bool success, Action<Vector3[], bool> callback)
    {
        this.success = success;
        this.path = path;
        this.callback = callback;
    }
}