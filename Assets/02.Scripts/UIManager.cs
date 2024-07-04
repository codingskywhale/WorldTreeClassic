using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI waterText;
    public TextMeshProUGUI lifeIncreaseText;
    public TextMeshProUGUI touchLevelText;
    public TextMeshProUGUI touchIncreaseText; // 추가된 텍스트 UI 요소
    public TextMeshProUGUI upgradelifeCostText;
    public TextMeshProUGUI rootLevelText;
    public TextMeshProUGUI rootUpgradeCostText;
    public TextMeshProUGUI spiritLevelText;
    public TextMeshProUGUI spiritUpgradeCostText;
    public Image currentTreeImage;
    public Image upgradedTreeImage;
    public SpriteRenderer outsideTreeSpriteRenderer;
    public TextMeshProUGUI upgradeRequirementText;
    public SpriteRenderer groundSpriteRenderer;
    public Sprite[] treeImages;
    

    public void UpdateLifeUI(int waterAmount, int waterNeededForCurrentLevel)
    {
        waterText.text = $" 생명력 : {waterAmount}";
    }

    public void UpdateTreeImages(int currentLevel, Sprite[] treeImages)
    {
        int currentIndex = currentLevel / 5;
        int nextIndex = (currentLevel + 1) / 5;
        currentTreeImage.sprite = treeImages[currentIndex];
        upgradedTreeImage.sprite = treeImages[nextIndex];
        outsideTreeSpriteRenderer.sprite = treeImages[currentIndex];
    }

    public void UpdateLifeIncreaseUI(int totalLifeIncrease)
    {
        lifeIncreaseText.text = "Life Increase Per Second: " + totalLifeIncrease.ToString();
    }

    public void UpdateTouchUI(int touchIncreaseLevel, int touchIncreaseAmount, int upgradelifeCost)
    {
        touchLevelText.text = $"외로운 나무 레벨: {touchIncreaseLevel}";
        upgradelifeCostText.text = $"강화 비용: {upgradelifeCost} 생명력";
    }

    public void UpdateRootLevelUI(int rootLevel, int upgradeCost)
    {
        rootLevelText.text = $"뿌리 레벨: {rootLevel}";
        rootUpgradeCostText.text = $"강화 비용: {upgradeCost} 물";
    }

    public void UpdateSpiritLevelUI(int spiritLevel, int upgradeCost)
    {
        spiritLevelText.text = $"정령 레벨: {spiritLevel}";
        spiritUpgradeCostText.text = $"강화 비용: {upgradeCost} 에너지";
    }

    public void UpdateTouchIncreaseUI(int touchIncreaseAmount)
    {
        touchIncreaseText.text = $"터치 생산량: {touchIncreaseAmount}";
    }
}
