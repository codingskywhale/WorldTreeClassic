using System.Numerics;
using TMPro;
using UnityEngine;

public interface IRoot
{
    void ApplyIncreaseRate(BigInteger rate);
    BigInteger GetTotalLifeGeneration();
    void Unlock();
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
    public TextMeshProUGUI rootUpgradeCostText;
    public TextMeshProUGUI generationRateText; // 생산률을 나타내는 텍스트 추가
    public TextMeshProUGUI unlockCostText; // 해금 비용을 나타내는 텍스트 추가
    public bool isUnlocked = false; // 잠금 상태를 나타내는 변수 추가

    private float timer;
    public TouchData touchData; // TouchData 참조
    public int unlockThreshold = 5; // 잠금 해제에 필요한 터치 레벨
    public GameObject objectPrefab;

    public delegate void LifeGenerated(BigInteger amount);
    protected event LifeGenerated OnLifeGenerated;
    public event System.Action OnGenerationRateChanged;

    protected CameraTransition cameraTransition; // CameraTransition 참조 추가

    protected virtual void Start()
    {
        OnLifeGenerated -= LifeManager.Instance.IncreaseWater;
        OnLifeGenerated += LifeManager.Instance.IncreaseWater;
        OnGenerationRateChanged?.Invoke(); // 초기화 시 이벤트 트리거
        UpdateUI();
        cameraTransition = FindObjectOfType<CameraTransition>(); // CameraTransition 컴포넌트 참조 초기화
        upgradeLifeCost = initialUpgradeCost; // 초기 레벨업 비용 설정
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
        BigInteger generatedLife = GetTotalLifeGeneration();
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
            return initialUpgradeCost;
        }
        else
        {
            return initialUpgradeCost * BigInteger.Pow(120, rootLevel) / BigInteger.Pow(100, rootLevel); // 1.2^rootLevel
        }
    }

    public virtual void UpgradeLifeGeneration()
    {
        if (!isUnlocked) return; // 잠금 해제된 경우에만 업그레이드 가능
        rootLevel++;
        if (rootLevel % 25 == 0)
        {
            baseLifeGeneration *= 2; // 25레벨마다 기본 생명력 생성량 두 배 증가
        }
        upgradeLifeCost = CalculateUpgradeCost();
        OnGenerationRateChanged?.Invoke();
        UpdateUI();
    }

    public virtual void UpdateUI()
    {
        UpdateRootLevelUI(rootLevel, upgradeLifeCost);
        UpdateGenerationRateUI(GetTotalLifeGeneration()); // 생산률 업데이트 추가
        UpdateUnlockCostUI(unlockCost); // 해금 비용 업데이트 추가
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
            rootLevelText.text = isUnlocked ? $"뿌리 레벨: {rootLevel}" : $"뿌리 레벨: 0";
        }

        if (rootUpgradeCostText != null)
        {
            rootUpgradeCostText.text = isUnlocked ? $"강화 비용: {BigIntegerUtils.FormatBigInteger(upgradeCost)} 물" : $"해금 비용: {BigIntegerUtils.FormatBigInteger(unlockCost)} 물 (레벨: {unlockThreshold} 필요)";
        }
    }

    public virtual void UpdateGenerationRateUI(BigInteger generationRate)
    {
        if (generationRateText != null)
        {
            generationRateText.text = isUnlocked ? $"생산률: {BigIntegerUtils.FormatBigInteger(generationRate)} 물/초" : $"잠금 해제 조건: 세계수 레벨 {unlockThreshold}\n식물 해금 시 배치 가능 동물 수 + 5";
        }
    }

    public virtual void UpdateUnlockCostUI(BigInteger unlockCost)
    {
        if (unlockCostText != null)
        {
            unlockCostText.text = $"해금 비용: {BigIntegerUtils.FormatBigInteger(unlockCost)} 물";
        }
    }

    public virtual BigInteger GetTotalLifeGeneration()
    {
        if (!isUnlocked || rootLevel == 0) return 0; // 잠금 해제 전이나 레벨이 0일 때는 0
        return baseLifeGeneration * BigInteger.Pow(103, rootLevel - 1) / BigInteger.Pow(100, rootLevel - 1); // 1.03^rootLevel-1
    }

    public void Unlock()
    {
        if (LifeManager.Instance.HasSufficientWater(unlockCost))
        {
            LifeManager.Instance.DecreaseWater(unlockCost);
            isUnlocked = true;
            rootLevel = 1; // 잠금 해제 시 레벨 1로 설정
            OnGenerationRateChanged?.Invoke(); // 잠금 해제 시 이벤트 트리거
            UpdateUI();
            CreateAndZoomObject(); // 오브젝트 생성 및 줌 효과 시작
        }
        else
        {
            Debug.Log("물이 부족하여 해금할 수 없습니다.");
        }
    }

    private void CheckUnlockCondition()
    {
        // 버튼 활성화 처리를 위해 조건 확인 로직을 유지
        if (!isUnlocked && touchData != null && touchData.touchIncreaseLevel >= unlockThreshold)
        {
            UpdateUI();
        }
    }

    protected virtual void CreateAndZoomObject()
    {
        if (objectPrefab != null)
        {
            UnityEngine.Vector3 spawnPosition = new UnityEngine.Vector3(0, 0, 9); // 기본 좌표 설정
            GameObject newObject = Instantiate(objectPrefab, spawnPosition, UnityEngine.Quaternion.identity);
            Debug.Log("Object created at position: " + spawnPosition);
            if (cameraTransition != null)
            {
               // StartCoroutine(cameraTransition.ZoomCamera(newObject.transform)); // 줌 효과 시작
            }
        }
        else
        {
            Debug.Log("Object prefab is not assigned.");
        }
    }
}

