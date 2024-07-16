using System.Numerics;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour
{
    public ResourceManager resourceManager;
    public RootBase root;
    //public TouchInputManager touchInputManager;

    public enum UpgradeType
    {
        Root,
        Touch,
        Tree
    }

    public UpgradeType upgradeType;
    public int upgradeAmount = 1;

    private Button upgradeButton;

    private void Start()
    {
        upgradeButton = GetComponent<Button>();
        upgradeButton.onClick.AddListener(OnUpgradeButtonClicked);
        resourceManager.UpdateUI();

        if (upgradeType == UpgradeType.Root && root != null)
        {
            UpdateUpgradeCostUI(root.rootLevel);
        }
        else if (upgradeType == UpgradeType.Tree && resourceManager != null)
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
            case UpgradeType.Root:
                canUpgrade = root != null && (!root.isUnlocked &&
                                              DataManager.Instance.touchData.touchIncreaseLevel >= root.unlockThreshold && 
                                              LifeManager.Instance.HasSufficientWater(root.CalculateUpgradeCost())) || 
                                              (root.isUnlocked && LifeManager.Instance.HasSufficientWater(root.CalculateUpgradeCost()));
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

    private void OnUpgradeButtonClicked()
    {
        switch (upgradeType)
        {
            case UpgradeType.Root:
                HandleRootUpgradeOrUnlock();
                break;
            case UpgradeType.Touch:
                HandleTouchUpgrade();
                break;
            case UpgradeType.Tree:
                HandleGeneralUpgrade();
                break;
        }
    }

    private void HandleRootUpgradeOrUnlock()
    {
        if (root == null) return;

        if (!root.isUnlocked)
        {
            HandleRootUnlock();
        }
        else
        {
            HandleRootUpgrade();
        }
    }

    private void HandleRootUnlock()
    {
        if (root == null || root.isUnlocked) return;

        BigInteger unlockCost = root.CalculateUpgradeCost();
        if (LifeManager.Instance.HasSufficientWater(unlockCost))
        {
            LifeManager.Instance.DecreaseWater(unlockCost);
            root.Unlock();
            root.UpdateUI();
            resourceManager.UpdateUI();
            resourceManager.GetTotalLifeGenerationPerSecond();
            DataManager.Instance.animalGenerateData.maxAnimalCount += 5;
            UIManager.Instance.SetAnimalCountStatus();
        }
        else
        {
            Debug.Log("물이 부족하여 해금할 수 없습니다.");
        }
    }

    private void HandleRootUpgrade()
    {
        if (root == null || !root.isUnlocked) return;

        BigInteger upgradeCost = root.CalculateUpgradeCost();
        if (LifeManager.Instance.HasSufficientWater(upgradeCost))
        {
            LifeManager.Instance.DecreaseWater(upgradeCost);
            root.UpgradeLifeGeneration();
            root.UpdateUI();
            resourceManager.UpdateLifeGenerationRatePerSecond();  // 초당 생명력 생성률 업데이트
            resourceManager.UpdateUI();
            resourceManager.GetTotalLifeGenerationPerSecond();        
        }
        else
        {
            Debug.Log("물이 부족하여 강화할 수 없습니다.");
        }
    }

    private void HandleTouchUpgrade()
    {
        TouchData touchData = DataManager.Instance.touchData;

        BigInteger upgradeLifeCost = DataManager.Instance.touchData.upgradeLifeCost;
        if (LifeManager.Instance.HasSufficientWater(upgradeLifeCost))
        {
            LifeManager.Instance.DecreaseWater(upgradeLifeCost);
            DataManager.Instance.touchData.UpgradeTouchGeneration();
            UIManager.Instance.touchData.UpdateTouchUI(touchData.touchIncreaseLevel, touchData.touchIncreaseAmount, touchData.upgradeLifeCost);
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
            resourceManager.UpdateGroundSize();
        }
        else
        {
            Debug.Log("물이 부족하여 강화할 수 없습니다.");
        }
    }

    public void UpdateUpgradeCostUI(int newLevel)
    { // 저장 때문에 퍼블릭으로 수정!!
        if (upgradeType == UpgradeType.Root && root != null)
        {
            BigInteger upgradeCost = root.CalculateUpgradeCost();
            Debug.Log($"UpdateUpgradeCostUI called for root level {this.name}, {root.rootLevel}, upgrade cost {upgradeCost}");
            //UIManager.Instance.root.UpdateRootLevelUI(root.rootLevel, upgradeCost);
            root.UpdateRootLevelUI(root.rootLevel, upgradeCost);
        }
        else if (upgradeType == UpgradeType.Touch) //&& touchInputManager != null
        {
            UIManager.Instance.touchData.UpdateTouchUI(DataManager.Instance.touchData.touchIncreaseLevel,
                                    DataManager.Instance.touchData.touchIncreaseAmount,
                                    DataManager.Instance.touchData.upgradeLifeCost);
        }
        else if (upgradeType == UpgradeType.Tree && resourceManager != null)
        {
            BigInteger waterNeededForCurrentLevel = LifeManager.Instance.CalculateWaterNeededForUpgrade(1);
            // uiManager.UpdateUpgradeRequirementUI(resourceManager.lifeManager.currentLevel, waterNeededForCurrentLevel);
        }
    }
}