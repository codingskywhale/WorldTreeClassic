using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleClickSkill : Skill
{
    public float clickInterval = 1f; // 자동 클릭 간격

    private Queue<GameObject> bubbleQueue = new Queue<GameObject>();

    protected override void Start()
    {
        skillDuration = 300f; // 스킬 지속 시간
        cooldownTime = 1800f; // 스킬 쿨타임 30분

        // 초기 UI 설정
        UpdateCooldownUI(0);
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
}
