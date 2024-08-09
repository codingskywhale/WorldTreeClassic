using System.Numerics;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public abstract class Artifact : MonoBehaviour
{
    public string artifactName; // 스킬 이름
    public int currentLevel = 0; // 현재 스킬 레벨 (0 = 잠금 상태)
    public TextMeshProUGUI upgradeCostText; // 업그레이드 비용을 표시할 텍스트
    public TextMeshProUGUI currentLevelText; // 현재 스킬 레벨 텍스트
    public TextMeshProUGUI skillInfoText; // 현재 스킬 설명 텍스트
    public BigInteger unlockCost = 100; // 해금 비용
    //public Button upgradeButton; // 해금/업그레이드 버튼
    public GameObject unlockObject; // 해금시 활성화 오브젝트
    public GameObject skillPopup; // 팝업 오브젝트
    public TextMeshProUGUI skillPopupInfoText; // 팝업에 표시될 텍스트
    public TextMeshProUGUI skillPopupCostText; // 팝업 비용 텍스트
    public Sprite artifactImage; // 아티팩트 이미지를 저장할 필드

    protected virtual void Start()
    {
        UpdateUI(); // 초기 UI 설정
        //CheckUnlockStatus(); // 해금 상태 확인 및 UI 업데이트

        // 팝업 초기 상태 비활성화
        if (skillPopup != null)
        {
            skillPopup.SetActive(false);
        }

        //if (upgradeButton != null)
        //{
        //    upgradeButton.onClick.AddListener(OnUpgradeButtonClicked);
        //}
    }

    public void UnlockOrUpgradeSkill()
    {
        if (currentLevel == 0)
        {
            UnlockArtifact();
        }
        else
        {
            ShowSkillInfoPopup();
        }
    }

    private void UnlockArtifact()
    {
        if (DiamondManager.Instance.HasSufficientDiamond(unlockCost))
        {
            DiamondManager.Instance.DecreaseDiamond(unlockCost);
            currentLevel = 1;
            ActiveObject();
            UpdateUpgradeCostUI(); // 업그레이드 비용 UI 업데이트
            UpdateUI(); // UI 업데이트
            //CheckUnlockStatus(); // 해금 후 상태 확인 및 UI 업데이트
        }
        else
        {
            ShowPopup("Not enough diamonds to unlock.");
        }
    }

    private void UpgradeArtifact()
    {
        if (currentLevel >= 10)
        {
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
            ShowPopup("Not enough diamonds to upgrade.");
        }
    }

    public BigInteger CalculateUpgradeCost(int level)
    {
        return unlockCost + (level * 10);
    }

    public void ActiveObject()
    {
        unlockObject.SetActive(true);
    }
        
    public void UpdateUI()
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
                    ? $"{BigIntegerUtils.FormatBigInteger(nextCost)}"
                    : $"{BigIntegerUtils.FormatBigInteger(nextCost)}";
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

    protected abstract void UpdateSkillInfoUI();
    protected abstract void UpdateClickValues();

    //private void CheckUnlockStatus()
    //{
    //    if (upgradeButton != null)
    //    {
    //        if (currentLevel >= 10)
    //        {
    //            upgradeButton.interactable = false;
    //        }
    //        else
    //        {
    //            bool hasSufficientDiamond = currentLevel == 0
    //                ? DiamondManager.Instance.HasSufficientDiamond(unlockCost)
    //                : DiamondManager.Instance.HasSufficientDiamond(CalculateUpgradeCost(currentLevel));
    //            upgradeButton.interactable = hasSufficientDiamond;
    //        }
    //    }
    //}

    // 팝업을 띄우는 함수
    public void ShowPopup(string message)
    {
        if (skillPopup != null && skillPopupInfoText != null)
        {
            skillPopupInfoText.text = message;
            skillPopup.SetActive(true);
        }
    }

    // 스킬 정보 팝업을 띄우는 함수
    public void ShowSkillInfoPopup()
    {
        if (skillPopup != null && skillPopupInfoText != null)
        {
           //string currentAbility = GetCurrentAbilityDescription();
            string nextAbility = GetNextAbilityDescription();

            skillPopupInfoText.text = $"아티팩트 이름: {artifactName}\n레벨업 시 능력: {nextAbility}";
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
        UpgradeArtifact();
        ActiveObject();
        ClosePopup();
    }
}
