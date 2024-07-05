using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance = null;

    public AudioSource bgmSource;
    public AudioSource sfxSource;

    public AudioClip[] bgmClips;
    public AudioClip[] sfxClips;

    private void Awake()
    {        
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //private void Start()
    //{        
    //    PlayBGM(bgmClips[0]);
    //}

    //public void PlayBGM(BGMClip bgmClip)
    //{
    //    bgmSource.clip = bgmClip.clip;
    //    bgmSource.volume = bgmClip.volume;
    //    bgmSource.Play();
    //}

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

}
