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
        Vector3 startPosition = Camera.main.transform.position; // 실제 위치
        Quaternion startRotation = Camera.main.transform.rotation;

        Vector3 endPosition = CameraSettings.Instance.GetInitialPosition(DataManager.Instance.touchData.touchIncreaseLevel);
        Quaternion endRotation = CameraSettings.Instance.GetFinalRotation();

        while (elapsedTime < CameraSettings.Instance.duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / CameraSettings.Instance.duration;

            Camera.main.transform.position = Vector3.Lerp(startPosition, endPosition, t);
            Camera.main.transform.rotation = Quaternion.Slerp(startRotation, endRotation, t);

            yield return null;
        }

        Camera.main.transform.position = endPosition;
        Camera.main.transform.rotation = endRotation;

        CameraSettings.Instance.currentCameraPosition = endPosition;
        CameraSettings.Instance.currentCameraRotation = endRotation;
        Debug.Log($"OpeningCamera 종료 직후 위치: {Camera.main.transform.position}");
        Debug.Log($"OpeningCamera 종료 직후 회전: {Camera.main.transform.eulerAngles}");

        CameraSettings.Instance.animationCompleted = true;
    }


    public IEnumerator ZoomCamera(Vector3 targetPosition, Quaternion targetRotation, float zoomDuration)
    {
        CameraSettings.Instance.isZooming = true;
        float elapsedTime = 0f;
        Vector3 startPosition = Camera.main.transform.position;  // 실제 카메라의 현재 위치를 가져옴
        Quaternion startRotation = Camera.main.transform.rotation;  // 실제 카메라의 현재 회전 값을 가져옴

        while (elapsedTime < zoomDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / zoomDuration;

            // 부드럽게 카메라의 위치와 회전 값 전환
            Vector3 newPosition = Vector3.Lerp(startPosition, targetPosition, t);
            Quaternion newRotation = Quaternion.Slerp(startRotation, targetRotation, t);

            Camera.main.transform.position = newPosition;
            Camera.main.transform.rotation = newRotation;

            yield return null;
        }

        // 전환 완료 후 정확한 위치와 회전 값 설정
        Camera.main.transform.position = targetPosition;
        Camera.main.transform.rotation = targetRotation;

        // 코루틴 완료 후 고정시점 모드 확인
        CameraSettings.Instance.isZooming = false;

        if (!CameraTargetHandler.Instance.isFreeCamera)
        {
            // 고정시점 모드로 돌아가는 경우
            Vector3 fixedPosition = CameraSettings.Instance.GetInitialPosition(DataManager.Instance.touchData.touchIncreaseLevel);
            Quaternion fixedRotation = CameraSettings.Instance.GetFinalRotation();

            Camera.main.transform.position = fixedPosition;
            Camera.main.transform.rotation = fixedRotation;

            // CameraSettings에 위치와 회전 상태 업데이트
            CameraSettings.Instance.currentCameraPosition = fixedPosition;
            CameraSettings.Instance.currentCameraRotation = fixedRotation;
        }
    }

    public void UpdateCameraState(float newFOV, Vector3 newPosition)
    {
        CameraSettings.Instance.UpdateCameraState(newFOV, newPosition, CameraSettings.Instance.currentCameraRotation);
    }
}

