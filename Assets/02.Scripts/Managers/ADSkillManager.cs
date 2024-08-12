using GoogleMobileAds.Api;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class ADSkillManager : MonoBehaviour
{
    public Skill[] allSkills;
    public Image adCooldownFillImage; // 광고 스킬 쿨타임을 표시할 이미지
    public TextMeshProUGUI adCooldownText; // 광고 스킬 쿨타임을 표시할 텍스트
    public Button adButton; // 광고 버튼
    public Image LockImage; //해금 이미지
    public BubbleClickSkill BubbleClickSkill;
    private bool adOnCooldown = false;
    private float adCooldownRemaining;
    private float adCooldownTime = 1800f; // 30분

    void Start()
    {
        // 모든 스킬을 초기화합니다.
        allSkills = FindObjectsOfType<Skill>();
        adButton.onClick.AddListener(OnShowAdButtonClicked); // 클릭 이벤트 리스너 추가
    }

    public void ResetAllSkillCooldowns()
    {
        foreach (var skill in allSkills)
        {
            skill.ResetCooldown();
        }
    }

    public void OnShowAdButtonClicked()
    {
        if (!adOnCooldown)
        {
            // 광고를 보고 난 후 모든 스킬의 쿨타임을 초기화하고, SkillManager의 쿨타임을 30분으로 설정합니다.
            RewardedAdExample.Instance.ShowRewardedAd((Reward reward) =>
            {
                // 광고가 성공적으로 표시된 후 실행되는 콜백
                ResetAllSkillCooldowns();
                SetAdCooldown(adCooldownTime); // 광고 스킬 쿨타임을 30분으로 설정
            });
        }
    }

    private void SetAdCooldown(float cooldownTime)
    {
        adOnCooldown = true;
        adCooldownRemaining = cooldownTime;
        UpdateAdCooldownUIElements(cooldownTime, cooldownTime); // 쿨타임 시작 시 fillAmount를 0으로 설정
        StartCoroutine(UpdateAdCooldownUI());
    }

    private IEnumerator UpdateAdCooldownUI()
    {
        while (adCooldownRemaining > 0)
        {
            adCooldownRemaining -= Time.deltaTime;
            UpdateAdCooldownUIElements(adCooldownRemaining, adCooldownTime);
            yield return null;
        }
        adOnCooldown = false;
        UpdateAdCooldownUIElements(0, adCooldownTime); // 최종적으로 쿨타임이 0이 되었을 때 UI 업데이트
    }

    private void UpdateAdCooldownUIElements(float remainingTime, float cooldownTime)
    {
        if (adCooldownFillImage != null)
        {
            adCooldownFillImage.fillAmount = (cooldownTime - remainingTime) / cooldownTime;
        }

        if (adCooldownText != null)
        {
            int minutes = Mathf.FloorToInt(remainingTime / 60);
            int seconds = Mathf.CeilToInt(remainingTime % 60);
            adCooldownText.text = $"{minutes:D2}:{seconds:D2}";
        }
    }
}
