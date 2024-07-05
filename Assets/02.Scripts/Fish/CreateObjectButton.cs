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

    public void ClickCreateAnimal()
    {
        // 가격 적용 필요
        if(LifeManager.Instance.lifeAmount >= 0)
        {
            GameObject go = Instantiate(animalData.animalPrefab);
            go.transform.position = (new Vector3(0, 0.5f, 10f));
            LifeManager.Instance.touchData.ApplyIncreaseRate(1f);
            //LifeManager.Instance.spiritData.ApplyIncreaseRate();
        }
    }
}
