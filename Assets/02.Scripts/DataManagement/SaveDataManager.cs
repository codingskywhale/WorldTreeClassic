using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class SaveDataManager
{
    public List<AnimalDataSO> animalDataList;

    public void SaveGameData(ResourceManager resourceManager, List<Skill> skillList, List<Artifact> artifactList)
    {
        List<RootBase> roots = resourceManager.roots;

        List<RootDataSave> rootDataList = new List<RootDataSave>();
        foreach (var root in roots)
        {
            rootDataList.Add(new RootDataSave
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

        // 스킬 저장
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

        // 아티팩트 데이터 저장
        List<ArtifactDataSave> artifactDataList = new List<ArtifactDataSave>();

        foreach (var artifact in artifactList)
        {
            BigInteger upgradeCost = artifact.currentLevel > 0 ? artifact.CalculateUpgradeCost(artifact.currentLevel) : artifact.unlockCost;
            ArtifactDataSave data = new ArtifactDataSave
            {
                artifactName = artifact.gameObject.name,
                currentLevel = artifact.currentLevel,
                upgradeCost = upgradeCost.ToString(),
                isUnlocked = artifact.currentLevel > 0 // currentLevel이 0보다 크면 해금된 것으로 간주
            };
            artifactDataList.Add(data);
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
            skillDataList = skillDataList,
            artifactDataList = artifactDataList,
            lastSkillSaveTime = DateTime.UtcNow.ToString("o")
        };
        PlayFabManager.Instance.SaveGameData(gameData);
    }

    public IEnumerator LoadGameDataCoroutine(ResourceManager resourceManager, List<Skill> skillList, List<Artifact> artifactList, WorldTree worldTree)
    {
        bool isDataLoaded = false;

        PlayFabManager.Instance.LoadGameData(gameData =>
        {
            if (gameData == null)
            {
                Debug.Log("gameData is not null");
                InitializeDefaultGameData(resourceManager);
                UIManager.Instance.createObjectButtonUnlockCount = 0;
                //UIManager.Instance.UpdateButtonUI();
                BigInteger bigNumber = BigInteger.Parse("900000000000000000000000000000000000000000000000000000000000000000000000000000000");
                LifeManager.Instance.lifeAmount = bigNumber;
                UIManager.Instance.touchData.upgradeLifeCost = new BigInteger(1000);
                UIManager.Instance.touchData.touchIncreaseAmount = new BigInteger(50);
            }
            else
            {
                ApplyLoadedGameData(gameData, resourceManager, skillList, artifactList, worldTree);
            }

            isDataLoaded = true;
        });


        while (!isDataLoaded)
        {
            yield return null;
        }
    }

    public void ApplyLoadedGameData(GameData gameData, ResourceManager resourceManager, List<Skill> skillList, List<Artifact> artifactList, WorldTree worldTree)
    {
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
            foreach (var animalState in gameData.animalData.animalStates)
            {
                GameObject animalObject = InstantiateAnimal(animalState.animalIndex);
                animalState.dataSO = GetAnimalDataByIndex(animalState.animalIndex);
                DataManager.Instance.spawnData.AddAnimalSpawnData(animalObject, animalState.dataSO);
                if (animalObject != null)
                {
                    UniqueID uniqueID = animalObject.AddComponent<UniqueID>();
                    uniqueID.uniqueID = animalState.uniqueID; // 고유 ID 설정
                    animalObject.transform.position = new UnityEngine.Vector3(animalState.posX, animalState.posY, animalState.posZ);

                    // 스폰 트랜스폼 설정
                    animalObject.transform.SetParent(DataManager.Instance.spawnData.spawnTr);

                    for (int i = 0; i < Mathf.Min(DataManager.Instance.spawnData.animalObjectList.Count, 2); i++)
                    {
                        ResourceManager.Instance.bubbleGeneratorPool.GenerateNewHeart();
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

        InitializeRoots(gameData.roots);

        // 초당 생명력 생성률 로드
        if (!string.IsNullOrEmpty(gameData.lifeGenerationRatePerSecond))
        {
            resourceManager.SetLifeGenerationRatePerSecond(BigInteger.Parse(gameData.lifeGenerationRatePerSecond));
        }

        // 초기화 후 모든 루트의 UI 업데이트
        foreach (var root in roots)
        {
            root.UpdateUI();
        }

        TimeSpan timeElapsed = TimeSpan.Zero;

        if (!string.IsNullOrEmpty(gameData.lastSkillSaveTime))
        {
            DateTime lastSkillSaveTime;
            if (DateTime.TryParse(gameData.lastSkillSaveTime, out lastSkillSaveTime))
            {
                timeElapsed = DateTime.UtcNow - lastSkillSaveTime;
            }
            else
            {
                Debug.LogError("Failed to parse last save time.");
            }
        }
        else
        {
            Debug.LogWarning("Last save time is null or empty, setting timeElapsed to zero.");
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
                        skill.cooldownRemaining = Mathf.Max(0, skillData.cooldownRemaining - (float)timeElapsed.TotalSeconds); // 경과 시간 반영
                        skill.UpdateUI();                                                
                    }
                }
            }
        }

        // 아티팩트 데이터 로드
        if (gameData.artifactDataList != null)
        {
            foreach (var artifactData in gameData.artifactDataList)
            {
                foreach (var artifact in artifactList)
                {
                    if (artifact.gameObject.name == artifactData.artifactName)
                    {
                        artifact.currentLevel = artifactData.currentLevel;

                        // UnlockOrUpgradeSkill 대신에 레벨이 0이 아닐 때만 상태를 업데이트
                        if (artifact.currentLevel > 0)
                        {
                            artifact.UpdateUpgradeCostUI(); // 업그레이드 비용 UI 업데이트
                            artifact.UpdateUI(); // UI 업데이트
                            artifact.ActiveObject(); // 아티팩트 활성화
                        }
                    }
                }
            }
        }

        worldTree.UpdateTreeMeshes(DataManager.Instance.touchData.touchIncreaseLevel);

        for (int i = 10; i <= DataManager.Instance.touchData.touchIncreaseLevel; i += 10)
        {
            worldTree.IncrementCameraFOV();
            worldTree.MoveCameraBackwards();
        }

        UIManager.Instance.createObjectButtonUnlockCount = gameData.createObjectButtonUnlockCount > 0 ? gameData.createObjectButtonUnlockCount : 1;
        DataManager.Instance.touchData.UpdateUI();
        ResourceManager.Instance.UpdateUI();
    }

    private void InitializeRoots(List<RootDataSave> rootDataList)
    {
        List<RootBase> roots = ResourceManager.Instance.roots;

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

        for (int i = 0; i <= (int)(level / 25); i++)
        {
            root.ActivateNextPlantObject();
        }

        root.UpdateUI();
    }

    public void InitializeDefaultGameData(ResourceManager resourceManager)
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
            GameObject animalObject = GameObject.Instantiate(animalData.animalPrefab, DataManager.Instance.spawnData.spawnTr); // 부모 설정 추가
            LifeManager.Instance.ApplyIncreaseRateToAllRoots(1);

            if (animalObject != null)
            {
                animalObject.name = animalData.animalPrefab.name; // 프리팹 이름으로 설정
            }
            return animalObject;
        }
        return null;
    }

    private AnimalDataSO GetAnimalDataByIndex(int index)
    {
        AnimalDataSO animalData = animalDataList.Find(data => data.animalIndex == index);

        return animalData;
    }

    private AnimalDataSO FindAnimalDataByGameObject(GameObject animal)
    {
        string animalNameWithoutClone = animal.name.Replace("(Clone)", "").Trim();
        foreach (var data in animalDataList)
        {
            if (animalNameWithoutClone.Equals(data.animalPrefab.name, StringComparison.OrdinalIgnoreCase))
            {
                return data;
            }
        }
        return null;
    }
}
