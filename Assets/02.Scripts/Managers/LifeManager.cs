using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class LifeManager : MonoBehaviour
{
    public static LifeManager Instance { get; private set; } // 싱글톤 인스턴스

    public BigInteger lifeAmount = new BigInteger(5000);
    //public int currentLevel = 1;
    //public int lifePerLevel = 10;
    public RootBase RootData;
    public List<IRoot> rootData = new List<IRoot>();
    public BubbleGenerator bubbleGenerator;
    public delegate void WaterChanged(BigInteger newAmount);
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

        bubbleGenerator = GetComponent<BubbleGenerator>();
    }
    private void Start()
    {
        Debug.Log("LifeManager 초기값: " + lifeAmount);

        // SaveDataManager의 LoadGameData 호출
        SaveDataManager saveDataManager = new SaveDataManager();
        ResourceManager resourceManager = FindObjectOfType<ResourceManager>();
        //saveDataManager.LoadGameData(skill);

        Debug.Log("LifeManager 로드 후 값: " + lifeAmount);
    }
    public void IncreaseWater(BigInteger amount)
    {
        lifeAmount += amount;
        OnWaterChanged?.Invoke(lifeAmount);
    }

    public void DecreaseWater(BigInteger amount)
    {
        lifeAmount -= amount;
        OnWaterChanged?.Invoke(lifeAmount);
    }

    public bool HasSufficientWater(BigInteger requiredAmount)
    {
        return lifeAmount >= requiredAmount;
    }

    public BigInteger CalculateWaterNeededForUpgrade(int amount)
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

    public void ApplyIncreaseRateToAllRoots(BigInteger rate)
    {
        foreach (var root in rootData)
        {
            root.ApplyIncreaseRate(rate);
        }
    }
}
