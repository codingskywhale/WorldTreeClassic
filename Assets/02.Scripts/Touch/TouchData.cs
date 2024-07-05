using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TouchData : MonoBehaviour
{
    public int touchIncreaseLevel = 1;
    public float touchIncreaseAmount = 10;
    public int upgradeLifeCost = 20;
    public int lifeGenerationPerLevel = 10; // 일정 증가량

    public TextMeshProUGUI touchLevelText;
    public TextMeshProUGUI touchIncreaseText; // 추가된 텍스트 UI 요소
    public TextMeshProUGUI upgradelifeCostText;

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
        UIManager.Instance.tree.UpdateTreeMeshes(touchIncreaseLevel); // 나무 모습 업데이트
    }

    public void ApplyIncreaseRate(float rate)
    {
        touchIncreaseAmount *= 1 + rate;
        
        UpdateUI();
    }

    public void UpdateUI()
    {
        UIManager.Instance.touchData.UpdateTouchUI(touchIncreaseLevel, touchIncreaseAmount, upgradeLifeCost);
    }

    public void UpdateTouchUI(int touchIncreaseLevel, float touchIncreaseAmount, int upgradelifeCost)
    {
        touchLevelText.text = $"외로운 나무 레벨: {touchIncreaseLevel}";
        touchIncreaseText.text = $"현재 터치당 얻는 생명력 : {touchIncreaseAmount}";
        upgradelifeCostText.text = $"강화 비용: {upgradelifeCost} 생명력";
    }
}
