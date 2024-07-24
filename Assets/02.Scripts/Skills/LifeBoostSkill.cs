using System.Collections;
using UnityEngine;
using System.Numerics;

public class LifeBoostSkill : Skill
{
    public BigInteger skillMultiplier = 5000; // 스킬 배수

    private TouchData touchData;

    protected override void Start()
    {
        skillName = "즉시 획득";
        cooldownTime = 1800f; // 스킬 쿨타임 30분
        currentLevel = 0; // 초기 레벨
        unlockCost = 300; // 해금 비용

        // TouchData 컴포넌트를 찾아서 참조합니다.
        touchData = FindObjectOfType<TouchData>();
        // 초기 UI 설정
        UpdateCooldownUI(0);
        UpdateUI();
    }

    public override string GetCurrentAbilityDescription()
    {
        return currentLevel > 0
            ? $"현재 즉시 획득 생명력: {skillMultiplier} 배"
            : "스킬이 해금되지 않았습니다";
    }

    public override string GetNextAbilityDescription()
    {
        BigInteger nextSkillMultiplier = currentLevel == 0 ? 5000 : 5000 + currentLevel * 1000;
        return $"다음 레벨 즉시 획득 생명력: {nextSkillMultiplier} 배";
    }

    public override void ActivateSkill()
    {
        if (!onCooldown && currentLevel > 0)
        {
            StartCoroutine(SkillEffect());
        }
    }

    protected override IEnumerator ApplySkillEffect()
    {
        // 터치 증가량의 5000배에 해당하는 생명력을 추가로 획득합니다.
        if (touchData != null)
        {
            BigInteger bonusLife = touchData.touchIncreaseAmount * skillMultiplier;
            LifeManager.Instance.IncreaseWater(bonusLife);
        }

        yield return null; // 즉시 완료
    }

    protected override void UpdateClickValues()
    {
        skillMultiplier = 5000 + (currentLevel - 1) * 1000; // 레벨당 증가 배수 계산
    }

    protected override void LevelUI()
    {
        currentLevelText.text = currentLevel > 0
            ? $"즉시 획득 스킬 레벨: {currentLevel}"
            : "즉시 획득 스킬이 해금되지 않았습니다";
    }

    protected override void NowskillInfoUI()
    {
        skillInfoText.text = currentLevel > 0
            ? $"현재 즉시 획득 생명력: {skillMultiplier} 배"
            : "스킬이 해금되지 않았습니다";
    }
}
