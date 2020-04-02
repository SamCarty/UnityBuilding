using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour {
    const float MAX_AUDIO_COOLDOWN = 0.1f;
    float audioCooldown = 0.1f;

    void Start() {
        WorldController.instance.world.RegisterInstalledObjectPlaced(OnInstalledObjectCreated);
        WorldController.instance.world.RegisterTileTypeChanged(OnTileTypeChanged);
    }

    void Update() {
        audioCooldown -= Time.deltaTime;
    }

    void OnInstalledObjectCreated(InstalledObject installedObject) {
        Debug.Log("Installed Object was placed.");
        AudioClip audioClip = Resources.Load<AudioClip>("Audio/Plate_Impact");
        PlaySound(audioClip, Camera.main.transform.position);
    }

    void OnTileTypeChanged(Tile tile) {
        Debug.Log("Tile type changed.");
        AudioClip audioClip = Resources.Load<AudioClip>("Audio/Heavy_Impact");
        PlaySound(audioClip, Camera.main.transform.position);
    }

    void PlaySound(AudioClip audioClip, Vector3 position) {
        if (audioCooldown > 0) {
            return;
        }

        AudioSource.PlayClipAtPoint(audioClip, position);
            audioCooldown = MAX_AUDIO_COOLDOWN;
    }
}
