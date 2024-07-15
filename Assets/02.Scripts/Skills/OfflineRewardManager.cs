using System;
using System.Numerics;
using UnityEngine;

public class OfflineRewardManager
{
    private ResourceManager resourceManager;
    private OfflineProgressCalculator offlineProgressCalculator;

    public OfflineRewardManager(ResourceManager resourceManager, OfflineProgressCalculator offlineProgressCalculator)
    {
        this.resourceManager = resourceManager;
        this.offlineProgressCalculator = offlineProgressCalculator;
    }

    public BigInteger CalculateTotalLifeIncrease(string lastSaveTime)
    {
        if (string.IsNullOrEmpty(lastSaveTime))
        {
            Debug.Log("마지막 저장 시간이 유효하지 않습니다.");
            return BigInteger.Zero;
        }

        TimeSpan offlineDuration = offlineProgressCalculator.CalculateOfflineDuration(lastSaveTime);
        Debug.Log($"오프라인 기간 (초): {offlineDuration.TotalSeconds}");

        BigInteger lifePerSecond = resourceManager.GetLifeGenerationRatePerSecond();
        Debug.Log($"초당 생명력 생성률: {lifePerSecond}");

        BigInteger totalLifeIncrease = lifePerSecond * (BigInteger)offlineDuration.TotalSeconds;
        Debug.Log($"총 오프라인 생명력 증가량: {totalLifeIncrease}");

        return totalLifeIncrease;
    }
}
