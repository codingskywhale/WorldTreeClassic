using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

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
    public List<CreateObjectButton> createAnimalButtons;
    public int createObjectButtonUnlockCount = 0;
    private bool isCreatedButton = false;

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

    public void SetAnimalCountStatus()
    {
        status.animalCountText.text = $"{DataManager.Instance.animalGenerateData.nowAnimalCount} / {DataManager.Instance.animalGenerateData.maxAnimalCount}";
    }

    public void UpdateButtonUI()
    {
        if (!isCreatedButton) return;

        for (int i = 0; i < createAnimalButtons.Count; i++)
        {
            createAnimalButtons[i].SetCostText();
        }
        for (int i = 0; i < createAnimalButtons.Count; i++)
        {
            createAnimalButtons[i].SetCostText();
        }
    }

    public void CheckEnoughCost(BigInteger amount)
    {
        if (isCreatedButton)
        {
            // createObjectButtonUnlockCount가 현재 버튼의 인덱스를 넘는지 확인.
            if (LifeManager.Instance.lifeAmount >= (BigInteger)DataManager.Instance.animalGenerateData.nowCreateCost)
            {
                for (int i = 0; i < createObjectButtonUnlockCount; i++)
                {
                    if (i < createAnimalButtons.Count)
                        createAnimalButtons[i].createButton.interactable = true;
                    if (i < createAnimalButtons.Count)
                        createAnimalButtons[i].createButton.interactable = true;
                }
            }
            else
            {
                for (int i = 0; i < createObjectButtonUnlockCount; i++)
                {
                    createAnimalButtons[i].createButton.interactable = false;
                    if (i < createAnimalButtons.Count)
                        createAnimalButtons[i].createButton.interactable = false;
                }
            }
        }
    }

    public void CheckConditionCleared()
    {
        int clearCount = 0;
        for (int i = 0; i < createAnimalButtons.Count; i++)
        {
            if (!createAnimalButtons[i].conditionCleared)
            {
                foreach (var condition in createAnimalButtons[i].animalData.animalUnlockConditions)
                {
                    switch (condition.conditionType)
                    {
                        case UnlockConditionType.AnimalCount:

                            Dictionary<string, Dictionary<EachCountType, int>> dic = DataManager.Instance.animalGenerateData.allTypeCountDic;
                            string name = GameManager.Instance.animalDataList[condition.requiredAnimalIndex - 1].animalNameEN;
                            if (dic.ContainsKey(name) && dic[name][EachCountType.Total] >= condition.requiredAnimalCount)
                            {
                                clearCount++;
                            }

                            break;
                        case UnlockConditionType.PlantCount:

                            if (AutoObjectManagerTest.Instance.roots[condition.requiredPlantIndex - 1].rootLevel > 0)
                            {
                                clearCount++;
                            }

                            break;
                        case UnlockConditionType.LevelReached:

                            if (DataManager.Instance.touchData.touchIncreaseLevel > condition.requiredWorldTreeLevel)
                            {
                                clearCount++;
                            }

                            break;
                    }
                }

                if (clearCount == createAnimalButtons[i].animalData.animalUnlockConditions.Length)
                {
                    createAnimalButtons[i].conditionCleared = true;
                    createAnimalButtons[i].SetLockImageOff();
                    createObjectButtonUnlockCount++;
                }

                clearCount = 0;
            }
        }
    }

    public void CreateAnimalButtons()
    {
        // 동물 데이터를 기반으로 동물 생성 버튼을 만들어준다. \
        // 트랜스폼을 캔버스의 해당 위치로 설정하자.
        GameObject go;
        for (int i = 0; i < GameManager.Instance.animalDataList.Count; i++)
        {
            go = Instantiate(CreateObjectButton);
            CreateObjectButton button;
            button = go.GetComponent<CreateObjectButton>();
            button.buttonIndex = i;
            button.InitailizeSet(GameManager.Instance.animalDataList[i]);
            createAnimalButtons.Add(button);
            go.transform.SetParent(CreateObjectButtonTr);
            go.transform.localScale = new UnityEngine.Vector3(1, 1, 1);
        } 

        isCreatedButton = true;
        UnlockButton();
    }

    public void UnlockButton()
    {
        for (int i = 0; i < createObjectButtonUnlockCount; i++)
        {
            createAnimalButtons[i].conditionCleared = true;
            createAnimalButtons[i].SetLockImageOff();
        }
    }

    public void LoadAnimalBuyStatus()
    {
        foreach (var kvp in DataManager.Instance.animalGenerateData.allTypeCountDic)
        {
            for(int i = 0; i < GameManager.Instance.animalDataList.Count; i++)
            {
                // 동물 이름과 애니멀 데이터가 일치한다면? 해당 애니멀 데이터에 해당하는 버튼을 활성화 시켜준다.
                if (kvp.Key == GameManager.Instance.animalDataList[i].animalNameEN)
                {
                    createAnimalButtons[i].isBuyAnimal = true;
                }
            }
        }
    }
    }
