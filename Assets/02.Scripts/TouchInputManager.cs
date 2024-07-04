using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchInputManager : MonoBehaviour
{
    public LifeManager lifeManager;
    public UIManager uiManagerBG;
    public int touchIncreaseLevel = 1;
    public int touchIncreaseAmount = 10;
    public int upgradeLifeCost = 20;
    public int lifeGenerationPerLevel = 10; // 일정 증가량

    private void Start()
    {
        UpdateUI();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isPointerOverUIElement())
        {
            lifeManager.IncreaseWater(touchIncreaseAmount);
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

    public void UpgradeTouchGeneration()
    {
        touchIncreaseLevel++;
        if (touchIncreaseLevel % 25 == 0)
        {
            touchIncreaseAmount *= 2; // 25레벨마다 두 배로 증가
        }
        else
        {
            touchIncreaseAmount += lifeGenerationPerLevel; // 일정하게 증가
        }
        upgradeLifeCost += 20; // 업그레이드 비용 증가
        UpdateUI();
        uiManagerBG.UpdateTreeMeshes(touchIncreaseLevel); // 나무 모습 업데이트
    }

    private void UpdateUI()
    {
        uiManagerBG.UpdateTouchUI(touchIncreaseLevel, touchIncreaseAmount, upgradeLifeCost);
    }
}
