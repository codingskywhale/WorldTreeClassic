using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class GameDataManager
{
    public void SaveGameData(ResourceManager resourceManager)
    {
        LifeManager lifeManager = resourceManager.lifeManager;
        List<RootBase> roots = resourceManager.roots;

        List<RootData> rootDataList = new List<RootData>();
        foreach (var root in roots)
        {
            rootDataList.Add(new RootData
            {
                rootLevel = root.rootLevel,
                isUnlocked = root.isUnlocked,
                upgradeLifeCost = root.upgradeLifeCost.ToString()
            });
        }

        BigInteger totalLifeIncrease = CalculateTotalLifeIncrease(roots);

        GameData gameData = new GameData
        {
            lifeAmount = lifeManager.lifeAmount.ToString(),
            totalLifeIncrease = totalLifeIncrease.ToString(),
            nowAnimalCount = lifeManager.animalData.nowAnimalCount,
            maxAnimalCount = lifeManager.animalData.maxAnimalCount,
            currentLevel = lifeManager.currentLevel,
            roots = rootDataList,
            animalData = new AnimalDataSave
            {
                nowCreateCost = lifeManager.animalData.nowCreateCost.ToString(),
                nowAnimalCount = lifeManager.animalData.nowAnimalCount,
                maxAnimalCount = lifeManager.animalData.maxAnimalCount
            },
            touchData = new TouchDataSave
            {
                touchIncreaseLevel = lifeManager.touchData.touchIncreaseLevel,
                touchIncreaseAmount = lifeManager.touchData.touchIncreaseAmount.ToString(),
                upgradeLifeCost = lifeManager.touchData.upgradeLifeCost.ToString()
            },
            lastSaveTime = DateTime.UtcNow.ToString("o")
        };
        SaveSystem.Save(gameData);
    }

    public void LoadGameData(ResourceManager resourceManager)
    {
        GameData gameData = SaveSystem.Load();

        if (gameData == null)
        {
            InitializeDefaultGameData(resourceManager);
            return;
        }

        LifeManager lifeManager = resourceManager.lifeManager;
        List<RootBase> roots = resourceManager.roots;

        lifeManager.lifeAmount = string.IsNullOrEmpty(gameData.lifeAmount) ? BigInteger.Zero : BigInteger.Parse(gameData.lifeAmount);
        lifeManager.currentLevel = gameData.currentLevel;
        lifeManager.animalData.nowAnimalCount = gameData.nowAnimalCount;
        lifeManager.animalData.maxAnimalCount = gameData.maxAnimalCount;

        if (gameData.animalData != null)
        {
            lifeManager.animalData.nowCreateCost = string.IsNullOrEmpty(gameData.animalData.nowCreateCost) ? BigInteger.Zero : BigInteger.Parse(gameData.animalData.nowCreateCost);
            lifeManager.animalData.nowAnimalCount = gameData.animalData.nowAnimalCount;
            lifeManager.animalData.maxAnimalCount = gameData.animalData.maxAnimalCount;
        }

        if (gameData.touchData != null)
        {
            lifeManager.touchData.touchIncreaseLevel = gameData.touchData.touchIncreaseLevel;
            lifeManager.touchData.touchIncreaseAmount = string.IsNullOrEmpty(gameData.touchData.touchIncreaseAmount) ? BigInteger.Zero : BigInteger.Parse(gameData.touchData.touchIncreaseAmount);
            lifeManager.touchData.upgradeLifeCost = string.IsNullOrEmpty(gameData.touchData.upgradeLifeCost) ? BigInteger.Zero : BigInteger.Parse(gameData.touchData.upgradeLifeCost);
        }

        InitializeRoots(resourceManager, gameData.roots);
    }

    private void InitializeRoots(ResourceManager resourceManager, List<RootData> rootDataList)
    {
        List<RootBase> roots = resourceManager.roots;

        for (int i = 0; i < roots.Count && i < rootDataList.Count; i++)
        {
            roots[i].rootLevel = rootDataList[i].rootLevel;
            roots[i].isUnlocked = rootDataList[i].isUnlocked;
            roots[i].upgradeLifeCost = string.IsNullOrEmpty(rootDataList[i].upgradeLifeCost) ? BigInteger.Zero : BigInteger.Parse(rootDataList[i].upgradeLifeCost);

            if (roots[i].isUnlocked)
            {
                InitializeRoot(roots[i], rootDataList[i].rootLevel);
            }

            roots[i].UpdateUI();
        }
    }

    private void InitializeRoot(RootBase root, int level)
    {
        root.rootLevel = level;
        root.upgradeLifeCost = root.CalculateUpgradeCost();
        root.UpdateUI();
    }

    private void InitializeDefaultGameData(ResourceManager resourceManager)
    {
        LifeManager lifeManager = resourceManager.lifeManager;
        lifeManager.lifeAmount = BigInteger.Zero;
        lifeManager.currentLevel = 1;
        lifeManager.animalData.nowAnimalCount = 0;
        lifeManager.animalData.maxAnimalCount = 5;
        lifeManager.animalData.nowCreateCost = lifeManager.animalData.createCostbase;
        lifeManager.touchData.touchIncreaseLevel = 1;
        lifeManager.touchData.touchIncreaseAmount = 10;
        lifeManager.touchData.upgradeLifeCost = 20;

        List<RootBase> roots = resourceManager.roots;
        foreach (var root in roots)
        {
            root.rootLevel = 0;
            root.isUnlocked = false;
            root.upgradeLifeCost = root.CalculateUpgradeCost();
            root.UpdateUI();
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
