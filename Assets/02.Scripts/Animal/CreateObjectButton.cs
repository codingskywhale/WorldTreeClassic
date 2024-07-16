using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Numerics;
using Vector3 = UnityEngine.Vector3;
using UnityEditor.Playables;
using System.Linq;
using Unity.VisualScripting;

public class CreateObjectButton : MonoBehaviour
{
    public AnimalDataSO animalData;
    public Image characterIcon;
    public Button characterIconButton;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI conditionText;
    // 생명 창조 ~~ cost
    public TextMeshProUGUI inButtonCostText;
    public Button createButton;
    public Transform animalSpawnTr;

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
        SetButtonLock();
        UIManager.Instance.UpdateButtonUI();
    }

    public void ClickCreateAnimal(int buttonIdx)
    {
        // 현재 생명력이 요구치보다 높을 때.
        if (LifeManager.Instance.lifeAmount >= (BigInteger)DataManager.Instance.animalGenerateData.nowCreateCost)
        {
            LifeManager.Instance.DecreaseWater(DataManager.Instance.animalGenerateData.nowCreateCost);

            // UnlockCount는 시작할 때 1이기 때문.
            if (buttonIdx + 1 == UIManager.Instance.createObjectButtonUnlockCount)
            {
                // 다음 걸 해금해 주어야 한다.
                UIManager.Instance.createObjectButtonUnlockCount++;
                characterIconButton.interactable = true;
                CheckConditionCleared(buttonIdx + 1);
                //해당 버튼에 대응되는 동물을 해금시켜준다.
                UIManager.Instance.bag.UnlockSlot(buttonIdx);
            }


            // 동물을 추가할 여유 공간이 있을 때
            if (DataManager.Instance.animalGenerateData.AddAnimal())
            {
                GameObject go = Instantiate(animalData.animalPrefab, animalSpawnTr);
                DataManager.Instance.spawnData.AddAnimalSpawnData(go, animalData);

                // 하트 버블 리스트에 추가
                LifeManager.Instance.bubbleGenerator.AddAnimalHeartBubbleList(go.GetComponent<Animal>().heart);

                if(DataManager.Instance.animalGenerateData.nowAnimalCount == 1 || DataManager.Instance.animalGenerateData.nowAnimalCount == 2)
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
            // 생산량 2배 증가.
            DataManager.Instance.touchData.ApplyIncreaseRate(1);
            LifeManager.Instance.ApplyIncreaseRateToAllRoots(1);

            UIManager.Instance.CheckEnoughCost(0);
            UIManager.Instance.UpdateButtonUI();
        }
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

    // 뭔가 버튼을 누르는 이벤트가 일어났을 때?
    // 어떠한 조건이 일어났을 때 확인
    public void CheckConditionCleared(int buttonIdx)
    {
        UIManager.Instance.createAnimalButtons[buttonIdx].conditionText.text = conditionV + animalData.animalUnlockConditions[0];
    }

    public void ClickAnimalIcon()
    {
        WindowsManager.Instance.animalInfoWnd.SetAnimalInfoWindowData(animalData);
    }
}
