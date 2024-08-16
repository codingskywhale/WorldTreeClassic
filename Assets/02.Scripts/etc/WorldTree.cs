using System.Collections.Generic;
using TMPro;
using Unity.AI.Navigation;
using UnityEngine;

public class WorldTree : MonoBehaviour
{
    public GameObject outsideTreeObject; // 3D 오브젝트
    public TextMeshProUGUI upgradeRequirementText;
    public MeshFilter groundMeshFilter; // 3D 오브젝트
    public GameObject[] treePrefabs; // 3D 모델 배열

    private GameObject currentTreeInstance; // 현재 인스턴스화된 트리 프리팹
    public Camera mainCamera;
    public CameraTransition cameraTransition;

    public float initialFOV = 60f; // 초기 FOV 값
    public float FOVIncrement = 2f; // 나무의 외형이 바뀔 때마다 증가할 FOV 값
    public float maxFOV = 100f; // 최대 FOV 값    
    public float positionIncrement = 0.4f;

    private Vector3 positionOffset = Vector3.zero;
    private float fovOffset = 0f;

    private void Start()
    {
        UpdateTreeMeshes(0);
    }

    public void UpdateTreeMeshes(int currentLevel)
    {
        int currentIndex = GetTreePrefabIndex(currentLevel);

        if (currentTreeInstance == null || currentTreeInstance.name != treePrefabs[currentIndex].name)
        {
            // 현재 트리 프리팹이 존재하면 제거
            if (currentTreeInstance != null)
            {
                Destroy(currentTreeInstance);
            }

            // 새로운 트리 프리팹 인스턴스화
            currentTreeInstance = Instantiate(treePrefabs[currentIndex], outsideTreeObject.transform.position, Quaternion.identity);
            currentTreeInstance.transform.SetParent(outsideTreeObject.transform);
            currentTreeInstance.name = treePrefabs[currentIndex].name; // 이름 설정
        }

        UpdateTreeScale(currentLevel);

        if (currentLevel % 10 == 0 && currentLevel != 0)
        {
            IncrementCameraFOV();
            MoveCameraBackwards();
            DataManager.Instance.animalSpawnTr.transform.position += Vector3.right / 10;
        }
    }

    private int GetTreePrefabIndex(int currentLevel)
    {
        if (currentLevel >= 500) return 6;
        if (currentLevel >= 400) return 5;
        if (currentLevel >= 300) return 4;
        if (currentLevel >= 200) return 3;
        if (currentLevel >= 150) return 2;
        if (currentLevel >= 100) return 1;
        return 0;
    }

    private void UpdateTreeScale(int currentLevel)
    {
        Vector3 newScale = Vector3.one;

        if (currentLevel < 100)
        {
            newScale = Vector3.one + Vector3.one * (currentLevel / 10) * 0.2f;
        }
        else if (currentLevel < 150)
        {
            newScale = Vector3.one * 0.3f + Vector3.one * ((currentLevel - 100) / 10) * 0.1f;
        }
        else if (currentLevel < 200)
        {
            newScale = Vector3.one * 0.8f + Vector3.one * ((currentLevel - 150) / 10) * 0.1f;
        }
        else if (currentLevel < 220)
        {
            newScale = Vector3.one * 0.9f;
        }
        else if (currentLevel < 240)
        {
            newScale = Vector3.one * 0.9f + Vector3.one * ((currentLevel - 220) / 10) * 0.1f;
        }
        else if (currentLevel < 280)
        {
            newScale = Vector3.one * 1.1f + Vector3.one * ((currentLevel - 240) / 10) * 0.1f;
        }
        else if (currentLevel < 300)
        {
            newScale = Vector3.one * 1.4f;
        }
        else if (currentLevel < 380)
        {
            newScale = Vector3.one * 1.4f + Vector3.one * ((currentLevel - 300) / 10) * 0.1f;
        }
        else if (currentLevel < 400)
        {
            newScale = Vector3.one * 1.5f;
        }
        else if (currentLevel < 480)
        {
            newScale = Vector3.one * 1.5f + Vector3.one * ((currentLevel - 400) / 10) * 0.1f;
        }
        else if (currentLevel < 500)
        {
            newScale = Vector3.one * 2f;
        }
        else if (currentLevel >= 500)
        {
            newScale = Vector3.one * 2f;
        }

        currentTreeInstance.transform.localScale = newScale;
    }

    public void IncrementCameraFOV()
    {
        if (mainCamera != null)
        {
            float newFOV = mainCamera.fieldOfView + FOVIncrement;
            mainCamera.fieldOfView = Mathf.Min(newFOV, maxFOV); // 최대 FOV를 넘지 않도록 제한
            fovOffset += FOVIncrement;
            UpdateCameraTransition();
        }
    }

    public void MoveCameraBackwards()
    {
        if (mainCamera != null)
        {
            Vector3 newPosition = mainCamera.transform.position - mainCamera.transform.forward * positionIncrement;
            mainCamera.transform.position = newPosition;
            positionOffset -= mainCamera.transform.forward * positionIncrement;
            UpdateCameraTransition();
        }
    }

    private void UpdateCameraTransition()
    {
        if (cameraTransition != null)
        {
            cameraTransition.UpdateCameraState(mainCamera.fieldOfView, mainCamera.transform.position);
        }
    }

    public Vector3 GetPositionOffset()
    {
        return positionOffset;
    }

    public float GetFovOffset()
    {
        return fovOffset;
    }
}
