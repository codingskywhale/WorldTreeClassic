using System.Numerics;
using TMPro;
using UnityEngine;

public class SkillPopUp : MonoBehaviour
{
    public Skill[] skills;
    public Artifact[] artifacts;

    public TextMeshProUGUI SkillNameText;
    public TextMeshProUGUI NowAbilityText;
    public TextMeshProUGUI LevelUpAbilityText;
    public TextMeshProUGUI UnlockExplainText;
    public TextMeshProUGUI ClickSkillCostText;

    public BigInteger cost;
    private Skill correspondingSkill;
    private Artifact correspondingArtifact;

    bool isArtifact;

    public void ShowSkillInfoPopup(int idx)
    {
        if(idx >= skills.Length)
        {
            isArtifact = true;
            correspondingArtifact = artifacts[idx - skills.Length];

            string currentAbility = correspondingArtifact.GetCurrentAbilityDescription();
            string nextAbility = correspondingArtifact.GetNextAbilityDescription();

            SetArtifactTexts(currentAbility, nextAbility);
        }

        else
        {
            isArtifact = false;
            correspondingSkill = skills[idx];

            string currentAbility = correspondingSkill.GetCurrentAbilityDescription();
            string nextAbility = correspondingSkill.GetNextAbilityDescription();

            SetSkillTexts(currentAbility, nextAbility);
        }
    }

    public void SetArtifactTexts(string nowAbilityText, string nextAbilityText)
    {
        SkillNameText.text = correspondingArtifact.artifactName;
        NowAbilityText.text = nowAbilityText;
        LevelUpAbilityText.text = nextAbilityText;
        if (correspondingArtifact.currentLevel > 0)
        {
            UnlockExplainText.text = "레벨업";
            ClickSkillCostText.text = BigIntegerUtils.FormatBigInteger(correspondingArtifact.CalculateUpgradeCost(correspondingArtifact.currentLevel));
        }
        else
            ClickSkillCostText.text = BigIntegerUtils.FormatBigInteger(correspondingArtifact.unlockCost);
    }

    public void SetSkillTexts(string nowAbilityText, string nextAbilityText)
    {
        SkillNameText.text = correspondingSkill.skillName;
        NowAbilityText.text = nowAbilityText;
        LevelUpAbilityText.text = nextAbilityText;
        if(correspondingSkill.currentLevel > 0)
        {
            UnlockExplainText.text = "레벨업";
            ClickSkillCostText.text = BigIntegerUtils.FormatBigInteger(correspondingSkill.CalculateUpgradeCost(correspondingSkill.currentLevel));
        }
        else
            ClickSkillCostText.text = BigIntegerUtils.FormatBigInteger(correspondingSkill.unlockCost);
    }

    public void OnUpgradeButtonClicked()
    {
        if(isArtifact)
            correspondingArtifact.OnUpgradeButtonClicked();
        else
            correspondingSkill.OnUpgradeButtonClicked();
    }
}
