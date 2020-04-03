using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JobSpriteController : MonoBehaviour
{
    // Uses the InstalledObjectSpriteController for the moment because the Job system will likely change in the
    // future!

    InstalledObjectSpriteController iosc;
    
    // Start is called before the first frame update
    void Start() {
        iosc = FindObjectOfType<InstalledObjectSpriteController>();
        
        WorldController.instance.world.jobQueue.RegisterJobCreatedCallback(OnJobCreated);
    }

    void OnJobCreated(Job job) {
        // TODO: only supports InstalledObjects currently
        job.RegisterJobCompleteCallback(OnJobEnded);
        job.RegisterJobCancelCallback(OnJobEnded);
    }

    void OnJobEnded(Job job) {
        // TODO: only supports InstalledObjects currently
    }
}
