using System.Collections;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public interface IRoot
{
    void ApplyIncreaseRate(BigInteger rate);
    BigInteger GetTotalLifeGeneration();
    void Unlock();
    void ApplyTemporaryBoost(BigInteger multiplier, float duration); // 임시 부스트 메서드 추가
}

public class RootBase : MonoBehaviour, IRoot
{
    public int rootLevel = 0; // 초기 레벨을 0으로 설정
    public BigInteger baseLifeGeneration = 1; // 기본 생명력 생성량
    public BigInteger initialUpgradeCost = 20; // 초기 레벨업 비용
    public BigInteger unlockCost = 0; // 해금 비용
    public BigInteger upgradeLifeCost;
    public float generationInterval = 1f;
    public TextMeshProUGUI rootLevelText;
    public TextMeshProUGUI generationRateText; // 생산률을 나타내는 텍스트 추가
    public TextMeshProUGUI rootUpgradeCostText;
    public Image lockImage; // 해금 이미지
    public TextMeshProUGUI lockText; // 해금 텍스트
    public bool isUnlocked = false; // 잠금 상태를 나타내는 변수 추가

    private float timer;
    public int unlockThreshold = 5; // 잠금 해제에 필요한 터치 레벨
    //public GameObject objectPrefab;

    public GameObject[] plantObjects; // 미리 배치된 식물 오브젝트 배열 추가
    public delegate void LifeGenerated(BigInteger amount);
    protected event LifeGenerated OnLifeGenerated;
    public event System.Action OnGenerationRateChanged;

    protected CameraTransition cameraTransition; // CameraTransition 참조 추가
    private BigInteger currentMultiplier; // 현재 적용 중인 배수
    private Coroutine boostCoroutine; // 부스트 코루틴 참조 변수

    protected virtual void Start()
    {
        OnLifeGenerated -= LifeManager.Instance.IncreaseWater;
        OnLifeGenerated += LifeManager.Instance.IncreaseWater;
        OnGenerationRateChanged += UpdateUI; // 이벤트 핸들러 추가
        OnGenerationRateChanged?.Invoke(); // 초기화 시 이벤트 트리거
        UpdateUI();
        cameraTransition = FindObjectOfType<CameraTransition>(); // CameraTransition 컴포넌트 참조 초기화
        //upgradeLifeCost = initialUpgradeCost; // 초기 레벨업 비용 설정
        currentMultiplier = 1;
    }


    protected virtual void Update()
    {
        CheckUnlockCondition(); // 업데이트 시 잠금 해제 조건 확인
        if (isUnlocked && rootLevel > 0)
        {
            timer += Time.deltaTime;
            if (timer >= generationInterval)
            {
                GenerateLife();
                timer = 0f;
            }
        }
    }

    protected virtual void GenerateLife()
    {
        if (!isUnlocked || rootLevel == 0) return; // 잠금 해제된 경우에만 생명력 생성
        BigInteger generatedLife = GetTotalLifeGeneration(); // currentMultiplier는 이미 GetTotalLifeGeneration에 반영됨
        InvokeLifeGenerated(generatedLife);
    }

    protected void InvokeLifeGenerated(BigInteger amount)
    {
        OnLifeGenerated?.Invoke(amount);
    }

    public BigInteger CalculateUpgradeCost()
    {
        if (rootLevel == 0)
        {
            return unlockCost;
        }
        else
        {
            return unlockCost * BigInteger.Pow(120, rootLevel) / BigInteger.Pow(100, rootLevel); // 1.2^rootLevel
        }
    }

    public virtual void UpgradeLifeGeneration()
    {
        if (!isUnlocked) return; // 잠금 해제된 경우에만 업그레이드 가능
        rootLevel++;
        //레벨이 1이라면 CreateAndZoomObject 메서드 호출
        if (rootLevel == 1)
        {
            //CreateAndZoomObject();
            ActivateNextPlantObject();
        }
        if (rootLevel % 25 == 0)
        {
            //CreateAndZoomObject();
            ActivateNextPlantObject();
            baseLifeGeneration *= 2; // 25레벨마다 기본 생명력 생성량 두 배 증가
        }
        upgradeLifeCost = CalculateUpgradeCost();
        OnGenerationRateChanged?.Invoke();
        UpdateUI();
    }
    private void ActivateNextPlantObject()
    {
        if (plantObjects == null || plantObjects.Length == 0) return;

        // 첫 번째 레벨일 때 첫 번째 오브젝트 활성화
        if (rootLevel == 1)
        {
            plantObjects[0].SetActive(true);
        }
        else if (rootLevel > 1 && rootLevel % 25 == 0)
        {
            int plantIndex = rootLevel / 25; // 현재 레벨에 해당하는 인덱스 계산
            if (plantIndex >= 0 && plantIndex < plantObjects.Length)
            {
                plantObjects[plantIndex].SetActive(true); // 해당 인덱스의 식물 오브젝트 활성화
            }
        }
    }

