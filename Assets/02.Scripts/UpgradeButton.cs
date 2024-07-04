using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour
{
    public LifeManager lifeManager;
    public UIManager uiManager;
    public ResourceManager resourceManager;
    public Root root;
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
                HandleRootUpgrade();
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

    private void HandleRootUpgrade()
    {
        int upgradeCost = root.CalculateUpgradeCost();
        if (lifeManager.HasSufficientWater(upgradeCost))
        {
            lifeManager.DecreaseWater(upgradeCost);
            root.UpgradeLifeGeneration(); // 조건에 따라 자동 생산량 증가
            root.UpdateUI();
            resourceManager.UpdateTotalLifeIncreaseUI(); // 총 생명력 증가량 UI 업데이트
        }
        else
        {
            Debug.Log("물이 부족하여 강화할 수 없습니다.");
        }
    }

    private void HandleSpiritUpgrade()
    {
        int upgradeCost = spirit.CalculateUpgradeCost();
        if (lifeManager.HasSufficientWater(upgradeCost))
        {
            lifeManager.DecreaseWater(upgradeCost);
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
        int upgradelifeCost = touchInputManager.upgradelifeCost;
        if (lifeManager.HasSufficientWater(upgradelifeCost))
        {
            lifeManager.DecreaseWater(upgradelifeCost);
            touchInputManager.touchIncreaseLevel++;
            touchInputManager.touchIncreaseAmount *= 2;
            touchInputManager.upgradelifeCost += 20;
            touchInputManager.UpdateTouchIncreaseUI(); // 터치 생산량 UI 업데이트
            uiManager.UpdateTouchUI(touchInputManager.touchIncreaseLevel, touchInputManager.touchIncreaseAmount, touchInputManager.upgradelifeCost);
        }
        else
        {
            Debug.Log("물이 부족하여 강화할 수 없습니다.");
        }
    }

    private void HandleGeneralUpgrade()
    {
        int waterNeededForUpgrade = lifeManager.CalculateWaterNeededForUpgrade(upgradeAmount);
        if (lifeManager.HasSufficientWater(waterNeededForUpgrade))
        {
            lifeManager.DecreaseWater(waterNeededForUpgrade);
            lifeManager.currentLevel += upgradeAmount;
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
            int upgradeCost = root.upgradeLifeCost;
            uiManager.UpdateRootLevelUI(root.rootLevel, upgradeCost);
        }
        else if (upgradeType == UpgradeType.Spirit && spirit != null)
        {
            int upgradeCost = spirit.upgradeEnergyCost;
            uiManager.UpdateSpiritLevelUI(spirit.spiritLevel, upgradeCost);
        }
        else if (upgradeType == UpgradeType.Touch && touchInputManager != null)
        {
            uiManager.UpdateTouchUI(touchInputManager.touchIncreaseLevel, touchInputManager.touchIncreaseAmount, touchInputManager.upgradelifeCost);
        }
        else if (upgradeType == UpgradeType.Tree && resourceManager != null)
        {
            int waterNeededForCurrentLevel = resourceManager.lifeManager.CalculateWaterNeededForUpgrade(1);
            // uiManager.UpdateUpgradeRequirementUI(resourceManager.lifeManager.currentLevel, waterNeededForCurrentLevel);
        }
    }
}
