using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Animal : MonoBehaviour, IClickableObject //카메라 적용 인터페이스
{
    [SerializeField] private HeartButton heart;
    // 테스트용 
    public float heartOnDelay = 30f;

    public void HeartTouch()
    {
        heart.TouchHeartBubble();
        StartCoroutine(heartGenerateDelay());
    }
    IEnumerator heartGenerateDelay()
    {
        yield return new WaitForSeconds(heartOnDelay);
        heart.gameObject.SetActive(true);
    }

    // 카메라 적용
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!CameraTargetHandler.Instance.IsFreeCamera())
        {            
            return;
        }
        CameraTargetHandler.Instance.SetTarget(transform);
    }
}
