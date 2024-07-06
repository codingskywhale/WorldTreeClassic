using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour
{
    public ResourceManager resourceManager;
    public RootBase root;
    public Spirit spirit;
    public TouchInputManager touchInputManager;

    public enum UpgradeType
    {
        Root,
        Spirit,
        Touch,
        Tree,
        MoveSpeed,
        WaterIncrease
    }

    public UpgradeType upgradeType;
    public int upgradeAmount = 1;

    private Button upgradeButton;

    private void Start()
    {
        upgradeButton = GetComponent<Button>();
        upgradeButton.onClick.AddListener(OnUpgradeButtonClicked);

        if (upgradeType == UpgradeType.Root && root != null)
        {
            UpdateUpgradeCostUI(root.rootLevel);
        }
        else if (upgradeType == UpgradeType.Spirit && spirit != null)
        {
            UpdateUpgradeCostUI(spirit.spiritLevel);
        }
        else if (upgradeType == UpgradeType.Tree && resourceManager != null)
        {
            UpdateUpgradeCostUI(resourceManager.lifeManager.currentLevel);
        }
    }

    private void OnUpgradeButtonClicked()
    {
        switch (upgradeType)
        {
            case UpgradeType.Root:
                HandleRootUpgradeOrUnlock();
                break;
            case UpgradeType.Spirit:
                HandleSpiritUpgrade();
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
        int unlockCost = root.CalculateUpgradeCost(); // 해금 비용을 계산 (처음 업그레이드 비용)
        if (LifeManager.Instance.HasSufficientWater(unlockCost))
        {
            LifeManager.Instance.DecreaseWater(unlockCost);
            root.Unlock();
            root.UpdateUI();
            resourceManager.UpdateTotalLifeIncreaseUI();
        }
        else
        {
            Debug.Log("물이 부족하여 해금할 수 없습니다.");
        }
    }

    private void HandleRootUpgrade()
    {
        int upgradeCost = root.CalculateUpgradeCost();
        if (LifeManager.Instance.HasSufficientWater(upgradeCost))
        {
            LifeManager.Instance.DecreaseWater(upgradeCost);
            root.UpgradeLifeGeneration();
            root.UpdateUI();
            resourceManager.UpdateTotalLifeIncreaseUI();
        }
        else
        {
            Debug.Log("물이 부족하여 강화할 수 없습니다.");
        }
    }

    private void HandleSpiritUpgrade()
    {
        int upgradeCost = spirit.CalculateUpgradeCost();
        if (LifeManager.Instance.HasSufficientWater(upgradeCost))
        {
            LifeManager.Instance.DecreaseWater(upgradeCost);
            spirit.spiritLevel++;
            spirit.upgradeEnergyCost *= 2;
            spirit.UpdateUI();
        }
        else
        {
            Debug.Log("에너지가 부족하여 강화할 수 없습니다.");
        }
    }

    private void HandleTouchUpgrade()
    {
        TouchData touchData = LifeManager.Instance.touchData;

        int upgradeLifeCost = LifeManager.Instance.touchData.upgradeLifeCost;
        if (LifeManager.Instance.HasSufficientWater(upgradeLifeCost))
        {
            LifeManager.Instance.DecreaseWater(upgradeLifeCost);
            LifeManager.Instance.touchData.UpgradeTouchGeneration();

            UIManager.Instance.touchData.UpdateTouchUI(touchData.touchIncreaseLevel, touchData.touchIncreaseAmount, touchData.upgradeLifeCost);
        }
        else
        {
            Debug.Log("물이 부족하여 강화할 수 없습니다.");
        }
    }

    private void HandleGeneralUpgrade()
    {
        int waterNeededForUpgrade = LifeManager.Instance.CalculateWaterNeededForUpgrade(upgradeAmount);
        if (LifeManager.Instance.HasSufficientWater(waterNeededForUpgrade))
        {
            LifeManager.Instance.DecreaseWater(waterNeededForUpgrade);
            LifeManager.Instance.currentLevel += upgradeAmount;
            resourceManager.UpdateUI();
            resourceManager.UpdateGroundSize();
        }
        else
        {
            Debug.Log("물이 부족하여 강화할 수 없습니다.");
        }
    }

    private void UpdateUpgradeCostUI(int newLevel)
    {
        if (upgradeType == UpgradeType.Root && root != null)
        {
            int upgradeCost = root.CalculateUpgradeCost();
            UIManager.Instance.root.UpdateRootLevelUI(root.rootLevel, upgradeCost);
        }
        else if (upgradeType == UpgradeType.Spirit && spirit != null)
        {
            int upgradeCost = spirit.upgradeEnergyCost;
            UIManager.Instance.spiritData.UpdateSpiritLevelUI(spirit.spiritLevel, upgradeCost);
        }
        else if (upgradeType == UpgradeType.Touch && touchInputManager != null)
        {
            UIManager.Instance.touchData.UpdateTouchUI(LifeManager.Instance.touchData.touchIncreaseLevel,
                                    LifeManager.Instance.touchData.touchIncreaseAmount,
                                    LifeManager.Instance.touchData.upgradeLifeCost);
        }
        else if (upgradeType == UpgradeType.Tree && resourceManager != null)
        {
            int waterNeededForCurrentLevel = resourceManager.lifeManager.CalculateWaterNeededForUpgrade(1);
            // uiManager.UpdateUpgradeRequirementUI(resourceManager.lifeManager.currentLevel, waterNeededForCurrentLevel);
        }
    }
}
