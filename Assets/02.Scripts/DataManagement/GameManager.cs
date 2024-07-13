using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public ResourceManager resourceManager;
    public List<UpgradeButton> upgradeButtons;

    private GameDataManager gameDataManager;
    private UIUpdater uiUpdater;
    private OfflineProgressCalculator offlineProgressCalculator;

    private void Awake()
    {
        gameDataManager = new GameDataManager();
        uiUpdater = new UIUpdater(resourceManager, upgradeButtons);
        offlineProgressCalculator = new OfflineProgressCalculator();
    }

    private void Start()
    {
        gameDataManager.LoadGameData(resourceManager);
        CalculateOfflineProgress();
        uiUpdater.UpdateAllUI();
    }

    private void CalculateOfflineProgress()
    {
        GameData gameData = SaveSystem.Load();
        if (gameData != null && !string.IsNullOrEmpty(gameData.lastSaveTime))
        {
            TimeSpan offlineDuration = offlineProgressCalculator.CalculateOfflineDuration(gameData.lastSaveTime);
            // 오프라인 보상 계산 로직 추가
        }
    }

    private void OnApplicationQuit()
    {
        gameDataManager.SaveGameData(resourceManager);
    }
}
