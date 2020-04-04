using System;
using System.Collections.Generic;
using UnityEngine;

public class JobQueue {
    
    // TODO replace with some dedicated class. For now this is just a public variable.
    public Queue<Job> jobs;

    Action<Job> cbJobCreated;

    public JobQueue() {
        jobs = new Queue<Job>();
    }

    public void Enqueue(Job job) {
        jobs.Enqueue(job);
        Debug.Log("JobQueue size is now: " + WorldController.instance.world.jobQueue.Count());
        
        cbJobCreated?.Invoke(job);
    }

    public Job Dequeue() {
        if (jobs.Count == 0) {
            Debug.Log("Job queue is empty!");
            return null;
        }
        
        Debug.Log("JobQueue size is now: " + (WorldController.instance.world.jobQueue.Count() - 1));
        return jobs.Dequeue();
    }

    public int Count() {
        return jobs.Count;
    }

    public void RegisterJobCreatedCallback(Action<Job> callback) {
        cbJobCreated += callback;
    }

    public void UnregisterJobCallback(Action<Job> callback) {
        cbJobCreated -= callback;
    }
    
}