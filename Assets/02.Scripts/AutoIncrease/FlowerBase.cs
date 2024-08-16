using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public interface IFlower
{
    void ApplyIncreaseRate(BigInteger rate);
    BigInteger GetTotalLifeGeneration();
    void Unlock();
    void ApplyTemporaryBoost(BigInteger multiplier, float duration);
}

public class FlowerBase : MonoBehaviour, IFlower
{
    public Terrain terrain;  // Terrain 오브젝트
    public GameObject flowerPrefab;  // 꽃 프리팹
    public float brushSize = 1f;  // 브러시 크기
    public int flowersPerPosition = 10;  // 각 위치당 심을 꽃의 개수
    public GameObject[] prePlacedFlowers; // 미리 배치된 꽃 배열
    public OfflineRewardAmountSkill offlineRewardAmountSkill; // 오프라인 보상 스킬 참조
    public SkillCoolDownReduction skillCoolDownReduction;

    protected List<Vector3> flowerPositions;  // 꽃 위치 리스트
    private int currentFlowerIndex = 0;  // 꽃 인덱스
    private int currentPrePlacedFlowerIndex = 0; // 미리 배치된 꽃 인덱스

    public int flowerLevel = 0; // 초기 레벨을 0으로 설정
    public BigInteger baseLifeGeneration = 1; // 기본 생명력 생성량
    public BigInteger initialUpgradeCost = 20; // 초기 레벨업 비용
    public BigInteger unlockCost = 0; // 해금 비용
    public BigInteger upgradeLifeCost;
    public TextMeshProUGUI flowerLevelText;
    public TextMeshProUGUI generationRateText; // 생산률을 나타내는 텍스트 추가
    public TextMeshProUGUI flowerUpgradeCostText;
    public Image lockImage; // 해금 이미지
    public TextMeshProUGUI lockText; // 해금 텍스트
    public bool isUnlocked = false; // 잠금 상태를 나타내는 변수 추가

    public int unlockThreshold = 5; // 잠금 해제에 필요한 터치 레벨
    public int requiredOfflineRewardSkillLevel = 1; // 오프라인 보상 스킬 해금 조건 레벨
    public int skillCoolDownReductionLevel = 1; // 스킬쿨다운 아티팩트 스킬 해금 조건 레벨

    public event Action OnGenerationRateChanged;

    protected CameraTransition cameraTransition; // CameraTransition 참조 추가
    private BigInteger currentMultiplier; // 현재 적용 중인 배수
    private Coroutine boostCoroutine; // 부스트 코루틴 참조 변수

    public FlowerDataSO flowerDataSO;

    // 특정 영역들을 정의 (중심과 반경으로)
    public List<Tuple<Vector3, float>> restrictedAreas;

    protected virtual void Start()
    {
        // 특정 영역들을 초기화
        restrictedAreas = new List<Tuple<Vector3, float>> {
            new Tuple<Vector3, float>(new Vector3(-38.777f, -2.15f, -9.51f), CalculateRadius(new Vector3(-38.777f, -2.15f, -9.51f),
                                                                                            new Vector3(-37.08f, -0.36f, 4.74f),
                                                                                            new Vector3(-37.08f, -0.36f, -23.21f)))
        };

        flowerPositions = new List<Vector3>();
        CalculateFlowerPositions();

        // FlowerDataSO를 이용한 초기화
        if (flowerDataSO != null)
        {
            unlockThreshold = flowerDataSO.unlockThreshold;
            baseLifeGeneration = BigInteger.Parse(flowerDataSO.baseLifeGenerationString);
            unlockCost = BigInteger.Parse(flowerDataSO.unlockCostString);
            requiredOfflineRewardSkillLevel = flowerDataSO.requiredOfflineRewardSkillLevel;
        }

        OnGenerationRateChanged += UpdateUI; // 이벤트 핸들러 추가
        currentMultiplier = 1;
        LifeManager.Instance.RegisterFlower(this);
        UpdateUI();
    }

