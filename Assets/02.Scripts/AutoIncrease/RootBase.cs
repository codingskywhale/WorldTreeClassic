using TMPro;
using UnityEngine;

public interface IRoot
{
    void ApplyIncreaseRate(float rate);
    float GetTotalLifeGeneration();
    void Unlock();
}

public class RootBase : MonoBehaviour, IRoot
{
    public int rootLevel = 0; // 초기 레벨을 0으로 설정
    public float baseLifeGeneration = 1;
    public int lifeGenerationPerLevel = 1;
    public int upgradeLifeCost = 20;
    public float generationInterval = 1f;
    public TextMeshProUGUI rootLevelText;
    public TextMeshProUGUI rootUpgradeCostText;
    public TextMeshProUGUI generationRateText; // 생산률을 나타내는 텍스트 추가
    public bool isUnlocked = false; // 잠금 상태를 나타내는 변수 추가

    private float timer;
    public TouchData touchData; // TouchData 참조
    public int unlockThreshold = 5; // 잠금 해제에 필요한 터치 레벨

    public delegate void LifeGenerated(float amount);
    protected event LifeGenerated OnLifeGenerated;
    public event System.Action OnGenerationRateChanged;

    protected virtual void Start()
    {
        OnLifeGenerated -= LifeManager.Instance.IncreaseWater;
        OnLifeGenerated += LifeManager.Instance.IncreaseWater;
        OnGenerationRateChanged?.Invoke(); // 초기화 시 이벤트 트리거
        UpdateUI();
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
        float generatedLife = GetTotalLifeGeneration();
        InvokeLifeGenerated(generatedLife);
    }

    protected void InvokeLifeGenerated(float amount)
    {
        OnLifeGenerated?.Invoke(amount);
    }

    public int CalculateUpgradeCost()
    {
        return (rootLevel + 1) * 20; // 업그레이드 비용은 다음 레벨 기준으로 계산
    }

    public virtual void UpgradeLifeGeneration()
    {
        if (!isUnlocked) return; // 잠금 해제된 경우에만 업그레이드 가능
        rootLevel++;
        if (rootLevel % 25 == 0)
        {
            baseLifeGeneration *= 2;
        }
        else
        {
            baseLifeGeneration += lifeGenerationPerLevel;
        }
        upgradeLifeCost = CalculateUpgradeCost();
        OnGenerationRateChanged?.Invoke();
        UpdateUI();
    }

    public virtual void UpdateUI()
    {
        UpdateRootLevelUI(rootLevel, upgradeLifeCost);
        UpdateGenerationRateUI(GetTotalLifeGeneration()); // 생산률 업데이트 추가
    }

    public virtual void ApplyIncreaseRate(float rate)
    {
        if (!isUnlocked) return; // 잠금 해제된 경우에만 적용 가능
        baseLifeGeneration *= 1 + rate;
        OnGenerationRateChanged?.Invoke();
        UpdateUI();
    }

    public virtual void UpdateRootLevelUI(int rootLevel, int upgradeCost)
    {
        if (rootLevelText != null)
        {
            if (isUnlocked)
            {
                rootLevelText.text = $"뿌리 레벨: {rootLevel}";
            }
            else
            {
                rootLevelText.text = $"뿌리 레벨: 0";
            }
        }

        if (rootUpgradeCostText != null)
        {
            rootUpgradeCostText.text = isUnlocked ? $"강화 비용: {upgradeCost} 물" : $"해금 비용: {upgradeCost} 물 (레벨: {unlockThreshold} 필요)";
        }
    }

    public virtual void UpdateGenerationRateUI(float generationRate)
    {
        if (generationRateText != null)
        {
            generationRateText.text = isUnlocked ? $"생산률: {generationRate} 물/초" : $"잠금 해제 조건: 세계수 레벨 {unlockThreshold}";
        }
    }

    public virtual float GetTotalLifeGeneration()
    {
        return isUnlocked && rootLevel > 0 ? baseLifeGeneration : 0; // 잠금 해제되고 레벨이 1 이상인 경우에만 생산
    }

    public void Unlock()
    {
        int unlockCost = CalculateUpgradeCost(); // 해금 비용 계산 (처음 업그레이드 비용)
        if (LifeManager.Instance.HasSufficientWater(unlockCost))
        {
            LifeManager.Instance.DecreaseWater(unlockCost);
            isUnlocked = true;
            rootLevel = 1; // 잠금 해제 시 레벨 1로 설정
            OnGenerationRateChanged?.Invoke(); // 잠금 해제 시 이벤트 트리거
            UpdateUI();
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
}