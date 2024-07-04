using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public LifeManager waterManager;
    public Root root;
    public UIManager uiManager;
    //public EnergyManager energyManager;

    private void Start()
    {
        waterManager.OnWaterChanged += UpdateWaterUI;
        root.OnWaterGenerated += waterManager.IncreaseWater;
       // energyManager.OnEnergyChanged += UpdateEnergyUI;
        UpdateUI();
    }

    public void UpdateGroundSize()
    {
        float groundScale = 8f + (waterManager.currentLevel / 10f);
        uiManager.groundSpriteRenderer.transform.localScale = new Vector3(groundScale, groundScale, groundScale);
    }

    public void UpdateUI()
    {
        int waterNeededForCurrentLevel = waterManager.CalculateWaterNeededForUpgrade(1);
        uiManager.UpdateWaterUI(waterManager.lifeAmount, waterNeededForCurrentLevel);
        uiManager.UpdateLevelUI(waterManager.currentLevel);
        uiManager.UpdateUpgradeRequirementUI(waterManager.currentLevel, waterNeededForCurrentLevel);
        uiManager.UpdateTreeImages(waterManager.currentLevel, uiManager.treeImages);
        UpdateRootUI();
        //UpdateEnergyUI(energyManager.energyAmount);
    }

    private void UpdateWaterUI(int newWaterAmount)
    {
        int waterNeededForCurrentLevel = waterManager.CalculateWaterNeededForUpgrade(1);
        uiManager.UpdateWaterUI(newWaterAmount, waterNeededForCurrentLevel);
    }

    private void UpdateRootUI()
    {
        int rootUpgradeCost = root.CalculateUpgradeCost();
        uiManager.UpdateRootLevelUI(root.rootLevel, rootUpgradeCost);
    }

    private void UpdateEnergyUI(int newEnergyAmount)
    {
        //uiManager.UpdateEnergyUI(newEnergyAmount, energyManager.maxEnergy);
    }
}


