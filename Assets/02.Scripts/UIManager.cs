using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI waterText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI touchLevelText;
    public TextMeshProUGUI moveSpeedLevelText;
    public TextMeshProUGUI moveSpeedUpgradeCostText;
    public TextMeshProUGUI waterIncreaseLevelText;
    public TextMeshProUGUI waterIncreaseUpgradeCostText;
    public TextMeshProUGUI upgradeWaterCostText;
    public TextMeshProUGUI energyText;
    public TextMeshProUGUI rootLevelText;
    public TextMeshProUGUI rootUpgradeCostText;
    public TextMeshProUGUI spiritLevelText;
    public TextMeshProUGUI spiritUpgradeCostText;
    public Image levelFillImage;
    public Image currentTreeImage;
    public Image upgradedTreeImage;
    public SpriteRenderer outsideTreeSpriteRenderer;
    public TextMeshProUGUI upgradeRequirementText;
    public SpriteRenderer groundSpriteRenderer;
    public Sprite[] treeImages;

    public void UpdateWaterUI(int waterAmount, int waterNeededForCurrentLevel)
    {
        waterText.text = $"물 : {waterAmount}";
        levelFillImage.fillAmount = (float)waterAmount / waterNeededForCurrentLevel;
    }

    public void UpdateEnergyUI(int energyAmount, int maxEnergy)
    {
        energyText.text = $"에너지 : {energyAmount}/{maxEnergy}";
    }

    public void UpdateLevelUI(int currentLevel)
    {
        levelText.text = $"Level: {currentLevel}";
    }

    public void UpdateUpgradeRequirementUI(int currentLevel, int waterNeededForCurrentLevel)
    {
        if (currentLevel % 5 == 4)
        {
            upgradeRequirementText.text = "성장";
        }
        else
        {
            upgradeRequirementText.text = $"필요한 재화: {waterNeededForCurrentLevel} 물";
        }
    }

    public void UpdateTreeImages(int currentLevel, Sprite[] treeImages)
    {
        int currentIndex = currentLevel / 5;
        int nextIndex = (currentLevel + 1) / 5;
        currentTreeImage.sprite = treeImages[currentIndex];
        upgradedTreeImage.sprite = treeImages[nextIndex];
        outsideTreeSpriteRenderer.sprite = treeImages[currentIndex];
    }

    public void UpdateTouchUI(int touchIncreaseLevel, int touchIncreaseAmount, int upgradeWaterCost)
    {
        touchLevelText.text = $"터치 강화 레벨: {touchIncreaseLevel}";
        upgradeWaterCostText.text = $"강화 비용: {upgradeWaterCost} 물";
    }

    public void UpdateMovementUI(int moveSpeedLevel, int moveUpgradeCost)
    {
        moveSpeedLevelText.text = $"주인공 스피드 강화 레벨 : {moveSpeedLevel}";
        moveSpeedUpgradeCostText.text = $"강화 비용 : {moveUpgradeCost}";
    }

    public void UpdateWaterIncreaseUI(int waterIncreaseLevel, int waterIncreaseAmount, int waterIncreaseUpgradeCost)
    {
        waterIncreaseLevelText.text = $"물 증가량 강화 레벨: {waterIncreaseLevel}";
        waterIncreaseUpgradeCostText.text = $"강화 비용: {waterIncreaseUpgradeCost} 물";
    }

    public void UpdateRootLevelUI(int rootLevel, int upgradeCost)
    {
        rootLevelText.text = $"뿌리 레벨: {rootLevel}";
        rootUpgradeCostText.text = $"강화 비용: {upgradeCost} 물";
    }

    public void UpdateSpiritLevelUI(int spiritLevel, int upgradeCost)
    {
        // 적절한 UI 요소 업데이트
        spiritLevelText.text = $"정령 레벨: {spiritLevel}";
        spiritUpgradeCostText.text = $"강화 비용: {upgradeCost} 에너지";
    }

}
