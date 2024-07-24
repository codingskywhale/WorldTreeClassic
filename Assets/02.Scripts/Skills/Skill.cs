using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Numerics;
using System.Collections;

public abstract class Skill : MonoBehaviour
{
    [Header("Skill Info")]
    public string skillName; // 스킬 이름
    public float skillDuration; // 스킬 지속시간
    public float cooldownTime = 15.0f; // 기본 쿨타임
    public TextMeshProUGUI cooldownText; // 쿨타임을 표시할 텍스트
    public Image cooldownImage; // 회전할 이미지

    public Button skillButton; // 스킬 버튼
    public Button upgradeButton; // 해금/업그레이드 버튼

    protected bool onCooldown = false;
    public float cooldownRemaining;

    public int currentLevel = 0; // 현재 스킬 레벨 (0 = 잠금 상태)
    public TextMeshProUGUI currentLevelText; // 현재 스킬 레벨 텍스트
    public TextMeshProUGUI skillInfoText; // 현재 스킬 설명 텍스트

    [Header("Upgrade")]
    public TextMeshProUGUI upgradeCostText; // 업그레이드 비용을 표시할 텍스트
    public TextMeshProUGUI upgradeCostText2; // 업그레이드 비용을 표시할 텍스트
    public BigInteger unlockCost = 200; // 해금 비용
    [Header("UnlockInfo")]
    public Image lockImage; // 해금 이미지
    public TextMeshProUGUI lockText; // 해금 텍스트
    private bool isUnlocked = false; // 해금 상태를 나타내는 변수 추가
    public int unlockThreshold; // 해금에 필요한 세계수 레벨
    [Header("PopUp Info")]
    // 팝업 관련 변수
    public GameObject skillPopup; // 팝업 오브젝트
    public TextMeshProUGUI skillPopupInfoText; // 팝업에 표시될 텍스트

    protected virtual void Start()
    {
        // 각 스킬의 지속시간과 쿨타임은 서브 클래스에서 설정됩니다.
        UpdateUpgradeCostUI(); // 초기 업그레이드 비용 UI 설정
        UpdateUI(); // 스킬 설명과 레벨 UI 업데이트
        CheckUnlockStatus(); // 해금 상태 확인 및 UI 업데이트

        // 팝업 초기 상태 비활성화
        if (skillPopup != null)
        {
            skillPopup.SetActive(false);
        }

        if (upgradeButton != null)
        {
            upgradeButton.onClick.AddListener(OnUpgradeButtonClicked);
        }

        if (skillButton != null)
        {
            skillButton.onClick.AddListener(ShowSkillInfoPopup);
        }
    }

    private void Update()
    {
        if (onCooldown)
        {
            cooldownRemaining -= Time.deltaTime;
            if (cooldownRemaining < 0) cooldownRemaining = 0;

            UpdateCooldownUI(cooldownRemaining);
        }

        CheckUnlockCondition();
    }

    public void UnlockOrUpgradeSkill()
    {
        if (currentLevel == 0)
        {
            UnlockSkill();
        }
        else
        {
            ShowSkillInfoPopup();
        }
    }

    private void UnlockSkill()
    {
        if (DataManager.Instance.touchData != null && DataManager.Instance.touchData.touchIncreaseLevel >= unlockThreshold)
        {
            isUnlocked = true;
            currentLevel = 1;
            UpdateClickValues();
            UpdateUpgradeCostUI(); // 업그레이드 비용 UI 업데이트
            UpdateUI(); // UI 업데이트
            CheckUnlockStatus(); // 해금 후 상태 확인 및 UI 업데이트
        }
        else
        {
            Debug.Log("Unlock condition not met.");
            ShowPopup("Unlock condition not met.");
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
        if (currentLevel >= 21)
        {
            Debug.Log("Maximum level reached.");
            ShowPopup("Maximum level reached.");
            return;
        }
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
            ShowPopup("Not enough diamonds to upgrade.");
        }
    }

    public BigInteger CalculateUpgradeCost(int level)
    {
        return level * 30; // 업그레이드 비용 (레벨) x 30
    }

    public virtual void UpdateUI()
    {
        UpdateUpgradeCostUI();
        NowskillInfoUI();
        LevelUI();
        UpdateUnlockUI(); // 해금 상태 UI 업데이트
    }

    protected void UpdateUpgradeCostUI()
    {
        if (upgradeCostText != null)
        {
            if (currentLevel >= 21)
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
    private void CheckUnlockCondition()
    {
        // 잠금 해제 조건 확인 로직
        if (!isUnlocked && DataManager.Instance.touchData != null
            && DataManager.Instance.touchData.touchIncreaseLevel >= unlockThreshold)
        {
            UnlockSkill(); // 잠금 해제 조건 만족 시 Unlock 호출
        }
    }
    private void UpdateUnlockUI()
    {
        if (!isUnlocked)
        {
            if (lockText != null)
            {
                lockText.text = $"잠금 해제 조건: 세계수 레벨 {unlockThreshold}";
            }

            if (lockImage != null)
            {
                lockImage.gameObject.SetActive(true);
            }
        }
        else
        {
            if (lockText != null)
            {
                lockText.gameObject.SetActive(false);
            }

            if (lockImage != null)
            {
                lockImage.gameObject.SetActive(false);
            }
        }
    }

    public void CheckUnlockStatus()
    {
        if (upgradeButton != null)
        {
            if (currentLevel >= 21)
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

        if (skillButton != null)
        {
            skillButton.interactable = currentLevel > 0 && !onCooldown;
        }
    }

    public void ReduceCooldown(float reductionPercentage)
    {
        cooldownTime = cooldownTime * (1 - (reductionPercentage / 100));
    }

    // 팝업을 띄우는 함수
    public void ShowPopup(string message)
    {
        if (skillPopup != null && skillPopupInfoText != null)
        {
            skillPopupInfoText.text = message;
            skillPopup.SetActive(true);
        }
    }

    public void ShowSkillInfoPopup()
    {
        if (skillPopup != null && skillPopupInfoText != null)
        {
            string currentAbility = GetCurrentAbilityDescription();
            string nextAbility = GetNextAbilityDescription();

            skillPopupInfoText.text = $"스킬 이름: {skillName}\n\n현재 능력: {currentAbility}\n\n레벨업 시 능력: {nextAbility}";
            skillPopup.SetActive(true);
        }
    }

    // 현재 능력 설명을 반환하는 함수 (각 스킬에서 오버라이드 필요)
    public abstract string GetCurrentAbilityDescription();

    // 레벨업 시 능력 설명을 반환하는 함수 (각 스킬에서 오버라이드 필요)
    public abstract string GetNextAbilityDescription();

    // 팝업을 닫는 함수
    public void ClosePopup()
    {
        if (skillPopup != null)
        {
            skillPopup.SetActive(false);
        }
    }

    public void OnUpgradeButtonClicked()
    {
        UpgradeSkill();
        ClosePopup();
    }
}
