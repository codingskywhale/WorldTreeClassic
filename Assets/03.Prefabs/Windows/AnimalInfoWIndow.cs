using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AnimalInfoWindow : MonoBehaviour
{
    public GameObject windowPopUP;
    // 동물에 관한 정보들을 담아놓은 창.
    // 동물 관련 UI 및 기능들을 담을 예정.
    public int totalGeneratedCounts;
    public int totalArrangedCount;
    public int totalStoredCount;

    public int animalGenerateCount = 1;

    private AnimalDataSO nowAnimaldataSO;
    public Image animalImage;

    [Header("Bag UI")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI totalGeneratedCountText;
    public TextMeshProUGUI totalActiveCountText;
    public TextMeshProUGUI totalStoredCountText;
    public Button storyButton;

    [Header("Create Animal UI")]
    public TextMeshProUGUI animalGenerateCountText;
    public TextMeshProUGUI createCostText;

    [Header("Create Animal Result UI")]
    public GameObject CreateResultUI;
    public GameObject CreateAnimalCountUI;
    public TextMeshProUGUI CreatedAnimalNameText;
    public TextMeshProUGUI CreatedAnimalCountText;
    public TextMeshProUGUI previousGetText;
    public TextMeshProUGUI currentGetText;

    [Header("UI GameObject")]
    public GameObject centerUIs;
    public GameObject BagBottomUI;
    public GameObject CreateAnimalBottomUI;
    public GameObject StoryCenterUI;

    public CreateObjectButton createObjectButton;

    private BigInteger totalGenerateValue;
    private BigInteger preCost;

    private void OnEnable()
    {
        animalGenerateCount = 1;
        animalGenerateCountText.text = animalGenerateCount.ToString();
        createCostText.text = BigIntegerUtils.FormatBigInteger(DataManager.Instance.animalGenerateData.nowCreateCost);
    }
    private void Start()
    {
        UpdateCreateCost();
    }
    // 이미지를 눌렀을 때 SetAnimalInfoWindowData 를 해주면 된다.
    public void SetAnimalInfoWindowData(AnimalDataSO dataSO)
    {
        nowAnimaldataSO = dataSO;
        animalImage.sprite = nowAnimaldataSO.animalIcon;
        nameText.text = nowAnimaldataSO.animalNameKR;
        if (BagBottomUI.activeInHierarchy)
        {
            totalGeneratedCountText.text = DataManager.Instance.animalGenerateData.allTypeCountDic[dataSO.animalNameEN][EachCountType.Total].ToString();
            totalActiveCountText.text = DataManager.Instance.animalGenerateData.allTypeCountDic[dataSO.animalNameEN][EachCountType.Active].ToString();
            totalStoredCountText.text = DataManager.Instance.animalGenerateData.allTypeCountDic[dataSO.animalNameEN][EachCountType.Stored].ToString();
        }
        // 총 생산 데이터는 SO에 담지 말자.
        // 해당 동물을 식별할 수 있는 데이터를 AnimalDataSO에 넣고 
        // 이를 바탕으로 판단해서 해당 오브젝트 내의 값에 접근하자 (생성 수, 보관 수, 활동중 수 등)
    }

    public void SetBasicData(string name)
    {
        nameText.text = name;
        storyButton.gameObject.SetActive(false);
    }

    public void CheckPointerClick(BaseEventData eventData)
    {
        PointerEventData pointerData = (PointerEventData)eventData;
        // 클릭한 위치가 패널 외부인지 확인
        if (!RectTransformUtility.RectangleContainsScreenPoint(windowPopUP.GetComponent<RectTransform>(), pointerData.position))
        {
            // 패널 닫기
            this.gameObject.SetActive(false);
        }
    }

    // 가방에 동물을 넣는다
    // 현재 맵에 있는 동물중 해당하는 동물을 선택해 제거한 뒤,
    // 활동 중을 -1, 보관 중을 +1로 설정한다.
    public void AnimalToBag()
    {
        int count = 0;
        // 활동 중인 동물이 있을 경우
        // 해당하는 동물을 찾아야 한다.
        if (DataManager.Instance.animalGenerateData.allTypeCountDic[nowAnimaldataSO.animalNameEN][EachCountType.Active] > 0)
        {
            foreach (var animalDataSO in DataManager.Instance.spawnData.animalDataSOList)
            {
                if (animalDataSO.animalNameEN == nowAnimaldataSO.animalNameEN)
                {
                    //LifeManager.Instance.bubbleGenerator.RemoveBubble(count);
                    ResourceManager.Instance.bubbleGeneratorPool.RemoveBubble(count);
                    DataManager.Instance.DestroyAnimal(animalDataSO, count);

                    break;
                }
                count++;
            }
            count = 0;

            DataManager.Instance.StoreAnimalCount(nowAnimaldataSO);

            SetActiveStoreCountUI();
        }
    }


    // 가방에서 동물을 꺼내 배치한다 (가득 찬 경우에는 배치할 수 없다.)
    // 가방에
    // 활동 중을 +1, 보관 중을 +1로 설정한다.
    public void TakeOutAnimalfromBag()
    {
        // 보관 중인 동물이 있을 경우
        // 해당 동물을 찾아야 한다.
        if (DataManager.Instance.animalGenerateData.allTypeCountDic[nowAnimaldataSO.animalNameEN][EachCountType.Stored] > 0
            && DataManager.Instance.animalGenerateData.CanAnimalReplace())
        {
            DataManager.Instance.animalGenerateData.allTypeCountDic[nowAnimaldataSO.animalNameEN][EachCountType.Stored]--;
            DataManager.Instance.animalGenerateData.allTypeCountDic[nowAnimaldataSO.animalNameEN][EachCountType.Active]++;

            if (DataManager.Instance.animalGenerateData.AddAnimal())
            {
                GameObject go = Instantiate(nowAnimaldataSO.animalPrefab);
                DataManager.Instance.spawnData.AddAnimalSpawnData(go, nowAnimaldataSO);

                // 하트 버블 리스트에 추가
                //LifeManager.Instance.bubbleGenerator.AddAnimalHeartBubbleList(go.GetComponent<Animal>().heart);

                if (DataManager.Instance.animalGenerateData.nowAnimalCount == 1 || DataManager.Instance.animalGenerateData.nowAnimalCount == 2)
                {
                    //LifeManager.Instance.bubbleGenerator.GenerateNewHeart();
                    ResourceManager.Instance.bubbleGeneratorPool.GenerateNewHeart();
                }
            }

            SetActiveStoreCountUI();
        }
    }

    public void SetActiveStoreCountUI()
    {
        totalActiveCountText.text = DataManager.Instance.animalGenerateData.allTypeCountDic[nowAnimaldataSO.animalNameEN][EachCountType.Active].ToString();
        totalStoredCountText.text = DataManager.Instance.animalGenerateData.allTypeCountDic[nowAnimaldataSO.animalNameEN][EachCountType.Stored].ToString();
    }

    public void ChangeBottomUI(bool isBag, bool isCreateResult = false)
    {
        if (isCreateResult)
        {
            CreateAnimalBottomUI.SetActive(false);
            CreateResultUI.SetActive(true);
            CreateAnimalCountUI.SetActive(true);
            previousGetText.text = (ResourceManager.Instance.lifeGenerationRatePerSecond / 2).ToString();
            currentGetText.text = ResourceManager.Instance.lifeGenerationRatePerSecond.ToString();
            return;
        }
        CreateResultUI.SetActive(false);
        CreateAnimalCountUI.SetActive(false);
        BagBottomUI.SetActive(isBag);
        storyButton.gameObject.SetActive(isBag);
        CreateAnimalBottomUI.SetActive(!isBag);
    }

    public void SetAnimalGenerateCountText(int count = 1)
    {
        // 선택된 동물을 몇 마리 생성할 건지 설정해야 함. - 기본 값 1로 설정.
        animalGenerateCountText.text = count.ToString();
    }

    public void AnimalCountPlus()
    {
        if (LifeManager.Instance.lifeAmount > totalGenerateValue + preCost * 4)
        {
            animalGenerateCount++;
            SetAnimalGenerateCountText(animalGenerateCount);
            UpdateCreateCost();
        }
    }

    public void AnimalCountMinus()
    {
        if (animalGenerateCount > 1)
        {
            animalGenerateCount--;
            SetAnimalGenerateCountText(animalGenerateCount);
            UpdateCreateCost();
        }
    }

    public void UpdateCreateCost()
    {
        BigInteger previousCost = DataManager.Instance.animalGenerateData.nowCreateCost;
        totalGenerateValue = previousCost;
        for (int i = 1; i < animalGenerateCount; i++)
        {
            previousCost *= 4;
            totalGenerateValue += previousCost;
        }
        preCost = previousCost;
        createCostText.text = BigIntegerUtils.FormatBigInteger(totalGenerateValue);
    }

    public void CreateAnimal()
    {
        WindowsManager.Instance.createAnimalWindow.previousCost = DataManager.Instance.animalGenerateData.nowCreateCost;

        createObjectButton.isBuyAnimal = true;

        for(int i = 0; i < animalGenerateCount; i++) createObjectButton.CreateAnimalToScene();

        WindowsManager.Instance.createAnimalWindow.SetData(createObjectButton.animalData.animalName, animalGenerateCount);
    }

    public void TouchMinButton()
    {
        animalGenerateCount = 1;
        SetAnimalGenerateCountText(animalGenerateCount);
        UpdateCreateCost();
    }

    public void TouchMaxButton()
    {
        BigInteger previousCost = DataManager.Instance.animalGenerateData.nowCreateCost;
        BigInteger totalCost = previousCost;
        animalGenerateCount = 1;

        // 총 비용이 소지 생명보다 적을 때만
        while (totalCost + previousCost * 4 < LifeManager.Instance.lifeAmount)
        {
            previousCost = previousCost * 4;
            totalCost += previousCost;
            animalGenerateCount++;
        }

        totalGenerateValue = totalCost;
        preCost = previousCost;

        animalGenerateCountText.text = animalGenerateCount.ToString();
        createCostText.text = BigIntegerUtils.FormatBigInteger(totalCost);
    }

    public void EnableAnimalCreateResultWindow()
    {
        nameText.text = "동물을 창조했습니다.";
        CreateResultUI.SetActive(true);
        ChangeBottomUI(false, true);
        CreatedAnimalNameText.text = nowAnimaldataSO.animalNameKR;
        CreatedAnimalCountText.text = animalGenerateCount.ToString();
    }

    public void ClickStoryReplaceButton()
    {
        centerUIs.SetActive(!centerUIs.activeSelf);
        StoryCenterUI.SetActive(!StoryCenterUI.activeSelf);
        BagBottomUI.SetActive(!BagBottomUI.activeSelf);
        CreateAnimalBottomUI.SetActive(false);
    }

    public void ActiveCenterUI()
    {
        StoryCenterUI.SetActive(false);
        centerUIs.SetActive(true);
        storyButton.gameObject.SetActive(true);
    }
}
