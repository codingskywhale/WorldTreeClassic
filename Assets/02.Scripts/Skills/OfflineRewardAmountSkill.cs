using UnityEngine;

public class OfflineRewardAmountSkill : Artifact
{
    public int rewardIncreasePerLevel = 10; // 레벨당 보상 양 증가 (% 단위)

    protected override void Start()
    {
        artifactName = "오프라인 보상 증가";
        base.Start();
    }
    protected override void UpdateSkillInfoUI()
    {
        if (skillInfoText != null)
        {
            skillInfoText.text = currentLevel > 0
                ? $"오프라인 보상량 증가: {rewardIncreasePerLevel * currentLevel}%"
                : "오프라인 보상 증가 스킬이 해금되지 않았습니다";
        }
    }

    protected override void UpdateClickValues()
    {
        // 오프라인 보상 양을 업데이트
    }

    public override string GetCurrentAbilityDescription()
    {
        return $"보상량 증가: {rewardIncreasePerLevel * currentLevel}%";
    }

    public override string GetNextAbilityDescription()
    {
        return $"보상량 증가: {rewardIncreasePerLevel * (currentLevel + 1)}%";
    }
}