    protected virtual void GenerateLife()
    {
        if (!isUnlocked || flowerLevel == 0) return; // 잠금 해제된 경우에만 생명력 생성
        BigInteger generatedLife = GetTotalLifeGeneration(); // currentMultiplier는 이미 GetTotalLifeGeneration에 반영됨
        InvokeLifeGenerated(generatedLife);
    }

    protected void InvokeLifeGenerated(BigInteger amount)
    {
        // 생명력 생성 이벤트 호출
    }
    
    public BigInteger CalculateUpgradeCost()
    {
        if (flowerLevel == 0)
        {
            return unlockCost;
        }
        else
        {
            return unlockCost * BigInteger.Pow(120, flowerLevel) / BigInteger.Pow(100, flowerLevel); // 1.2^flowerLevel
        }
    }

    public virtual void UpgradeLifeGeneration()
    {
        if (!isUnlocked) return; // 잠금 해제된 경우에만 업그레이드 가능
        flowerLevel++;
        //ActivateNextPlantObject();
        if (flowerLevel == 1 || flowerLevel % 25 == 0)
        {
            UIManager.Instance.CheckConditionCleared();
            ActivateNextPlantObject();
            if (flowerLevel % 25 == 0)
            {
                baseLifeGeneration *= 2; // 25레벨마다 기본 생명력 생성량 두 배 증가
            }
        }
        upgradeLifeCost = CalculateUpgradeCost();
        OnGenerationRateChanged?.Invoke();

        AutoObjectManager.Instance.CalculateTotalAutoGeneration();
        UpdateUI();
    }

    public virtual void CalculateFlowerPositions()
    {
        Vector3 terrainCenter = new Vector3(-7.3f, -0.66f, -5f); // 고정된 중심 위치

        float baseRadius = brushSize / 4;  // 첫 번째 꽃의 위치를 위한 기본 반경
        float flowerRadius = 0; // 각 Flower에 고유한 반경

        int flowerIndex = Array.IndexOf(AutoObjectManager.Instance.flowers, this);

        if (flowerIndex < 4)
        {
            flowerRadius = baseRadius * (flowerIndex + 1); // 초기 Flower는 기본 반경 증가
        }
        else if (flowerIndex >= 6)
        {
            flowerRadius = baseRadius * 5 + (flowerIndex - 6) * baseRadius; // Flower7부터는 이전 Flower의 반경을 기반으로 증가
        }
        else
        {
            // Flower5와 Flower6은 반경을 사용하지 않음
            return;
        }

        // 반경에 따라 배치할 위치의 수를 동적으로 설정
        int numPositions = Mathf.CeilToInt(flowerRadius * 2);  // 반경에 비례하여 위치 수를 증가시킴

        flowerPositions = new List<Vector3>(); // flowerPositions 리스트 초기화
        HashSet<Vector3> usedPositions = new HashSet<Vector3>(); // 사용된 위치를 저장하는 집합

        for (int i = 0; i < numPositions; i++)
        {
            float angle = Mathf.Deg2Rad * (360f / numPositions * i);  // 각도 계산
            float x = terrainCenter.x + flowerRadius * Mathf.Cos(angle);
            float z = terrainCenter.z + flowerRadius * Mathf.Sin(angle);
            Vector3 newPosition = new Vector3(x, terrainCenter.y, z);

            // 위치가 이미 사용된 경우 반복
            while (usedPositions.Contains(newPosition) || IsInRestrictedArea(newPosition))
            {
                angle += Mathf.Deg2Rad * (360f / numPositions); // 각도를 변경하여 위치 재계산
                x = terrainCenter.x + flowerRadius * Mathf.Cos(angle);
                z = terrainCenter.z + flowerRadius * Mathf.Sin(angle);
                newPosition = new Vector3(x, terrainCenter.y, z);
            }

            flowerPositions.Add(newPosition);
            usedPositions.Add(newPosition); // 위치 저장
        }
    }

