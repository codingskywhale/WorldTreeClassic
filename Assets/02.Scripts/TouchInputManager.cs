using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchInputManager : MonoBehaviour
{
    public LifeManager lifeManager;
    public UIManager uIManagerBG;
    public int touchIncreaseLevel = 1;
    public int touchIncreaseAmount = 10;
    public int upgradelifeCost = 20;

    private void Start()
    {
        UpdateUI();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isPointerOverUIElement())
        {
            lifeManager.IncreaseWater(touchIncreaseAmount);
            UpdateTouchIncreaseUI();
        }
    }

    private bool isPointerOverUIElement()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z);

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    private void UpdateUI()
    {
        uIManagerBG.UpdateTouchUI(touchIncreaseLevel, touchIncreaseAmount, upgradelifeCost);
        UpdateTouchIncreaseUI();
    }

    public void UpdateTouchIncreaseUI()
    {
        uIManagerBG.UpdateTouchIncreaseUI(touchIncreaseAmount);
    }
}
