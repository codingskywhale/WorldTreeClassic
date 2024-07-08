using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateObjectButton : MonoBehaviour
{
    public AnimalDataSO animalData;
    public Image characterIcon;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI conditionText;
    // 생명 창조 ~~ cost
    public TextMeshProUGUI inButtonCostText;
    public Button createButton;

    private string conditionX = "(X) ";
    private string conditionV = "(V) ";

    private void Awake()
    {
        InitailizeSet();
    }
    private void InitailizeSet()
    {
        characterIcon.sprite = animalData.animalIcon;
        nameText.text = animalData.animalName;
        conditionText.text = conditionX + animalData.animalUnlockConditions[0];
        // 일단 첫 번째 버튼은 해금된 상태여야 함.
        CheckConditionCleared(0);
        UIManager.Instance.UpdateButtonUI();

        SetButtonLock();
    }

    public void ClickCreateAnimal(int buttonIdx)
    {
        // 현재 생명력이 요구치보다 높을 때.
        if (LifeManager.Instance.lifeAmount > LifeManager.Instance.animalData.nowCreateCost)
        {
            LifeManager.Instance.DecreaseWater((int)LifeManager.Instance.animalData.nowCreateCost);

            // 동물을 추가할 여유 공간이 있을 때
            if (LifeManager.Instance.animalData.AddAnimal())
            {
                GameObject go = Instantiate(animalData.animalPrefab);
                go.transform.position = (new Vector3(0, 0.5f, 10f));
            }
            // 생산량 2배 증가.
            LifeManager.Instance.touchData.ApplyIncreaseRate(1f);
            LifeManager.Instance.rootData.ApplyIncreaseRate(1f);
            // UI 적용
            
            // UnlockCount는 시작할 때 1이기 때문.
            if(buttonIdx + 1 == UIManager.Instance.createObjectButtonUnlockCount)
            {
                // 다음 걸 해금해 주어야 한다.
                CheckConditionCleared(buttonIdx + 1);
                UIManager.Instance.createObjectButtonUnlockCount++;
            }
            UIManager.Instance.UpdateButtonUI();
        }
    }

    // 모든 버튼에 적용 시켜야함
    public void SetCostText()
    {
        inButtonCostText.text = LifeManager.Instance.animalData.nowCreateCost.ToString();
    }

    // 잠김 기능을 처리할 수 있는 메서드
    public void SetButtonLock()
    {
        for(int i = 0; i< UIManager.Instance.createAnimalButtons.Length; i++)
        {
            if (i > UIManager.Instance.createObjectButtonUnlockCount)
            {
                createButton.interactable = false;
                inButtonCostText.text = "잠김";
            }
        }
    }

    // 뭔가 버튼을 누르는 이벤트가 일어났을 때?
    // 어떠한 조건이 일어났을 때 확인
    public void CheckConditionCleared(int buttonIdx)
    {
        UIManager.Instance.createAnimalButtons[buttonIdx].conditionText.text = conditionV + animalData.animalUnlockConditions[0];
    }
}
