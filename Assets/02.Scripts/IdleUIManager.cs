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

    private float timer;
    private bool isIdle;

    private void Start()
    {
        timer = idleTime;
        isIdle = false;
        ShowMainUI();
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

            if (timer <= 0)
            {
                EnterIdleMode();
            }
        }

        if (Input.anyKeyDown || Input.GetMouseButton(0))
        {
            ResetTimer();
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
        songTitleText.text = GetCurrentSong(); 
        timeText.text = DateTime.Now.ToString("HH:mm:ss");
        dateText.text = GetFormattedDate(DateTime.Now);
    }

    private string GetFormattedDate(DateTime dateTime)
    {
        string dayOfWeek = dateTime.ToString("dddd", new System.Globalization.CultureInfo("ko-KR")); 
        return $"{dateTime.Month}.{dateTime.Day}.{dayOfWeek}";
    }

    private string GetCurrentSong()
    {   
        // 추후에 노래 추가
        return "Sample Song";
    }
}
