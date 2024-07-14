using System;
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

    private AnimalDataSO nowAnimaldataSO;
    public Image animalImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI conditionTexts;
    public TextMeshProUGUI totalGeneratedCountText;
    public TextMeshProUGUI totalActiveCountText;
    public TextMeshProUGUI totalStoredCountText;

    // 이미지를 눌렀을 때 SetAnimalInfoWindowData 를 해주면 된다.
    public void SetAnimalInfoWindowData(AnimalDataSO dataSO)
    {
        nowAnimaldataSO = dataSO;
        animalImage.sprite = dataSO.animalIcon;
        nameText.text = dataSO.animalName;
        conditionTexts.text = dataSO.animalUnlockConditions[0];
        totalGeneratedCountText.text = DataManager.Instance.animalGenerateData.allTypeCountDic[dataSO.animalName][EachCountType.Total].ToString();
        totalActiveCountText.text = DataManager.Instance.animalGenerateData.allTypeCountDic[dataSO.animalName][EachCountType.Active].ToString();
        totalStoredCountText.text = DataManager.Instance.animalGenerateData.allTypeCountDic[dataSO.animalName][EachCountType.Stored].ToString();
        // 총 생산 데이터는 SO에 담지 말자.
        // 해당 동물을 식별할 수 있는 데이터를 AnimalDataSO에 넣고 
        // 이를 바탕으로 판단해서 해당 오브젝트 내의 값에 접근하자 (생성 수, 보관 수, 활동중 수 등)
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
        if (DataManager.Instance.animalGenerateData.allTypeCountDic[nowAnimaldataSO.animalName][EachCountType.Active] > 0)
        {
            foreach(var animalDataSO in DataManager.Instance.spawnData.animalDataSOList)
            {
                if (animalDataSO.animalName == nowAnimaldataSO.animalName)
                {
                    DataManager.Instance.DestroyAnimal(animalDataSO, count);
                    LifeManager.Instance.bubbleGenerator.RemoveIdxFromNowBubbleList(count);

                    break;
                }
                count++;
            }
            count = 0;

            DataManager.Instance.StoreAnimalCount(nowAnimaldataSO);

            // animalIndex가 아니라 그냥 그 오브젝트 자체의 인덱스 값을 가져와야함.....
            

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
        if (DataManager.Instance.animalGenerateData.allTypeCountDic[nowAnimaldataSO.animalName][EachCountType.Stored] > 0)
        {
            DataManager.Instance.animalGenerateData.allTypeCountDic[nowAnimaldataSO.animalName][EachCountType.Stored]--;

            if (DataManager.Instance.animalGenerateData.AddAnimal())
            {
                GameObject go = Instantiate(nowAnimaldataSO.animalPrefab);
                DataManager.Instance.spawnData.AddAnimalSpawnData(go, nowAnimaldataSO);

                // 하트 버블 리스트에 추가
                LifeManager.Instance.bubbleGenerator.AddAnimalHeartBubbleList(go.GetComponent<Animal>().heart);

                if (DataManager.Instance.animalGenerateData.nowAnimalCount == 1 || DataManager.Instance.animalGenerateData.nowAnimalCount == 2)
                {
                    LifeManager.Instance.bubbleGenerator.GenerateNewHeart();
                }

                DataManager.Instance.animalGenerateData.AddAnimalToDictionary(nowAnimaldataSO.animalName, true);
            }

            SetActiveStoreCountUI();
        }
    }

    public void SetActiveStoreCountUI()
    {
        totalActiveCountText.text = DataManager.Instance.animalGenerateData.allTypeCountDic[nowAnimaldataSO.animalName][EachCountType.Active].ToString();
        totalStoredCountText.text = DataManager.Instance.animalGenerateData.allTypeCountDic[nowAnimaldataSO.animalName][EachCountType.Stored].ToString();
    }
}
