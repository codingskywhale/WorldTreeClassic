using System.Linq;

public class SkillCoolDownReduction : Artifact
{
    public float baseCooldownReduction = 2f; // 레벨당 쿨타임 감소 비율

    protected override void UpdateSkillInfoUI()
    {
        if (skillInfoText != null)
        {
            skillInfoText.text = currentLevel > 0
                ? $"현재 스킬 쿨타임 감소: {baseCooldownReduction * currentLevel}%"
                : "스킬쿨타임 감소 스킬이 해금되지 않았습니다";
        }
    }

    protected override void Start()
    {
        base.Start();
        ApplyCooldownReduction();
    }

    private void ApplyCooldownReduction()
    {
        Skill[] allSkills = FindObjectsOfType<Skill>();
        foreach (Skill skill in allSkills)
        {
            skill.ReduceCooldown(baseCooldownReduction * currentLevel);
        }
    }
}
