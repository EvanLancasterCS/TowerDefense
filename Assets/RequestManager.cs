using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RequestManager : MonoBehaviour
{
    public static RequestManager instance;
    Queue<PathRequest> requestQueue = new Queue<PathRequest>();
    PathRequest currentRequest;

    bool processingPath = false;

    void Awake()
    {
        instance = this;
    }

    public static void RequestPath(Coordinate start, Coordinate end, Action<List<Coordinate>, bool> callback)
    {
        PathRequest newRequest = new PathRequest(start, end, callback);
        instance.requestQueue.Enqueue(newRequest);
        instance.TryProcessNext();
    }

    private void TryProcessNext()
    {
        if(!processingPath && requestQueue.Count != 0)
        {
            currentRequest = requestQueue.Dequeue();
            processingPath = true;
            AStarManager.instance.GetPath(currentRequest.start, currentRequest.end);
        }
    }

    public void FinishedProcessingPath(List<Coordinate> path, bool success)
    {
        // i dont really know why but regular null check just straight up does not work.
        // so instead we do some dumb stuff to check for null i guess
        bool isNull = (currentRequest.callback.Target.ToString().StartsWith("null"));
        if (!isNull)
            currentRequest.callback(path, success);
        processingPath = false;
        TryProcessNext();
    }
    
    struct PathRequest
    {
        public Coordinate start;
        public Coordinate end;
        public Action<List<Coordinate>, bool> callback;

        public PathRequest(Coordinate _start, Coordinate _end, Action<List<Coordinate>, bool> _callback)
        {
            start = _start;
            end = _end;
            callback = _callback;
        }
    }
}
