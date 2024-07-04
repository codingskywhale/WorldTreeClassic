using UnityEngine;

public class Spirit : MonoBehaviour
{
    public LifeManager waterManager;
    public UIManager uiManager;
    public int spiritLevel = 1;
    public int baseEnergyGeneration = 1;
    public int energyGenerationPerLevel = 1;
    public int upgradeEnergyCost = 20;
    public float generationInterval = 60f;
    private float timer;

    public delegate void EnergyGenerated(int amount);
    public event EnergyGenerated OnEnergyGenerated;

    public delegate void SpiritLevelChanged(int newLevel);
    public event SpiritLevelChanged OnSpiritLevelChanged;

    private void Start()
    {
        UpdateUI();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= generationInterval)
        {
            GenerateEnergy();
            timer = 0f;
        }
    }

    private void GenerateEnergy()
    {
        int generatedEnergy = baseEnergyGeneration + (spiritLevel * energyGenerationPerLevel);
        OnEnergyGenerated?.Invoke(generatedEnergy);
    }

    public int CalculateUpgradeCost()
    {
        return spiritLevel * 20;
    }

    public void UpdateUI()
    {
        uiManager.UpdateSpiritLevelUI(spiritLevel, upgradeEnergyCost);
    }
}
