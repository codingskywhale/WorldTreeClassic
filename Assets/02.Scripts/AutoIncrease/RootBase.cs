using TMPro;
using UnityEngine;

public interface IRoot
{
    void ApplyIncreaseRate(float rate);
    float GetTotalLifeGeneration();
}

public class RootBase : MonoBehaviour, IRoot
{
    public int rootLevel = 1;
    public float baseLifeGeneration = 1;
    public int lifeGenerationPerLevel = 1;
    public int upgradeLifeCost = 20;
    public float generationInterval = 1f;
    public TextMeshProUGUI rootLevelText;
    public TextMeshProUGUI rootUpgradeCostText;
    public TextMeshProUGUI generationRateText; // 생산률을 나타내는 텍스트 추가

    private float timer;

    public delegate void LifeGenerated(float amount);
    protected event LifeGenerated OnLifeGenerated;
    public event System.Action OnGenerationRateChanged;

    protected virtual void Start()
    {
        OnLifeGenerated -= LifeManager.Instance.IncreaseWater;
        OnLifeGenerated += LifeManager.Instance.IncreaseWater;
        UpdateUI();
    }

    protected virtual void Update()
    {
        timer += Time.deltaTime;
        if (timer >= generationInterval)
        {
            GenerateLife();
            timer = 0f;
        }
    }

    protected virtual void GenerateLife()
    {
        float generatedLife = GetTotalLifeGeneration();
        InvokeLifeGenerated(generatedLife);
    }

    protected void InvokeLifeGenerated(float amount)
    {
        OnLifeGenerated?.Invoke(amount);
    }

    public int CalculateUpgradeCost()
    {
        return rootLevel * 20;
    }

    public virtual void UpgradeLifeGeneration()
    {
        rootLevel++;
        if (rootLevel % 25 == 0)
        {
            baseLifeGeneration *= 2;
        }
        else
        {
            baseLifeGeneration += lifeGenerationPerLevel;
        }
        upgradeLifeCost += 20;
        OnGenerationRateChanged?.Invoke();
        UpdateUI();
    }

    public virtual void UpdateUI()
    {
        UpdateRootLevelUI(rootLevel, upgradeLifeCost);
        UpdateGenerationRateUI(GetTotalLifeGeneration()); // 생산률 업데이트 추가
    }

    public virtual void ApplyIncreaseRate(float rate)
    {
        baseLifeGeneration *= 1 + rate;
        OnGenerationRateChanged?.Invoke();
        UpdateUI();
    }

    public virtual void UpdateRootLevelUI(int rootLevel, int upgradeCost)
    {
        rootLevelText.text = $"뿌리 레벨: {rootLevel}";
        rootUpgradeCostText.text = $"강화 비용: {upgradeCost} 물";
    }

    public virtual void UpdateGenerationRateUI(float generationRate)
    {
        if (generationRateText != null)
        {
            generationRateText.text = $"생산률: {generationRate} 물/초";
        }
    }

    public virtual float GetTotalLifeGeneration()
    {
        return baseLifeGeneration;
    }
}
