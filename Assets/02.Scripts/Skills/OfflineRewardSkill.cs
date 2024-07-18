using UnityEngine;
using System.Numerics;
using TMPro;
using UnityEngine.UI;

public class OfflineRewardSkill : MonoBehaviour
{
    public int currentLevel = 0; // 현재 스킬 레벨 (0 = 잠금 상태)
    public TextMeshProUGUI upgradeCostText; // 업그레이드 비용을 표시할 텍스트
    public TextMeshProUGUI currentLevelText; // 현재 스킬 레벨 텍스트
    public TextMeshProUGUI skillInfoText; // 현재 스킬 설명 텍스트
    public BigInteger unlockCost = 200; // 해금 비용
    public Button upgradeButton; // 해금/업그레이드 버튼

    private void Start()
    {
        UpdateUI(); // 초기 UI 설정
        CheckUnlockStatus(); // 해금 상태 확인 및 UI 업데이트
    }

    public void UnlockOrUpgradeSkill()
    {
        if (currentLevel == 0)
        {
            UnlockSkill();
        }
        else
        {
            UpgradeSkill();
        }
    }

    private void UnlockSkill()
    {
        if (DiamondManager.Instance.HasSufficientDiamond(unlockCost))
        {
            DiamondManager.Instance.DecreaseDiamond(unlockCost);
            currentLevel = 1;
            UpdateUpgradeCostUI(); // 업그레이드 비용 UI 업데이트
            UpdateUI(); // UI 업데이트
            CheckUnlockStatus(); // 해금 후 상태 확인 및 UI 업데이트
        }
        else
        {
            Debug.Log("Not enough diamonds to unlock.");
        }
    }

    private void UpgradeSkill()
    {
        if (currentLevel >= 10)
        {
            Debug.Log("Maximum level reached.");
            return;
        }
        BigInteger upgradeCost = CalculateUpgradeCost(currentLevel);
        if (DiamondManager.Instance.HasSufficientDiamond(upgradeCost))
        {
            DiamondManager.Instance.DecreaseDiamond(upgradeCost);
            currentLevel++;
            UpdateUpgradeCostUI(); // 업그레이드 비용 UI 업데이트
            UpdateUI(); // UI 업데이트
            CheckUnlockStatus(); // 업그레이드 후 상태 확인 및 UI 업데이트
        }
        else
        {
            Debug.Log("Not enough diamonds to upgrade.");
        }
    }

    public BigInteger CalculateUpgradeCost(int level)
    {
        return unlockCost + (level * 10);;
    }

    private void UpdateUI()
    {
        UpdateUpgradeCostUI();
        UpdateSkillInfoUI();
        UpdateLevelUI();
    }

    private void UpdateUpgradeCostUI()
    {
        if (upgradeCostText != null)
        {
            if (currentLevel >= 10)
            {
                upgradeCostText.text = "최대 레벨";
            }
            else
            {
                BigInteger nextCost = currentLevel > 0 ? CalculateUpgradeCost(currentLevel) : unlockCost;
                upgradeCostText.text = currentLevel > 0
                    ? $"업그레이드 비용: {BigIntegerUtils.FormatBigInteger(nextCost)} 다이아"
                    : $"해금 비용: {BigIntegerUtils.FormatBigInteger(nextCost)} 다이아";
            }
        }
    }

    private void UpdateLevelUI()
    {
        if (currentLevelText != null)
        {
            currentLevelText.text = currentLevel > 0
                ? $"현재 스킬 레벨: {currentLevel}"
                : "스킬이 해금되지 않았습니다";
        }
    }

    private void UpdateSkillInfoUI()
    {
        if (skillInfoText != null)
        {
            skillInfoText.text = currentLevel > 0
                ? $"오프라인 보상 시간 증가: {currentLevel * 60}분"
                : "스킬이 해금되지 않았습니다";
        }
    }

    private void CheckUnlockStatus()
    {
        if (currentLevel >= 10)
        {
            upgradeButton.interactable = false;
        }
        else
        {
            upgradeButton.interactable = currentLevel == 0
                ? DiamondManager.Instance.HasSufficientDiamond(unlockCost)
                : DiamondManager.Instance.HasSufficientDiamond(CalculateUpgradeCost(currentLevel));
        }
    }
}
