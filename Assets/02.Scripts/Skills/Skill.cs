using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Numerics;

public abstract class Skill : MonoBehaviour
{
    public float skillDuration; // 스킬 지속시간
    public float cooldownTime = 15.0f; // 기본 쿨타임
    public TextMeshProUGUI cooldownText; // 쿨타임을 표시할 텍스트
    public Image cooldownImage; // 회전할 이미지

    public Button skillButton; // 스킬 버튼
    public Button upgradeButton; // 해금/업그레이드 버튼

    protected bool onCooldown = false;
    protected float cooldownRemaining;

    public int currentLevel = 0; // 현재 스킬 레벨 (0 = 잠금 상태)
    public TextMeshProUGUI upgradeCostText; // 업그레이드 비용을 표시할 텍스트
    public TextMeshProUGUI currentLevelText; // 현재 스킬 레벨 텍스트
    public TextMeshProUGUI skillInfoText; // 현재 스킬 설명 텍스트
    public BigInteger unlockCost = 200; // 해금 비용


    protected virtual void Start()
    {
        // 각 스킬의 지속시간과 쿨타임은 서브 클래스에서 설정됩니다.
        UpdateUpgradeCostUI(); // 초기 업그레이드 비용 UI 설정
        UpdateUI(); // 스킬 설명과 레벨 UI 업데이트
        CheckUnlockStatus(); // 해금 상태 확인 및 UI 업데이트
    }

    private void Update()
    {
        if (onCooldown)
        {
            cooldownRemaining -= Time.deltaTime;
            if (cooldownRemaining < 0) cooldownRemaining = 0;

            UpdateCooldownUI(cooldownRemaining);
        }

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
            UpdateClickValues();
            UpdateUpgradeCostUI(); // 업그레이드 비용 UI 업데이트
            UpdateUI(); // UI 업데이트
            CheckUnlockStatus(); // 해금 후 상태 확인 및 UI 업데이트
        }
        else
        {
            Debug.Log("Not enough diamonds to unlock.");
        }
    }

    public abstract void ActivateSkill();

    protected IEnumerator SkillEffect()
    {
        onCooldown = true;
        cooldownRemaining = cooldownTime;

        yield return StartCoroutine(ApplySkillEffect());

        while (cooldownRemaining > 0)
        {
            yield return null;
        }

        onCooldown = false;
        UpdateCooldownUI(0);
    }

    public void UpdateCooldownUI(float remaining)
    {
        if (cooldownText != null)
        {
            if (remaining > 0)
            {
                int minutes = Mathf.FloorToInt(remaining / 60); // 분 계산
                int seconds = Mathf.CeilToInt(remaining % 60);  // 초 계산
                cooldownText.text = $"{minutes:D2}:{seconds:D2}"; // 분:초 형식으로 텍스트 설정
            }
            else
            {
                cooldownText.text = ""; // 00:00일 때는 텍스트를 빈 문자열로 설정
            }
        }

        if (cooldownImage != null)
        {
            float fillAmount = 1 - (remaining / cooldownTime);
            cooldownImage.fillAmount = fillAmount;
        }
    }

    protected abstract IEnumerator ApplySkillEffect();

    private void UpgradeSkill()
    {
        BigInteger upgradeCost = CalculateUpgradeCost(currentLevel);
        if (DiamondManager.Instance.HasSufficientDiamond(upgradeCost))
        {
            DiamondManager.Instance.DecreaseDiamond(upgradeCost);
            currentLevel++;
            UpdateClickValues(); // 필요 시 업데이트
            UpdateUpgradeCostUI(); // 업그레이드 비용 UI 업데이트
            UpdateUI();
        }
        else
        {
            Debug.Log("Not enough diamonds to upgrade.");
        }
    }

    public BigInteger CalculateUpgradeCost(int level)
    {
        if (level >= 1 && level <= 10)
        {
            return level * 10;
        }
        else if (level >= 11 && level <= 21)
        {
            return (level - 10) * 100;
        }
        else
        {
            return BigInteger.Zero; // 임의로 추가한 범위 외의 레벨은 업그레이드 불가
        }
    }

    public virtual void UpdateUI()
    {
        UpdateUpgradeCostUI();
        NowskillInfoUI();
        LevelUI();
    }

    protected void UpdateUpgradeCostUI()
    {
        if (upgradeCostText != null)
        {
            BigInteger nextCost = currentLevel > 0 ? CalculateUpgradeCost(currentLevel) : unlockCost;
            upgradeCostText.text = currentLevel > 0
                ? $"업그레이드 비용: {BigIntegerUtils.FormatBigInteger(nextCost)} 다이아"
                : $"해금 비용: {BigIntegerUtils.FormatBigInteger(nextCost)} 다이아";
        }
    }

    protected virtual void LevelUI()
    {
        if (currentLevelText != null)
        {
            currentLevelText.text = currentLevel > 0
                ? $"현재 스킬 레벨: {currentLevel}"
                : "스킬이 해금되지 않았습니다";
        }
    }

    protected virtual void NowskillInfoUI()
    {
        if (skillInfoText != null)
        {
            skillInfoText.text = currentLevel > 0
                ? $"현재 스킬 능력치: "
                : "스킬이 해금되지 않았습니다";
        }
    }

    protected virtual void UpdateClickValues() { }

    private void CheckUnlockStatus()
    {
        if (upgradeButton != null)
        {
            upgradeButton.interactable = currentLevel == 0
                ? DiamondManager.Instance.HasSufficientDiamond(unlockCost)
                : DiamondManager.Instance.HasSufficientDiamond(CalculateUpgradeCost(currentLevel));
        }

        if (skillButton != null)
        {
            skillButton.interactable = currentLevel > 0 && !onCooldown;
        }
    }
}
