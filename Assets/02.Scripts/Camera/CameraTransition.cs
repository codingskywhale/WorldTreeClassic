using System.Collections;
using UnityEngine;

public class CameraTransition : MonoBehaviour
{
    public Vector3 initialPosition = new Vector3(-1.7f, 2.5f, -2.3f); // 카메라의 초기 위치
    public Quaternion initialRotation = Quaternion.Euler(-90, -116, 0); // 카메라의 초기 회전
    public Quaternion finalRotation = Quaternion.Euler(20, -116, 0); // 오프닝 애니메이션 후 카메라의 회전
    public Vector3 zoomInPosition = new Vector3(-1.7f, 2.3f, -2.3f); // 줌인 위치 (UI 창이 열릴 때)
    public Quaternion zoomInRotation = Quaternion.Euler(25, -116, 0); // 줌인 상태에서의 회전
    public float duration = 2.5f; // 애니메이션 시간
    public float zoomDuration = 1.0f; // 줌 애니메이션 시간

    public bool animationCompleted = false;
    public bool isZooming = false;

    private Camera mainCamera;
    public WorldTree worldTree;

    private Vector3 currentCameraPosition;
    private Quaternion currentCameraRotation;
    public float currentCameraFOV;

    private void Awake()
    {
        mainCamera = Camera.main;

        int currentLevel = DataManager.Instance.touchData.touchIncreaseLevel;
        AdjustInitialCameraValues(currentLevel);

        currentCameraPosition = mainCamera.transform.position;
        currentCameraRotation = mainCamera.transform.rotation;
        currentCameraFOV = mainCamera.fieldOfView;
    }

    private void AdjustInitialCameraValues(int treeLevel)
    {
        int levelFactor = treeLevel / 10;

        if (levelFactor > 0)
        {
            initialPosition -= mainCamera.transform.forward * (worldTree.positionIncrement * levelFactor);
        }
    }

    public IEnumerator OpeningCamera()
    {
        float elapsedTime = 0f;

        // 카메라 초기 위치와 회전 설정
        mainCamera.transform.position = currentCameraPosition;
        mainCamera.transform.rotation = currentCameraRotation;
        mainCamera.fieldOfView = currentCameraFOV;

        // 카메라 회전 애니메이션
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            mainCamera.transform.rotation = Quaternion.Slerp(initialRotation, finalRotation, t);

            yield return null;
        }

        // 애니메이션 완료
        animationCompleted = true;
    }

    public IEnumerator ZoomCamera(Vector3 targetPosition, Quaternion targetRotation)
    {
        isZooming = true; // 줌 애니메이션 시작
        float startTime = Time.time;
        Vector3 startPosition = mainCamera.transform.position;
        Quaternion startRotation = mainCamera.transform.rotation;

        while (Time.time < startTime + zoomDuration)
        {
            float t = (Time.time - startTime) / zoomDuration;
            mainCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            mainCamera.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
            yield return null;
        }

        mainCamera.transform.position = targetPosition;
        mainCamera.transform.rotation = targetRotation;
        isZooming = false; // 줌 애니메이션 종료
    }

    public void UpdateCameraState(float newFOV, Vector3 newPosition)
    {
        mainCamera.fieldOfView = newFOV;
        mainCamera.transform.position = newPosition;
    }
}

