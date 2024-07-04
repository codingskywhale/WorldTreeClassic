using UnityEngine;

public class LifeManager : MonoBehaviour
{
    public static LifeManager Instance { get; private set; } // 싱글톤 인스턴스

    public int lifeAmount = 0;
    public int currentLevel = 1;
    public int lifePerLevel = 10;

    public delegate void WaterChanged(int newAmount);
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
    }

    public void IncreaseWater(int amount)
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
}