    public virtual void UpdateUI()
    {
        UpdateRootLevelUI(rootLevel, upgradeLifeCost);
        UpdateGenerationRateUI(GetTotalLifeGeneration()); // 생산률 업데이트 추가
        UpdateUnlockUI(); // 잠금 해제 UI 업데이트 추가
    }

    public virtual void ApplyIncreaseRate(BigInteger rate)
    {
        if (!isUnlocked) return; // 잠금 해제된 경우에만 적용 가능
        baseLifeGeneration = baseLifeGeneration * (1 + rate);
        OnGenerationRateChanged?.Invoke();
        UpdateUI();
    }

    public virtual void UpdateRootLevelUI(int rootLevel, BigInteger upgradeCost)
    {
        if (rootLevelText != null)
        {
            rootLevelText.text = isUnlocked ? $"꽃 레벨: {rootLevel}" : $"꽃 레벨: 0";
        }

        if (rootUpgradeCostText != null)
        {
            rootUpgradeCostText.text = //isUnlocked ?
                                       $"강화 비용: {BigIntegerUtils.FormatBigInteger(upgradeCost)} 물"; //: $"해금 비용: {BigIntegerUtils.FormatBigInteger(unlockCost)} 물 (레벨: {unlockThreshold} 필요)";

        }

        
    }

    public virtual void UpdateGenerationRateUI(BigInteger generationRate)
    {
        if (generationRateText != null)
        {

            generationRateText.text = $"생산률: {BigIntegerUtils.FormatBigInteger(generationRate)} 물/초";

            if (isUnlocked && rootLevel == 0)
            {
                // 1레벨일 때의 생산률 계산
                BigInteger levelOneGenerationRate = baseLifeGeneration * BigInteger.Pow(103, 0) / BigInteger.Pow(100, 0); // 1.03^0 / 1.00^0
                generationRateText.text = $"생산률: {BigIntegerUtils.FormatBigInteger(generationRate)} 물/초 \n1레벨 업그레이드시 자동생산: {BigIntegerUtils.FormatBigInteger(levelOneGenerationRate)} 물/초";
            }
            if (!isUnlocked && rootLevel == 0)
            {
                // 1레벨일 때의 생산률 계산
                BigInteger levelOneGenerationRate = baseLifeGeneration * BigInteger.Pow(103, 0) / BigInteger.Pow(100, 0); // 1.03^0 / 1.00^0
                generationRateText.text = $"생산률: {BigIntegerUtils.FormatBigInteger(generationRate)} 물/초 \n1레벨 업그레이드시 자동생산: {BigIntegerUtils.FormatBigInteger(levelOneGenerationRate)} 물/초";
            }
        }
    }
    

    public virtual void UpdateUnlockUI()
    {
        if (!isUnlocked)
        {
            if (lockText != null)
            {
                lockText.text = $"잠금 해제 조건: 세계수 레벨 {unlockThreshold}\n꽃 해금 시 배치 가능 동물 수 + 5";
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
        if (!isUnlocked || rootLevel == 0) return 0; // 잠금 해제 전이나 레벨이 0일 때는 0
        BigInteger baseGeneration = baseLifeGeneration * BigInteger.Pow(103, rootLevel - 1) / BigInteger.Pow(100, rootLevel - 1); // 1.03^rootLevel-1
        BigInteger totalGeneration = baseGeneration * currentMultiplier; // currentMultiplier를 곱하여 반환
        //Debug.Log($"Total Generation ({this.name}): " + totalGeneration);
        return totalGeneration;
    }

    public void Unlock()
    {
        isUnlocked = true;
        //rootLevel = 1; // 잠금 해제 시 레벨 1로 설정
        upgradeLifeCost = CalculateUpgradeCost(); // 업그레이드 비용 업데이트
        OnGenerationRateChanged?.Invoke(); // 잠금 해제 시 이벤트 트리거
        DataManager.Instance.animalGenerateData.AddMaxAnimalCount();
        UpdateUI();
        Debug.Log("Unlocked successfully.");
    }

    private void CheckUnlockCondition()
    {
        // 잠금 해제 조건 확인 로직
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

    //protected virtual void CreateAndZoomObject()
    //{
    //    if (objectPrefab != null)
    //    {
    //        float radius = 1.5f; // 원하는 원의 반지름
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
    //}
}