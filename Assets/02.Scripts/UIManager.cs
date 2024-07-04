using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; } // 싱글톤 인스턴스

    public TextMeshProUGUI waterText;
    public TextMeshProUGUI lifeIncreaseText;
    public TextMeshProUGUI touchLevelText;
    public TextMeshProUGUI touchIncreaseText; // 추가된 텍스트 UI 요소
    public TextMeshProUGUI upgradelifeCostText;
    public TextMeshProUGUI rootLevelText;
    public TextMeshProUGUI rootUpgradeCostText;
    public TextMeshProUGUI spiritLevelText;
    public TextMeshProUGUI spiritUpgradeCostText;
    public MeshFilter outsideTreeMeshFilter; // 3D 오브젝트
    public TextMeshProUGUI upgradeRequirementText;
    public MeshFilter groundMeshFilter; // 3D 오브젝트
    public Mesh[] treeMeshes; // 3D 모델 배열

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

    public void UpdateLifeUI(int waterAmount, int waterNeededForCurrentLevel)
    {
        waterText.text = $" 생명력 : {waterAmount}";
    }

    public void UpdateTreeMeshes(int currentLevel)
    {
        int currentIndex = (currentLevel / 5) % treeMeshes.Length;
        outsideTreeMeshFilter.sharedMesh = treeMeshes[currentIndex];
    }

    public void UpdateLifeIncreaseUI(int totalLifeIncrease)
    {
        lifeIncreaseText.text = "Life Increase Per Second: " + totalLifeIncrease.ToString();
    }

    public void UpdateTouchUI(int touchIncreaseLevel, int touchIncreaseAmount, int upgradelifeCost)
    {
        touchLevelText.text = $"외로운 나무 레벨: {touchIncreaseLevel}";
        touchIncreaseText.text = $"현재 터치당 얻는 생명력 : {touchIncreaseAmount}";
        upgradelifeCostText.text = $"강화 비용: {upgradelifeCost} 생명력";
    }

    public void UpdateRootLevelUI(int rootLevel, int upgradeCost)
    {
        rootLevelText.text = $"뿌리 레벨: {rootLevel}";
        rootUpgradeCostText.text = $"강화 비용: {upgradeCost} 물";
    }

    public void UpdateSpiritLevelUI(int spiritLevel, int upgradeCost)
    {
        spiritLevelText.text = $"정령 레벨: {spiritLevel}";
        spiritUpgradeCostText.text = $"강화 비용: {upgradeCost} 에너지";
    }

    public void UpdateTouchIncreaseUI(int touchIncreaseAmount)
    {
        touchIncreaseText.text = $"터치 생산량: {touchIncreaseAmount}";
    }
}
