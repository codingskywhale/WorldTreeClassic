public class OfflineRewardAmountSkill : Artifact
{
    protected override void UpdateSkillInfoUI()
    {
        if (skillInfoText != null)
        {
            skillInfoText.text = currentLevel > 0
                ? $"오프라인 보상량 증가: {currentLevel * 10}%"
                : "스킬이 해금되지 않았습니다";
        }
    }
}
