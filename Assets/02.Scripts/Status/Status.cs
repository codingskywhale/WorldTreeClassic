using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Status : MonoBehaviour
{
    public TextMeshProUGUI waterText;
    public TextMeshProUGUI lifeIncreaseText;
    public TextMeshProUGUI animalCountText;
    public void UpdateLifeUI(float waterAmount, int waterNeededForCurrentLevel)
    {
        waterText.text = $" 생명력 : {waterAmount}";
    }

    public void UpdateLifeIncreaseUI(float totalLifeIncrease)
    {
        lifeIncreaseText.text = "Life Increase Per Second: " + totalLifeIncrease.ToString();
    }

    public void UpdateAnimalCountText(int nowAnimalCount, int maxAnimalCount)
    {
        animalCountText.text = $"{nowAnimalCount} / {maxAnimalCount}";
    }
}