    void PlaceFlowers(List<Vector3> flowerPositions, GameObject flowerPrefab, ref int currentFlowerIndex)
    {
        if (flowerPositions == null || flowerPositions.Count == 0)
        {
            return;
        }

        if (currentFlowerIndex >= flowerPositions.Count)
        {
            currentFlowerIndex = 0;  // 인덱스 초기화
        }

        Vector3 flowerPosition = flowerPositions[currentFlowerIndex];

        for (int i = 0; i < flowersPerPosition; i++)
        {
            // 원형 오프셋 생성
            float angle = Random.Range(0f, 2f * Mathf.PI);
            float radius = Random.Range(0f, brushSize / 6);
            Vector3 randomOffset = new Vector3(radius * Mathf.Cos(angle), 0, radius * Mathf.Sin(angle));
            Vector3 finalPosition = flowerPosition + randomOffset;

            if (IsInRestrictedArea(finalPosition)) // 제한된 영역을 확인
            {
                continue;
            }

            // Terrain 높이 샘플링 및 최종 위치 조정
            float y = terrain.SampleHeight(finalPosition) + terrain.transform.position.y;
            finalPosition.y = y;

            GameObject newFlower = Instantiate(flowerPrefab, finalPosition, Quaternion.identity);
            newFlower.transform.parent = terrain.transform;  // Terrain 오브젝트의 자식으로 설정

            // 경사면에 맞게 회전
            Vector3 terrainNormal = terrain.terrainData.GetInterpolatedNormal(
                (finalPosition.x - terrain.transform.position.x) / terrain.terrainData.size.x,
                (finalPosition.z - terrain.transform.position.z) / terrain.terrainData.size.z
            );
            newFlower.transform.up = terrainNormal;

            newFlower.isStatic = true;  // Static Batching 적용

            // Material에 GPU Instancing 활성화
            foreach (var renderer in newFlower.GetComponentsInChildren<Renderer>())
            {
                if (renderer.sharedMaterial != null)
                {
                    renderer.sharedMaterial.enableInstancing = true;
                }
            }
        }

        currentFlowerIndex++;
        if (currentFlowerIndex >= flowerPositions.Count)
        {
            currentFlowerIndex = 0;  // 인덱스 초기화
        }
    }

    // 특정 위치가 제한된 영역 내에 있는지 확인
    private bool IsInRestrictedArea(Vector3 position)
    {
        foreach (var restrictedArea in restrictedAreas)
        {
            if (Vector3.Distance(position, restrictedArea.Item1) < restrictedArea.Item2)
            {
                return true;
            }
        }
        return false;
    }

    // 두 좌표 사이의 최대 거리를 계산하여 반경을 구함
    private float CalculateRadius(Vector3 center, Vector3 point1, Vector3 point2)
    {
        float distance1 = Vector3.Distance(center, point1);
        float distance2 = Vector3.Distance(center, point2);
        return Mathf.Max(distance1, distance2);
    }

    public void ActivateNextPlantObject()
    {
        if (currentPrePlacedFlowerIndex < prePlacedFlowers.Length)
        {
            prePlacedFlowers[currentPrePlacedFlowerIndex].SetActive(true);
            currentPrePlacedFlowerIndex++;
        }
        else
        {
            PlaceFlowers(flowerPositions, flowerPrefab, ref currentFlowerIndex);
        }
    }

    public virtual void UpdateUI()
    {
        UpdateFlowerLevelUI(flowerLevel, upgradeLifeCost);
        UpdateGenerationRateUI(GetTotalLifeGeneration());
        UpdateUnlockUI();
    }

    public virtual void ApplyIncreaseRate(BigInteger rate)
    {
        baseLifeGeneration = baseLifeGeneration * (1 + rate);
        OnGenerationRateChanged?.Invoke();
        UpdateUI();
    }

    public virtual void UpdateFlowerLevelUI(int flowerLevel, BigInteger upgradeCost)
    {
        if (flowerLevelText != null)
        {
            flowerLevelText.text = isUnlocked ? $"꽃 레벨: {flowerLevel}" : $"꽃 레벨: 0";
        }

        if (flowerUpgradeCostText != null)
        {
            flowerUpgradeCostText.text = $"{BigIntegerUtils.FormatBigInteger(upgradeCost)}";
        }
    }

