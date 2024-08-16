using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FlowerDataSO", menuName = "FlowerData/Default", order = 0)]
public class FlowerDataSO : ScriptableObject
{
    public int unlockThreshold;
    public string baseLifeGenerationString;
    public string unlockCostString;
    public string unlockConditionText; // 잠금 해제 조건 텍스트 추가
    public int requiredOfflineRewardSkillLevel; // 오프라인 보상 스킬 해금 조건 레벨 추가
    public int skillCoolDownReductionLevel;
}
