using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class ResourceManager : MonoBehaviour
{
    public List<RootBase> roots = new List<RootBase>();
    public BigInteger lifeGenerationRatePerSecond;

    private void Start()
    {
        RegisterAllRoots();
        LifeManager.Instance.OnWaterChanged += UpdateLifeUI;

        // 초당 생명력 생성률을 로드
        LoadLifeGenerationRate();
        Debug.Log($"로드된 초당 생명력 생성률 (시작 시): {lifeGenerationRatePerSecond}");
        UpdateUI();
    }

    private void RegisterAllRoots()
    {
        //RootBase[] rootBases = FindObjectsOfType<RootBase>();
        foreach (var root in roots)
        {
            RegisterRoot(root);
        }
        UpdateLifeGenerationRatePerSecond();
    }

    private void RegisterRoot(RootBase root)
    {
        if (!roots.Contains(root))
        {
            roots.Add(root);
            root.OnGenerationRateChanged += UpdateLifeGenerationRatePerSecond;
        }
    }

    private void UnregisterRoot(RootBase root)
    {
        if (roots.Contains(root))
        {
            roots.Remove(root);
            root.OnGenerationRateChanged -= UpdateLifeGenerationRatePerSecond;
        }
    }

    public void UpdateGroundSize()
    {
        float groundScale = 8f + (LifeManager.Instance.currentLevel / 10f);
        UIManager.Instance.tree.groundMeshFilter.transform.localScale = new Vector3(groundScale, groundScale, groundScale);
    }

    public void UpdateUI()
    {
        BigInteger lifeNeededForCurrentLevel = LifeManager.Instance.CalculateWaterNeededForUpgrade(1);
        UIManager.Instance.status.UpdateLifeUI(LifeManager.Instance.lifeAmount, lifeNeededForCurrentLevel);
        UIManager.Instance.status.UpdateLifeIncreaseUI(lifeGenerationRatePerSecond);
    }

    private void UpdateLifeUI(BigInteger newWaterAmount)
    {
        BigInteger lifeNeededForCurrentLevel = LifeManager.Instance.CalculateWaterNeededForUpgrade(1);
        UIManager.Instance.status.UpdateLifeUI(newWaterAmount, lifeNeededForCurrentLevel);
    }

    public void UpdateLifeGenerationRatePerSecond()
    {
        lifeGenerationRatePerSecond = GetTotalLifeGenerationPerSecond();
        Debug.Log($"초당 생명력 생성률: {lifeGenerationRatePerSecond}");
    }

    public BigInteger GetTotalLifeGenerationPerSecond()
    {
        BigInteger totalLifeIncrease = 0;
        foreach (var root in roots)
        {
            totalLifeIncrease += root.GetTotalLifeGeneration();
        }
        Debug.Log($"토탈 : {totalLifeIncrease}");
        return totalLifeIncrease;
    }

    public BigInteger GetLifeGenerationRatePerSecond()
    {
        return lifeGenerationRatePerSecond;
    }

    public void SetLifeGenerationRatePerSecond(BigInteger rate)
    {
        lifeGenerationRatePerSecond = rate;
    }

    public void LoadLifeGenerationRate()
    {
        GameData gameData = SaveSystem.Load();
        if (gameData != null && !string.IsNullOrEmpty(gameData.lifeGenerationRatePerSecond))
        {
            lifeGenerationRatePerSecond = BigInteger.Parse(gameData.lifeGenerationRatePerSecond);
            Debug.Log($"로드된 초당 생명력 생성률: {lifeGenerationRatePerSecond}");
        }
        else
        {
            UpdateLifeGenerationRatePerSecond();  // 초기 값을 계산
        }
    }

    private void OnDestroy()
    {
        LifeManager.Instance.OnWaterChanged -= UpdateLifeUI;
        foreach (var root in roots)
        {
            root.OnGenerationRateChanged -= UpdateLifeGenerationRatePerSecond;
        }
    }
}
