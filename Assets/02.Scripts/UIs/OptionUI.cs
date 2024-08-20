using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OptionUI : MonoBehaviour
{
    public GameObject OptionCanvas;
    public GameObject OptionPopup;

    public void CheckPointerClick(BaseEventData eventData)
    {
        PointerEventData pointerData = (PointerEventData)eventData;
        // 클릭한 위치가 패널 외부인지 확인
        if (!RectTransformUtility.RectangleContainsScreenPoint(OptionPopup.GetComponent<RectTransform>(), pointerData.position))
        {
            // 패널 닫기
            OptionCanvas.SetActive(false);
        }
    }
}
