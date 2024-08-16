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
    public Button skillButton; // 스킬 버튼
    public Sprite skillImage; // 스킬 이미지를 저장할 필드

    [Header("ButtonLock")]
    public Image useSkillbuttonLockImage; // 해금/업그레이드 버튼

    [Header("Cool Down")]
    protected bool onCooldown = false;
    public float cooldownRemaining;
    public TextMeshProUGUI cooldownText; // 쿨타임을 표시할 텍스트
    public Image cooldownImage; // 회전할 이미지

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
    public ADSkill adSkill;

    [Header("Skill Info Display")]
    public GameObject skillInfoObject; // 스킬 정보를 표시할 오브젝트
    public TextMeshProUGUI skillInfoTextField; // 스킬 정보를 표시할 텍스트 필드
    public Button skillInfoButton; // 스킬 정보를 표시할 오브젝트의 버튼
    public Image skillDurationImage; // 스킬 지속 시간을 나타내는 이미지

    [Header("PopUp Info")]
    // 팝업 관련 변수
    public GameObject skillPopup; // 팝업 오브젝트

    [Header("Skill Text Info")]
    public GameObject skillTextObject; // 스킬 텍스트를 표시할 오브젝트
    public float fadeOutDuration = 2.0f; // 서서히 사라지는 시간

    private float skillTimeRemaining;
    private Coroutine disableCoroutine;
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

        // 스킬 정보 오브젝트 초기 상태 비활성화
        if (skillInfoObject != null)
        {
            skillInfoObject.SetActive(false);
        }

        // 스킬 텍스트 오브젝트 초기 상태 비활성화
        if (skillTextObject != null)
        {
            skillTextObject.SetActive(false);
        }

        // 스킬 버튼 이벤트 리스너 추가
        if (skillButton != null)
        {
            skillButton.onClick.AddListener(OnSkillButtonClick);
        }

        // 스킬 정보 오브젝트의 버튼 이벤트 리스너 추가
        if (skillInfoButton != null)
        {
            skillInfoButton.onClick.AddListener(OnSkillInfoButtonClick);
        }

        // 초기 스킬 시간 설정
        skillTimeRemaining = skillDuration;
    }

    private void Update()
    {
        if (onCooldown)
        {
            cooldownRemaining -= Time.deltaTime;
            if (cooldownRemaining < 0) cooldownRemaining = 0;

            UpdateCooldownUI(cooldownRemaining);
        }

        if (skillInfoObject != null && skillInfoObject.activeSelf)
        {
            UpdateSkillInfoUI();
        }

        // 터치 이벤트를 체크하여 skillTextObject 비활성화
        if (skillTextObject != null && Input.GetMouseButtonDown(0) && skillTextObject.activeSelf)
        {
            skillTextObject.SetActive(false);
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
            SetLockImangeOff();
            currentLevel = 1;
            UpdateClickValues();
            UpdateUpgradeCostUI(); // 업그레이드 비용 UI 업데이트
            UpdateUI(); // UI 업데이트
            CheckUnlockStatus(); // 해금 후 상태 확인 및 UI 업데이트
        }
        else
        {
            ShowPopup("Unlock condition not met.");
        }
    }

    public abstract void ActivateSkill();

    protected IEnumerator SkillEffect()
    {
        onCooldown = true;
        cooldownRemaining = cooldownTime;
        skillTimeRemaining = skillDuration;

        // 스킬 정보 오브젝트 활성화
        if (skillInfoObject != null)
        {
            skillInfoObject.SetActive(true);
        }

        // 스킬 지속 시간 동안 대기
        float elapsedSkillTime = 0f;
        while (elapsedSkillTime < skillDuration)
        {
            elapsedSkillTime += Time.deltaTime;
            yield return null;
        }

        // 스킬 지속 시간이 끝났을 때 스킬 정보 오브젝트 비활성화
        if (skillInfoObject != null)
        {
            skillInfoObject.SetActive(false);
        }

        // 쿨다운 시간 동안 대기
        while (cooldownRemaining > 0)
        {
            cooldownRemaining -= Time.deltaTime;
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
            ShowPopup("Maximum level reached.");
            return;
        }
        BigInteger upgradeCost = CalculateUpgradeCost(currentLevel);
        if (LifeManager.Instance.diamond.HasSufficientDiamond(upgradeCost))
        {
            LifeManager.Instance.diamond.DecreaseDiamond(upgradeCost);
            currentLevel++;
            UpdateClickValues(); // 필요 시 업데이트
            UpdateUpgradeCostUI(); // 업그레이드 비용 UI 업데이트
            UpdateUI();
        }
        else
        {
            ShowPopup("Not enough diamonds to upgrade.");
        }
    }

    public BigInteger CalculateUpgradeCost(int level)
    {
        return level * 30; // 업그레이드 비용 (레벨) x 30
    }

    public virtual void UpdateUI()
    {
        adSkill.UnlockCondition();
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
                    ? $" {BigIntegerUtils.FormatBigInteger(nextCost)}"
                    : $"{BigIntegerUtils.FormatBigInteger(nextCost)}";
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
        if (skillPopup != null)
        {
            skillPopup.SetActive(true);
        }
    }

    public void ShowSkillInfoPopup()
    {
        if (skillPopup != null)
        {
            //string currentAbility = GetCurrentAbilityDescription();
            string nextAbility = GetNextAbilityDescription();

            skillPopup.SetActive(true);
        }
    }

    // 현재 능력 설명을 반환하는 함수 (각 스킬에서 오버라이드 필요)
    //public abstract string GetCurrentAbilityDescription();

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
    public void ResetCooldown()
    {
        onCooldown = false;
        cooldownRemaining = 0;
        UpdateCooldownUI(0);
    }
    public void SetCooldown(float cooldownTime)
    {
        this.cooldownTime = cooldownTime;
        onCooldown = true;
        cooldownRemaining = cooldownTime;
        UpdateCooldownUI(cooldownRemaining);
    }

    public void SetLockImangeOff()
    {
        Destroy(useSkillbuttonLockImage.gameObject);
    }

    // 스킬 버튼 클릭 시 호출되는 함수
    private void OnSkillButtonClick()
    {
        if (skillInfoObject != null)
        {
            skillInfoObject.SetActive(true);
            skillTimeRemaining = skillDuration; // 스킬 정보 오브젝트가 활성화될 때 남은 시간 초기화
        }
    }

    // 스킬 정보 오브젝트의 버튼 클릭 시 호출되는 함수
    public void OnSkillInfoButtonClick()
    {
        if (skillTextObject != null)
        {
            skillTextObject.SetActive(true);

            // 이미 코루틴이 실행 중이라면 중지하고 새로 시작
            if (disableCoroutine != null)
            {
                StopCoroutine(disableCoroutine);
            }

            disableCoroutine = StartCoroutine(DisableAfterDelay(skillTextObject, 2.0f)); // 2초 후에 비활성화
        }
    }

    private void UpdateSkillInfoUI()
    {
        if (skillInfoTextField != null)
        {
            int minutes = Mathf.FloorToInt(skillTimeRemaining / 60);
            int seconds = Mathf.CeilToInt(skillTimeRemaining % 60);
            skillInfoTextField.text = $"스킬 이름: {skillName}\n남은 시간: {minutes:D2}:{seconds:D2}초";

            skillTimeRemaining -= Time.deltaTime;
            if (skillTimeRemaining < 0) skillTimeRemaining = 0;
        }

        if (skillDurationImage != null)
        {
            skillDurationImage.fillAmount = skillTimeRemaining / skillDuration;
        }
    }

    private IEnumerator DisableAfterDelay(GameObject target, float delay)
    {
        yield return new WaitForSeconds(delay);

        // 코루틴이 끝나기 전에 타겟이 비활성화 상태가 되었는지 확인
        if (target.activeSelf)
        {
            target.SetActive(false);
        }
        disableCoroutine = null; // 코루틴이 끝난 후 null로 설정
    }
}
