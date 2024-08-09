using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    [Header("Buttons")]
    public GameObject Buttons;
    public Button treeButton;
    public Button plantButton;
    public Button animalButton;
    public Button menuButton;
    public Button upgradeButton;

    [Header("SpeechBubble")]
    public GameObject speechBubbleObject;
    public Button speechBubbleButton;

    [Header("Text")]
    public TextMeshProUGUI dialogueText;
    private int nowDialogueIndex = 0;
    private List<string> dialogueTextList = new List<string>();

    [Header("ArrowTr")]
    public RectTransform lifeIncreseRatePerSecTr;
    public RectTransform animalCountTr;
    public RectTransform animalTabTr;

    [Header("Etc")]
    public Image arrow;
    public Transform createObjectButtonParentTr;

    private bool isMenuClickEnd = false;
    private bool isPlantConditionEnd = false;
    private bool isAnimalConditionEnd = false;
    private bool isAnimalTabConditionCleared = false;

    private bool isCoroutineEnd = true;

    private Coroutine ReadTextCoroutine;

    private CreateObjectButton createAnimalButton;


    private void Awake()
    {
        DialogueAddToList();
        treeButton.interactable = false;
        plantButton.interactable = false;
        animalButton.interactable = false;
        menuButton.interactable = false;
    }

    public void StartTutorial()
    {
        ReadTextCoroutine = StartCoroutine(ReadText(dialogueTextList[nowDialogueIndex]));
        createAnimalButton = createObjectButtonParentTr.GetComponentInChildren<CreateObjectButton>();
    }

    IEnumerator ReadText(string text)
    {
        isCoroutineEnd = false;
        WaitForSeconds wait = new WaitForSeconds(0.05f);
        string currentText = "";

        switch (nowDialogueIndex)
        {
            case 1:
                menuButton.interactable = true;          
                arrow.gameObject.SetActive(true);
                arrow.rectTransform.position = menuButton.GetComponent<RectTransform>().position;
                arrow.rectTransform.position += new Vector3(-35f, 115f, 0f);
                break;
            case 2:
                yield return new WaitForSeconds(0.5f);
                upgradeButton.gameObject.SetActive(true);
                upgradeButton.GetComponentInChildren<TextMeshProUGUI>().text = "해금하기";
                arrow.gameObject.SetActive(true);
                arrow.rectTransform.position = upgradeButton.GetComponent<RectTransform>().position;
                arrow.rectTransform.position += new Vector3(-125f, 125f, 0f);
                break;
            case 3:
                arrow.rectTransform.position = lifeIncreseRatePerSecTr.GetComponent<RectTransform>().position;
                arrow.rectTransform.position += new Vector3(130f, 130f, 0f);
                break;
            case 4:
                arrow.rectTransform.position = animalCountTr.GetComponent<RectTransform>().position;
                arrow.rectTransform.position += new Vector3(100f, 130f, 0f);
                break;
            case 5:
                plantButton.interactable = false;
                animalButton.interactable = true;
                arrow.rectTransform.position = animalTabTr.GetComponent<RectTransform>().position;
                arrow.rectTransform.position += new Vector3(0f, 125f, 0f);
                break;
            case 6:
                upgradeButton.gameObject.SetActive(true);
                arrow.rectTransform.position = upgradeButton.GetComponent<RectTransform>().position;
                arrow.rectTransform.position += new Vector3(-125f, 125f, 0f);
                break;
            case 7:
                arrow.gameObject.SetActive(false);
                break;
        }


        for (int i = 0; i <= text.Length; i++)
        {
            currentText = text.Substring(0, i);
            dialogueText.text = currentText;
            yield return wait;
        }
        isCoroutineEnd = true;
    }

    // 다음 설명으로 이동함.
    private void GoToNextdialogue()
    {
        if (nowDialogueIndex < dialogueTextList.Count - 1)
        {
            switch (nowDialogueIndex)
            {
                case 0:
                    break;
                //메뉴 탭을 눌러야 할때.
                case 1:
                    if (!isMenuClickEnd) return;
                    else break;
                case 2:
                    if (!isPlantConditionEnd) return;
                    else break;
                case 5:
                    if (!isAnimalTabConditionCleared) return;
                    else break;
                case 6:
                    upgradeButton.interactable = true;
                    if (!isAnimalConditionEnd) return;
                    else break;
                default:
                    break;
            }

            StopCoroutine(ReadTextCoroutine);
            ReadTextCoroutine = StartCoroutine(ReadText(dialogueTextList[++nowDialogueIndex]));
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void ClickBubbleButton()
    {
        isCoroutineEnd = true;
        StopCoroutine(ReadTextCoroutine);
        dialogueText.text = dialogueTextList[nowDialogueIndex];
    }

    public void ButtonClickedToNextDialogue()
    {
        GoToNextdialogue();
    }

    public void SetMenuConditionCleared()
    {
        isMenuClickEnd = true;
        arrow.gameObject.SetActive(false);
    }

    public void SetAnimalTabConditionCleared()
    {
        isAnimalTabConditionCleared = true;
    }

    public void SetCreateConditionCleared()
    {
        if (nowDialogueIndex < 3)
        {
            isPlantConditionEnd = true;
            upgradeButton.gameObject.SetActive(false);
        }
        else
            isAnimalConditionEnd = true;
    }

    public void CreateAnimal()
    {
        if(nowDialogueIndex > 5)
        { 
            createAnimalButton.CreateAnimalToScene();
        }
    }

    private void DialogueAddToList()
    {
        dialogueTextList.Add("세계수 키우기에 오신 것을 환영해요!");
        dialogueTextList.Add("메뉴 창을 눌러 보세요.");
        dialogueTextList.Add("식물을 구매해 세계수를 꾸며 보세요!");
        dialogueTextList.Add("식물을 구매하면 자동으로 생명력을 생산합니다.");
        dialogueTextList.Add("또한 동물들이 살 수 있는 환경을 만들어줘요.");
        dialogueTextList.Add("이번엔 동물을 구매해 볼까요?");
        dialogueTextList.Add("다람쥐를 창조해 보세요.");
        dialogueTextList.Add("동물을 창조하면 생명력 생산량이 증가합니다.");
    }
}
