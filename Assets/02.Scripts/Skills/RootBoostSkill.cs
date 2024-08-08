using System.Collections;
using UnityEngine;
using System.Numerics;

public class RootBoostSkill : Skill
{
    public BigInteger baseBoostMultiplier = 100; // 초기 부스트 배수
    public BigInteger boostMultiplier; // 현재 부스트 배수
    public float boostDuration = 300f; // 부스트 지속 시간 (5분)
    private IRoot[] roots;

    protected override void Start()
    {
        skillName = "획득량 증가";
        cooldownTime = 7200f; // 스킬 쿨타임 120분 (2시간)
        //currentLevel = 0;

        // IRoot 컴포넌트를 찾아서 참조합니다.
        roots = FindObjectsOfType<RootBase>();

        // 초기 부스트 배수 설정
        boostMultiplier = baseBoostMultiplier;

        // 초기 UI 설정
        UpdateCooldownUI(0);
        UpdateUI();
        CheckUnlockStatus(); // 해금 상태를 초기화
        Debug.Log($"{currentLevel}루트부스트 레벨");
    }

    //public override string GetCurrentAbilityDescription()
    //{
    //    return currentLevel > 0
    //        ? $"현재 부스트 배수: {boostMultiplier}, 부스트 지속 시간: {boostDuration / 60}분"
    //        : "스킬이 해금되지 않았습니다";
    //}

    public override string GetNextAbilityDescription()
    {
        BigInteger nextBoostMultiplier = currentLevel == 0
            ? baseBoostMultiplier
            : baseBoostMultiplier + currentLevel * 100;
        return currentLevel > 0
            ? $"{boostMultiplier} → {nextBoostMultiplier}"
            : $"부스트 배수: {nextBoostMultiplier}, 부스트 지속 시간: {boostDuration / 60}분";
    }

    public override void ActivateSkill()
    {
        if (!onCooldown && roots != null && roots.Length > 0)
        {
            StartCoroutine(SkillEffect());
        }
    }

    protected override IEnumerator ApplySkillEffect()
    {
        // 터치 데이터의 증가량에 부스트 적용
        BigInteger originalTouchIncreaseAmount = DataManager.Instance.touchData.touchIncreaseAmount;
        DataManager.Instance.touchData.touchIncreaseAmount *= boostMultiplier;
        DataManager.Instance.touchData.UpdateUI();

        // 5분간 생산량을 증가
        foreach (var root in roots)
        {
            root.ApplyTemporaryBoost(boostMultiplier, boostDuration);
        }

        yield return new WaitForSeconds(boostDuration); // 부스트 지속 시간 동안 대기

        // 부스트 지속 시간이 끝나면 원래 값으로 복원
        DataManager.Instance.touchData.touchIncreaseAmount = originalTouchIncreaseAmount;
        DataManager.Instance.touchData.UpdateUI();
    }

    protected override void UpdateClickValues()
    {
        boostMultiplier = baseBoostMultiplier + (currentLevel - 1) * 100; // 레벨당 증가 배수 계산
        if (currentLevel == 0)
        {
            boostMultiplier = baseBoostMultiplier;
        }
    }

    protected override void LevelUI()
    {
        currentLevelText.text = currentLevel > 0
            ? $"획득량 증가 스킬 레벨: {currentLevel}"
            : "획득량 증가 스킬이 해금되지 않았습니다";
    }

    protected override void NowskillInfoUI()
    {
        skillInfoText.text = currentLevel > 0
            ? $"현재 부스트 배수: {boostMultiplier}, 부스트 지속시간: {boostDuration / 60}분"
            : "스킬이 해금되지 않았습니다";
    }
}
