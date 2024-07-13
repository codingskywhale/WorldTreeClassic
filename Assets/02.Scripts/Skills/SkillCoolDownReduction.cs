using System.Collections;
using UnityEngine;
using System.Numerics;
using TMPro;
using UnityEngine.UI;

public class SkillCoolDownReduction : MonoBehaviour
{
    public float cooldownReductionPercentage = 2f; // 스킬 쿨타임 감소 비율
    public float baseCooldownReduction = 2f; // 레벨당 쿨타임 감소 비율

    public int currentLevel = 0; // 현재 스킬 레벨 (0 = 잠금 상태)
    public BigInteger unlockCost = 100; // 해금 비용
    public TextMeshProUGUI upgradeCostText; // 업그레이드 비용을 표시할 텍스트
    public TextMeshProUGUI currentLevelText; // 현재 스킬 레벨 텍스트
    public TextMeshProUGUI skillInfoText; // 현재 스킬 설명 텍스트
    public Button upgradeButton; // 해금/업그레이드 버튼

    private void Start()
    {
        // 초기 UI 설정
        UpdateUpgradeCostUI();
        UpdateUI();
        CheckUnlockStatus();
    }

    private void Update()
    {
        CheckUnlockStatus();
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
            ApplyCooldownReduction();
            UpdateUpgradeCostUI();
            UpdateUI();
            CheckUnlockStatus();
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
            ApplyCooldownReduction();
            UpdateUpgradeCostUI();
            UpdateUI();
        }
        else
        {
            Debug.Log("Not enough diamonds to upgrade.");
        }
    }

    private void ApplyCooldownReduction()
    {
        Skill[] allSkills = FindObjectsOfType<Skill>();
        foreach (Skill skill in allSkills)
        {
            skill.ReduceCooldown(baseCooldownReduction * currentLevel);
        }
    }

    public BigInteger CalculateUpgradeCost(int level)
    {
        return (level - 1) + 10;
    }

    private void UpdateUpgradeCostUI()
    {
        if (upgradeCostText != null)
        {
            if (currentLevel >= 10)
            {
                upgradeCostText.text = "최대 레벨";
                upgradeButton.interactable = false;
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

    private void UpdateUI()
    {
        if (currentLevelText != null)
        {
            currentLevelText.text = currentLevel > 0
                ? $"현재 스킬 레벨: {currentLevel}"
                : "스킬이 해금되지 않았습니다";
        }

        if (skillInfoText != null)
        {
            skillInfoText.text = currentLevel > 0
                ? $"현재 스킬 쿨타임 감소: {cooldownReductionPercentage * currentLevel}%"
                : "스킬이 해금되지 않았습니다";
        }
    }

    private void CheckUnlockStatus()
    {
        if (upgradeButton != null)
        {
            upgradeButton.interactable = currentLevel == 0
                ? DiamondManager.Instance.HasSufficientDiamond(unlockCost)
                : currentLevel < 10 && DiamondManager.Instance.HasSufficientDiamond(CalculateUpgradeCost(currentLevel));
        }
    }
}
