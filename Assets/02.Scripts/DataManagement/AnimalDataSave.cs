using System.Collections.Generic;

[System.Serializable]
public class AnimalDataSave
{
    public string nowCreateCost;
    public int nowAnimalCount;
    public int maxAnimalCount;
    public List<AnimalState> animalStates = new List<AnimalState>();

    [System.Serializable]
    public class AnimalState
    {
        public string animalType;
        public float posX;
        public float posY;
        public float posZ;
    }
}
