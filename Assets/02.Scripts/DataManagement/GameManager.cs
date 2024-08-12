using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public ResourceManager resourceManager;
    public List<UpgradeButton> upgradeButtons;
    public List<AnimalDataSO> animalDataList;
    public List<SkillCoolDownReduction> skillCoolDownReductions; 
    public List<Skill> skills;
    public List<Artifact> artifacts;
    public WorldTree worldTree;
    public TouchInput touchInput;
    public OfflineRewardUIManager offlineRewardUIManager; // 오프라인 보상 UI 매니저
    public IntroManager introManager; // 인트로 매니저
    public CameraTransition cameraTransition; // 카메라 트랜지션

    public SaveDataManager saveDataManager;
    private UIUpdater uiUpdater;
    private OfflineProgressCalculator offlineProgressCalculator;
    private OfflineRewardManager offlineRewardManager;
    private OfflineRewardSkill offlineRewardSkill;
    private OfflineRewardAmountSkill offlineRewardAmountSkill;
    private int maxOfflineDurationMinutes = 120; // 최대 오프라인 기간 설정 (기본값 120분)
    private int saveBufferCounter = 0; // 저장 버퍼 카운터
    private const int SaveBufferThreshold = 10; // 생명력 업그레이드 저장 버퍼 임계값
    private Coroutine logoutCoroutine;

    public Tutorial TutorialObject;

    private void Awake()
    {
        base.Awake();
        InitializeGame();
    }

    private void InitializeGame()
    {           
        saveDataManager = new SaveDataManager();
        saveDataManager.animalDataList = animalDataList;
        uiUpdater = new UIUpdater(resourceManager, upgradeButtons);
        uiUpdater.SetSkills(skills);
        uiUpdater.SetArtifacts(artifacts);
        offlineProgressCalculator = new OfflineProgressCalculator();
        // OfflineRewardSkill 인스턴스 생성 및 초기화
        offlineRewardSkill = FindObjectOfType<OfflineRewardSkill>();
        offlineRewardAmountSkill = FindObjectOfType<OfflineRewardAmountSkill>();
        offlineRewardManager = new OfflineRewardManager(resourceManager, offlineProgressCalculator,
                                                        offlineRewardSkill, offlineRewardAmountSkill);
        touchInput = GetComponent<TouchInput>();
        offlineRewardUIManager.Initialize(offlineRewardManager); // UI 매니저 초기화        
    }    

    public void OnIntroAndOpeningCompleted()
    {
        LifeManager.Instance.bubbleGenerator.InitialBubbleSet();
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
        saveDataManager.SaveGameData(resourceManager, skills, artifacts);        
    }

    private void CalculateOfflineProgress(GameData gameData)
    {
        if (gameData != null && !string.IsNullOrEmpty(gameData.lastSaveTime))
        {
            BigInteger totalLifeIncrease = offlineRewardManager.CalculateTotalLifeIncrease(gameData.lastSaveTime);
            double offlineDurationInSeconds = offlineRewardManager.CalculateOfflineDurationInSeconds(gameData.lastSaveTime);
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
    private void SaveGameIfLoggedIn()
    {
        if (PlayFabClientAPI.IsClientLoggedIn())
        {
            saveDataManager.SaveGameData(resourceManager, skills, artifacts);
            PlayerPrefs.SetInt("GuestLoggedIn", 1);
            PlayerPrefs.Save();
        }
        else
        {
            Debug.LogWarning("Not logged in. Skipping save game data.");
        }
    }

    private IEnumerator DelayedQuit()
    {
        SaveGameIfLoggedIn(); // 먼저 저장 수행
        yield return new WaitForSeconds(10); // 10초 대기
        Application.Quit(); // 게임 종료
    }

    private void OnApplicationQuit()
    {
        if (logoutCoroutine != null)
        {
            StopCoroutine(logoutCoroutine); // 종료 지연 코루틴이 이미 실행 중이라면 중단
        }
        SaveGameIfLoggedIn();
        Application.Quit(); // 앱 종료
    }


    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            if (logoutCoroutine != null)
            {
                StopCoroutine(logoutCoroutine); // 이전 코루틴이 있으면 중단
            }
            logoutCoroutine = StartCoroutine(DelayedQuit()); // 저장 후 10초 지연 후 앱 종료
        }
        else
        {
            if (logoutCoroutine != null)
            {
                StopCoroutine(logoutCoroutine);
            }
        }
    }
}
