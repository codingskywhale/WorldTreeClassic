using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public LifeManager lifeManager;
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
    public void UpdateGroundSize()
    {
        float groundScale = 8f + (lifeManager.currentLevel / 10f);
        UIManager.Instance.tree.groundMeshFilter.transform.localScale = new Vector3(groundScale, groundScale, groundScale);
    }

    public void UpdateUI()
    {
        int lifeNeededForCurrentLevel = lifeManager.CalculateWaterNeededForUpgrade(1);
        UIManager.Instance.status.UpdateLifeUI(lifeManager.lifeAmount, lifeNeededForCurrentLevel);
        UpdateTotalLifeIncreaseUI();
    }

    private void UpdateLifeUI(float newWaterAmount)
    {
        int lifeNeededForCurrentLevel = lifeManager.CalculateWaterNeededForUpgrade(1);
        UIManager.Instance.status.UpdateLifeUI(newWaterAmount, lifeNeededForCurrentLevel);
    }

    public void UpdateTotalLifeIncreaseUI()
    {
        float totalLifeIncrease = 0;
        foreach (var root in roots)
        {
            totalLifeIncrease += root.baseLifeGeneration;
        }
        UIManager.Instance.status.UpdateLifeIncreaseUI(totalLifeIncrease);
    }
}
