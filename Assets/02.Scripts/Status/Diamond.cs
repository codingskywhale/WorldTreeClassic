using System.Numerics;
using UnityEngine;

public class Diamond : MonoBehaviour
{
    public BigInteger diamondAmount = 10000000; // 초기 다이아몬드 양
    public delegate void DiamondChanged(BigInteger newAmount);
    public event DiamondChanged OnDiamondChanged;

    private void Start()
    {
        OnDiamondChanged += UIManager.Instance.status.UpdateDiamondUI;
        UIManager.Instance.status.UpdateDiamondUI(diamondAmount);
    }
    public void IncreaseDiamond(BigInteger amount)
    {
        diamondAmount += amount;
        OnDiamondChanged?.Invoke(diamondAmount);
    }

    public void DecreaseDiamond(BigInteger amount)
    {
        diamondAmount -= amount;
        OnDiamondChanged?.Invoke(diamondAmount);
    }

    public bool HasSufficientDiamond(BigInteger requiredAmount)
    {
        return diamondAmount >= requiredAmount;
    }
}
