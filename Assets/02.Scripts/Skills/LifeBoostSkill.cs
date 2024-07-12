using System.Collections;
using UnityEngine;
using System.Numerics;

public class LifeBoostSkill : Skill
{
    public BigInteger skillMultiplier = 5000; // 스킬 배수

    private TouchData touchData;

    protected override void Start()
    {
        cooldownTime = 10f; // 스킬 쿨타임 30분
        currentLevel = 0;
        // TouchData 컴포넌트를 찾아서 참조합니다.
        touchData = FindObjectOfType<TouchData>();
        unlockCost = 300;
        // 초기 UI 설정
        UpdateCooldownUI(0);
        UpdateUI();
    }

    public override void ActivateSkill()
    {
        if (!onCooldown)
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

    protected override void LevelUI()
    {
        currentLevelText.text = currentLevel > 0
            ? $"즉시 획득 스킬 레벨: {currentLevel}"
            : "즉시 획득 스킬이 해금되지 않았습니다";
    }
    protected override void NowskillInfoUI()
    {
        skillInfoText.text = currentLevel > 0
            ? $"현재 즉시 획득 생명력: {skillMultiplier}"
            : "스킬이 해금되지 않았습니다";
    }
}

