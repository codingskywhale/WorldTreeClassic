using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateAnimalButton : MonoBehaviour
{
    public AnimalDataSO[] data;
    public void CreateAnimal(int idx)
    {
        //if(LifeManager.Instance.lifeAmount > data[idx])
        Vector3 spawnVector = new Vector3(0, 0.5f, 10f);
        GameObject go = data[idx].animalPrefab;
        Instantiate(go);
        go.transform.position = spawnVector;
        LifeManager.Instance.touchData.ApplyIncreaseRate(1f);
    }
}
