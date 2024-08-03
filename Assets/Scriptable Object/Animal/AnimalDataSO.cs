using UnityEngine;

[CreateAssetMenu(fileName = "AnimalData", menuName = "AnimalData/Default", order = 0)]
public class AnimalDataSO : ScriptableObject
{
    public int animalIndex;
    public Sprite animalIcon;
    public string animalName;
    public string animalNameEN;
    public string animalNameKR;
    public UnlockCondition[] animalUnlockConditions;
    public GameObject animalPrefab;
    public string storyText;
    public string fullStoryText;
}
