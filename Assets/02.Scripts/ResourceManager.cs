using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public LifeManager lifeManager;
    public UIManager uiManager;
    private List<Root> roots = new List<Root>();

    private void Start()
    {
        lifeManager.OnWaterChanged += UpdateLifeUI;
        UpdateUI();

        // 모든 Root 인스턴스를 찾고 이벤트 구독
        foreach (var root in FindObjectsOfType<Root>())
        {
            roots.Add(root);
            root.OnGenerationRateChanged += UpdateTotalLifeIncreaseUI;
        }

        UpdateTotalLifeIncreaseUI(); // 초기화 시 생명력 증가율 계산
    }

    private void Update()
    {
        // 매 프레임마다 호출할 필요 없음
    }

    public void UpdateGroundSize()
    {
        float groundScale = 8f + (lifeManager.currentLevel / 10f);
        uiManager.groundSpriteRenderer.transform.localScale = new Vector3(groundScale, groundScale, groundScale);
    }

    public void UpdateUI()
    {
        int lifeNeededForCurrentLevel = lifeManager.CalculateWaterNeededForUpgrade(1);
        uiManager.UpdateLifeUI(lifeManager.lifeAmount, lifeNeededForCurrentLevel);
        UpdateTotalLifeIncreaseUI();
    }

    private void UpdateLifeUI(int newWaterAmount)
    {
        int lifeNeededForCurrentLevel = lifeManager.CalculateWaterNeededForUpgrade(1);
        uiManager.UpdateLifeUI(newWaterAmount, lifeNeededForCurrentLevel);
    }

    public void UpdateTotalLifeIncreaseUI()
    {
        int totalLifeIncrease = 0;
        foreach (var root in roots)
        {
            totalLifeIncrease += root.baseLifeGeneration;
        }
        uiManager.UpdateLifeIncreaseUI(totalLifeIncrease);
    }
}
