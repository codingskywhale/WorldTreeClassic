using System.Collections.Generic;
using UnityEngine;

public class LifeManager : MonoBehaviour
{
    public static LifeManager Instance { get; private set; } // 싱글톤 인스턴스

    public float lifeAmount = 0;
    public int currentLevel = 1;
    public int lifePerLevel = 10;
    public TouchData touchData;
    public Spirit spiritData;
    public RootBase RootData;
    public List<IRoot> rootData = new List<IRoot>();
    public delegate void WaterChanged(float newAmount);
    public event WaterChanged OnWaterChanged;

    private void Awake()
    {
        // 싱글톤 인스턴스 설정
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 인스턴스가 파괴되지 않도록 설정
        }
        else
        {
            Destroy(gameObject); // 이미 인스턴스가 존재하면 중복 생성된 객체 파괴
        }
        touchData = GetComponent<TouchData>();

        touchData.UpdateUI();
    }

    public void IncreaseWater(float amount)
    {
        lifeAmount += amount;
        OnWaterChanged?.Invoke(lifeAmount);
    }

    public void DecreaseWater(int amount)
    {
        lifeAmount -= amount;
        OnWaterChanged?.Invoke(lifeAmount);
    }

    public bool HasSufficientWater(int requiredAmount)
    {
        return lifeAmount >= requiredAmount;
    }

    public int CalculateWaterNeededForUpgrade(int amount)
    {
        return (currentLevel + amount) * lifePerLevel;
    }

    public void RegisterRoot(IRoot root)
    {
        if (!rootData.Contains(root))
        {
            rootData.Add(root);
        }
    }

    public void ApplyIncreaseRateToAllRoots(float rate)
    {
        foreach (var root in rootData)
        {
            root.ApplyIncreaseRate(rate);
        }
    }
}
