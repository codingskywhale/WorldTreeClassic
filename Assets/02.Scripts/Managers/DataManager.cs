using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; } // 싱글톤 인스턴스

    public AnimalGenerateData animalGenerateData;
    public AnimalSpawnData spawnData;
    public TouchData touchData;
    public Bag bag;
    public List<AnimalDataSO> animalDataList = new List<AnimalDataSO>();

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
        animalGenerateData = new AnimalGenerateData();

        touchData = GetComponent<TouchData>();        
    }

    // 지정 동물을 파괴하는 기능
    public void DestroyAnimal(AnimalDataSO animalDataSO, int count)
    {
        Destroy(spawnData.animalObjectList[count]);
        spawnData.animalObjectList.Remove(spawnData.animalObjectList[count]);
        spawnData.animalDataSOList.Remove(animalDataSO);
    }

    // 가방에 보관하는 기능.
    public void StoreAnimalCount(AnimalDataSO animalDataSO)
    {
        animalGenerateData.allTypeCountDic[animalDataSO.animalName][EachCountType.Active]--;
        animalGenerateData.allTypeCountDic[animalDataSO.animalName][EachCountType.Stored]++;

        animalGenerateData.nowAnimalCount--;
        //UI 적용
        animalGenerateData.UpdateUIText();
    }
}
