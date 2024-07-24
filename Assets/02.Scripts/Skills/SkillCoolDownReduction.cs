using UnityEngine;

public class SkillCoolDownReduction : Artifact
{
    public float baseCooldownReduction = 2f; // 레벨당 쿨타임 감소 비율

    protected override void Start()
    {
        artifactName = "스킬 쿨다운";
        base.Start();
        ApplyCooldownReduction();
    }

    protected override void UpdateSkillInfoUI()
    {
        if (skillInfoText != null)
        {
            skillInfoText.text = currentLevel > 0
                ? $"현재 스킬 쿨타임 감소: {baseCooldownReduction * currentLevel}%"
                : "스킬쿨타임 감소 스킬이 해금되지 않았습니다";
        }
    }

    private void ApplyCooldownReduction()
    {
        Skill[] allSkills = FindObjectsOfType<Skill>();
        foreach (Skill skill in allSkills)
        {
            skill.ReduceCooldown(baseCooldownReduction * currentLevel);
        }
    }

    protected override void UpdateClickValues()
    {
        // 쿨타임 감소 값을 업데이트
        ApplyCooldownReduction();
    }

    //public override string GetCurrentAbilityDescription()
    //{
    //    return $"쿨타임 감소: {baseCooldownReduction * currentLevel}%";
    //}

    public override string GetNextAbilityDescription()
    {
        return $"{baseCooldownReduction * currentLevel}->{baseCooldownReduction * (currentLevel + 1)}%";
    }
}
