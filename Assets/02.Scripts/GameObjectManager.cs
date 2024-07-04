using UnityEngine;

public class GameObjectManager : MonoBehaviour
{
    public GameObject[] gameObjects; // 게임 오브젝트들을 저장할 배열
    private GameObject currentActiveObject; // 현재 활성화된 오브젝트

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
    }
}
