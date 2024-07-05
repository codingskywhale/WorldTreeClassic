using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; } // 싱글톤 인스턴스

    public Status status;
    public Root root;
    public TouchData touchData;
    public Spirit spiritData;
    public WorldTree tree;

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
}
