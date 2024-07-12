using System.Numerics;
using UnityEngine;

public class DiamondManager : MonoBehaviour
{
    public static DiamondManager Instance { get; private set; } // 싱글톤 인스턴스

    public BigInteger diamondAmount = 1000; // 초기 다이아몬드 양
    public delegate void DiamondChanged(BigInteger newAmount);
    public event DiamondChanged OnDiamondChanged;

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
