using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalSpawnData : MonoBehaviour
{
    public List<AnimalDataSO> animalDataSOList;
    public List<GameObject> animalObjectList;
    public Transform spawnTr;

    private void Awake()
    {
        spawnTr = this.transform;
    }
    public void AddAnimalSpawnData(GameObject go, AnimalDataSO dataSO)
    {
        animalObjectList.Add(go);
        animalDataSOList.Add(dataSO);
    }
}
