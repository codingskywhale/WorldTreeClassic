using System.Collections;
using UnityEngine;

public class CameraTransition : MonoBehaviour
{
    public Vector3 initialPosition = new Vector3(0, 6.5f, -11); // 카메라의 초기 위치
    public Quaternion initialRotation = Quaternion.Euler(20, 0, 0); // 카메라의 초기 회전
    public Quaternion finalRotation = Quaternion.Euler(20, 0, 0); // 오프닝 애니메이션 후 카메라의 회전
    public Vector3 zoomInPosition = new Vector3(0, 5, -10); // 줌인 위치 (UI 창이 열릴 때)
    public Quaternion zoomInRotation = Quaternion.Euler(20, 0, 0); // 줌인 상태에서의 회전
    public float duration = 2.5f; // 애니메이션 시간
    public float zoomDuration = 1.0f; // 줌 애니메이션 시간

    public bool animationCompleted = false;
    public bool isZooming = false;

    public IEnumerator OpeningCamera()
    {
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            // 카메라 회전 Lerp
            Camera.main.transform.rotation = Quaternion.Slerp(initialRotation, finalRotation, t);

            yield return null;
        }

        // 애니메이션 완료
        animationCompleted = true;
    }

    public IEnumerator ZoomCamera(Vector3 targetPosition, Quaternion targetRotation)
    {
        isZooming = true; // 줌 애니메이션 시작
        float startTime = Time.time;
        Vector3 startPosition = Camera.main.transform.position;
        Quaternion startRotation = Camera.main.transform.rotation;

        while (Time.time < startTime + zoomDuration)
        {
            float t = (Time.time - startTime) / zoomDuration;
            Camera.main.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            Camera.main.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
            yield return null;
        }

        Camera.main.transform.position = targetPosition;
        Camera.main.transform.rotation = targetRotation;
        isZooming = false; // 줌 애니메이션 종료
    }
}
