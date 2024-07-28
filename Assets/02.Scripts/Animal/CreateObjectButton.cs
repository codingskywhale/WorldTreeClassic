using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateObjectButton : MonoBehaviour
{
    [Header("Animal Basic Datas")]
    public AnimalDataSO animalData;
    public Image characterIcon;
    public Button characterIconButton;

    [Header("UIs")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI conditionText;
    // 생명 창조 ~~ cost
    public TextMeshProUGUI inButtonCostText;
    public Button createButton;

    public Transform animalSpawnTr;

    [Header("Unlock Image")]
    public GameObject lockImage;
    public TextMeshProUGUI lockConditionText;

    private int buttonIndex;

    private string conditionX = "(X) ";
    private string conditionV = "(V) ";

    int count = 0;
    public bool conditionCleared = false;

    private void Awake()
    {
        InitailizeSet();
    }
    private void InitailizeSet()
    {
        nameText.text = animalData.animalName;
        characterIcon.sprite = animalData.animalIcon;
        if (lockImage != null)
        {
            lockConditionText.text = GetConditions();
        }

        for (int i = 0; i < UIManager.Instance.createObjectButtonUnlockCount; i++)
            UIManager.Instance.CheckConditionCleared();

        UIManager.Instance.UpdateButtonUI();
    }

    public void ClickCreateAnimal(int buttonIdx)
    {
        // 현재 생명력이 요구치보다 높을 때.
        if (LifeManager.Instance.lifeAmount >= (BigInteger)DataManager.Instance.animalGenerateData.nowCreateCost)
        {
            WindowsManager.Instance.animalInfoWnd.createObjectButton = this;
            WindowsManager.Instance.animalInfoWnd.animalImage.sprite = animalData.animalIcon;
            buttonIndex = buttonIdx;

            WindowsManager.Instance.animalInfoWnd.SetBasicData(animalData.animalName);
        }
    }

    public void CreateAnimalToScene()
    {
        LifeManager.Instance.DecreaseWater(DataManager.Instance.animalGenerateData.nowCreateCost);

        AddAnimal();

        UnlockButton(buttonIndex);
    }

    public void UnlockButton(int buttonIdx)
    {
        // UnlockCount는 시작할 때 1이기 때문.
        if (buttonIdx + 1 == UIManager.Instance.createObjectButtonUnlockCount)
        {
            // 다음 걸 해금해 주어야 한다.
            UIManager.Instance.createObjectButtonUnlockCount++;
            characterIconButton.interactable = true;

            if (UIManager.Instance.createAnimalButtons.Length > buttonIdx + 1)
                UIManager.Instance.CheckConditionCleared();

        }
        //해당 버튼에 대응되는 동물을 해금시켜준다.
        UIManager.Instance.bag.UnlockSlot(buttonIdx);
    }

    public void AddAnimal()
    {
        // 동물을 추가할 여유 공간이 있을 때
        if (DataManager.Instance.animalGenerateData.AddAnimal(true))
        {
            GameObject go = Instantiate(animalData.animalPrefab, animalSpawnTr);
            DataManager.Instance.spawnData.AddAnimalSpawnData(go, animalData);

            // 하트 버블 리스트에 추가
            LifeManager.Instance.bubbleGenerator.AddAnimalHeartBubbleList(go.GetComponent<Animal>().heart);

            if (DataManager.Instance.animalGenerateData.nowAnimalCount == 1 || DataManager.Instance.animalGenerateData.nowAnimalCount == 2)
            {
                LifeManager.Instance.bubbleGenerator.GenerateNewHeart();
            }

            DataManager.Instance.animalGenerateData.AddAnimalToDictionary(animalData.animalName, true);
        }

        // 여유 공간이 없을 때
        else
        {
            // 가방으로 이동하도록 해야함
            DataManager.Instance.animalGenerateData.AddAnimalToDictionary(animalData.animalName, false);
        }

        DataManager.Instance.bag.UpdateSlotDataUI(buttonIndex);
        // 생산량 2배 증가.
        DataManager.Instance.touchData.ApplyIncreaseRate(1);
        LifeManager.Instance.ApplyIncreaseRateToAllRoots(1);
        UIManager.Instance.status.UpdateLifeIncreaseUI(ResourceManager.Instance.GetTotalLifeGenerationPerSecond());

        UIManager.Instance.CheckEnoughCost(0);
        UIManager.Instance.UpdateButtonUI();

        UIManager.Instance.CheckConditionCleared();
    }
    // 모든 버튼에 적용 시켜야함
    public void SetCostText()
    {
        inButtonCostText.text = (BigIntegerUtils.FormatBigInteger(DataManager.Instance.animalGenerateData.nowCreateCost)).ToString();
    }

    // 초기 잠김 기능을 처리할 수 있는 메서드
    public void SetButtonLock()
    {
        characterIconButton.interactable = false;
        createButton.interactable = false;
        inButtonCostText.text = "잠김";
    }

    public string GetConditions()
    {
        string conditions = "";
        foreach (var condition in animalData.animalUnlockConditions)
        {
            conditions += condition.ShowCondition();
        }
        if (conditions.Length == 0) conditions = "조건 없음";

        return conditions;
    }

    public void ClickAnimalIcon()
    {
        WindowsManager.Instance.animalInfoWnd.SetAnimalInfoWindowData(animalData);
        WindowsManager.Instance.animalInfoWnd.ChangeBottomUI(true);
    }

    public void SetLockImageText()
    {

    }

    public void SetLockImageOff()
    {
        characterIconButton.interactable = true;
        lockImage.SetActive(false);
    }

    public CreateObjectButton GetNextButton()
    {
        if (buttonIndex <= UIManager.Instance.createAnimalButtons.Length)
            return UIManager.Instance.createAnimalButtons[buttonIndex + 1];

        else
            return null;
    }
}
