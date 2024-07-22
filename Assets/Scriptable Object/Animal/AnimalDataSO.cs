using UnityEngine;

[CreateAssetMenu(fileName = "AnimalData", menuName = "AnimalData/Default", order = 0)]
public class AnimalDataSO : ScriptableObject
{
    public int animalIndex;
    public Sprite animalIcon;
    public string animalName;
    public string[] animalUnlockConditions;
    public GameObject animalPrefab;
    public string storyText;
}
