using Unity.VisualScripting;
using UnityEngine;

public class OfflineRewardSkill : Artifact
{
    public int rewardIncreasePerLevel = 60; // 레벨당 보상 시간 증가 (분 단위)

    protected override void Start()
    {
        artifactName = "오프라인 시간 증가";
        base.Start();
    }
    protected override void UpdateSkillInfoUI()
    {
        if (skillInfoText != null)
        {
            skillInfoText.text = currentLevel > 0
                ? $"오프라인 보상 시간 증가: {rewardIncreasePerLevel * currentLevel}분"
                : "오프라인 보상 시간 증가 스킬이 해금되지 않았습니다";
        }
    }

    protected override void UpdateClickValues()
    {
        // 오프라인 보상 시간을 업데이트
    }

    public override string GetCurrentAbilityDescription()
    {
        return $"보상 시간 증가: {rewardIncreasePerLevel * currentLevel}분";
    }

    public override string GetNextAbilityDescription()
    {
        return $"보상 시간 증가: {rewardIncreasePerLevel * (currentLevel + 1)}분";
    }
}
