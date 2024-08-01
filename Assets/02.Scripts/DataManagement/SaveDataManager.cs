using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class SaveDataManager
{
    public List<AnimalDataSO> animalDataList;    

    public void SaveGameData(ResourceManager resourceManager, List<Skill> skillList)
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
        GameObject[] animalObjects = GameObject.FindGameObjectsWithTag("Animal");
        Debug.Log($"Found {animalObjects.Length} animals with 'Animal' tag");

        foreach (var animal in animalObjects)
        {
            AnimalDataSO animalData = FindAnimalDataByGameObject(animal);
            UniqueID uniqueID = animal.GetComponent<UniqueID>();
            if (animalData != null && uniqueID != null)
            {
                animalStates.Add(new AnimalDataSave.AnimalState
                {
                    dataSO = animalData,
                    uniqueID = uniqueID.uniqueID,
                    animalIndex = animalData.animalIndex,
                    posX = animal.transform.position.x,
                    posY = animal.transform.position.y,
                    posZ = animal.transform.position.z
                });
                Debug.Log($"Animal state saved: {animalData.animalName}, Index: {animalData.animalIndex}, Position: ({animal.transform.position.x}, {animal.transform.position.y}, {animal.transform.position.z})");
            }
            else
            {
                Debug.LogError($"Animal data not found or UniqueID component missing for game object: {animal.name}");
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

        // 딕셔너리 변환
        var serializableDict = new SerializableDictionary<string, SerializableDictionary<EachCountType, int>>();
        foreach (var kvp in DataManager.Instance.animalGenerateData.allTypeCountDic)
        {
            var innerDict = new SerializableDictionary<EachCountType, int>();
            foreach (var innerKvp in kvp.Value)
            {
                innerDict[innerKvp.Key] = innerKvp.Value;
            }
            serializableDict[kvp.Key] = innerDict;
        }

        List<SkillDataSave> skillDataList = new List<SkillDataSave>();

        foreach (var skill in skillList)
        {
            BigInteger upgradeCost = skill.currentLevel > 0 ? skill.CalculateUpgradeCost(skill.currentLevel) : skill.unlockCost;
            SkillDataSave data = new SkillDataSave
            {
                skillName = skill.gameObject.name,
                currentLevel = skill.currentLevel,
                upgradeCost = upgradeCost.ToString(),
                cooldownRemaining = skill.cooldownRemaining 
            };
            skillDataList.Add(data);
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
            lifeGenerationRatePerSecond = resourceManager.GetTotalLifeGenerationPerSecond().ToString(),
            allTypeCountDic = serializableDict, // 직렬화된 딕셔너리 저장
            createObjectButtonUnlockCount = UIManager.Instance.createObjectButtonUnlockCount,
            skillDataList = skillDataList
        };
        Debug.Log("Saving JSON: " + JsonUtility.ToJson(gameData, true)); // 저장되는 JSON 출력
        SaveSystem.Save(gameData);
    }

    public void LoadGameData(ResourceManager resourceManager, List<Skill> skillList)
    {
        GameData gameData = SaveSystem.Load();

        if (gameData == null)
        {
            InitializeDefaultGameData(resourceManager);
            UIManager.Instance.createObjectButtonUnlockCount = 1;
            UIManager.Instance.UpdateButtonUI();
            LifeManager.Instance.lifeAmount = new BigInteger(9000000000000000000);
            UIManager.Instance.touchData.upgradeLifeCost = new BigInteger(1000);
            UIManager.Instance.touchData.touchIncreaseAmount = new BigInteger(50);
            return;
        }                

        //gameData = new GameData();

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


            // 직렬화된 딕셔너리 로드
            var deserializedDict = new Dictionary<string, Dictionary<EachCountType, int>>();
            foreach (var kvp in gameData.allTypeCountDic)
            {
                var innerDict = new Dictionary<EachCountType, int>();
                foreach (var innerKvp in kvp.Value)
                {
                    innerDict[innerKvp.Key] = innerKvp.Value;
                }
                deserializedDict[kvp.Key] = innerDict;
            }
            DataManager.Instance.animalGenerateData.allTypeCountDic = deserializedDict;

            // 동물 상태 로드
            Debug.Log($"Loading {gameData.animalData.animalStates.Count} animal states");
            foreach (var animalState in gameData.animalData.animalStates)
            {
                GameObject animalObject = InstantiateAnimal(animalState.animalIndex);
                DataManager.Instance.spawnData.animalDataSOList.Add(animalState.dataSO);
                if (animalObject != null)
                {
                    UniqueID uniqueID = animalObject.AddComponent<UniqueID>();
                    uniqueID.uniqueID = animalState.uniqueID; // 고유 ID 설정
                    animalObject.transform.position = new UnityEngine.Vector3(animalState.posX, animalState.posY, animalState.posZ);
                    Debug.Log($"Animal instantiated at position {animalObject.transform.position}");

                    // 스폰 트랜스폼 설정
                    animalObject.transform.SetParent(DataManager.Instance.spawnData.spawnTr);

                    // 하트 버블 추가
                    var heartButton = animalObject.GetComponent<Animal>().heart;
                    if (heartButton != null)
                    {
                        LifeManager.Instance.bubbleGenerator.AddAnimalHeartBubbleList(heartButton);
                    }
                }
                else
                {
                    Debug.LogError($"Failed to instantiate animal with index {animalState.animalIndex}");
                }
            }

            // 가방 슬롯 업데이트
            foreach (var kvp in deserializedDict)
            {
                var animalDataSO = animalDataList.Find(data => data.animalName == kvp.Key);
                if (animalDataSO != null)
                {
                    var animalSlot = DataManager.Instance.bag.slots.Find(slot => slot.slotAnimalDataSO == animalDataSO);
                    if (animalSlot != null)
                    {
                        animalSlot.isUnlocked = kvp.Value[EachCountType.Active] > 0 || kvp.Value[EachCountType.Stored] > 0;
                        if (animalSlot.isUnlocked)
                        {
                            animalSlot.SetSlotData();
                        }
                    }
                }
            }
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

        if (gameData.skillDataList != null)
        {
            foreach (var skillData in gameData.skillDataList)
            {
                foreach (var skill in skillList)
                {
                    if (skill.gameObject.name == skillData.skillName)
                    {
                        skill.currentLevel = skillData.currentLevel;
                        skill.cooldownRemaining = skillData.cooldownRemaining; // 쿨다운 남은 시간 불러오기
                        skill.UpdateUI();
                    }
                }
            }
        }

        UIManager.Instance.createObjectButtonUnlockCount = gameData.createObjectButtonUnlockCount > 0 ? gameData.createObjectButtonUnlockCount : 1;
        UIManager.Instance.UpdateButtonUI();
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
            GameObject animalObject = GameObject.Instantiate(animalData.animalPrefab, DataManager.Instance.spawnData.spawnTr); // 부모 설정 추가
            DataManager.Instance.spawnData.animalObjectList.Add(animalObject);
            if (animalObject != null)
            {
                animalObject.name = animalData.animalPrefab.name; // 프리팹 이름으로 설정
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
        Debug.Log($"GetAnimalDataByIndex - Start for index: {index}");
        AnimalDataSO animalData = animalDataList.Find(data => data.animalIndex == index);
        if (animalData == null)
        {
            Debug.LogError($"GetAnimalDataByIndex - No animal data found for index: {index}");
        }
        else
        {
            Debug.Log($"GetAnimalDataByIndex - Animal data found for index: {index}");
        }
        return animalData;
    }

    private AnimalDataSO FindAnimalDataByGameObject(GameObject animal)
    {
        string animalNameWithoutClone = animal.name.Replace("(Clone)", "").Trim();
        Debug.Log($"Searching AnimalDataSO for game object name: {animalNameWithoutClone}");
        foreach (var data in animalDataList)
        {
            Debug.Log($"Comparing with AnimalDataSO name: {data.animalPrefab.name}");
            if (animalNameWithoutClone.Equals(data.animalPrefab.name, StringComparison.OrdinalIgnoreCase))
            {
                Debug.Log($"Found matching AnimalDataSO: {data.animalPrefab.name} for game object name: {animalNameWithoutClone}");
                return data;
            }
        }
        Debug.LogError($"AnimalDataSO not found for game object name: {animalNameWithoutClone}");
        return null;
    }
}
