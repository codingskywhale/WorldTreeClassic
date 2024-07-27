using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal_GreatDane : Animal, iNeedAnimalCondition
{
    public int needCount { get; set; } = 3;
    [field: SerializeField]
    public AnimalDataSO[] needAnimalSO { get ; set ; }
    public bool isAnimalConditionCleared { get; set; } = false;

    public void CheckAnimalConditionCleared()
    {       
        for (int i = 0; i < needAnimalSO.Length; i++)
        {
            Dictionary<string, Dictionary<EachCountType, int>> dic = DataManager.Instance.animalGenerateData.allTypeCountDic;

            // 해당 동물이 딕셔너리에 있으면
            if (dic.ContainsKey(needAnimalSO[i].animalName))
            {
                // 총 생산량이 조건을 충족하는 경우.
                if (dic[needAnimalSO[i].animalName][EachCountType.Total] >= needCount)
                {
                    isAnimalConditionCleared = true;
                }            
            }
            else
            {
                Debug.Log($"{animalDataSO.animalName}의 생성 수가 부족합니다.");
            }
        }
    }
}
