using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class Skill : MonoBehaviour
{
    public float skillDuration; // 스킬 지속시간
    public float cooldownTime = 15.0f; // 기본 쿨타임
    public TextMeshProUGUI cooldownText; // 쿨타임을 표시할 텍스트
    public Image cooldownImage; // 회전할 이미지

    protected bool onCooldown = false;
    protected float cooldownRemaining;

    protected virtual void Start()
    {
        // 각 스킬의 지속시간과 쿨타임은 서브 클래스에서 설정됩니다.
    }

    private void Update()
    {
        if (onCooldown)
        {
            cooldownRemaining -= Time.deltaTime;
            if (cooldownRemaining < 0) cooldownRemaining = 0;

            UpdateCooldownUI(cooldownRemaining);
        }
    }

    public abstract void ActivateSkill();

    protected IEnumerator SkillEffect()
    {
        onCooldown = true;
        cooldownRemaining = cooldownTime;

        yield return StartCoroutine(ApplySkillEffect());

        while (cooldownRemaining > 0)
        {
            yield return null;
        }

        onCooldown = false;
        UpdateCooldownUI(0);
    }

    protected abstract IEnumerator ApplySkillEffect();

    public void UpdateCooldownUI(float remaining)
    {
        if (cooldownText != null)
        {
            cooldownText.text = $"{Mathf.CeilToInt(remaining)}s";
        }

        if (cooldownImage != null)
        {
            float fillAmount = 1 - (remaining / cooldownTime);
            cooldownImage.fillAmount = fillAmount;            
        }
    }
}
