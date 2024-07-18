using System.Numerics;
using UnityEngine;

public class Root2 : RootBase
{
    protected override void Start()
    {
        unlockThreshold = 10;
        baseLifeGeneration = 500;
        unlockCost = BigInteger.Parse("78400");
        base.Start();
        LifeManager.Instance.RegisterRoot(this);
        // 업그레이드 비용을 다시 계산하여 UI 업데이트
        upgradeLifeCost = CalculateUpgradeCost();
        UpdateUI();
    }

    protected override void GenerateLife()
    {
        BigInteger generatedLife = GetTotalLifeGeneration();
        InvokeLifeGenerated(generatedLife);
    }
    //public override void UpdateRootLevelUI(int rootLevel, BigInteger upgradeCost)
    //{
    //    rootUpgradeCostText.text = isUnlocked ? $"강화 비용: {BigIntegerUtils.FormatBigInteger(upgradeCost)} 물" :
    //        $"해금 비용: {BigIntegerUtils.FormatBigInteger(unlockCost)} 물 (레벨: {unlockThreshold} 필요)";
    //}
    public override void UpdateUI()
    {
        base.UpdateUI();
    }

    public override BigInteger GetTotalLifeGeneration()
    {
        return base.GetTotalLifeGeneration(); // 기본 클래스의 동작을 유지
    }

    protected override void CreateAndZoomObject()
    {
        if (objectPrefab != null)
        {
            UnityEngine.Vector3 spawnPosition = new UnityEngine.Vector3(2, 0, 9); // 새로운 좌표로 설정
            GameObject newObject = Instantiate(objectPrefab, spawnPosition, UnityEngine.Quaternion.identity);
            Debug.Log("Root2 object created at position: " + spawnPosition);
            if (cameraTransition != null)
            {
                //StartCoroutine(cameraTransition.ZoomCamera(newObject.transform)); // 줌 효과 시작
            }
        }
        else
        {
            Debug.Log("Object prefab is not assigned.");
        }
    }
}
