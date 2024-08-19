using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public List<UpgradeButton> upgradeButtons;
    public List<AnimalDataSO> animalDataList;
    public List<SkillCoolDownReduction> skillCoolDownReductions; 
    public List<Skill> skills;
    public List<Artifact> artifacts;
    public WorldTree worldTree;
    public TouchInput touchInput;
    public OfflineRewardUI offlineRewardUIManager; // 오프라인 보상 UI 매니저
    public Intro introManager; // 인트로 매니저
    public CameraTransition cameraTransition; // 카메라 트랜지션

    public SaveDataManager saveDataManager;
    private UIUpdater uiUpdater;
    private OfflineProgressCalculator offlineProgressCalculator;
    private OfflineRewardManager offlineRewardManager;
    private OfflineRewardSkill offlineRewardSkill;
    private OfflineRewardAmountSkill offlineRewardAmountSkill;
    private const int SaveBufferThreshold = 10; // 생명력 업그레이드 저장 버퍼 임계값
    private Coroutine exitCoroutine;
    private bool isPaused = false;

    public Tutorial TutorialObject;

    protected override void Awake()
    {
        base.Awake();
        InitializeGame();
    }

    private void InitializeGame()
    {           
        saveDataManager = new SaveDataManager();
        saveDataManager.animalDataList = animalDataList;
        uiUpdater = new UIUpdater(upgradeButtons);
        uiUpdater.SetSkills(skills);
        uiUpdater.SetArtifacts(artifacts);
        offlineProgressCalculator = new OfflineProgressCalculator();
        // OfflineRewardSkill 인스턴스 생성 및 초기화
        offlineRewardSkill = FindObjectOfType<OfflineRewardSkill>();
        offlineRewardAmountSkill = FindObjectOfType<OfflineRewardAmountSkill>();
        offlineRewardManager = new OfflineRewardManager(offlineProgressCalculator,
                                                        offlineRewardSkill, offlineRewardAmountSkill);
        touchInput = GetComponent<TouchInput>();
        offlineRewardUIManager.Initialize(offlineRewardManager); // UI 매니저 초기화        
    }    

    public void OnIntroAndOpeningCompleted()
    {
        saveDataManager.animalDataList = animalDataList;
        UIManager.Instance.CreateAnimalButtons();
        UIManager.Instance.bag.CreateSlot();
        PlayFabManager.Instance.LoadGameData(OnGameDataLoaded);
        UIManager.Instance.LoadAnimalBuyStatus();
        uiUpdater.UpdateAllUI();
        DataManager.Instance.animalGenerateData.SetSlotData();

        worldTree.UpdateTreeMeshes(DataManager.Instance.touchData.touchIncreaseLevel);

        for (int i = 10; i <= DataManager.Instance.touchData.touchIncreaseLevel; i += 10)
        {
            worldTree.IncrementCameraFOV();
            worldTree.MoveCameraBackwards();
        }

        InvokeRepeating(nameof(AutoSaveGame), 180f, 180f);
    }

    private void OnGameDataLoaded(GameData gameData)
    {
        if (gameData != null)
        {
            CalculateOfflineProgress(gameData);
            uiUpdater.UpdateAllUI();
        }
        else
        {
            Debug.LogWarning("Failed to load game data. Offline rewards will not be calculated.");
            TutorialObject.gameObject.SetActive(true);
            TutorialObject.StartTutorial();
            uiUpdater.UpdateAllUI(); // 데이터 로드 실패 시에도 UI 업데이트
        }
    }

    private void AutoSaveGame()
    {
        saveDataManager.SaveGameData(skills, artifacts);        
    }

    private void CalculateOfflineProgress(GameData gameData)
    {
        if (gameData != null && !string.IsNullOrEmpty(gameData.lastSaveTime))
        {
            double offlineDurationInSeconds = offlineRewardManager.CalculateOfflineDurationInSeconds(gameData.lastSaveTime);

            if (offlineDurationInSeconds >= 10) // 오프라인 시간이 10초 이상인지 확인
            {
                BigInteger totalLifeIncrease = offlineRewardManager.CalculateTotalLifeIncrease(gameData.lastSaveTime);
                double maxOfflineDurationInSeconds = offlineRewardManager.GetMaxOfflineDurationInSeconds();

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
    }

    private void SaveGameIfLoggedIn()
    {
        if (PlayFabClientAPI.IsClientLoggedIn())
        {
            saveDataManager.SaveGameData(skills, artifacts);
            PlayerPrefs.SetInt("GuestLoggedIn", 1);
            PlayerPrefs.Save();
        }
        else
        {
            Debug.LogWarning("Not logged in. Skipping save game data.");
        }
    }

    private void OnApplicationQuit()
    {
        SaveGameIfLoggedIn();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            if (!isPaused)
            {
                isPaused = true;
                SaveGameIfLoggedIn();  // 백그라운드 전환 시 게임 저장
                PlayerPrefs.SetString("lastSaveTime", DateTime.UtcNow.ToString("o"));  // 로컬에 현재 시간 저장
                PlayerPrefs.Save();
            }
        }
        else
        {
            if (isPaused)
            {
                isPaused = false;
                UseLocalLastSaveTime();  // 로컬에 저장된 시간으로 오프라인 보상 처리
            }
        }
    }

    private void UseLocalLastSaveTime()
    {
        // 로컬에 저장된 lastSaveTime 사용
        if (PlayerPrefs.HasKey("lastSaveTime"))
        {
            string lastSaveTime = PlayerPrefs.GetString("lastSaveTime");
            ProcessBackgroundRewards(lastSaveTime);  // 로컬 데이터를 사용하여 오프라인 보상 처리
        }
    }

    private void ProcessBackgroundRewards(string lastSaveTime)
    {
        if (!string.IsNullOrEmpty(lastSaveTime))
        {
            double offlineDurationInSeconds = offlineRewardManager.CalculateOfflineDurationInSeconds(lastSaveTime);

            if (offlineDurationInSeconds >= 10)  // 오프라인 시간이 10초 이상인지 확인
            {
                BigInteger totalLifeIncrease = offlineRewardManager.CalculateTotalLifeIncrease(lastSaveTime);
                double maxOfflineDurationInSeconds = offlineRewardManager.GetMaxOfflineDurationInSeconds();

                // 보상 UI 표시
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
    }


}
