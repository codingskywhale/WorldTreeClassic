using UnityEngine;
using System.Numerics;

public class AnimalGenerateData
{
    // 생성 시 가격
    public readonly BigInteger createCostbase = 250;
    public static readonly float createCostMultiple = 4;

    public BigInteger nowCreateCost;
    public int nowAnimalCount;
    public int maxAnimalCount;

    public int maxAnimalCountPlus = 5;

    public AnimalGenerateData()
    {
        nowCreateCost = createCostbase;
        maxAnimalCount = 5;

        if (!UIManager.Instance)
        {
            UIManager.Instance.status.UpdateAnimalCountText(nowAnimalCount, maxAnimalCount);
        }
    }

    public AnimalGenerateData(BigInteger nowCreateCost, int maxAnimalCount)
    {
        this.nowCreateCost = nowCreateCost;
        this.maxAnimalCount = maxAnimalCount;

        UIManager.Instance.status.UpdateAnimalCountText(nowAnimalCount, maxAnimalCount);
    }
    // 동물이 추가될 때 데이터에 반영
    public bool AddAnimal()
    {
        nowCreateCost = (BigInteger)((float)nowCreateCost * createCostMultiple);

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
}
