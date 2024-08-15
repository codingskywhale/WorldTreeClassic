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

        while (elapsedTime < CameraSettings.Instance.zoomDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / CameraSettings.Instance.zoomDuration;

            CameraSettings.Instance.currentCameraPosition = Vector3.Lerp(startPosition, targetPosition, t);
            CameraSettings.Instance.currentCameraRotation = Quaternion.Slerp(startRotation, targetRotation, t);

            Camera.main.transform.position = CameraSettings.Instance.currentCameraPosition;
            Camera.main.transform.rotation = CameraSettings.Instance.currentCameraRotation;

            yield return null;
        }

        // 카메라 위치와 회전을 목표값으로 설정
        Camera.main.transform.position = targetPosition;
        Camera.main.transform.rotation = targetRotation;

        CameraSettings.Instance.isZooming = false;

        // 고정 시점 모드로 돌아올 때 이 값을 무시하고 초기화
        if (!CameraTargetHandler.Instance.isFreeCamera)
        {
            Vector3 fixedPosition = CameraSettings.Instance.GetInitialPosition(DataManager.Instance.touchData.touchIncreaseLevel);
            Quaternion fixedRotation = CameraSettings.Instance.GetFinalRotation();

            Camera.main.transform.position = fixedPosition;
            Camera.main.transform.rotation = fixedRotation;
        }
    }

    public void UpdateCameraState(float newFOV, Vector3 newPosition)
    {
        CameraSettings.Instance.UpdateCameraState(newFOV, newPosition, CameraSettings.Instance.currentCameraRotation);
    }
}

