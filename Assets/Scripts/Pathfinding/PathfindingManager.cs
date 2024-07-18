using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PathfindingManager : MonoBehaviour
{
    // Start is called before the first frame update
    Queue<PathReqest> pathRequests = new Queue<PathReqest>();
    PathReqest currPR;
    static PathfindingManager instance;
    Pathfinding pathfinding;
    bool isProcessingPath;
    void Awake() {
        instance = this;
        pathfinding = GetComponent<Pathfinding>();
    }
    public static void RequestPath(Vector3 start, Vector3 end, Action<Vector3[], bool> callback) {
        PathReqest newRequest = new PathReqest(start, end, callback);
        instance.pathRequests.Enqueue(newRequest);
        instance.TryProcessNext();
    }
    void TryProcessNext() {
        if(!isProcessingPath && pathRequests.Count > 0) {
            currPR = pathRequests.Dequeue();
            isProcessingPath = true;
            pathfinding.StartFindPath(currPR.start, currPR.end);
        }
    }

    public void FinishedProcessingPath(Vector3[] path, bool done) {
        currPR.callback(path, done);
        isProcessingPath = false;
        TryProcessNext();
    }
    

    struct PathReqest {
        public Vector3 start;
        public Vector3 end;
        public Action<Vector3[], bool> callback;

        public PathReqest (Vector3 start, Vector3 end, Action<Vector3[], bool> callback) {
            this.start = start;
            this.end = end;
            this.callback = callback;
        }
    }

    
}
