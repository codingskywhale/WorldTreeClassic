public class OfflineRewardSkill : Artifact
{
    protected override void UpdateSkillInfoUI()
    {
        if (skillInfoText != null)
        {
            skillInfoText.text = currentLevel > 0
                ? $"오프라인 보상 시간 증가: {currentLevel * 60}분"
                : "스킬이 해금되지 않았습니다";
        }
    }
}
