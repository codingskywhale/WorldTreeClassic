using System.Collections.Generic;
using TMPro;
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
    public float maxFOV = 120f; // 최대 FOV 값    
    public float positionIncrement = 0.3f;

    private Vector3 positionOffset = Vector3.zero;
    private float fovOffset = 0f;

    private void Start()
    {
        UpdateTreeMeshes(0);
    }

    public void UpdateTreeMeshes(int currentLevel)
    {
        int currentIndex = (currentLevel / 5) % treePrefabs.Length;

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

        if (currentLevel % 5 == 0 && currentLevel != 0)
        {
            IncrementCameraFOV();
            MoveCameraBackwards();
        }
    }

    private void IncrementCameraFOV()
    {
        if (mainCamera != null)
        {
            float newFOV = mainCamera.fieldOfView + FOVIncrement;
            mainCamera.fieldOfView = Mathf.Min(newFOV, maxFOV); // 최대 FOV를 넘지 않도록 제한
            fovOffset += FOVIncrement;
            UpdateCameraTransition();
        }
    }

    private void MoveCameraBackwards()
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
