using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Bag_AnimalSlot : MonoBehaviour
{
    //한 가지의 동물 종류가 들어있는 슬롯
    public AnimalDataSO slotAnimalDataSO;

    public Image animalIcon;
    public TextMeshProUGUI explainText;
    public Button slotButton;

    public bool isUnlocked = false;

    private void Awake()
    {
        slotButton = GetComponent<Button>();
        if (!isUnlocked)
        {
            animalIcon.color = Color.black;

            slotButton.interactable = false;
            explainText.text = string.Empty;
        }
    }        
     
    public void SetSlotData()
    {
        animalIcon.color = Color.white; 
        slotButton.interactable = true;

        explainText.text = $"{DataManager.Instance.animalGenerateData.allTypeCountDic[slotAnimalDataSO.animalName][EachCountType.Active]} " +
                            $"/ {DataManager.Instance.animalGenerateData.allTypeCountDic[slotAnimalDataSO.animalName][EachCountType.Total]}";
    }

    public void ClickAnimalIcon()
    {
        if (isUnlocked)
        {
            WindowsManager.Instance.animalInfoWnd.gameObject.SetActive(true);
            WindowsManager.Instance.animalInfoWnd.SetAnimalInfoWindowData(slotAnimalDataSO);
        }
    }

    public void UpdateUI()
    {
        explainText.text = $"{DataManager.Instance.animalGenerateData.allTypeCountDic[slotAnimalDataSO.animalName][EachCountType.Active]} " +
                            $"/ {DataManager.Instance.animalGenerateData.allTypeCountDic[slotAnimalDataSO.animalName][EachCountType.Total]}";
    }
}
