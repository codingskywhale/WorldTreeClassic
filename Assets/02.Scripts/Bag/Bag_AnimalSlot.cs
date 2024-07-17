using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Bag_AnimalSlot : MonoBehaviour
{
    //한 가지의 동물 종류가 들어있는 슬롯
    public AnimalDataSO slotAnimalDataSO;

    public Image animalIcon;
    public TextMeshProUGUI explainText;

    public bool isUnlocked = false;

    private void Awake()
    {
        animalIcon.sprite = slotAnimalDataSO.animalIcon;
        //animalIcon.color = Color.black;

        explainText.text = string.Empty;
    }        

    public void SetSlotData()
    {
        animalIcon.color = Color.white;

        explainText.text = slotAnimalDataSO.animalName;
    }

    public void ClickAnimalIcon()
    {
        if (isUnlocked)
        {
            WindowsManager.Instance.animalInfoWnd.gameObject.SetActive(true);
            WindowsManager.Instance.animalInfoWnd.SetAnimalInfoWindowData(slotAnimalDataSO);
        }
    }
}
