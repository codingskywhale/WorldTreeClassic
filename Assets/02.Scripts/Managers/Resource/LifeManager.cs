using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class LifeManager : Singleton<LifeManager>
{
    [Header("Life")]
    public BigInteger lifeAmount;
    public int currentLevel = 1;
    public int lifePerLevel = 10;
    public delegate void LifeChanged(BigInteger newAmount);
    public event LifeChanged OnLifeChanged;

    [Header("Diamond")]
    public Diamond diamond;

    [Header("Flower")]
    public FlowerBase FlowerData;
    public List<IFlower> rootData = new List<IFlower>();

    [Header("Bubble")] // Resource로 이전
    public BubbleGenerator bubbleGenerator;
    public BubbleGeneratorPool bubbleGenratorPool;


    protected override void Awake()
    {
        base.Awake();
        diamond = GetComponent<Diamond>();
        bubbleGenerator = GetComponent<BubbleGenerator>();
    }
    private void Start()
    {
        // SaveDataManager의 LoadGameData 호출
        SaveDataManager saveDataManager = new SaveDataManager();
        ResourceManager resourceManager = FindObjectOfType<ResourceManager>();
        //saveDataManager.LoadGameData(skill);
    }
    public void IncreaseWater(BigInteger amount)
    {
        lifeAmount += amount;
        OnLifeChanged?.Invoke(lifeAmount);
    }

    public void DecreaseWater(BigInteger amount)
    {
        lifeAmount -= amount;
        OnLifeChanged?.Invoke(lifeAmount);
    }

    public bool HasSufficientWater(BigInteger requiredAmount)
    {
        return lifeAmount >= requiredAmount;
    }

    public BigInteger CalculateWaterNeededForUpgrade(int amount)
    {
        return (currentLevel + amount) * lifePerLevel;
    }

    public void RegisterFlower(IFlower root)
    {
        if (!rootData.Contains(root))
        {
            rootData.Add(root);
        }
    }

    public void ApplyIncreaseRateToAllFlowers(BigInteger rate)
    {
        foreach (var root in rootData)
        {
            root.ApplyIncreaseRate(rate);
        }
    }
}
