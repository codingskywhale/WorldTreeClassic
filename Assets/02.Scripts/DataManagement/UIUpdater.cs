using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class UIUpdater
{
    private ResourceManager resourceManager;
    private List<UpgradeButton> upgradeButtons;
    private List<Skill> skills;
    private List<Artifact> artifacts;

    public UIUpdater(ResourceManager resourceManager, List<UpgradeButton> upgradeButtons)
    {
        this.resourceManager = resourceManager;
        this.upgradeButtons = upgradeButtons;
    }

    public void SetSkills(List<Skill> skills)
    {
        this.skills = skills;
    }

    public void SetArtifacts(List<Artifact> artifacts)
    {
        this.artifacts = artifacts;
    }

    public void UpdateAllUI()
    {
        DataManager.Instance.touchData.UpdateUI();
        UIManager.Instance.status.UpdateLifeUI(LifeManager.Instance.lifeAmount, LifeManager.Instance.CalculateWaterNeededForUpgrade(1));
        UIManager.Instance.status.UpdateAnimalCountText(DataManager.Instance.animalGenerateData.nowAnimalCount, DataManager.Instance.animalGenerateData.maxAnimalCount);

        foreach (var root in resourceManager.roots)
        {
            if (root.isUnlocked)
            {                
                root.UpdateUI();
            }
        }

        foreach (var button in upgradeButtons)
        {
            if (button.upgradeType == UpgradeButton.UpgradeType.Root)
            {
                button.UpdateUpgradeCostUI(button.root.rootLevel);
            }
            else if (button.upgradeType == UpgradeButton.UpgradeType.Tree)
            {
                button.UpdateUpgradeCostUI(LifeManager.Instance.currentLevel);
            }
            else if (button.upgradeType == UpgradeButton.UpgradeType.Touch)
            {
                button.UpdateUpgradeCostUI(DataManager.Instance.touchData.touchIncreaseLevel);
            }
        }

        foreach (var root in resourceManager.roots)
        {
            if (root.isUnlocked)
            {
                root.UpdateUI();
            }
            else
            {
                root.UpdateRootLevelUI(0, root.unlockCost);
            }
        }

        UpdateAnimalButtons();
        UpdateSkillUI();
        UpdateArtifactUI();
    }

    private void UpdateAnimalButtons()
    {
        var createAnimalButtons = UIManager.Instance.createAnimalButtons; 
        var nowAnimalCount = UIManager.Instance.createObjectButtonUnlockCount; // 현재 생성된 동물

        for (int i = 0; i < createAnimalButtons.Length; i++) 
        {
            var button = createAnimalButtons[i]; 
            var animalData = button.animalData; 

            if (animalData != null) 
            {
                button.nameText.text = animalData.animalName; // 버튼의 이름을 동물 이름으로 설정

                // 해금 상태에 따라 설명 텍스트 업데이트
                bool conditionCleared = i < nowAnimalCount - 1; 
                button.conditionText.text = conditionCleared ? "(V) " + animalData.animalUnlockConditions[0] : "(X) " + animalData.animalUnlockConditions[0];

                // 비용 텍스트 업데이트
                button.SetCostText(); 

                button.conditionCleared = conditionCleared;

                // 버튼 상호작용 가능 여부 업데이트
                button.createButton.interactable = conditionCleared && LifeManager.Instance.lifeAmount >= (BigInteger)DataManager.Instance.animalGenerateData.nowCreateCost;

                // 아이콘 클릭 가능 여부 업데이트
                button.characterIconButton.interactable = conditionCleared;
            }
        }
    }

    private void UpdateSkillUI()
    {
        if (skills != null)
        {
            foreach (var skill in skills)
            {
                skill.UpdateUI(); // 각 스킬의 UI 업데이트                
            }
        }
    }

    private void UpdateArtifactUI()
    {
        if (artifacts != null)
        {
            foreach (var artifact in artifacts)
            {
                artifact.UpdateUI(); // 각 아티팩트의 UI 업데이트                
            }
        }
    }

    private BigInteger CalculateTotalLifeIncrease(List<RootBase> roots)
    {
        BigInteger totalLifeIncrease = 0;
        foreach (var root in roots)
        {
            if (root.isUnlocked)
            {
                totalLifeIncrease += root.GetTotalLifeGeneration();
            }
        }
        return totalLifeIncrease;
    }
}