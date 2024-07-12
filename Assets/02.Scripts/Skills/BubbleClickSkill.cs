using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleClickSkill : Skill
{
    public float clickInterval = 1f; // 자동 클릭 간격
    public float baseClickIntervalIncrease = 1; // 클릭 시간 증가량
    public int baseClickRateIncrease = 1; // 클릭 횟수 증가량

    private Queue<GameObject> bubbleQueue = new Queue<GameObject>();

    protected override void Start()
    {
        skillDuration = 300f; // 스킬 지속 시간
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

    protected override IEnumerator ApplySkillEffect()
    {
        float elapsedTime = 0f;

        // 모든 버블을 큐에 추가
        GameObject[] bubbles = GameObject.FindGameObjectsWithTag("Bubble");
        foreach (GameObject bubble in bubbles)
        {
            bubbleQueue.Enqueue(bubble);
        }

        while (elapsedTime < skillDuration && bubbleQueue.Count > 0)
        {
            ClickNextBubble();
            yield return new WaitForSeconds(clickInterval);
            elapsedTime += clickInterval;
        }

        bubbleQueue.Clear(); // 스킬 종료 후 큐를 비웁니다.
    }

    private void ClickNextBubble()
    {
        if (bubbleQueue.Count > 0)
        {
            GameObject bubble = bubbleQueue.Dequeue();
            var heartButton = bubble.GetComponent<HeartButton>();
            if (heartButton != null)
            {
                heartButton.TouchHeartBubble();
            }
            else
            {
                Debug.LogError("HeartButton component not found on: " + bubble.name);
            }
        }
    }

    protected override void UpdateClickValues()
    {
        clickInterval = Mathf.Max(0.1f, clickInterval - baseClickIntervalIncrease * currentLevel);
        baseClickRateIncrease = 1 + currentLevel; // 클릭 횟수 증가량은 레벨에 비례하여 증가
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
            ? $"현재 클릭 간격: {clickInterval}초 \n 클릭 횟수: {baseClickRateIncrease}"
            : "스킬이 해금되지 않았습니다";
    }
}
