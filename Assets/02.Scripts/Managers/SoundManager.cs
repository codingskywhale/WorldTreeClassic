using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class BGMClip
{
    public AudioClip clip;
    public string Idletitle;
    public string title;
    public string artist;
    public string description;
}

[System.Serializable]
public class SFXClip
{
    public AudioClip clip;
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance = null;

    public AudioSource bgmSource;
    public AudioSource sfxSource;

    public BGMClip[] bgmClips;
    public SFXClip[] sfxClips;

    public Slider bgmVolumeSlider;
    public Slider sfxVolumeSlider;

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
        // 슬라이더 값이 바뀔 때마다 볼륨을 업데이트
        bgmVolumeSlider.onValueChanged.AddListener(SetBGMVolume);
        sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);

        // 초기 BGM 및 SFX 볼륨을 슬라이더 값으로 설정
        bgmSource.volume = bgmVolumeSlider.value;
        sfxSource.volume = sfxVolumeSlider.value;

        // 첫 번째 BGM 재생
        PlayBGM(bgmClips[0]);
    }

    public void PlayBGM(BGMClip bgmClip)
    {
        // BGM 클립 재생
        bgmSource.clip = bgmClip.clip;
        bgmSource.Play();
        currentBGM = bgmClip;
    }

    public void PlaySFX(SFXClip sfxClip)
    {
        sfxSource.PlayOneShot(sfxClip.clip);
    }

    public void SetBGMVolume(float volume)
    {
        bgmSource.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
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