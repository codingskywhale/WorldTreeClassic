using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class ResourceManager : Singleton<ResourceManager>
{
    public List<FlowerBase> flowers = new List<FlowerBase>();
    public ObjectPool objectPool;
    public BubbleGeneratorPool bubbleGeneratorPool;
    public BigInteger lifeGenerationRatePerSecond;

    protected override void Awake()
    {
        base.Awake();

        objectPool = GetComponent<ObjectPool>();
        bubbleGeneratorPool = GetComponent<BubbleGeneratorPool>();
    }
    private void Start()
    {
        RegisterAllFlowers();
        LifeManager.Instance.OnLifeChanged += UpdateLifeUI;

        // 초당 생명력 생성률을 로드
        LoadLifeGenerationRate();
        UpdateUI();
    }

    private void RegisterAllFlowers()
    {
        //FlowerBase[] flowerBases = FindObjectsOfType<FlowerBase>();
        foreach (var flower in flowers)
        {
            RegisterFlower(flower);
        }
        UpdateLifeGenerationRatePerSecond();
    }

    private void RegisterFlower(FlowerBase flower)
    {
        if (!flowers.Contains(flower))
        {
            flowers.Add(flower);
            flower.OnGenerationRateChanged += UpdateLifeGenerationRatePerSecond;
        }
    }

    private void UnregisterFlower(FlowerBase flower)
    {
        if (flowers.Contains(flower))
        {
            flowers.Remove(flower);
            flower.OnGenerationRateChanged -= UpdateLifeGenerationRatePerSecond;
        }
    }

    public void UpdateGroundSize()
    {
        float groundScale = 8f + (LifeManager.Instance.currentLevel / 10f);
        UIManager.Instance.tree.groundMeshFilter.transform.localScale = new Vector3(groundScale, groundScale, groundScale);
    }

    public void UpdateUI()
    {
        BigInteger lifeNeededForCurrentLevel = LifeManager.Instance.CalculateWaterNeededForUpgrade(1);
        UIManager.Instance.status.UpdateLifeUI(LifeManager.Instance.lifeAmount, lifeNeededForCurrentLevel);
        UIManager.Instance.status.UpdateLifeIncreaseUI(lifeGenerationRatePerSecond);
    }

    private void UpdateLifeUI(BigInteger newWaterAmount)
    {
        BigInteger lifeNeededForCurrentLevel = LifeManager.Instance.CalculateWaterNeededForUpgrade(1);
        UIManager.Instance.status.UpdateLifeUI(newWaterAmount, lifeNeededForCurrentLevel);
    }

    public void UpdateLifeGenerationRatePerSecond()
    {
        lifeGenerationRatePerSecond = GetTotalLifeGenerationPerSecond();
    }

    public BigInteger GetTotalLifeGenerationPerSecond()
    {
        BigInteger totalLifeIncrease = 0;
        foreach (var flower in flowers)
        {
            totalLifeIncrease += flower.GetTotalLifeGeneration();
        }
        return totalLifeIncrease;
    }

    public BigInteger GetLifeGenerationRatePerSecond()
    {
        return lifeGenerationRatePerSecond;
    }

    public void SetLifeGenerationRatePerSecond(BigInteger rate)
    {
        lifeGenerationRatePerSecond = rate;
    }

    public void LoadLifeGenerationRate()
    {
        GameData gameData = SaveSystem.Load();
        if (gameData != null && !string.IsNullOrEmpty(gameData.lifeGenerationRatePerSecond))
        {
            lifeGenerationRatePerSecond = BigInteger.Parse(gameData.lifeGenerationRatePerSecond);
        }
        else
        {
            UpdateLifeGenerationRatePerSecond();  // 초기 값을 계산
        }
    }
}
