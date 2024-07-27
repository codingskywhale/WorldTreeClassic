using UnityEngine;

public enum UnlockConditionType
{
    AnimalCount,
    PlantCount,
    LevelReached
}

[System.Serializable]
public class UnlockCondition
{
    public UnlockConditionType conditionType;
    public string targetName; // 동물명 또는 식물명
    public int requiredAnimalIndex;
    public int requiredAnimalCount;
    public int requiredPlantIndex;
    public int requiredWorldTreeLevel;

    public string ShowCondition()
    {
        switch(conditionType)
        {
            case UnlockConditionType.AnimalCount:
                return $"{targetName} x {requiredAnimalCount}\n";
            case UnlockConditionType.PlantCount:
                return $"식물 {requiredPlantIndex + 1}단계 해금\n";
            case UnlockConditionType.LevelReached:
                return $"세계수 {requiredWorldTreeLevel}레벨\n";
            default:
                return "조건 없음";
        }
    }
}
