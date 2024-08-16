using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour
{
    public FlowerBase flower;
    public bool isTutorial = false;

    [Header("MultiUpgrade")]
    public Button x10Button;
    public Button x100Button;
    public TextMeshProUGUI x10Text;
    public TextMeshProUGUI x100Text;
    //public TouchInputManager touchInputManager;
    private int buttonIdx;

    public enum UpgradeType
    {
        Flower,
        Touch,
        Tree
    }

    public UpgradeType upgradeType;
    public int upgradeAmount = 1;

    private Button upgradeButton;

    private void Start()
    {
        upgradeButton = GetComponent<Button>();
        if (!isTutorial)
            upgradeButton.onClick.AddListener(OnUpgradeButtonClicked);
        ResourceManager.Instance.UpdateUI();

        if (upgradeType == UpgradeType.Flower && flower != null)
        {
            UpdateUpgradeCostUI(flower.flowerLevel);
        }
        else if (upgradeType == UpgradeType.Tree && ResourceManager.Instance != null)
        {
            UpdateUpgradeCostUI(LifeManager.Instance.currentLevel);
        }
    }

    private void Update()
    {
        UpdateButtonInteractable();
    }

    private void UpdateButtonInteractable()
    {
        bool canUpgrade = false;

        switch (upgradeType)
        {
            case UpgradeType.Flower:
                canUpgrade = flower != null && (!flower.isUnlocked &&
                                              DataManager.Instance.touchData.touchIncreaseLevel >= flower.unlockThreshold &&
                                              LifeManager.Instance.HasSufficientWater(flower.CalculateUpgradeCost())) ||
                                              (flower.isUnlocked && LifeManager.Instance.HasSufficientWater(flower.CalculateUpgradeCost()));
                break;
            case UpgradeType.Touch:
                canUpgrade = LifeManager.Instance.HasSufficientWater(DataManager.Instance.touchData.upgradeLifeCost);
                break;
            case UpgradeType.Tree:
                canUpgrade = LifeManager.Instance.HasSufficientWater(LifeManager.Instance.CalculateWaterNeededForUpgrade(upgradeAmount));
                break;
        }

        upgradeButton.interactable = canUpgrade;
    }

    public void OnUpgradeButtonClicked()
    {
        switch (upgradeType)
        {
            case UpgradeType.Flower:
                HandleFlowerUpgradeOrUnlock();
                break;
            case UpgradeType.Touch:
                HandleTouchUpgrade();
                break;
            case UpgradeType.Tree:
                HandleGeneralUpgrade();
                break;
        }
    }

    private void HandleFlowerUpgradeOrUnlock()
    {
        if (flower == null) return;

        //if (!flower.isUnlocked)
        //{
        //    //HandleFlowerUnlock();
        //}
        //else
        //{
        HandleFlowerUpgrade();
        //}
    }

    //private void HandleFlowerUnlock()
    //{
    //    if (flower == null || flower.isUnlocked) return;

    //    BigInteger unlockCost = flower.unlockCost; // CalculateUpgradeCost가 아닌 unlockCost 사용
    //    Debug.Log($"HandleFlowerUnlock - Unlock Cost: {unlockCost}, Current Water: {LifeManager.Instance.lifeAmount}");
    //    if (LifeManager.Instance.HasSufficientWater(unlockCost))
    //    {
    //        LifeManager.Instance.DecreaseWater(unlockCost);
    //        flower.Unlock();
    //        resourceManager.UpdateLifeGenerationRatePerSecond();  // 초당 생명력 생성률 업데이트
    //        flower.UpdateUI();
    //        resourceManager.UpdateUI();
    //        resourceManager.GetTotalLifeGenerationPerSecond();
    //        DataManager.Instance.animalGenerateData.maxAnimalCount += 5;
    //        UIManager.Instance.SetAnimalCountStatus();
    //    }
    //    else
    //    {
    //        Debug.Log("물이 부족하여 해금할 수 없습니다.");
    //    }
    //}

    public void HandleFlowerUpgrade()
    {
        if (flower == null || !flower.isUnlocked) return;

        BigInteger upgradeCost = flower.CalculateUpgradeCost();
        if (LifeManager.Instance.HasSufficientWater(upgradeCost))
        {
            LifeManager.Instance.DecreaseWater(upgradeCost);
            flower.UpgradeLifeGeneration();
            flower.UpdateUI();
            ResourceManager.Instance.UpdateLifeGenerationRatePerSecond();  // 초당 생명력 생성률 업데이트
            ResourceManager.Instance.UpdateUI();
            ResourceManager.Instance.GetTotalLifeGenerationPerSecond();
        }
        else
        {
            Debug.Log("물이 부족하여 강화할 수 없습니다.");
        }
    }

    public void HandleTouchUpgrade()
    {
        TouchData touchData = DataManager.Instance.touchData;
        if (touchData == null)
        {
            Debug.LogError("touchData is null in HandleTouchUpgrade.");
            return;
        }
        BigInteger upgradeLifeCost = DataManager.Instance.touchData.upgradeLifeCost;
        if (LifeManager.Instance.HasSufficientWater(upgradeLifeCost))
        {
            LifeManager.Instance.DecreaseWater(upgradeLifeCost);
            DataManager.Instance.touchData.UpgradeTouchGeneration();
            UIManager.Instance.touchData.UpdateTouchUI(touchData.touchIncreaseLevel, touchData.touchIncreaseAmount,
                                                        touchData.upgradeLifeCost);
            UIManager.Instance.CheckConditionCleared();
        }
        else
        {
            Debug.Log("물이 부족하여 강화할 수 없습니다.");
        }
    }

    private void HandleGeneralUpgrade()
    {
        BigInteger waterNeededForUpgrade = LifeManager.Instance.CalculateWaterNeededForUpgrade(upgradeAmount);
        if (LifeManager.Instance.HasSufficientWater(waterNeededForUpgrade))
        {
            LifeManager.Instance.DecreaseWater(waterNeededForUpgrade);
            LifeManager.Instance.currentLevel += upgradeAmount;
            ResourceManager.Instance.UpdateGroundSize();
        }
        else
        {
            Debug.Log("물이 부족하여 강화할 수 없습니다.");
        }
    }

    public void UpdateUpgradeCostUI(int newLevel)
    { // 저장 때문에 퍼블릭으로 수정!!
        if (upgradeType == UpgradeType.Flower && flower != null)
        {
            BigInteger upgradeCost = flower.CalculateUpgradeCost();
            //Debug.Log($"UpdateUpgradeCostUI called for flower level {this.name}, {flower.flowerLevel}, upgrade cost {upgradeCost}");
            //UIManager.Instance.flower.UpdateFlowerLevelUI(flower.flowerLevel, upgradeCost);
            flower.UpdateFlowerLevelUI(flower.flowerLevel, upgradeCost);
        }
        else if (upgradeType == UpgradeType.Touch) //&& touchInputManager != null
        {
            UIManager.Instance.touchData.UpdateTouchUI(DataManager.Instance.touchData.touchIncreaseLevel,
                                    DataManager.Instance.touchData.touchIncreaseAmount,
                                    DataManager.Instance.touchData.upgradeLifeCost);
        }
        else if (upgradeType == UpgradeType.Tree && ResourceManager.Instance != null)
        {
            BigInteger waterNeededForCurrentLevel = LifeManager.Instance.CalculateWaterNeededForUpgrade(1);
            // uiManager.UpdateUpgradeRequirementUI(resourceManager.lifeManager.currentLevel, waterNeededForCurrentLevel);
        }
    }

    public void SetButtonIndex(int buttonIdx)
    {
        this.buttonIdx = buttonIdx;
    }

    public void SetMultiUpgradeButton()
    {
        int maxUpgradeCount = GetMaxUpgradeCount();

        x10Button.gameObject.SetActive(maxUpgradeCount > 1);
        x100Button.gameObject.SetActive(maxUpgradeCount > 10);
    }

    public void SetMultiTreeUpgradeText()
    {
        int maxUpgradeCount = Mathf.Min(100, GetMaxUpgradeCount());
        x10Text.text = Mathf.Min(maxUpgradeCount, 10).ToString();
        x100Text.text = maxUpgradeCount.ToString();
    }

    private int GetMaxUpgradeCount()
    {
        int count = 0;
        BigInteger cost = upgradeType == UpgradeType.Touch ? DataManager.Instance.touchData.upgradeLifeCost : AutoObjectManager.Instance.flowers[buttonIdx].GetTotalLifeGeneration();
        BigInteger totalCost = cost;

        while (LifeManager.Instance.lifeAmount >= totalCost)
        {
            count++;
            cost = cost * 120 / 100;
            totalCost += cost;
        }

        return count;
    }
}