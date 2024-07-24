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
    public TouchInput touchInput;
    public OfflineRewardUIManager offlineRewardUIManager; // 오프라인 보상 UI 매니저
    public IntroManager introManager; // 인트로 매니저
    public CameraTransition cameraTransition; // 카메라 트랜지션

    private SaveDataManager saveDataManager;
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
        }
        else
        {
            Destroy(gameObject); // 이미 인스턴스가 존재하면 중복 생성된 객체 파괴
        }

        saveDataManager = new SaveDataManager();
        saveDataManager.animalDataList = animalDataList;        
        uiUpdater = new UIUpdater(resourceManager, upgradeButtons);
        uiUpdater.SetSkills(skills);
        offlineProgressCalculator = new OfflineProgressCalculator();
        // OfflineRewardSkill 인스턴스 생성 및 초기화
        offlineRewardSkill = FindObjectOfType<OfflineRewardSkill>();
        offlineRewardAmountSkill = FindObjectOfType<OfflineRewardAmountSkill>();
        offlineRewardManager = new OfflineRewardManager(resourceManager, offlineProgressCalculator, 
                                                        offlineRewardSkill, offlineRewardAmountSkill);
        SaveSystem.DeleteSave();  // 개발 중에만 사용
        touchInput = GetComponent<TouchInput>();
        offlineRewardUIManager.Initialize(offlineRewardManager); // UI 매니저 초기화
    }

    private void Start()
    {
        StartCoroutine(PlayIntroAndOpening());        
    }

    private IEnumerator PlayIntroAndOpening()
    {
        // 인트로 애니메이션 실행
        yield return StartCoroutine(introManager.PlayIntro());

        // 오프닝 애니메이션 실행
        //yield return StartCoroutine(cameraTransition.OpeningCamera());

        // 오프닝 애니메이션이 완료된 후 게임 로직 실행
        OnIntroAndOpeningCompleted();
    }

    private void OnIntroAndOpeningCompleted()
    {
        saveDataManager.LoadGameData(resourceManager, skills);
        LifeManager.Instance.bubbleGenerator.InitialBubbleSet();
        saveDataManager.animalDataList = animalDataList;
        CalculateOfflineProgress();
        uiUpdater.UpdateAllUI();
        Debug.Log($"초기 생명력: {LifeManager.Instance.lifeAmount}");

        InvokeRepeating(nameof(AutoSaveGame), 180f, 180f);
    }

    private void AutoSaveGame()
    {
        saveDataManager.SaveGameData(resourceManager, skills);
        Debug.Log("Game data saved automatically.");
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
        saveDataManager.SaveGameData(resourceManager, skills);
    }
}
