using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public interface iNeedAnimalCondition
{
    // 몇 마리가 필요한지.
    public int needCount { get; set; }
    public bool isAnimalConditionCleared { get; set; }
    public AnimalDataSO[] needAnimalSO {  get; set; }
    public void CheckAnimalConditionCleared();
}

public interface iNeedPlantCondition
{
    public void CheckPlantConditionCleared(string name);
}

public interface iNeedWorldTreeCondition
{
    public void CheckWorldTreeConditionCleared();
}
public class Animal : MonoBehaviour, IClickableObject //카메라 적용 인터페이스
{
    public HeartButton heart;
    public AnimalDataSO animalDataSO;

    public void HeartTouch()
    {
        heart.TouchHeartBubble();
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
