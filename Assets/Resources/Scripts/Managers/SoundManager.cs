using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SoundManager : MonoBehaviour
{
    // singleton을 위한 개체
    public static SoundManager Instance
    {
        get;
        private set;
    }

    public AudioClip[] audioClips = null;
    public AudioSource oneShotAudio = null;
    public AudioSource loopAudio = null;
    private Dictionary<string, AudioClip> mAudioMap = null;

    public void PlayOneShot(string name)
    {
        if(oneShotAudio == null)
        {
            return;
        }

        oneShotAudio.PlayOneShot(mAudioMap[name]);
    }

    public void PlayLoop(string name)
    {
        if(loopAudio == null)
        {
            return;
        }

        if (name == null)
        {
            loopAudio.Stop();
        }
        else
        {
            loopAudio.Stop();
            loopAudio.clip = mAudioMap[name];
            loopAudio.Play();
        }
    }

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            initialize();
        }
        else if(Instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    void OnDestroy()
    {
        PlayLoop(null);
    }

    private void initialize()
    {
        mAudioMap = new Dictionary<string, AudioClip>();
        foreach (AudioClip ac in audioClips)
        {
            mAudioMap.Add(ac.name, ac);
        }
        PlayLoop("out-game-normal");
    }
}
