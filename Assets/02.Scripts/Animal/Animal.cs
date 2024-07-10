using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Animal : MonoBehaviour, IClickableObject //카메라 적용 인터페이스
{
    public HeartButton heart;

    public void HeartTouch()
    {
        heart.TouchHeartBubble();
        LifeManager.Instance.bubbleGenerator.GenerateNewHeart();
    }

    // 카메라 적용
    public void OnPointerClick(PointerEventData eventData)
    {
        CameraTargetHandler.Instance.SetTarget(transform);
    }
}
