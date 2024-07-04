using UnityEngine;

public class LifeManager : MonoBehaviour
{
    public int lifeAmount = 0;
    public int currentLevel = 1;
    public int lifePerLevel = 10;

    public delegate void WaterChanged(int newAmount);
    public event WaterChanged OnWaterChanged;

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
