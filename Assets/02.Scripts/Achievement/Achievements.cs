using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Achievements : MonoBehaviour
{
    //public Achievement_Tab[] achievements;

    public TextMeshProUGUI worldTreeLevel;

    private void OnEnable()
    {
        UIManager.Instance.touchData.OnworldTreeLevel += UpdateAchievement;
    }

    private void OnDisable()
    {
        UIManager.Instance.touchData.OnworldTreeLevel -= UpdateAchievement;
    }

    public void UpdateAchievement(int value)
    {
        worldTreeLevel.text = "세계수 100레벨 달성하기" + $"현재{value}";
    }
}
