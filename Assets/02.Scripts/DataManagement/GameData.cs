using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    public string lifeAmount;
    public string totalLifeIncrease;
    public int nowAnimalCount;
    public int maxAnimalCount;
    public int currentLevel;
    public List<FlowerDataSave> flowers = new List<FlowerDataSave>();
    public AnimalDataSave animalData;
    public TouchDataSave touchData;
    public string lastSaveTime;
    public string lastSkillSaveTime;
    public string lifeGenerationRatePerSecond;  // 추가된 필드
    public List<int> activeHeartBubbles = new List<int>(); // 활성화된 허트버블 인덱스 추가
    public SerializableDictionary<string, SerializableDictionary<EachCountType, int>> allTypeCountDic = new SerializableDictionary<string, SerializableDictionary<EachCountType, int>>();
    public int createObjectButtonUnlockCount = 0;
    public List<SkillDataSave> skillDataList = new List<SkillDataSave>();
    public List<ArtifactDataSave> artifactDataList = new List<ArtifactDataSave>();
}
