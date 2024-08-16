using System.Collections;
using UnityEngine;

public class CameraTransition : MonoBehaviour
{    
    private void Start()
    {
        int currentLevel = DataManager.Instance.touchData.touchIncreaseLevel;
        CameraSettings.Instance.GetInitialPosition(currentLevel);
        CameraSettings.Instance.currentCameraPosition = CameraSettings.Instance.GetInitialPosition(currentLevel);
        CameraSettings.Instance.currentCameraRotation = CameraSettings.Instance.GetInitialRotation();
        CameraSettings.Instance.currentCameraFOV = Camera.main.fieldOfView;
    }

    public IEnumerator OpeningCamera()
    {
        float elapsedTime = 0f;
        Vector3 startPosition = CameraSettings.Instance.currentCameraPosition;
        Quaternion startRotation = CameraSettings.Instance.currentCameraRotation;

        while (elapsedTime < CameraSettings.Instance.duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / CameraSettings.Instance.duration;

            CameraSettings.Instance.currentCameraRotation = Quaternion.Slerp(startRotation, CameraSettings.Instance.GetFinalRotation(), t);
            Camera.main.transform.rotation = CameraSettings.Instance.currentCameraRotation;

            yield return null;
        }

        CameraSettings.Instance.animationCompleted = true;
    }

    public IEnumerator ZoomCamera(Vector3 targetPosition, Quaternion targetRotation, float zoomDuration)
    {
        CameraSettings.Instance.isZooming = true;
        float elapsedTime = 0f;
        Vector3 startPosition = CameraSettings.Instance.currentCameraPosition;
        Quaternion startRotation = CameraSettings.Instance.currentCameraRotation;

        while (elapsedTime < zoomDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / zoomDuration;

            CameraSettings.Instance.currentCameraPosition = Vector3.Lerp(startPosition, targetPosition, t);
            CameraSettings.Instance.currentCameraRotation = Quaternion.Slerp(startRotation, targetRotation, t);

            Camera.main.transform.position = CameraSettings.Instance.currentCameraPosition;
            Camera.main.transform.rotation = CameraSettings.Instance.currentCameraRotation;

            yield return null;
        }

        // 코루틴이 완료된 후 고정시점 모드 상태를 확인하고 필요 시 초기화
        CameraSettings.Instance.isZooming = false;
        if (!CameraTargetHandler.Instance.isFreeCamera)
        {
            Camera.main.transform.position = CameraSettings.Instance.GetInitialPosition(DataManager.Instance.touchData.touchIncreaseLevel);
            Camera.main.transform.rotation = CameraSettings.Instance.GetFinalRotation();
        }
    }

    public void UpdateCameraState(float newFOV, Vector3 newPosition)
    {
        CameraSettings.Instance.UpdateCameraState(newFOV, newPosition, CameraSettings.Instance.currentCameraRotation);
    }
}

