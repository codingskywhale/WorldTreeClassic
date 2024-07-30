using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class Root1 : RootBase
{
    protected override void Start()
    {
        unlockThreshold = 5;
        baseLifeGeneration = 100;
        unlockCost = 1600;
        base.Start();
        LifeManager.Instance.RegisterRoot(this);
        UpdateUI();
        CalculateFlowerPositions();
    }


    protected override void GenerateLife()
    {
        BigInteger generatedLife = GetTotalLifeGeneration();
        InvokeLifeGenerated(generatedLife);
    }

    public override void UpdateUI()
    {
        base.UpdateUI();
    }
    public override BigInteger GetTotalLifeGeneration()
    {
        return base.GetTotalLifeGeneration(); // 기본 클래스의 동작을 유지
    }


}