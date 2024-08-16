using System;
using System.Numerics;
using UnityEngine;

public class OfflineRewardManager
{
    private OfflineProgressCalculator offlineProgressCalculator;
    private OfflineRewardSkill offlineRewardSkill;
    private OfflineRewardAmountSkill offlineRewardAmountSkill;
    private int maxOfflineDurationMinutes;

    public int AdditionalOfflineRewardMinutes { get; private set; } = 0;

    public OfflineRewardManager(OfflineProgressCalculator offlineProgressCalculator,
                                OfflineRewardSkill offlineRewardSkill, OfflineRewardAmountSkill offlineRewardAmountSkill,
                                int maxOfflineDurationMinutes = 120) // 기본값 120분
    {
        this.offlineProgressCalculator = offlineProgressCalculator;
        this.offlineRewardSkill = offlineRewardSkill;
        this.offlineRewardAmountSkill = offlineRewardAmountSkill;
        this.maxOfflineDurationMinutes = maxOfflineDurationMinutes; // 최대 오프라인 기간 설정
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
            AdditionalOfflineRewardMinutes = 0; // 스킬이 해금되지 않았을 때 추가 시간 없음
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

        TimeSpan totalOfflineDuration;
        if (offlineRewardSkill.currentLevel == 0)
        {
            // 스킬이 해금되지 않았을 때 최대 오프라인 기간으로 제한
            totalOfflineDuration = TimeSpan.FromMinutes(Math.Min(maxOfflineDurationMinutes, offlineDuration.TotalMinutes));
        }
        else
        {
            totalOfflineDuration = offlineDuration + TimeSpan.FromMinutes(AdditionalOfflineRewardMinutes);
            // 최대 오프라인 기간으로 제한
            totalOfflineDuration = TimeSpan.FromMinutes(Math.Min(maxOfflineDurationMinutes, totalOfflineDuration.TotalMinutes));
        }

        BigInteger lifePerSecond = ResourceManager.Instance.GetLifeGenerationRatePerSecond();

        BigInteger totalLifeIncrease = lifePerSecond * (BigInteger)totalOfflineDuration.TotalSeconds;

        if (offlineRewardAmountSkill != null && offlineRewardAmountSkill.currentLevel > 0)
        {
            float rewardMultiplier = 1.0f + (offlineRewardAmountSkill.currentLevel * 0.10f);
            totalLifeIncrease = BigInteger.Multiply(totalLifeIncrease, new BigInteger(rewardMultiplier));
        }

        return totalLifeIncrease;
    }

    public double CalculateOfflineDurationInSeconds(string lastSaveTime)
    {
        TimeSpan offlineDuration = offlineProgressCalculator.CalculateOfflineDuration(lastSaveTime);
        if (offlineRewardSkill.currentLevel == 0 && offlineDuration.TotalMinutes > maxOfflineDurationMinutes)
        {
            offlineDuration = TimeSpan.FromMinutes(maxOfflineDurationMinutes); // 해금되지 않았을 시 최대 오프라인 기간으로 제한
        }
        return offlineDuration.TotalSeconds;
    }

    public double GetMaxOfflineDurationInSeconds()
    {
        return maxOfflineDurationMinutes * 60;
    }
}
