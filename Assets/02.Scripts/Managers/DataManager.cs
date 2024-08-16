using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;

public class DataManager : Singleton<DataManager>
{
    public AnimalGenerateData animalGenerateData;
    public AnimalSpawnData spawnData;
    public TouchData touchData;
    public Bag bag;
    public List<AnimalDataSO> animalDataList = new List<AnimalDataSO>();
    public Transform animalSpawnTr;

    protected override void Awake()
    {
        base.Awake();
        animalGenerateData = new AnimalGenerateData();

        touchData = GetComponent<TouchData>();
    }

    // 지정 동물을 파괴하는 기능
    public void DestroyAnimal(AnimalDataSO animalDataSO, int count)
    {
        if (spawnData.animalObjectList[0] == null)
            spawnData.animalObjectList.Remove(spawnData.animalObjectList[0]);

        Canvas heartCanvas = spawnData.animalObjectList[count].transform.GetComponentInChildren<Canvas>();
        if (heartCanvas != null)
        {
            heartCanvas.transform.SetParent(ResourceManager.Instance.transform);
            heartCanvas.gameObject.SetActive(false);
        }
        GameObject go = spawnData.animalObjectList[count];
        spawnData.animalObjectList.Remove(go);
        spawnData.animalDataSOList.Remove(animalDataSO);
        Destroy(go);
    }

    public void DestroyAllAnimal()
    {
        for (int i = 0; i < spawnData.animalObjectList.Count; i++)
        {
            GameObject go = spawnData.animalObjectList[i];
            Destroy(go);
        }
        spawnData.animalObjectList.Clear();
        spawnData.animalDataSOList.Clear();
    }

    // 가방에 보관하는 기능.
    public void StoreAnimalCount(AnimalDataSO animalDataSO)
    {
        animalGenerateData.allTypeCountDic[animalDataSO.animalNameEN][EachCountType.Active]--;
        animalGenerateData.allTypeCountDic[animalDataSO.animalNameEN][EachCountType.Stored]++;

        animalGenerateData.nowAnimalCount--;
        //UI 적용
        animalGenerateData.UpdateUIText();
    }
}
