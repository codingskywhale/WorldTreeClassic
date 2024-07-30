using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; } // 싱글톤 인스턴스

    [Header("Datas")]
    public Status status;
    public RootBase root;
    public TouchData touchData;
    public WorldTree tree;
    public Bag bag;

    [Header("CreateObjectButton")]
    public GameObject CreateObjectButton;
    public Transform CreateObjectButtonTr;
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
        SetAnimalCountStatus();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Q))
        {

            CreateAnimalButtons();
        }
    }

    public void SetAnimalCountStatus()
    {
        status.animalCountText.text = $"{DataManager.Instance.animalGenerateData.nowAnimalCount} / {DataManager.Instance.animalGenerateData.maxAnimalCount}";
    }

    public void UpdateButtonUI()
    {
        for (int i = 0; i < createAnimalButtons.Length; i++)
        {
            createAnimalButtons[i].SetCostText();
        }
    }

    public void CheckEnoughCost(BigInteger amount)
    {
        // createObjectButtonUnlockCount가 현재 버튼의 인덱스를 넘는지 확인.
        if (LifeManager.Instance.lifeAmount >= (BigInteger)DataManager.Instance.animalGenerateData.nowCreateCost)
        {
            for(int i = 0; i < createObjectButtonUnlockCount; i++)
            {
                if(i < createAnimalButtons.Length)
                    createAnimalButtons[i].createButton.interactable = true;
            }
        }
        else
        {
            for (int i = 0; i < createObjectButtonUnlockCount; i++)
            {
                createAnimalButtons[i].createButton.interactable = false;
            }
        }
    }

    public void CheckConditionCleared()
    {
        int clearCount = 0;
        for (int i = 3; i < createAnimalButtons.Length; i++)
        {
            if (!createAnimalButtons[i].conditionCleared)
            {
                foreach (var condition in createAnimalButtons[i].animalData.animalUnlockConditions)
                {
                    switch (condition.conditionType)
                    {                      
                        case UnlockConditionType.AnimalCount:

                            Dictionary<string, Dictionary<EachCountType, int>> dic = DataManager.Instance.animalGenerateData.allTypeCountDic;
                            string name = GameManager.Instance.animalDataList[condition.requiredAnimalIndex].animalName;
                            if (dic.ContainsKey(name) && dic[name][EachCountType.Total] >= condition.requiredAnimalCount)
                            {
                                clearCount++;
                                Debug.Log($"{createAnimalButtons[i].animalData.animalName} 버튼 동물 조건 충족 완료 ");
                            }

                            break;
                        case UnlockConditionType.PlantCount:

                            if (AutoObjectManager.Instance.roots[condition.requiredPlantIndex].isUnlocked)
                            {
                                clearCount++;
                                Debug.Log($"{createAnimalButtons[i].animalData.animalName} 버튼 식물 조건 충족 완료 ");
                            }

                            break;
                        case UnlockConditionType.LevelReached:

                            if (DataManager.Instance.touchData.touchIncreaseLevel > condition.requiredWorldTreeLevel)
                            {
                                clearCount++;
                                Debug.Log($"{createAnimalButtons[i].animalData.animalName} 버튼 레벨 조건 충족 완료 ");
                            }

                            break;
                    }
                }

                if (clearCount == createAnimalButtons[i].animalData.animalUnlockConditions.Length)
                {
                    createAnimalButtons[i].conditionCleared = true;
                    createAnimalButtons[i].SetLockImageOff();
                    Debug.Log($"{createAnimalButtons[i].animalData.animalName} 락 해제 완료 {createAnimalButtons[i].animalData.animalUnlockConditions.Length}, {clearCount} 개의 조건을 수행함");
                }
                
                clearCount = 0;
            }
        }
    }

    public void CreateAnimalButtons()
    {
        // 동물 데이터를 기반으로 동물 생성 버튼을 만들어준다. \
        // 트랜스폼을 캔버스의 해당 위치로 설정하자.
        GameObject go = Instantiate(CreateObjectButton, CreateObjectButtonTr);
        CreateObjectButton button = go.AddComponent<CreateObjectButton>();

        for (int i = 0; i < GameManager.Instance.animalDataList.Count; i++)
        {
            button.InitailizeSet(GameManager.Instance.animalDataList[i]);

        }
    }
}
