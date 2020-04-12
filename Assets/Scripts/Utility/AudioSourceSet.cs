using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtensionMethods;

public class AudioSourceSet : MonoBehaviour {
    public AudioClip[] clips;
    public AudioSource source;

    [Range(0.0f, 2.0f)]
    public float minPitch = 0.9f;
    [Range(0.0f, 2.0f)]
    public float maxPitch = 1.1f;

    private AudioClip[] shuffledClips;
    private int nextClip;

    // Use this for initialization
    void Start () {
        if(clips == null || clips.Length == 0)
        {
            Debug.LogWarningFormat("{0} has no clips set", this.PathID());
        }

        ReshuffleClips();
    }

    private void ReshuffleClips()
    {
        shuffledClips = Chance.sShuffle<AudioClip>(clips);
        nextClip = 0;
    }

    private AudioClip PickClip()
    {
        if (shuffledClips == null || shuffledClips.Length == 0)
        {
            Debug.LogWarningFormat("{0} has no clips set", this.PathID());
            return null;
        }

        var Result = shuffledClips[nextClip];

        nextClip++;

        if (nextClip == shuffledClips.Length) ReshuffleClips();

        return Result;
    }

    public void Play()
    {
        var Clip = PickClip();

        if (Clip == null)
        {
            Debug.LogWarningFormat("{0} Play pick returned no clip", this.PathID());
            return;
        }

        source.pitch = Chance.sRandom(maxPitch, minPitch);
        source.clip = Clip;
        source.Play();
    }

    public void PlayOne()
    {
        var Clip = PickClip();

        if (Clip == null)
        {
            Debug.LogWarningFormat("{0} PlayOne pick returned no clip", this.PathID());
            return;
        }

        Debug.LogFormat("{0} PlayOne is playing {1}", this.PathID(), Clip.name);
        source.pitch = Chance.sRandom(maxPitch, minPitch);
        source.clip = Clip;
        source.PlayOneShot(Clip);
    }
}
