using System.Collections;
using UnityEngine;

public class BubbleClickSkill : Skill
{
    public float clickInterval = 0.1f; // 자동 클릭 간격 (초당 10회)
    public float skillDurationIncrease = 60f; // 레벨당 스킬 지속 시간 증가량 (1분)
    public int baseClickRateIncrease = 10; // 초당 클릭 횟수

    public bool isUseSkill = false;

    protected override void Start()
    {
        skillName = "자동 클릭";
        skillDuration = 60f; // 스킬 지속 시간 1분
        cooldownTime = 1800f; // 스킬 쿨타임 30분
        currentLevel = 0;

        // 초기 UI 설정
        UpdateUI();
        UpdateCooldownUI(0);
    }

    public override void ActivateSkill()
    {
        if (!onCooldown && currentLevel > 0) // 해금된 경우에만 스킬 사용 가능
        {
            StartCoroutine(SkillEffect());
        }
    }

    //public override string GetCurrentAbilityDescription()
    //{
    //    return currentLevel > 0
    //        ? $"지속 시간: {skillDuration / 60:F0}분"
    //        : "스킬이 해금되지 않았습니다";
    //}

    public override string GetNextAbilityDescription()
    {
        float nextSkillDuration = skillDuration + skillDurationIncrease;

        return currentLevel > 0
            ? $"{skillDuration / 60:F0} → {nextSkillDuration / 60:F0}"
            : $"지속 시간: {nextSkillDuration / 60:F0}분";
    }

    protected override IEnumerator ApplySkillEffect()
    {
        isUseSkill = true;
        float elapsedTime = 0f;
        WaitForSeconds waitTime = new WaitForSeconds(clickInterval);

        while (elapsedTime < skillDuration)
        {
            if (LifeManager.Instance.bubbleGenerator.GetNowBubbleList().Count > 0)
            {
                ClickNextBubble();
            }

            yield return waitTime;
            elapsedTime += clickInterval;
        }

        isUseSkill = false;
    }

    private void ClickNextBubble()
    {
        if (LifeManager.Instance.bubbleGenerator.GetNowBubbleList().Count > 0)
        {
            HeartButton bubble = LifeManager.Instance.bubbleGenerator.GetNowBubbleList()[0];
            if (bubble != null)
            {
                bubble.TouchHeartBubble();
            }
        }
    }

    protected override void UpdateClickValues()
    {
        skillDuration += skillDurationIncrease;
    }

    protected override void LevelUI()
    {
        currentLevelText.text = currentLevel > 0
            ? $"자동 클릭 스킬 레벨: {currentLevel}"
            : "자동 클릭 스킬이 해금되지 않았습니다";
    }

    protected override void NowskillInfoUI()
    {
        skillInfoText.text = currentLevel > 0
            ? $"현재 클릭 간격: {clickInterval:F2}초 \n 지속 시간: {skillDuration / 60:F0}분"
            : "스킬이 해금되지 않았습니다";
    }
}
