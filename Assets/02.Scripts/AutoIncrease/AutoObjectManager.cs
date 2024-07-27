

using System;
using System.Numerics;
using UnityEngine;
using static RootBase;

public class AutoObjectManager : MonoBehaviour
{
    public static AutoObjectManager Instance;
    public RootBase[] roots;
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