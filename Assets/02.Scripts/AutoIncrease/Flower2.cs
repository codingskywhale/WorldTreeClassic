using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class Flower2 : FlowerBase
{
    //protected override void Start()
    //{
    //    //    unlockThreshold = 10;
    //    //    baseLifeGeneration = 500;
    //    //    unlockCost = BigInteger.Parse("78400");
    //    //    base.Start();
    //    //    LifeManager.Instance.RegisterFlower(this);
    //    //    // 업그레이드 비용을 다시 계산하여 UI 업데이트
    //    //    upgradeLifeCost = CalculateUpgradeCost();
    //    //    UpdateUI();
    //    CalculateFlowerPositions();
    //}

    protected override void GenerateLife()
    {
        BigInteger generatedLife = GetTotalLifeGeneration();
        InvokeLifeGenerated(generatedLife);
    }

    //public override void UpdateUI()
    //{
    //    base.UpdateUI();
    //}

    public override BigInteger GetTotalLifeGeneration()
    {
        return base.GetTotalLifeGeneration(); // 기본 클래스의 동작을 유지
    }

}

