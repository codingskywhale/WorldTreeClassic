using System;
using System.Numerics;
using UnityEngine;

public class OfflineRewardManager
{
    private ResourceManager resourceManager;
    private OfflineProgressCalculator offlineProgressCalculator;
    private OfflineRewardSkill offlineRewardSkill;
    private OfflineRewardAmountSkill offlineRewardAmountSkill;

    // AdditionalOfflineRewardMinutes 변수 선언
    public int AdditionalOfflineRewardMinutes { get; private set; } = 0;
    public OfflineRewardManager(ResourceManager resourceManager, OfflineProgressCalculator offlineProgressCalculator, 
                                OfflineRewardSkill offlineRewardSkill, OfflineRewardAmountSkill offlineRewardAmountSkill)
    {
        this.resourceManager = resourceManager;
        this.offlineProgressCalculator = offlineProgressCalculator;
        this.offlineRewardSkill = offlineRewardSkill;
        this.offlineRewardAmountSkill = offlineRewardAmountSkill;
        UpdateAdditionalRewardMinutes();
    }

    private void UpdateAdditionalRewardMinutes()
    {
        if (offlineRewardSkill != null && offlineRewardSkill.currentLevel > 0)
        {
            AdditionalOfflineRewardMinutes = offlineRewardSkill.currentLevel * 60;
        }
        else
        {
            AdditionalOfflineRewardMinutes = 120; // 스킬이 해금되지 않았을 때 기본 1분
        }
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

        // 스킬 적용 시간을 추가, 기본 1분 제한 적용
        TimeSpan totalOfflineDuration = offlineDuration;
        if (offlineRewardSkill.currentLevel == 0 && offlineDuration.TotalMinutes > AdditionalOfflineRewardMinutes)
        {
            totalOfflineDuration = TimeSpan.FromMinutes(AdditionalOfflineRewardMinutes);
        }
        else
        {
            totalOfflineDuration += TimeSpan.FromMinutes(AdditionalOfflineRewardMinutes);
        }

        Debug.Log($"총 오프라인 기간 (스킬 적용) (초): {totalOfflineDuration.TotalSeconds}");

        BigInteger lifePerSecond = resourceManager.GetLifeGenerationRatePerSecond();
        Debug.Log($"초당 생명력 생성률: {lifePerSecond}");

        BigInteger totalLifeIncrease = lifePerSecond * (BigInteger)totalOfflineDuration.TotalSeconds;

        // 오프라인 보상량 증가 스킬 적용
        if (offlineRewardAmountSkill != null && offlineRewardAmountSkill.currentLevel > 0)
        {
            float rewardMultiplier = 1.0f + (offlineRewardAmountSkill.currentLevel * 0.10f);
            totalLifeIncrease = BigInteger.Multiply(totalLifeIncrease, new BigInteger(rewardMultiplier));
            Debug.Log($"오프라인 보상량 증가 적용 후: {totalLifeIncrease}");
        }

        Debug.Log($"총 오프라인 생명력 증가량: {totalLifeIncrease}");

        return totalLifeIncrease;
    }
}
