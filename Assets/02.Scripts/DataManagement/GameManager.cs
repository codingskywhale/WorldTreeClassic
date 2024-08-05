using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
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

    private void Awake()
    {
        base.Awake();
        InitializeGame();
    }

    private void InitializeGame()
    {

        if (PlayFabManager.Instance != null)
        {
            PlayFabManager.Instance.OnLoginSuccessEvent += OnPlayFabLoginSuccess;
        }
        else
        {
            Debug.LogError("PlayFabManager.Instance가 null입니다.");
        }
                
        //DeleteAllUserData(); // 계정 데이터 삭제       
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

    private void OnPlayFabLoginSuccess(LoginResult result)
    {
        // 로그인 성공 후 데이터 삭제
        DeleteAllUserData();
    }

    public void OnIntroAndOpeningCompleted()
    {
        LifeManager.Instance.bubbleGenerator.InitialBubbleSet();
        saveDataManager.animalDataList = animalDataList;
        CalculateOfflineProgress();
        UIManager.Instance.CreateAnimalButtons();
        UIManager.Instance.bag.CreateSlot();
        PlayFabManager.Instance.LoadGameData(OnGameDataLoaded);
        uiUpdater.UpdateAllUI();

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

    private void OnApplicationQuit()
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

    public void ResetLogin()
    {
        PlayerPrefs.DeleteKey("GuestLoggedIn");
        PlayerPrefs.DeleteKey("GoogleLoggedIn");
        PlayFabClientAPI.ForgetAllCredentials();
    }

    public void DeleteAllUserData()
    {
        // 먼저 모든 사용자 데이터를 가져옵니다.
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), result =>
        {
            if (result.Data != null)
            {
                var keysToRemove = new List<string>(result.Data.Keys);

                // 모든 키를 삭제 요청합니다.
                var deleteRequest = new UpdateUserDataRequest
                {
                    KeysToRemove = keysToRemove
                };
                PlayFabClientAPI.UpdateUserData(deleteRequest, OnDataDeleteSuccess, OnDataDeleteFailure);
            }
        }, error =>
        {
            Debug.LogError("Failed to get user data: " + error.GenerateErrorReport());
        });
    }

    private void OnDataDeleteSuccess(UpdateUserDataResult result)
    {
        Debug.Log("User data deleted successfully.");
    }

    private void OnDataDeleteFailure(PlayFabError error)
    {
        Debug.LogError("Failed to delete user data: " + error.GenerateErrorReport());
    }
}
