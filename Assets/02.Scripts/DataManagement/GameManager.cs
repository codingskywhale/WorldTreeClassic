using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public ResourceManager resourceManager;
    public List<UpgradeButton> upgradeButtons;
    public List<AnimalDataSO> animalDataList;
    public TouchInput touchInput;

    private SaveDataManager saveDataManager;
    private UIUpdater uiUpdater;
    private OfflineProgressCalculator offlineProgressCalculator;
    private OfflineRewardManager offlineRewardManager;

    private void Awake()
    {
        saveDataManager = new SaveDataManager();
        saveDataManager.animalDataList = animalDataList;
        uiUpdater = new UIUpdater(resourceManager, upgradeButtons);
        offlineProgressCalculator = new OfflineProgressCalculator();
        offlineRewardManager = new OfflineRewardManager(resourceManager, offlineProgressCalculator);
        SaveSystem.DeleteSave();  // 개발 중에만 사용
        touchInput = GetComponent<TouchInput>();
    }

    private void Start()
    {
        saveDataManager.LoadGameData(resourceManager);
        LifeManager.Instance.bubbleGenerator.InitialBubbleSet();
        saveDataManager.animalDataList = animalDataList;
        CalculateOfflineProgress();
        uiUpdater.UpdateAllUI();
        Debug.Log($"초기 생명력: {LifeManager.Instance.lifeAmount}");          
    }

    private void CalculateOfflineProgress()
    {
        GameData gameData = SaveSystem.Load();
        if (gameData != null && !string.IsNullOrEmpty(gameData.lastSaveTime))
        {
            TimeSpan offlineDuration = offlineProgressCalculator.CalculateOfflineDuration(gameData.lastSaveTime);
            Debug.Log($"오프라인 기간: {offlineDuration.TotalSeconds}초");

            // lifeGenerationRatePerSecond 값을 로드된 데이터에서 가져옵니다.
            resourceManager.LoadLifeGenerationRate();
        
            BigInteger lifeGenerationRatePerSecond = resourceManager.GetLifeGenerationRatePerSecond();
            Debug.Log($"로드된 초당 생명력 생성률: {lifeGenerationRatePerSecond}");

            BigInteger totalLifeIncrease = offlineRewardManager.CalculateTotalLifeIncrease(gameData.lastSaveTime);
            Debug.Log($"계산된 오프라인 보상 생명력: {totalLifeIncrease}");

            LifeManager.Instance.IncreaseWater(totalLifeIncrease);
            Debug.Log($"보상 적용 후 생명력: {LifeManager.Instance.lifeAmount}");

            // 오프라인 보상 계산 로직 추가
        }
    }

    private void OnApplicationQuit()
    {
        saveDataManager.SaveGameData(resourceManager);
    }
}
