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
    public OfflineRewardUIManager offlineRewardUIManager; // 오프라인 보상 UI 매니저

    private GameDataManager gameDataManager;
    private UIUpdater uiUpdater;
    private OfflineProgressCalculator offlineProgressCalculator;
    private OfflineRewardManager offlineRewardManager;
    private OfflineRewardSkill offlineRewardSkill;
    private OfflineRewardAmountSkill offlineRewardAmountSkill;
    private int maxOfflineDurationMinutes = 120; // 최대 오프라인 기간 설정 (기본값 120분)
    private void Awake()
    {
        gameDataManager = new GameDataManager();
        uiUpdater = new UIUpdater(resourceManager, upgradeButtons);
        offlineProgressCalculator = new OfflineProgressCalculator();
        // OfflineRewardSkill 인스턴스 생성 및 초기화
        offlineRewardSkill = FindObjectOfType<OfflineRewardSkill>();
        offlineRewardAmountSkill = FindObjectOfType<OfflineRewardAmountSkill>();
        offlineRewardManager = new OfflineRewardManager(resourceManager, offlineProgressCalculator, 
                                                        offlineRewardSkill, offlineRewardAmountSkill);
        //SaveSystem.DeleteSave();  // 개발 중에만 사용
        touchInput = GetComponent<TouchInput>();
        offlineRewardUIManager.Initialize(offlineRewardManager); // UI 매니저 초기화
    }

    private void Start()
    {
        gameDataManager.LoadGameData(resourceManager);
        gameDataManager.animalDataList = animalDataList;
        CalculateOfflineProgress();
        uiUpdater.UpdateAllUI();
        Debug.Log($"초기 생명력: {LifeManager.Instance.lifeAmount}");          
    }

    private void CalculateOfflineProgress()
    {
        GameData gameData = SaveSystem.Load();
        if (gameData != null && !string.IsNullOrEmpty(gameData.lastSaveTime))
        {
            BigInteger totalLifeIncrease = offlineRewardManager.CalculateTotalLifeIncrease(gameData.lastSaveTime);
            double offlineDurationInSeconds = offlineRewardManager.CalculateOfflineDurationInSeconds(gameData.lastSaveTime);
            double maxOfflineDurationInSeconds = offlineRewardManager.GetMaxOfflineDurationInSeconds();
            Debug.Log($"오프라인 기간 (초): {offlineDurationInSeconds}");
            Debug.Log($"총 오프라인 기간 (스킬 적용) (초): {maxOfflineDurationMinutes}");
            if (totalLifeIncrease > 0)
            {
                offlineRewardUIManager.ShowOfflineRewardUI(totalLifeIncrease, offlineDurationInSeconds, maxOfflineDurationInSeconds);
            }
            else
            {
                offlineRewardUIManager.HideOfflineRewardUI();
            }
        }
    }
    private void OnApplicationQuit()
    {
        gameDataManager.SaveGameData(resourceManager);
    }
}

