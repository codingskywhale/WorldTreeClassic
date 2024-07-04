using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateObjectButton : MonoBehaviour
{
    public AnimalDataSO animalData;
    public Image characterIcon;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI conditionText;
    public TextMeshProUGUI inButtonCostText;

    private void Awake()
    {
        InitailizeSet();
    }
    private void InitailizeSet()
    {
        characterIcon.sprite = animalData.animalIcon;
        nameText.text = animalData.animalName;
        conditionText.text = animalData.animalUnlockConditions[0];
    }
}
