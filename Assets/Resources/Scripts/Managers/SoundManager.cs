using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SoundManager : MonoBehaviour
{
    // singleton을 위한 개체
    public static SoundManager Instance = null;

    public AudioClip[] AudioClips = null;
    public AudioSource OneShotAudio = null;
    public AudioSource LoopAudio = null;
    private Dictionary<string, AudioClip> mAudioMap = null;

    public void PlayOneShot(string name)
    {
        if(OneShotAudio == null)
        {
            return;
        }

        OneShotAudio.PlayOneShot(mAudioMap[name]);
    }

    public void PlayLoop(string name)
    {
        if(LoopAudio == null)
        {
            return;
        }

        if (name == null)
        {
            LoopAudio.Stop();
        }
        else
        {
            LoopAudio.Stop();
            LoopAudio.clip = mAudioMap[name];
            LoopAudio.Play();
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
        foreach (AudioClip ac in AudioClips)
        {
            mAudioMap.Add(ac.name, ac);
        }
        PlayLoop("out-game-normal");
    }
}
