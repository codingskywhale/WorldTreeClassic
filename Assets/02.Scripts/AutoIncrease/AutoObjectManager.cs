using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class AutoObjectManager : MonoBehaviour
{
    public static AutoObjectManager Instance;
    public FlowerBase[] roots;
    BigInteger totalGeneration = BigInteger.Zero;
    public float generationInterval = 1f;
    public delegate void LifeGenerated(BigInteger amount);
    public event LifeGenerated OnLifeGenerated;
    private float timer;

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
    }

    private void Start()
    {
        OnLifeGenerated -= LifeManager.Instance.IncreaseWater;
        OnLifeGenerated += LifeManager.Instance.IncreaseWater;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= generationInterval)
        {
            CalculateTotalAutoGeneration(); // totalGeneration 값을 다시 계산
            InvokeLifeGeneration();
            timer = 0f;
        }
        CheckUnlockCondition();
    }

    public void CheckUnlockCondition()
    {
        foreach (var root in roots)
        {
            // 잠금 해제 조건 확인 로직
            if (!root.isUnlocked && DataManager.Instance.touchData != null
                && DataManager.Instance.touchData.touchIncreaseLevel >= root.unlockThreshold)
            {
                root.Unlock(); // 잠금 해제 조건 만족 시 Unlock 호출
            }
            // 오프라인 보상 스킬 해금 조건 확인
            if (!root.isUnlocked && root.offlineRewardAmountSkill != null
                && root.offlineRewardAmountSkill.currentLevel >= root.requiredOfflineRewardSkillLevel)
            {
                root.Unlock(); // 오프라인 보상 스킬 레벨 조건 만족 시 Unlock 호출
            }

            // 스킬 쿨다운 감소 해금 조건 확인
            if (!root.isUnlocked && root.skillCoolDownReduction != null
                && root.skillCoolDownReduction.currentLevel >= root.skillCoolDownReductionLevel)
            {
                root.Unlock(); // 스킬 쿨다운 감소 스킬 레벨 조건 만족 시 Unlock 호출
            }
        }
    }

    public void CalculateTotalAutoGeneration()
    {
        totalGeneration = BigInteger.Zero;
        foreach (var root in roots)
        {
            totalGeneration += root.GetTotalLifeGeneration();
        }
    }

    public void InvokeLifeGeneration()
    {
        OnLifeGenerated?.Invoke(totalGeneration);
    }
}
