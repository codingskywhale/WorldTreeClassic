using UnityEngine;

public class GameObjectManager : MonoBehaviour
{
    public GameObject[] gameObjects; // ���� ������Ʈ���� ������ �迭
    private GameObject currentActiveObject; // ���� Ȱ��ȭ�� ������Ʈ

    // ��ư Ŭ�� �� ȣ��� �޼���
    public void ShowGameObject(int index)
    {
        // ��ȿ�� �ε������� Ȯ��
        if (index < 0 || index >= gameObjects.Length)
        {
            Debug.LogError("Invalid game object index");
            return;
        }

        // ���� Ȱ��ȭ�� ������Ʈ�� ��Ȱ��ȭ
        if (currentActiveObject != null)
        {
            currentActiveObject.SetActive(false);
        }

        // ���ο� ������Ʈ�� Ȱ��ȭ
        currentActiveObject = gameObjects[index];
        currentActiveObject.SetActive(true);
    }
}
