using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public LifeManager lifeManager;
    private List<RootBase> roots = new List<RootBase>();

    private void Start()
    {
        RegisterAllRoots();
        lifeManager.OnWaterChanged += UpdateLifeUI;

        // 모든 Root를 등록한 후 생명력 증가율 업데이트
        UpdateTotalLifeIncreaseUI();
        UpdateUI();
    }

    private void RegisterAllRoots()
    {
        // 모든 Root 인스턴스를 찾고 이벤트 구독
        foreach (var root in FindObjectsOfType<RootBase>())
        {
            RegisterRoot(root);
        }
    }

    private void RegisterRoot(RootBase root)
    {
        if (!roots.Contains(root))
        {
            roots.Add(root);
            root.OnGenerationRateChanged += UpdateTotalLifeIncreaseUI;
        }
    }

    private void UnregisterRoot(RootBase root)
    {
        if (roots.Contains(root))
        {
            roots.Remove(root);
            root.OnGenerationRateChanged -= UpdateTotalLifeIncreaseUI;
        }
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
            totalLifeIncrease += root.GetTotalLifeGeneration();
        }
        UIManager.Instance.status.UpdateLifeIncreaseUI(totalLifeIncrease);
    }

    private void OnDestroy()
    {
        lifeManager.OnWaterChanged -= UpdateLifeUI;
        foreach (var root in roots)
        {
            root.OnGenerationRateChanged -= UpdateTotalLifeIncreaseUI;
        }
    }
}
