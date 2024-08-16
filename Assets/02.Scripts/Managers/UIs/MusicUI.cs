using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MusicUI : MonoBehaviour
{
    public GameObject musicPanel; 
    public Button openMusicButton; 
    public Button closeMusicButton;

    public TMP_Text songTitleText; // 노래 제목 텍스트
    public TMP_Text artistNameText; // 가수 이름 텍스트
    public TMP_Text descriptionText; // 노래 설명 텍스트

    public Image songImage; // 중앙에서 회전할 재생 중인 노래 이미지
    public Button[] songButtons; // 스크롤뷰 안의 노래 이미지 버튼들
    public ScrollRect scrollRect; // 스크롤뷰
    public float rotationSpeed = 10f; // 회전 속도

    public Button deactivationButton;

    private int currentIndex = 0;

    public UIOpenClose uiOpenCloseManager;

    private void Start()
    {
        // 초기 설정: 첫 번째 노래 이미지로 설정
        Initialize();

        openMusicButton.onClick.AddListener(OpenMusicPanel);
        closeMusicButton.onClick.AddListener(CloseMusicPanel);                
    }

    private void Update()
    {
        // 중앙의 노래 이미지 회전
        songImage.transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
    }

    private void Initialize()
    {
        // 첫 번째 노래 이미지로 초기 설정
        if (songButtons.Length > 0)
        {
            UpdateSongImage(0);
            CenterOnItem(songButtons[0].GetComponent<RectTransform>());
            PlaySelectedSong(0);
        }
    }

    public void OnSongImageClick(int index)
    {
        // 선택된 노래로 업데이트
        UpdateSongImage(index);
        CenterOnItem(songButtons[index].GetComponent<RectTransform>());
        PlaySelectedSong(index);
    }

    private void UpdateSongImage(int index)
    {
        songImage.sprite = songButtons[index].GetComponent<Image>().sprite;
        currentIndex = index;
    }

    private void UpdateSongInfo()
    {
        var currentBGM = SoundManager.instance.GetCurrentBGM();
        if (currentBGM != null)
        {
            songTitleText.text = currentBGM.title;
            artistNameText.text = currentBGM.artist;
            descriptionText.text = currentBGM.description;
        }
    }

    private void CenterOnItem(RectTransform target)
    {
        Canvas.ForceUpdateCanvases();

        // Viewport의 절반 크기
        float viewportHalfWidth = scrollRect.viewport.rect.width / 2;

        // 타겟의 로컬 위치
        Vector2 targetLocalPosition = target.localPosition;

        // Content의 새로운 위치 계산 (타겟이 뷰포트의 중앙에 오도록)
        Vector2 newLocalPosition = new Vector2(-targetLocalPosition.x + viewportHalfWidth, scrollRect.content.localPosition.y);

        // 스크롤뷰의 Content 위치 설정
        StartCoroutine(SmoothMove(scrollRect.content.localPosition, newLocalPosition, 0.5f));
    }

    private IEnumerator SmoothMove(Vector2 start, Vector2 end, float duration)
    {
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            scrollRect.content.localPosition = Vector2.Lerp(start, end, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        scrollRect.content.localPosition = end;
    }

    private void PlaySelectedSong(int index)
    {
        if (SoundManager.instance != null && index < SoundManager.instance.bgmClips.Length)
        {
            SoundManager.instance.PlayBGM(SoundManager.instance.bgmClips[index]);
            UpdateSongInfo();
        }
    }

    private void OpenMusicPanel()
    {
        musicPanel.SetActive(true);
        if (deactivationButton != null)
        {
            deactivationButton.gameObject.SetActive(false); 
        }
    }

    private void CloseMusicPanel()
    {
        musicPanel.SetActive(false);
        if (deactivationButton != null)
        {
            if (uiOpenCloseManager != null && uiOpenCloseManager.IsPanelOpen)
            {
                deactivationButton.gameObject.SetActive(false); // 패널이 열려 있는 경우 비활성화
            }
            else
            {
                deactivationButton.gameObject.SetActive(true); // 패널이 닫혀 있는 경우 활성화
            }
        }
    }
}
