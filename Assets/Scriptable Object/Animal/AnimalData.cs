using System;
using UnityEngine;

[Serializable]
public class AnimalData
{
    public int animalIndex;
    public Sprite animalIcon;
    public string animalNameEN;
    public string animalNameKR;
    public UnlockCondition[] animalUnlockConditions;
    public GameObject animalPrefab;
    public string storyText;
}
