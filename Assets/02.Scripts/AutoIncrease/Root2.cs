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
    //        float radius = 2.0f; // 원하는 원의 반지름
    //        int numberOfObjects = 20; // 생성할 오브젝트 수
    //        UnityEngine.Vector3 centerPosition = new UnityEngine.Vector3(0, 0, 10); // 중심 좌표

    //        for (int i = 0; i < numberOfObjects; i++)
    //        {
    //            float angle = i * Mathf.PI * 2 / numberOfObjects;
    //            float x = Mathf.Cos(angle) * radius;
    //            float z = Mathf.Sin(angle) * radius;
    //            UnityEngine.Vector3 spawnPosition = centerPosition + new UnityEngine.Vector3(x, 0, z);

    //            GameObject newObject = Instantiate(objectPrefab, spawnPosition, UnityEngine.Quaternion.identity);
    //            Debug.Log("Object created at position: " + spawnPosition);

    //            if (cameraTransition != null)
    //            {
    //                // StartCoroutine(cameraTransition.ZoomCamera(newObject.transform)); // 줌 효과 시작
    //            }
    //        }
    //    }
    //    else
    //    {
    //        Debug.Log("Object prefab is not assigned.");
    //    }

    //if (objectPrefab != null)
    //{
    //    UnityEngine.Vector3 spawnPosition = new UnityEngine.Vector3(2, 0, 9); // 새로운 좌표로 설정
    //    GameObject newObject = Instantiate(objectPrefab, spawnPosition, UnityEngine.Quaternion.identity);
    //    Debug.Log("Root2 object created at position: " + spawnPosition);
    //    if (cameraTransition != null)
    //    {
    //        //StartCoroutine(cameraTransition.ZoomCamera(newObject.transform)); // 줌 효과 시작
    //    }
    //}
    //else
    //{
    //    Debug.Log("Object prefab is not assigned.");
    //}
}

