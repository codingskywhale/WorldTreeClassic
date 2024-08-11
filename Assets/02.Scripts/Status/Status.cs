using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;

public class Status : MonoBehaviour
{
    public TextMeshProUGUI waterText;
    public TextMeshProUGUI lifeIncreaseText;
    public TextMeshProUGUI animalCountText;
    public TextMeshProUGUI diamondAmountText; // 다이아몬드 UI 요소 추가
    public void UpdateLifeUI(BigInteger waterAmount, BigInteger waterNeededForCurrentLevel)
    {        
        waterText.text = $"{BigIntegerUtils.FormatBigInteger(waterAmount)}";
    }

    public void UpdateLifeIncreaseUI(BigInteger totalLifeIncrease)
    {        
        lifeIncreaseText.text = $"{BigIntegerUtils.FormatBigInteger(totalLifeIncrease)} /s";
    }

    public void UpdateAnimalCountText(int nowAnimalCount, int maxAnimalCount)
    {        
        animalCountText.text = $"{nowAnimalCount} / {maxAnimalCount}";
    }

    public void UpdateDiamondUI(BigInteger diamondAmount)
    {
        diamondAmountText.text = $"{BigIntegerUtils.FormatBigInteger(diamondAmount)}";
    }
}
