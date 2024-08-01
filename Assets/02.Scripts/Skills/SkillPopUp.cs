using System.Numerics;
using TMPro;
using UnityEngine;

public class SkillPopUp : MonoBehaviour
{
    public Skill[] skills;
    public Artifact[] artifacts;

    public TextMeshProUGUI SkillNameText;
    public TextMeshProUGUI NowLevelText; // 현재 레벨을 표시할 텍스트로 사용
    public TextMeshProUGUI LevelUpAbilityText;
    public TextMeshProUGUI UnlockExplainText;
    public TextMeshProUGUI ClickSkillCostText;

    public BigInteger cost;
    private Skill correspondingSkill;
    private Artifact correspondingArtifact;

    bool isArtifact;

    public void ShowSkillInfoPopup(int idx)
    {
        if (idx >= skills.Length)
        {
            isArtifact = true;
            correspondingArtifact = artifacts[idx - skills.Length];

            string nextAbility = correspondingArtifact.GetNextAbilityDescription();

            SetArtifactTexts(nextAbility);
        }
        else
        {
            isArtifact = false;
            correspondingSkill = skills[idx];

            string nextAbility = correspondingSkill.GetNextAbilityDescription();

            SetSkillTexts(nextAbility);
        }
    }

    public void SetArtifactTexts(string nextAbilityText)
    {
        SkillNameText.text = correspondingArtifact.artifactName;
        NowLevelText.text = $"{correspondingArtifact.currentLevel}"; // 현재 레벨 표시
        LevelUpAbilityText.text = nextAbilityText;
        if (correspondingArtifact.currentLevel > 0)
        {
            UnlockExplainText.text = "레벨업";
            ClickSkillCostText.text = BigIntegerUtils.FormatBigInteger(correspondingArtifact.CalculateUpgradeCost(correspondingArtifact.currentLevel));
        }
        else
        {
            ClickSkillCostText.text = BigIntegerUtils.FormatBigInteger(correspondingArtifact.unlockCost);
        }
    }

    public void SetSkillTexts(string nextAbilityText)
    {
        SkillNameText.text = correspondingSkill.skillName;
        NowLevelText.text = $"{correspondingSkill.currentLevel}"; // 현재 레벨 표시
        LevelUpAbilityText.text = nextAbilityText;
        if (correspondingSkill.currentLevel > 0)
        {
            UnlockExplainText.text = "레벨업";
            ClickSkillCostText.text = BigIntegerUtils.FormatBigInteger(correspondingSkill.CalculateUpgradeCost(correspondingSkill.currentLevel));
        }
        else
        {
            ClickSkillCostText.text = BigIntegerUtils.FormatBigInteger(correspondingSkill.unlockCost);
        }
        Debug.Log($"Skill popup updated: {correspondingSkill.skillName}, Level={correspondingSkill.currentLevel}");
    }

    public void OnUpgradeButtonClicked()
    {
        if (isArtifact)
            correspondingArtifact.OnUpgradeButtonClicked();
        else
            correspondingSkill.OnUpgradeButtonClicked();
    }
}
