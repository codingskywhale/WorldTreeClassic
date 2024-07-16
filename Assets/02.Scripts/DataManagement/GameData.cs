using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    public string lifeAmount;
    public string totalLifeIncrease;
    public int nowAnimalCount;
    public int maxAnimalCount;
    public int currentLevel;
    public List<RootData> roots = new List<RootData>();
    public AnimalDataSave animalData;
    public TouchDataSave touchData;
    public string lastSaveTime;
    public string lifeGenerationRatePerSecond;  // 추가된 필드
    public List<int> activeHeartBubbles = new List<int>(); // 활성화된 허트버블 인덱스 추가
    public List<int> unlockedSlots = new List<int>(); // 도감 슬롯 해금 상태 추가
}
