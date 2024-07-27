using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TouchData : MonoBehaviour
{
    public int touchIncreaseLevel = 1;
    public BigInteger touchIncreaseAmount = 50;
    public BigInteger upgradeLifeCost = 1000;
    public BigInteger lifeGenerationPerLevel = 10; // 일정 증가량

    public TextMeshProUGUI touchLevelText;
    public TextMeshProUGUI touchIncreaseText; // 추가된 텍스트 UI 요소
    public TextMeshProUGUI upgradelifeCostText;

    private void Start()
    {
        UpdateUI();
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
            touchIncreaseAmount = touchIncreaseAmount * 104 / 100; // n레벨 터치 생명력 생산량 공식 적용
        }
        // 업그레이드 비용 공식 적용
        upgradeLifeCost = upgradeLifeCost * 120 / 100; // n레벨 업그레이드 비용 공식 적용
        UIManager.Instance.tree.UpdateTreeMeshes(touchIncreaseLevel); // 나무 모습 업데이트
        AutoObjectManager.Instance.CheckUnlockCondition();
        UpdateUI();
    }

    public void ApplyIncreaseRate(BigInteger rate)
    {
        touchIncreaseAmount *= 1 + rate;
        
        UpdateUI();
    }

    public void UpdateUI()
    {
        UpdateTouchUI(touchIncreaseLevel, touchIncreaseAmount, upgradeLifeCost);
    }

    public void UpdateTouchUI(int touchIncreaseLevel, BigInteger touchIncreaseAmount, BigInteger upgradelifeCost)
    {
        touchLevelText.text = $"외로운 나무 레벨:{BigIntegerUtils.FormatBigInteger(touchIncreaseLevel)}";
        touchIncreaseText.text = $"현재 터치당 얻는 생명력 : {BigIntegerUtils.FormatBigInteger(touchIncreaseAmount)}";
        upgradelifeCostText.text = $"강화 비용: {BigIntegerUtils.FormatBigInteger(upgradelifeCost)} 생명력";
    }
}
