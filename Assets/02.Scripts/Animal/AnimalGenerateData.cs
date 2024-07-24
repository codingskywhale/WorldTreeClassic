using UnityEngine;
using System.Numerics;
using System.Collections.Generic;

public enum EachCountType
{
    Total,
    Active,
    Stored
}
public class AnimalGenerateData
{
    // 생성 시 가격
    public readonly BigInteger createCostbase = 250;
    public static readonly float createCostMultiple = 4;

    public BigInteger nowCreateCost;
    public int nowAnimalCount;
    public int maxAnimalCount;

    public int maxAnimalCountPlus = 5;
    // enum Type에 맞는 데이터를 보관하는 Dictionary
    private Dictionary<EachCountType, int> countDic;
    // 동물의 이름별로 총 생성된 데이터를 가지고 있는 Dictionary
    public Dictionary<string, Dictionary<EachCountType, int>> allTypeCountDic;

    public AnimalGenerateData()
    {
        nowCreateCost = createCostbase;
        maxAnimalCount = 5;
        allTypeCountDic = new Dictionary<string, Dictionary<EachCountType, int>>();
        UIManager.Instance.status.UpdateAnimalCountText(nowAnimalCount, maxAnimalCount);
    }

    public AnimalGenerateData(BigInteger nowCreateCost, int maxAnimalCount)
    {
        this.nowCreateCost = nowCreateCost;
        this.maxAnimalCount = maxAnimalCount;

        UIManager.Instance.status.UpdateAnimalCountText(nowAnimalCount, maxAnimalCount);
    }
    // 동물이 추가될 때 데이터에 반영
    public bool AddAnimal(bool isNew = false)
    {
        if(isNew) nowCreateCost = (BigInteger)((float)nowCreateCost * createCostMultiple);

        // 카운트 늘리지 않기.
        if (nowAnimalCount >= maxAnimalCount) return false;

        nowAnimalCount++;
        UIManager.Instance.status.UpdateAnimalCountText(nowAnimalCount, maxAnimalCount);

        return true;
    }

    // 새로운 산호가 해금되었을 때 데이터에 반영
    public void AddMaxAnimalCount()
    {
        maxAnimalCount += maxAnimalCountPlus;
        UIManager.Instance.status.UpdateAnimalCountText(nowAnimalCount, maxAnimalCount);
    }

    public void AddAnimalToDictionary(string name, bool canActive)
    {
        if (allTypeCountDic == null)
        {
            allTypeCountDic = new Dictionary<string, Dictionary<EachCountType, int>>();
        }

        // Dic에 이미 있는 데이터일 경우
        if (allTypeCountDic.ContainsKey(name))
        {
            allTypeCountDic[name][EachCountType.Total]++;

            // 활동 중 혹은 보관중 체크
            if (canActive)
                allTypeCountDic[name][EachCountType.Active]++;
            else
                allTypeCountDic[name][EachCountType.Stored]++;
        }
        // 처음 추가하는 데이터일 경우
        else
        {
            countDic = new Dictionary<EachCountType, int>();
            countDic.Add(EachCountType.Total, 1);
            countDic.Add(EachCountType.Active, 0);
            countDic.Add(EachCountType.Stored, 0);

            allTypeCountDic.Add(name, countDic);

            // 활동중 혹은 보관중 체크
            if (canActive)
                allTypeCountDic[name][EachCountType.Active]++;
            else
                allTypeCountDic[name][EachCountType.Stored]++;
        }
    }

    public void UpdateUIText()
    {
        UIManager.Instance.status.UpdateAnimalCountText(nowAnimalCount, maxAnimalCount);
    }

    public bool CanAnimalReplace()
    {
        return nowAnimalCount < maxAnimalCount;
    }
}
