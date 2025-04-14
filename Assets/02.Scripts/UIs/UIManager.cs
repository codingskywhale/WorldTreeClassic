using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [Header("Datas")]
    public Status status;
    public FlowerBase flower;
    public TouchData touchData;
    public WorldTree tree;
    public Bag bag;

    [Header("CreateObjectButton")]
    public GameObject CreateObjectButton;
    public Transform CreateObjectButtonTr;
    public List<CreateObjectButton> createAnimalButtons;
    public int createObjectButtonUnlockCount = 0;
    private bool isCreatedButton = false;

    private void Start()
    {
        LifeManager.Instance.OnLifeChanged += CheckEnoughCost;
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
            Debug.Log($"[{i}] conditionCleared: {createAnimalButtons[i].conditionCleared}");

            if (!createAnimalButtons[i].conditionCleared)
            {
                var conditions = createAnimalButtons[i].animalData.animalUnlockConditions;
                Debug.Log($"[Check] Button {i}: 조건 개수 = {conditions.Length}");

                foreach (var condition in conditions)
                {
                    switch (condition.conditionType)
                    {
                        case UnlockConditionType.AnimalCount:
                            Dictionary<string, Dictionary<EachCountType, int>> dic = DataManager.Instance.animalGenerateData.allTypeCountDic;
                            string name = GameManager.Instance.animalDataList[condition.requiredAnimalIndex].animalNameEN;

                            Debug.Log($"[Check] AnimalCount 조건 확인 중 - 대상: {name}, 필요 수량: {condition.requiredAnimalCount}");

                            if (dic.ContainsKey(name))
                            {
                                int current = dic[name][EachCountType.Total];
                                Debug.Log($"[Check] 현재 {name} 수량: {current}");

                                if (current >= condition.requiredAnimalCount)
                                {
                                    Debug.Log($"[UnlockCheck] AnimalCount 조건 만족 - {name} ({current}/{condition.requiredAnimalCount})");
                                    clearCount++;
                                }
                                else
                                {
                                    Debug.Log($"[UnlockCheck] AnimalCount 조건 불충족 - {name} ({current}/{condition.requiredAnimalCount})");
                                }
                            }
                            else
                            {
                                Debug.LogWarning($"[Check] allTypeCountDic에 {name} 없음!");
                            }

                            break;

                        case UnlockConditionType.PlantCount:
                            int index = condition.requiredPlantIndex;
                            if (index >= 0 && index < AutoObjectManager.Instance.flowers.Length)
                            {
                                int level = AutoObjectManager.Instance.flowers[index].flowerLevel;
                                Debug.Log($"[Check] Plant 조건 확인 - index: {index}, 현재 레벨: {level}");

                                if (level > 0)
                                {
                                    Debug.Log($"[UnlockCheck] PlantCount 조건 만족");
                                    clearCount++;
                                }
                                else
                                {
                                    Debug.Log($"[UnlockCheck] PlantCount 조건 불충족");
                                }
                            }
                            else
                            {
                                Debug.LogWarning($"[CheckConditionCleared] 잘못된 requiredPlantIndex: {index}, flowers.Length: {AutoObjectManager.Instance.flowers.Length}");
                            }

                            break;

                        case UnlockConditionType.LevelReached:
                            int treeLevel = DataManager.Instance.touchData.touchIncreaseLevel;
                            Debug.Log($"[Check] WorldTree 조건 확인 - 현재 레벨: {treeLevel}, 필요 레벨: {condition.requiredWorldTreeLevel}");

                            if (treeLevel > condition.requiredWorldTreeLevel)
                            {
                                Debug.Log($"[UnlockCheck] LevelReached 조건 만족");
                                clearCount++;
                            }
                            else
                            {
                                Debug.Log($"[UnlockCheck] LevelReached 조건 불충족");
                            }

                            break;
                    }
                }

                Debug.Log($"[Check] Button {i}: clearCount = {clearCount}, 필요 조건 수 = {createAnimalButtons[i].animalData.animalUnlockConditions.Length}");

                if (clearCount == createAnimalButtons[i].animalData.animalUnlockConditions.Length)
                {
                    Debug.Log($"[Unlock] Button {i} 해제됨!");
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
            createAnimalButtons[i].characterIconButton.interactable = true;
            createAnimalButtons[i].SetLockImageOff();

            if (DataManager.Instance.animalGenerateData.allTypeCountDic.ContainsKey(createAnimalButtons[i].animalData.animalNameEN))
            {
                createAnimalButtons[i].AnimalCountObject.SetActive(true);
                createAnimalButtons[i].AnimalCountText.text = ($"{DataManager.Instance.animalGenerateData.allTypeCountDic[createAnimalButtons[i].animalData.animalNameEN][EachCountType.Active]} / {DataManager.Instance.animalGenerateData.allTypeCountDic[createAnimalButtons[i].animalData.animalNameEN][EachCountType.Total]}");
            }
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
