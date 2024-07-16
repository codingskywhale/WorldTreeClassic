using System.Collections.Generic;

[System.Serializable]
public class AnimalDataSave
{
    public string nowCreateCost;
    public int nowAnimalCount;
    public int maxAnimalCount;
    public List<AnimalState> animalStates = new List<AnimalState>();
    public List<SerializedAnimalTypeCount> animalTypeCountSerialized;

    [System.Serializable]
    public class AnimalState
    {
        public string uniqueID;
        public int animalIndex;
        public float posX;
        public float posY;
        public float posZ;
    }

    [System.Serializable]
    public class SerializedAnimalTypeCount
    {
        public string AnimalName;
        public int Total;
        public int Active;
        public int Stored;
    }
}