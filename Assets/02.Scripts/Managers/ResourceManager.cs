using System.Collections.Generic;
using System.Numerics;
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
        // 모든 Root 인스턴스를 찾아서 리스트에 추가하고 이벤트를 구독합니다.
        RootBase[] rootBases = FindObjectsOfType<RootBase>();
        foreach (var root in rootBases)
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
        UIManager.Instance.tree.groundMeshFilter.transform.localScale = new UnityEngine.Vector3(groundScale, groundScale, groundScale);
    }

    public void UpdateUI()
    {
        BigInteger lifeNeededForCurrentLevel = lifeManager.CalculateWaterNeededForUpgrade(1);
        UIManager.Instance.status.UpdateLifeUI(lifeManager.lifeAmount, lifeNeededForCurrentLevel);
        UpdateTotalLifeIncreaseUI();
    }

    private void UpdateLifeUI(BigInteger newWaterAmount)
    {
        BigInteger lifeNeededForCurrentLevel = lifeManager.CalculateWaterNeededForUpgrade(1);
        UIManager.Instance.status.UpdateLifeUI(newWaterAmount, lifeNeededForCurrentLevel);
    }

    public void UpdateTotalLifeIncreaseUI()
    {
        BigInteger totalLifeIncrease = 0;
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
