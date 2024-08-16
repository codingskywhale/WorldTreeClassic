using System.Numerics;
using UnityEngine;

public class Flower10 : FlowerBase
{
    protected override void Start()
    {
        unlockThreshold = 100;
        baseLifeGeneration = BigInteger.Parse("29957670763179500000000000");
        unlockCost = BigInteger.Parse("7001128979230350000000000000000000000000000000000");
        base.Start();
        LifeManager.Instance.RegisterFlower(this);
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

    //protected override void CreateAndZoomObject()
    //{
    //    if (objectPrefab != null)
    //    {
    //        UnityEngine.Vector3 spawnPosition = new UnityEngine.Vector3(6, 0, 6); // 새로운 좌표로 설정
    //        GameObject newObject = Instantiate(objectPrefab, spawnPosition, UnityEngine.Quaternion.identity);
    //        Debug.Log("Flower2 object created at position: " + spawnPosition);
    //        if (cameraTransition != null)
    //        {
    //            //StartCoroutine(cameraTransition.ZoomCamera(newObject.transform)); // 줌 효과 시작
    //        }
    //    }
    //    else
    //    {
    //        Debug.Log("Object prefab is not assigned.");
    //    }
    //}
}