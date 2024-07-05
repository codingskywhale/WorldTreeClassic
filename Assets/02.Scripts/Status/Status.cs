using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Status : MonoBehaviour
{
    public TextMeshProUGUI waterText;
    public TextMeshProUGUI lifeIncreaseText;


    public void UpdateLifeUI(float waterAmount, int waterNeededForCurrentLevel)
    {
        waterText.text = $" 생명력 : {waterAmount}";
    }

    public void UpdateLifeIncreaseUI(int totalLifeIncrease)
    {
        lifeIncreaseText.text = "Life Increase Per Second: " + totalLifeIncrease.ToString();
    }
}