    public virtual void UpdateGenerationRateUI(BigInteger generationRate)
    {
        if (generationRateText != null)
        {
            if (isUnlocked && flowerLevel == 0)
            {
                BigInteger levelOneGenerationRate = baseLifeGeneration * BigInteger.Pow(103, 0) / BigInteger.Pow(100, 0);
                generationRateText.text = $"1레벨 업그레이드시 자동생산: {BigIntegerUtils.FormatBigInteger(levelOneGenerationRate)}/s";
            }
            else if (!isUnlocked && flowerLevel == 0)
            {
                BigInteger levelOneGenerationRate = baseLifeGeneration * BigInteger.Pow(103, 0) / BigInteger.Pow(100, 0);
                generationRateText.text = $"1레벨 업그레이드시 자동생산: {BigIntegerUtils.FormatBigInteger(levelOneGenerationRate)}/s";
            }
            else
            {
                generationRateText.text = $"{BigIntegerUtils.FormatBigInteger(generationRate)}/s";
            }
        }
    }


    public virtual void UpdateUnlockUI()
    {
        if (!isUnlocked)
        {
            if (lockText != null)
            {
                lockText.gameObject.SetActive(true);
                if (flowerDataSO != null)
                {
                    lockText.text = flowerDataSO.unlockConditionText.Replace("\\n", "\n");
                }
                else
                {
                    lockText.text = $"잠금 해제 조건: 세계수 레벨 {unlockThreshold}\n꽃 해금 시 배치 가능 동물 수 + 5";
                }
            }
            if (lockImage != null)
            {
                lockImage.gameObject.SetActive(true);
            }
        }
        else
        {
            if (lockText != null)
            {
                lockText.gameObject.SetActive(false);
            }

            if (lockImage != null)
            {
                lockImage.gameObject.SetActive(false);
            }
        }
    }

    public virtual BigInteger GetTotalLifeGeneration()
    {
        if (!isUnlocked || flowerLevel == 0) return 0; // 잠금 해제 전이나 레벨이 0일 때는 0
        BigInteger baseGeneration = baseLifeGeneration * BigInteger.Pow(103, flowerLevel - 1) / BigInteger.Pow(100, flowerLevel - 1); // 1.03^flowerLevel-1
        BigInteger totalGeneration = baseGeneration * currentMultiplier; // currentMultiplier를 곱하여 반환
        return totalGeneration;
    }

    public void Unlock()
    {
        isUnlocked = true;
        upgradeLifeCost = CalculateUpgradeCost(); // 업그레이드 비용 업데이트
        OnGenerationRateChanged?.Invoke(); // 잠금 해제 시 이벤트 트리거
        DataManager.Instance.animalGenerateData.AddMaxAnimalCount();
        UpdateUI();
    }

    private void CheckUnlockCondition()
    {
        if (!isUnlocked && DataManager.Instance.touchData != null
            && DataManager.Instance.touchData.touchIncreaseLevel >= unlockThreshold)
        {
            Unlock(); // 잠금 해제 조건 만족 시 Unlock 호출
        }
    }

    public void ApplyTemporaryBoost(BigInteger multiplier, float duration)
    {
        if (boostCoroutine != null)
        {
            StopCoroutine(boostCoroutine);
        }
        boostCoroutine = StartCoroutine(TemporaryBoost(multiplier, duration));
    }

    private IEnumerator TemporaryBoost(BigInteger multiplier, float duration)
    {
        currentMultiplier = multiplier;
        OnGenerationRateChanged?.Invoke(); // 부스트 시작 시 생산률 업데이트 이벤트 호출
        UpdateUI(); // 부스트 시작 시 UI 업데이트
        yield return new WaitForSeconds(duration);
        currentMultiplier = 1; // 부스트가 끝나면 배수를 초기값으로 되돌림
        OnGenerationRateChanged?.Invoke(); // 생산률 업데이트 이벤트 호출
        UpdateUI(); // 부스트가 끝난 후 UI 업데이트
    }
}
