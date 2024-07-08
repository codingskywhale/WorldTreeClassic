using System.Numerics;
using UnityEngine;

public class Root3 : RootBase
{
    protected override void Start()
    {
        unlockThreshold = 20;
        baseLifeGeneration = BigInteger.Parse("4000");
        unlockCost = BigInteger.Parse("268000"); // b 단위로 설정
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

    protected override void CreateAndZoomObject()
    {
        if (objectPrefab != null)
        {
            UnityEngine.Vector3 spawnPosition = new UnityEngine.Vector3(4, 0, 9); // 새로운 좌표로 설정
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