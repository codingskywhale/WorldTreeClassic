using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BagUIManager : MonoBehaviour
{
    public GameObject bagCanvas;
    public GameObject bagPopup;

    private bool isBagOpen = false;

    private void Start()
    {
        bagCanvas.SetActive(false);
    }

    public void OnBagButtonClick()
    {
        if (!isBagOpen)
        {
            bagCanvas.SetActive(true);
            isBagOpen = true;
        }
    }

    public void OnBackgroundClick(BaseEventData data)
    {
        PointerEventData pointerData = (PointerEventData)data;
        if (isBagOpen && !RectTransformUtility.RectangleContainsScreenPoint(bagPopup.GetComponent<RectTransform>(), pointerData.position))
        {
            bagCanvas.SetActive(false);
            isBagOpen = false;            
        }
    }
}
