using UnityEngine;

[System.Serializable]
public class BGMClip
{
    public AudioClip clip;
    public float volume = 1.0f;
}

[System.Serializable]
public class SFXClip
{
    public AudioClip clip;
    public float volume = 1.0f;
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance = null;

    public AudioSource bgmSource;
    public AudioSource sfxSource;

    public BGMClip[] bgmClips;
    public SFXClip[] sfxClips;

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

    private void Start()
    {
        PlayBGM(bgmClips[0]);
    }

    public void PlayBGM(BGMClip bgmClip)
    {
        bgmSource.clip = bgmClip.clip;
        bgmSource.volume = bgmClip.volume;
        bgmSource.Play();
    }

    public void PlaySFX(SFXClip sfxClip)
    {
        sfxSource.clip = sfxClip.clip;
        sfxSource.volume = sfxClip.volume;
        sfxSource.PlayOneShot(sfxClip.clip);
    }
}
