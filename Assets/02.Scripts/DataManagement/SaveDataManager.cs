using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;

public class SaveDataManager
{
    public List<AnimalDataSO> animalDataList;

    public void SaveGameData(List<Skill> skillList, List<Artifact> artifactList)
    {
        List<FlowerBase> flowers = ResourceManager.Instance.flowers;

        List<FlowerDataSave> flowerDataList = new List<FlowerDataSave>();
        foreach (var flower in flowers)
        {
            flowerDataList.Add(new FlowerDataSave
            {
                flowerLevel = flower.flowerLevel,
                isUnlocked = flower.isUnlocked,
                upgradeLifeCost = flower.upgradeLifeCost.ToString()
            });
        }

        BigInteger totalLifeIncrease = CalculateTotalLifeIncrease(flowers);

        // 동물 상태 저장
        List<AnimalDataSave.AnimalState> animalStates = new List<AnimalDataSave.AnimalState>();
        GameObject[] animalObjects = DataManager.Instance.animalSpawnTr.GetComponentsInChildren<Transform>()
                                          .Where(t => t != DataManager.Instance.animalSpawnTr) // 부모 자신을 제외
                                          .Select(t => t.gameObject) // Transform에서 GameObject로 변환
                                          .ToArray();

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
            flowers = flowerDataList,
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
            lifeGenerationRatePerSecond = ResourceManager.Instance.GetTotalLifeGenerationPerSecond().ToString(),
            allTypeCountDic = serializableDict, // 직렬화된 딕셔너리 저장
            createObjectButtonUnlockCount = UIManager.Instance.createObjectButtonUnlockCount,
            skillDataList = skillDataList,
            artifactDataList = artifactDataList,
            lastSkillSaveTime = DateTime.UtcNow.ToString("o")
        };
        PlayFabManager.Instance.SaveGameData(gameData);
    }

    public void SaveOfflineTime(float offlineTime)
    {
        // 예시: 오프라인 시간을 PlayerPrefs에 저장
        PlayerPrefs.SetFloat("OfflineTime", offlineTime);
        PlayerPrefs.Save();
    }

    public IEnumerator LoadGameDataCoroutine(List<Skill> skillList, List<Artifact> artifactList, WorldTree worldTree)
    {
        bool isDataLoaded = false;

        PlayFabManager.Instance.LoadGameData(gameData =>
        {
            if (gameData == null)
            {
                Debug.Log("gameData is not null");
                InitializeDefaultGameData();
                UIManager.Instance.createObjectButtonUnlockCount = 0;
                //UIManager.Instance.UpdateButtonUI();
                BigInteger bigNumber = BigInteger.Parse("900000000000000000000000000000000000000000000" +
                    "000000000000000000000000000000000000000000000000000000000000000000000000000");
                LifeManager.Instance.lifeAmount = bigNumber;
                UIManager.Instance.touchData.upgradeLifeCost = new BigInteger(1000);
                UIManager.Instance.touchData.touchIncreaseAmount = new BigInteger(50);
            }
            else
            {
                ApplyLoadedGameData(gameData, skillList, artifactList, worldTree);
            }

            isDataLoaded = true;
        });


        while (!isDataLoaded)
        {
            yield return null;
        }
    }

    public void ApplyLoadedGameData(GameData gameData, List<Skill> skillList, List<Artifact> artifactList, WorldTree worldTree)
    {
        List<FlowerBase> flowers = ResourceManager.Instance.flowers;

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

        InitializeFlowers(gameData.flowers);

        // 초당 생명력 생성률 로드
        if (!string.IsNullOrEmpty(gameData.lifeGenerationRatePerSecond))
        {
            ResourceManager.Instance.SetLifeGenerationRatePerSecond(BigInteger.Parse(gameData.lifeGenerationRatePerSecond));
        }

        // 초기화 후 모든 루트의 UI 업데이트
        foreach (var flower in flowers)
        {
            flower.UpdateUI();
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
        DataManager.Instance.animalSpawnTr.transform.position = new UnityEngine.Vector3(0, 0, (DataManager.Instance.touchData.touchIncreaseLevel / 10) * 0.1f);

        for (int i = 10; i <= DataManager.Instance.touchData.touchIncreaseLevel; i += 10)
        {
            worldTree.IncrementCameraFOV();
            worldTree.MoveCameraBackwards();
        }

        UIManager.Instance.createObjectButtonUnlockCount = gameData.createObjectButtonUnlockCount > 0 ? gameData.createObjectButtonUnlockCount : 1;
        DataManager.Instance.touchData.UpdateUI();
        ResourceManager.Instance.UpdateUI();
    }

    private void InitializeFlowers(List<FlowerDataSave> flowerDataList)
    {
        List<FlowerBase> flowers = ResourceManager.Instance.flowers;

        for (int i = 0; i < flowers.Count && i < flowerDataList.Count; i++)
        {
            flowers[i].flowerLevel = flowerDataList[i].flowerLevel;
            flowers[i].isUnlocked = flowerDataList[i].isUnlocked;
            flowers[i].upgradeLifeCost = string.IsNullOrEmpty(flowerDataList[i].upgradeLifeCost) ? BigInteger.Zero : BigInteger.Parse(flowerDataList[i].upgradeLifeCost);

            if (flowers[i].isUnlocked)
            {
                InitializeFlower(flowers[i], flowerDataList[i].flowerLevel);
            }

            flowers[i].UpdateUI();
        }
    }

    private void InitializeFlower(FlowerBase flower, int level)
    {
        flower.flowerLevel = level;
        flower.upgradeLifeCost = flower.CalculateUpgradeCost();

        for (int i = 0; i <= (int)(level / 25); i++)
        {
            flower.ActivateNextPlantObject();
        }

        flower.UpdateUI();
    }

    public void InitializeDefaultGameData()
    {
        LifeManager.Instance.lifeAmount = BigInteger.Zero;
        LifeManager.Instance.currentLevel = 1;
        DataManager.Instance.animalGenerateData.nowAnimalCount = 0;
        DataManager.Instance.animalGenerateData.maxAnimalCount = 5;
        DataManager.Instance.animalGenerateData.nowCreateCost = DataManager.Instance.animalGenerateData.createCostbase;
        DataManager.Instance.touchData.touchIncreaseLevel = 1;
        DataManager.Instance.touchData.touchIncreaseAmount = 10;
        DataManager.Instance.touchData.upgradeLifeCost = 20;

        List<FlowerBase> flowers = ResourceManager.Instance.flowers;
        foreach (var flower in flowers)
        {
            flower.flowerLevel = 0;
            flower.isUnlocked = false;
            flower.upgradeLifeCost = flower.CalculateUpgradeCost();
            flower.UpdateUI();
        }

        ResourceManager.Instance.SetLifeGenerationRatePerSecond(BigInteger.Zero); // 초기 값 설정
    }

    private BigInteger CalculateTotalLifeIncrease(List<FlowerBase> flowers)
    {
        BigInteger totalLifeIncrease = 0;
        foreach (var flower in flowers)
        {
            if (flower.isUnlocked)
            {
                totalLifeIncrease += flower.GetTotalLifeGeneration();
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
            LifeManager.Instance.ApplyIncreaseRateToAllFlowers(1);

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
