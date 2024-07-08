using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; } // 싱글톤 인스턴스

    public Status status;
    public Root root;
    public TouchData touchData;
    public Spirit spiritData;
    public WorldTree tree;
    public CreateObjectButton[] createAnimalButtons;
    public int createObjectButtonUnlockCount = 1;

    private void Awake()
    {
        // 싱글톤 인스턴스 설정
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 인스턴스가 파괴되지 않도록 설정
        }
        else
        {
            Destroy(gameObject); // 이미 인스턴스가 존재하면 중복 생성된 객체 파괴
        }

        
    }

    private void Start()
    {
        LifeManager.Instance.OnWaterChanged += CheckEnoughCost;
    }

    public void UpdateButtonUI()
    {
        for (int i = 0; i < createObjectButtonUnlockCount; i++)
        {
            createAnimalButtons[i].SetCostText();
        }
    }

    public void CheckEnoughCost(float amount = 0f)
    {
        // createObjectButtonUnlockCount가 현재 버튼의 인덱스를 넘는지 확인.
        if (LifeManager.Instance.lifeAmount > LifeManager.Instance.animalData.nowCreateCost)
        {
            for(int i = 0; i < createObjectButtonUnlockCount; i++)
            {
                createAnimalButtons[i].createButton.interactable = true;
            }
        }
        else
        {
            foreach (CreateObjectButton button in createAnimalButtons)
            {
                button.createButton.interactable = false;
            }
        }
    }
}
