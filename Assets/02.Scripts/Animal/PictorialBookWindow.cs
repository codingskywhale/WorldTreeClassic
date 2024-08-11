using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PictorialBookWindow : MonoBehaviour
{
    public TextMeshProUGUI animalCountText;
    public Slider collectionSlider;

    private void Awake()
    {
        WindowsManager.Instance.bookWindow = this;
    }
    private void OnEnable()
    {
        animalCountText.text = $"활동 중 : {DataManager.Instance.animalGenerateData.nowAnimalCount} / {DataManager.Instance.animalGenerateData.maxAnimalCount}";
        ApplyCollectionRate();
    }
    public void TouchRandomReplaceButton()
    {
        //랜덤으로 모든 동물을 배치하는 기능.
        foreach(var datas in DataManager.Instance.animalGenerateData.allTypeCountDic)
        {

        }
    }

    public void TouchStoreAllButton()
    {
        //모든 동물을 보관하는 기능.
        // 모든 동물의 Active를 0으로 만든다
        foreach (KeyValuePair<string, Dictionary<EachCountType,int>> datas in DataManager.Instance.animalGenerateData.allTypeCountDic)
        {
            // 활동중인 동물이 있다면?
            // 1. 해당되는 동물을 씬에서 제거해야 한다.
            // 줄여주고 딕셔너리에 데이터 추가.
            // 버블을 비워준다.
            while (datas.Value[EachCountType.Active] > 0)
            {
                datas.Value[EachCountType.Active]--;
                datas.Value[EachCountType.Stored]++;

                DataManager.Instance.animalGenerateData.nowAnimalCount--;
                DataManager.Instance.animalGenerateData.UpdateUIText();
            }

            LifeManager.Instance.bubbleGenerator.ClearAllBubbles();
            DataManager.Instance.DestroyAllAnimal();
        }
    }

    public void ApplyCollectionRate()
    {
        collectionSlider.value = (float)DataManager.Instance.animalGenerateData.allTypeCountDic.Count / (float)GameManager.Instance.animalDataList.Count;
    }

    public void ApplyActiveAnimalUI()
    {
        animalCountText.text = $"활동 중 : {DataManager.Instance.animalGenerateData.nowAnimalCount} / {DataManager.Instance.animalGenerateData.maxAnimalCount}";
    }
}
