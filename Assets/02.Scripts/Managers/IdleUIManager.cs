using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class IdleUIManager : MonoBehaviour
{
    public GameObject mainUI;
    public GameObject mainUI2;
    public GameObject idleUIContainer;
    public TMP_Text gameTitleText; // 게임 제목 텍스트
    public TMP_Text songTitleText; // 현재 노래 제목 텍스트
    public TMP_Text timeText; // 현재 시간 텍스트
    public TMP_Text dateText; // 오늘 날짜 텍스트
    public float idleTime = 10f; // 방치 시간 (초)
    public float scrollSpeed = 0.5f;

    private float timer;
    private bool isIdle;
    private string currentSongTitle;
    private UIOpenCloseManager uiOpenCloseManager;

    private void Start()
    {
        timer = idleTime;
        isIdle = false;
        uiOpenCloseManager = FindObjectOfType<UIOpenCloseManager>();
        ShowMainUI();
        currentSongTitle = SoundManager.instance.GetCurrentBGMTitle();
        StartCoroutine(ScrollSongTitle());
    }


    private void Update()
    {
        if (isIdle)
        {
            UpdateIdleUI();
        }
        else
        {
            timer -= Time.deltaTime;

            if (timer <= 0 && !uiOpenCloseManager.IsPanelOpen)
            {
                EnterIdleMode();
            }
        }

        if (Input.anyKeyDown || Input.GetMouseButton(0))
        {
            ResetTimer();
        }
        UpdateCurrentSongTitle();
    }

    private void UpdateCurrentSongTitle()
    {
        var currentBGM = SoundManager.instance.GetCurrentBGM();
        if (currentBGM != null)
        {
            currentSongTitle = currentBGM.Idletitle;
        }
    }

    private void ResetTimer()
    {
        timer = idleTime;

        if (isIdle)
        {
            ExitIdleMode();
        }
    }

    private void EnterIdleMode()
    {
        isIdle = true;
        mainUI.SetActive(false);
        mainUI2.SetActive(false);
        idleUIContainer.SetActive(true);
    }

    private void ExitIdleMode()
    {
        isIdle = false;
        ShowMainUI();
    }

    private void ShowMainUI()
    {
        mainUI.SetActive(true);
        mainUI2.SetActive(true);
        idleUIContainer.SetActive(false);
    }

    private void UpdateIdleUI()
    {
        gameTitleText.text = "세계수 키우기";        
        timeText.text = DateTime.Now.ToString("HH:mm:ss");
        dateText.text = GetFormattedDate(DateTime.Now);
        //songTitleText.text = currentSongTitle;
    }

    private string GetFormattedDate(DateTime dateTime)
    {
        string dayOfWeek = dateTime.ToString("dddd", new System.Globalization.CultureInfo("ko-KR")); 
        return $"{dateTime.Month}.{dateTime.Day}.{dayOfWeek}";
    }

    private IEnumerator ScrollSongTitle()
    {
        while (true)
        {
            string displayText = currentSongTitle + "   ";
            for (int i = 0; i < displayText.Length; i++)
            {
                songTitleText.text = displayText.Substring(i) + displayText.Substring(0, i);
                yield return new WaitForSeconds(scrollSpeed);
            }
        }
    }
}
