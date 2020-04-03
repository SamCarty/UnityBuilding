using System;
using System.Collections.Generic;

public class JobQueue {
    
    // TODO replace with some dedicated class. For now this is just a public variable.
    public Queue<Job> jobs;

    Action<Job> cbJobCreated;

    public JobQueue() {
        jobs = new Queue<Job>();
    }

    public void Enqueue(Job job) {
        jobs.Enqueue(job);
        cbJobCreated?.Invoke(job);
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