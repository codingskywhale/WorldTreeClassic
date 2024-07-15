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
}
