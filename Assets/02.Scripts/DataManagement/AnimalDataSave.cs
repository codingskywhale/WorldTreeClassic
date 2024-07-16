using System.Collections.Generic;

[System.Serializable]
public class AnimalDataSave
{
    public string nowCreateCost;
    public int nowAnimalCount;
    public int maxAnimalCount;
    public List<AnimalState> animalStates = new List<AnimalState>();
    public Dictionary<string, Dictionary<EachCountType, int>> animalTypeCount;

    [System.Serializable]
    public class AnimalState
    {
        public int animalIndex;
        public float posX;
        public float posY;
        public float posZ;
    }
}