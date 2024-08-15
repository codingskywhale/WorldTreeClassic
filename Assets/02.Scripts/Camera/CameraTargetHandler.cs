using System.Collections;
using UnityEngine;

public class CameraTargetHandler : MonoBehaviour
{
    public static CameraTargetHandler Instance { get; private set; }

    public Transform currentTarget; // 현재 타겟
    public bool isObjectTarget = false;    
    private CameraTransition cameraTransition;

    public bool isFreeCamera = false; // 자유시점 모드 여부
        
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }       
    private void Start()
    {
        cameraTransition = GetComponent<CameraTransition>();
    }

    public void SetTarget(Transform newTarget)
    {
        if (!isFreeCamera) // 자유시점 모드가 아닌 경우 타겟 변경 무시
        {
            return;
        }

        if (currentTarget == newTarget) // 이미 타겟이 설정된 경우 이벤트 무시
        {
            return;
        }

        currentTarget = newTarget;
        isObjectTarget = true;

        // 여기에 기존 카메라 위치를 초기화하여 누적 오프셋을 방지
        CameraSettings.Instance.currentCameraPosition = Vector3.zero;

        StartCoroutine(ZoomToTarget(newTarget));
    }

    private IEnumerator ZoomToTarget(Transform newTarget)
    {
        CameraSettings.Instance.isZooming = true;
        float startTime = Time.time;
        Vector3 startPosition = Camera.main.transform.position;
        Quaternion startRotation = Camera.main.transform.rotation;

        // 카메라와 타겟 사이의 적절한 거리 유지
        Vector3 direction = (startPosition - newTarget.position).normalized;
        Vector3 targetPosition = newTarget.position + direction * (startPosition - newTarget.position).magnitude;

        Quaternion targetRotation = Quaternion.LookRotation(newTarget.position - targetPosition);

        while (Time.time < startTime + CameraSettings.Instance.zoomDuration)
        {
            float t = (Time.time - startTime) / CameraSettings.Instance.zoomDuration;
            Camera.main.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            Camera.main.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
            yield return null;
        }

        Camera.main.transform.position = targetPosition;
        Camera.main.transform.rotation = targetRotation;
        CameraSettings.Instance.isZooming = false;
    }

    public void FollowObject()
    {
        if (currentTarget != null)
        {
            // 현재 카메라의 회전 값을 저장
            Quaternion originalRotation = Camera.main.transform.rotation;

            // 타겟의 위치를 기준으로 카메라 위치를 업데이트
            Vector3 offset = (Camera.main.transform.position - currentTarget.position).normalized * CameraSettings.Instance.currentCameraPosition.magnitude;
            Camera.main.transform.position = currentTarget.position + offset;

            // 저장한 회전 값을 다시 설정하여 회전 값이 변경되지 않도록 함
            Camera.main.transform.rotation = originalRotation;
        }
    }

    public void SetFreeCameraMode(bool isFree)
    {
        isFreeCamera = isFree;
    }

    public bool IsFreeCamera()
    {
        return isFreeCamera;
    }
}

