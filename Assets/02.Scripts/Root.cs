using UnityEngine;

public class Root : MonoBehaviour
{
    public LifeManager waterManager;
    public UIManager uiManager;
    public int rootLevel = 1;
    public int baseWaterGeneration = 1;
    public int waterGenerationPerLevel = 1;
    public int upgradeWaterCost = 20;
    public float generationInterval = 60f;
    private float timer;

    public delegate void WaterGenerated(int amount);
    public event WaterGenerated OnWaterGenerated;

    private void Start()
    {
        UpdateUI();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= generationInterval)
        {
            GenerateWater();
            timer = 0f;
        }
    }

    private void GenerateWater()
    {
        int generatedWater = baseWaterGeneration + (rootLevel * waterGenerationPerLevel);
        OnWaterGenerated?.Invoke(generatedWater);
    }

    public int CalculateUpgradeCost()
    {
        return rootLevel * 20;
    }

    public void UpdateUI()
    {
        uiManager.UpdateRootLevelUI(rootLevel, upgradeWaterCost);
    }
}
