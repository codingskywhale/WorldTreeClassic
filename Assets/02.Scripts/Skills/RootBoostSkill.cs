using System.Collections;
using UnityEngine;
using System.Numerics;

public class RootBoostSkill : Skill
{
    public BigInteger boostMultiplier = 500; // 부스트 배수
    public float boostDuration = 300f; // 부스트 지속 시간 (5분)
    private IRoot[] roots;

    protected override void Start()
    {
        cooldownTime = 1800f; // 스킬 쿨타임 30분

        // IRoot 컴포넌트를 찾아서 참조합니다.
        roots = FindObjectsOfType<RootBase>();

        // 초기 UI 설정
        UpdateCooldownUI(0);
    }

    public override void ActivateSkill()
    {
        // Debug.Log("Skill button clicked.");
        if (!onCooldown && roots != null && roots.Length > 0)
        {
            Debug.Log("Skill button clicked.");
            StartCoroutine(SkillEffect());
        }
    }

    protected override IEnumerator ApplySkillEffect()
    {
        
        Debug.Log("실행됨");
        // 5분간 생산량을 500배로 증가
        foreach (var root in roots)
        {
            root.ApplyTemporaryBoost(boostMultiplier, boostDuration);
        }

        yield return null; // 즉시 완료
    }
}
