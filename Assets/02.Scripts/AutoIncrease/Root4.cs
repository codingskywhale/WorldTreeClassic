using System.Numerics;
using UnityEngine;

public class Root4 : RootBase
{
    protected override void Start()
    {
        unlockThreshold = 40;
        baseLifeGeneration = BigInteger.Parse("320000");
        unlockCost = BigInteger.Parse("64500000000");
        base.Start();
        LifeManager.Instance.RegisterRoot(this);
        UpdateUI();
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