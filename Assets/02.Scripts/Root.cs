using UnityEngine;

public class Root : MonoBehaviour
{
    public LifeManager lifeManager;
    public UIManager uiManager;
    public int rootLevel = 1;
    public int baseLifeGeneration = 1; // 초기 생성량 1로 설정
    public int lifeGenerationPerLevel = 1;
    public int upgradeLifeCost = 20;
    public float generationInterval = 1f; // 초 단위로 설정
    private float timer;

    public delegate void LifeGenerated(int amount);
    public event LifeGenerated OnLifeGenerated;
    public event System.Action OnGenerationRateChanged; // 생명력 증가율 변경 이벤트

    private void Start()
    {
        // OnLifeGenerated 이벤트 중복 구독 방지
        OnLifeGenerated -= lifeManager.IncreaseWater;
        OnLifeGenerated += lifeManager.IncreaseWater;

        UpdateUI();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= generationInterval)
        {
            GenerateLife();
            timer = 0f;
        }
    }

    private void GenerateLife()
    {
        int generatedLife = baseLifeGeneration; // 기본 생성량만 사용
        OnLifeGenerated?.Invoke(generatedLife);
    }

    public int CalculateUpgradeCost()
    {
        return rootLevel * 20;
    }

    public void UpgradeLifeGeneration()
    {
        rootLevel++;
        if (rootLevel % 25 == 0)
        {
            baseLifeGeneration *= 2; // 25레벨마다 기본 생성량을 2배로 증가
        }
        else
        {
            baseLifeGeneration += lifeGenerationPerLevel; // 그 외에는 일정하게 증가
        }
        upgradeLifeCost += 20; // 업그레이드 비용 증가
        OnGenerationRateChanged?.Invoke(); // 생명력 증가율 변경 이벤트 호출
        UpdateUI();
    }

    public void UpdateUI()
    {
        int totalLifeIncrease = baseLifeGeneration; // 총 증가량 계산 수정
        uiManager.UpdateRootLevelUI(rootLevel, upgradeLifeCost);
        uiManager.UpdateLifeIncreaseUI(totalLifeIncrease);
    }
}
