using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Numerics;
using Vector3 = UnityEngine.Vector3;


public class CreateAnimalButton : MonoBehaviour
{
    AnimalDataSO[] data;
    public void CreateAnimal(int idx)
    {
        if (LifeManager.Instance.lifeAmount > (BigInteger)LifeManager.Instance.animalData.nowCreateCost)
        {
            LifeManager.Instance.animalData.AddAnimal();
            Vector3 spawnVector = new Vector3(0, 0.5f, 10f);
            GameObject go = data[idx].animalPrefab;
            Instantiate(go);
            go.transform.position = spawnVector;
            LifeManager.Instance.touchData.ApplyIncreaseRate(1);
        }
    }
}
