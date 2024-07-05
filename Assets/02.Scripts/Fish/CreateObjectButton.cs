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
        //if (LifeManager.Instance.HasSufficientWater())
        //{
        //    // 구매 비용 적용 LifeManager.DecreaseWater(구매 비용);
        //}
        // 가격 적용 필요

        GameObject go = Instantiate(animalData.animalPrefab);
        go.transform.position = (new Vector3(0, 0.5f, 10f));
        LifeManager.Instance.touchData.ApplyIncreaseRate(1f);
        LifeManager.Instance.ApplyIncreaseRateToAllRoots(1f);

    }
}
