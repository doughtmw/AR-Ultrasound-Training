using System;
using System.Collections.Generic;
using UnityEngine;

internal class UnityMainThread : MonoBehaviour
{
    public int maxJobs = 5;
    internal static UnityMainThread wkr;
    Queue<Action> jobs = new Queue<Action>();

    void Awake()
    {
        wkr = this;
    }

    void Update()
    {
        while (jobs.Count > 0)
        {
            jobs.Dequeue().Invoke();
        }
    }

    internal void AddJob(Action newJob)
    {
        if (jobs.Count >= maxJobs)
        {
            jobs.Clear();
        }

        jobs.Enqueue(newJob);
    }
}