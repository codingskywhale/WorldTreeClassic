using UnityEngine;

public class GameObjectManager : MonoBehaviour
{
    public GameObject[] gameObjects; // 게임 오브젝트들을 저장할 배열
    public GameObject currentActiveObject; // 현재 활성화된 오브젝트
    public GameObject[] tabButtons; // 탭 버튼들을 저장할 배열
    private AlphaChanger alphaChanger;
    public Intro introObject;

    private void Awake()
    {
        alphaChanger = gameObject.AddComponent<AlphaChanger>();
        InitializeTabs();
    }

    private void Start()
    {
        introObject.gameObject.SetActive(true);
    }

    private void InitializeTabs()
    {
        int defaultIndex = 0; // 기본으로 선택할 탭 인덱스 (나무탭)

        // 모든 탭의 알파값을 초기화
        for (int i = 0; i < tabButtons.Length; i++)
        {
            if (i == defaultIndex)
            {
                alphaChanger.SetAlpha(tabButtons[i], 1.0f); // 기본 선택된 탭의 알파값을 1로 설정
            }
            else
            {
                alphaChanger.SetAlpha(tabButtons[i], 0.4f); // 다른 탭의 알파값을 0.4로 설정
            }
        }

        // 기본으로 선택된 오브젝트 활성화
        currentActiveObject = gameObjects[defaultIndex];
        currentActiveObject.SetActive(true);
    }

    // 버튼 클릭 시 호출될 메서드
    public void ShowGameObject(int index)
    {
        // 유효한 인덱스인지 확인
        if (index < 0 || index >= gameObjects.Length)
        {
            Debug.LogError("Invalid game object index");
            return;
        }

        // 이전 활성화된 오브젝트를 비활성화
        if (currentActiveObject != null)
        {
            currentActiveObject.SetActive(false);
        }

        // 새로운 오브젝트를 활성화
        currentActiveObject = gameObjects[index];
        currentActiveObject.SetActive(true);

        for (int i = 0; i < tabButtons.Length; i++)
        {
            if (i == index)
            {
                alphaChanger.SetAlpha(tabButtons[i], 1.0f); // 활성화된 탭의 알파값을 1로 설정
            }
            else
            {
                alphaChanger.SetAlpha(tabButtons[i], 0.4f); // 비활성화된 탭의 알파값을 0.4로 설정
            }
        }
    }
}
