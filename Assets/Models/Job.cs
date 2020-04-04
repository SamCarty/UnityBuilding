using System;
using UnityEngine;

public class Job
{
    // Holds information about a queued job, which includes placing InstalledObject, moving LooseObjects,
    // working at a research bench etc.

    public Tile tile { get; protected set; }
    float jobTime;
    
    Action<Job> cbJobComplete;
    Action<Job> cbJobCancel;

    public Job(Tile tile, Action<Job> cbJobComplete, float jobTime = 1f) {
        this.tile = tile;
        this.jobTime = jobTime;
        this.cbJobComplete = cbJobComplete;
    }
    
    public void Work(float workTime) {
        jobTime -= workTime;

        if (jobTime <= 0) {
            cbJobComplete?.Invoke(this);
        }
    }

    public void Cancel() {
        cbJobCancel?.Invoke(this);
    }

    public void RegisterJobCompleteCallback(Action<Job> callback) {
        cbJobComplete += callback;
    }

    public void UnregisterJobCompleteCallback(Action<Job> callback) {
        cbJobComplete -= callback;
    }
    
    public void RegisterJobCancelCallback(Action<Job> callback) {
        cbJobCancel += callback;
    }

    public void UnregisterJobCancelCallback(Action<Job> callback) {
        cbJobCancel -= callback;
    }

}
