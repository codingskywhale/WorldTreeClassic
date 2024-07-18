using UnityEngine;

[System.Serializable]
public class BGMClip
{
    public AudioClip clip;
    public float volume = 1.0f;
    public string Idletitle;
    public string title;
    public string artist; // 추가: 가수 이름
    public string description; // 추가: 노래 설명
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

    private BGMClip currentBGM;

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
        currentBGM = bgmClip;
    }

    public void PlaySFX(SFXClip sfxClip)
    {
        sfxSource.clip = sfxClip.clip;
        sfxSource.volume = sfxClip.volume;
        sfxSource.PlayOneShot(sfxClip.clip);
    }
    public string GetCurrentBGMTitle()
    {
        return currentBGM != null ? currentBGM.Idletitle : "";
    }

    public BGMClip GetCurrentBGM()
    {
        return currentBGM;
    }
}
