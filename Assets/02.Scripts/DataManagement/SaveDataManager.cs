using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class SaveDataManager
{
    public List<AnimalDataSO> animalDataList;

    public void SaveGameData(ResourceManager resourceManager)
    {
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

        // 동물 상태 저장
        List<AnimalDataSave.AnimalState> animalStates = new List<AnimalDataSave.AnimalState>();
        foreach (var animal in GameObject.FindObjectsOfType<GameObject>())
        {
            if (animal.CompareTag("Animal"))
            {
                AnimalDataSO animalData = FindAnimalDataByGameObject(animal);
                if (animalData != null)
                {
                    animalStates.Add(new AnimalDataSave.AnimalState
                    {
                        animalIndex = animalData.animalIndex,
                        posX = animal.transform.position.x,
                        posY = animal.transform.position.y,
                        posZ = animal.transform.position.z
                    });
                }
            }
        }

        // animalTypeCount 직렬화
        var animalTypeCountSerialized = new List<AnimalDataSave.SerializedAnimalTypeCount>();
        foreach (var kvp in DataManager.Instance.animalGenerateData.allTypeCountDic)
        {
            var counts = kvp.Value;
            var serializedCount = new AnimalDataSave.SerializedAnimalTypeCount
            {
                AnimalName = kvp.Key,
                Total = counts[EachCountType.Total],
                Active = counts[EachCountType.Active],
                Stored = counts[EachCountType.Stored]
            };
            animalTypeCountSerialized.Add(serializedCount);
        }

        // 게임 데이터 생성 및 저장
        GameData gameData = new GameData
        {
            lifeAmount = LifeManager.Instance.lifeAmount.ToString(),
            totalLifeIncrease = totalLifeIncrease.ToString(),
            nowAnimalCount = DataManager.Instance.animalGenerateData.nowAnimalCount,
            maxAnimalCount = DataManager.Instance.animalGenerateData.maxAnimalCount,
            currentLevel = LifeManager.Instance.currentLevel,
            roots = rootDataList,
            animalData = new AnimalDataSave
            {
                nowCreateCost = DataManager.Instance.animalGenerateData.nowCreateCost.ToString(),
                nowAnimalCount = DataManager.Instance.animalGenerateData.nowAnimalCount,
                maxAnimalCount = DataManager.Instance.animalGenerateData.maxAnimalCount,
                animalStates = animalStates,
                animalTypeCountSerialized = animalTypeCountSerialized // 직렬화된 데이터를 저장
            },
            touchData = new TouchDataSave
            {
                touchIncreaseLevel = DataManager.Instance.touchData.touchIncreaseLevel,
                touchIncreaseAmount = DataManager.Instance.touchData.touchIncreaseAmount.ToString(),
                upgradeLifeCost = DataManager.Instance.touchData.upgradeLifeCost.ToString()
            },
            lastSaveTime = DateTime.UtcNow.ToString("o"),
            lifeGenerationRatePerSecond = resourceManager.GetTotalLifeGenerationPerSecond().ToString()
        };
        Debug.Log("Saving JSON: " + JsonUtility.ToJson(gameData, true)); // 저장되는 JSON 출력
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

        List<RootBase> roots = resourceManager.roots;

        LifeManager.Instance.lifeAmount = string.IsNullOrEmpty(gameData.lifeAmount) ? BigInteger.Zero : BigInteger.Parse(gameData.lifeAmount);
        LifeManager.Instance.currentLevel = gameData.currentLevel;
        DataManager.Instance.animalGenerateData.nowAnimalCount = gameData.nowAnimalCount;
        DataManager.Instance.animalGenerateData.maxAnimalCount = gameData.maxAnimalCount;

        if (gameData.animalData != null)
        {
            DataManager.Instance.animalGenerateData.nowCreateCost = string.IsNullOrEmpty(gameData.animalData.nowCreateCost) ? BigInteger.Zero : BigInteger.Parse(gameData.animalData.nowCreateCost);
            DataManager.Instance.animalGenerateData.nowAnimalCount = gameData.animalData.nowAnimalCount;
            DataManager.Instance.animalGenerateData.maxAnimalCount = gameData.animalData.maxAnimalCount;

            // 동물 상태 로드
            Debug.Log($"Loading {gameData.animalData.animalStates.Count} animal states");
            foreach (var animalState in gameData.animalData.animalStates)
            {
                GameObject animalObject = InstantiateAnimal(animalState.animalIndex);
                if (animalObject != null)
                {
                    animalObject.transform.position = new UnityEngine.Vector3(animalState.posX, animalState.posY, animalState.posZ);
                    Debug.Log($"Animal instantiated at position {animalObject.transform.position}");
                }
                else
                {
                    Debug.LogError($"Failed to instantiate animal with index {animalState.animalIndex}");
                }
            }

            var animalTypeCountDeserialized = new Dictionary<string, Dictionary<EachCountType, int>>();
            foreach (var serializedCount in gameData.animalData.animalTypeCountSerialized)
            {
                var counts = new Dictionary<EachCountType, int>
            {
                { EachCountType.Total, serializedCount.Total },
                { EachCountType.Active, serializedCount.Active },
                { EachCountType.Stored, serializedCount.Stored }
            };
                animalTypeCountDeserialized[serializedCount.AnimalName] = counts;
            }
            DataManager.Instance.animalGenerateData.allTypeCountDic = animalTypeCountDeserialized;
        }

        if (gameData.touchData != null)
        {
            DataManager.Instance.touchData.touchIncreaseLevel = gameData.touchData.touchIncreaseLevel;
            DataManager.Instance.touchData.touchIncreaseAmount = string.IsNullOrEmpty(gameData.touchData.touchIncreaseAmount) ? BigInteger.Zero : BigInteger.Parse(gameData.touchData.touchIncreaseAmount);
            DataManager.Instance.touchData.upgradeLifeCost = string.IsNullOrEmpty(gameData.touchData.upgradeLifeCost) ? BigInteger.Zero : BigInteger.Parse(gameData.touchData.upgradeLifeCost);
        }

        InitializeRoots(resourceManager, gameData.roots);

        // 초당 생명력 생성률 로드
        if (!string.IsNullOrEmpty(gameData.lifeGenerationRatePerSecond))
        {
            resourceManager.SetLifeGenerationRatePerSecond(BigInteger.Parse(gameData.lifeGenerationRatePerSecond));
        }
        Debug.Log($"LoadGameData - LifeAmount: {LifeManager.Instance.lifeAmount}");
        // 초기화 후 모든 루트의 UI 업데이트
        foreach (var root in roots)
        {
            root.UpdateUI();
        }
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
        // 디버그 로그 추가
        Debug.Log($"Root initialized: {root.name}, Level: {level}, UpgradeCost: {root.upgradeLifeCost}");
    }

    private void InitializeDefaultGameData(ResourceManager resourceManager)
    {
        LifeManager.Instance.lifeAmount = BigInteger.Zero;
        LifeManager.Instance.currentLevel = 1;
        DataManager.Instance.animalGenerateData.nowAnimalCount = 0;
        DataManager.Instance.animalGenerateData.maxAnimalCount = 5;
        DataManager.Instance.animalGenerateData.nowCreateCost = DataManager.Instance.animalGenerateData.createCostbase;
        DataManager.Instance.touchData.touchIncreaseLevel = 1;
        DataManager.Instance.touchData.touchIncreaseAmount = 10;
        DataManager.Instance.touchData.upgradeLifeCost = 20;

        List<RootBase> roots = resourceManager.roots;
        foreach (var root in roots)
        {
            root.rootLevel = 0;
            root.isUnlocked = false;
            root.upgradeLifeCost = root.CalculateUpgradeCost();
            root.UpdateUI();
        }

        resourceManager.SetLifeGenerationRatePerSecond(BigInteger.Zero); // 초기 값 설정
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

    private GameObject InstantiateAnimal(int animalIndex)
    {
        AnimalDataSO animalData = GetAnimalDataByIndex(animalIndex);
        if (animalData != null)
        {
            Debug.Log($"Animal prefab found for index: {animalIndex}");
            GameObject animalObject = GameObject.Instantiate(animalData.animalPrefab);
            if (animalObject != null)
            {
                Debug.Log($"Animal instantiated successfully at index: {animalIndex}");
            }
            else
            {
                Debug.LogError($"Failed to instantiate animal at index: {animalIndex}");
            }
            return animalObject;
        }
        Debug.LogError($"Animal prefab not found for index: {animalIndex}");
        return null;
    }

    private AnimalDataSO GetAnimalDataByIndex(int index)
    {
        return animalDataList.Find(data => data.animalIndex == index);
    }

    private AnimalDataSO FindAnimalDataByGameObject(GameObject animal)
    {
        foreach (var data in animalDataList)
        {
            if (animal.name.Contains(data.animalPrefab.name))
            {
                return data;
            }
        }
        return null;
    }
}
