using PlayFab;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

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
        // 싱글톤 인스턴스 설정
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 인스턴스가 파괴되지 않도록 설정

            //ResetLogin();     //테스트용 로그인 초기화
        }
        else
        {
            Destroy(gameObject); // 이미 인스턴스가 존재하면 중복 생성된 객체 파괴
        }

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
        //SaveSystem.DeleteSave();  // 개발 중에만 사용
        touchInput = GetComponent<TouchInput>();
        offlineRewardUIManager.Initialize(offlineRewardManager); // UI 매니저 초기화
    }

    public void OnIntroAndOpeningCompleted()
    {
        LifeManager.Instance.bubbleGenerator.InitialBubbleSet();
        saveDataManager.animalDataList = animalDataList;
        CalculateOfflineProgress();
        uiUpdater.UpdateAllUI();

        InvokeRepeating(nameof(AutoSaveGame), 180f, 180f);
    }

    private void AutoSaveGame()
    {
        saveDataManager.SaveGameData(resourceManager, skills, artifacts);        
    }

    private void CalculateOfflineProgress()
    {
        GameData gameData = SaveSystem.Load();
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
        Debug.Log("Login reset complete. You can now log in again.");
    }
}
