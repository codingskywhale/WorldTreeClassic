using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class UIUpdater
{
    private ResourceManager resourceManager;
    private List<UpgradeButton> upgradeButtons;

    public UIUpdater(ResourceManager resourceManager, List<UpgradeButton> upgradeButtons)
    {
        this.resourceManager = resourceManager;
        this.upgradeButtons = upgradeButtons;
    }

    public void UpdateAllUI()
    {
        DataManager.Instance.touchData.UpdateUI();
        UIManager.Instance.status.UpdateLifeUI(LifeManager.Instance.lifeAmount, LifeManager.Instance.CalculateWaterNeededForUpgrade(1));
        UIManager.Instance.status.UpdateAnimalCountText(DataManager.Instance.animalGenerateData.nowAnimalCount, DataManager.Instance.animalGenerateData.maxAnimalCount);

        foreach (var root in resourceManager.roots)
        {
            if (root.isUnlocked)
            {
                Debug.Log($"Updating UI for unlocked root: Level={root.rootLevel}, UpgradeCost={root.upgradeLifeCost}");
                root.UpdateUI();
            }
        }

        foreach (var button in upgradeButtons)
        {
            if (button.upgradeType == UpgradeButton.UpgradeType.Root)
            {
                button.UpdateUpgradeCostUI(button.root.rootLevel);
            }
            else if (button.upgradeType == UpgradeButton.UpgradeType.Tree)
            {
                button.UpdateUpgradeCostUI(LifeManager.Instance.currentLevel);
            }
            else if (button.upgradeType == UpgradeButton.UpgradeType.Touch)
            {
                button.UpdateUpgradeCostUI(DataManager.Instance.touchData.touchIncreaseLevel);
            }
        }

        foreach (var root in resourceManager.roots)
        {
            if (root.isUnlocked)
            {
                root.UpdateUI();
            }
            else
            {
                root.UpdateRootLevelUI(0, root.unlockCost);
            }
        }
    }

    private BigInteger CalculateTotalLifeIncrease(List<RootBase> roots)
    {
        BigInteger totalLifeIncrease = 0;
        foreach (var root in roots)
        {
            if (root.isUnlocked)
            {
                totalLifeIncrease += root.GetTotalLifeGeneration();
            }
        }
        return totalLifeIncrease;
    }
}