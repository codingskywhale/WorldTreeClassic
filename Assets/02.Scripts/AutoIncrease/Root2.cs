using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class Root2 : RootBase
{
    protected override void Start()
    {
        //    unlockThreshold = 10;
        //    baseLifeGeneration = 500;
        //    unlockCost = BigInteger.Parse("78400");
        //    base.Start();
        //    LifeManager.Instance.RegisterRoot(this);
        //    // 업그레이드 비용을 다시 계산하여 UI 업데이트
        //    upgradeLifeCost = CalculateUpgradeCost();
        //    UpdateUI();
        CalculateFlowerPositions();
    }

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

    protected override Vector3 GetRootCenterPosition()
    {
        Vector3 centerPosition = new Vector3(7.3f, -0.66f, -5); // Root2의 중심 위치
        Debug.Log($"Root2 중심 위치: {centerPosition}"); // 중심 위치 로그 추가
        return centerPosition;
    }
}

