using System.Collections;
using UnityEngine;

public class CameraTargetHandler : MonoBehaviour
{
    public static CameraTargetHandler Instance { get; private set; }
    public Transform currentTarget; // 현재 타겟
    public bool isObjectTarget = false;
    private Vector3 offset = new Vector3(0, 5f, -15); // 타겟에 대한 카메라 오프셋
    private CameraTransition cameraTransition;

    public float minVerticalAngle = 0f; // 최소 각도 제한
    public float maxVerticalAngle = 30f; // 최대 각도 제한
    public float minHeight = 1f; // 카메라의 최소 높이 제한
    public float maxHeight = 10f; // 카메라의 최대 높이 제한

    private bool isFreeCamera = false;

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
        currentTarget = newTarget;
        isObjectTarget = true;
        StartCoroutine(ZoomToTarget(newTarget));
    }

    private IEnumerator ZoomToTarget(Transform newTarget)
    {
        cameraTransition.isZooming = true;
        float startTime = Time.time;
        Vector3 startPosition = Camera.main.transform.position;
        Quaternion startRotation = Camera.main.transform.rotation;

        Vector3 targetPosition = newTarget.position + offset; // 타겟의 위치를 기준으로 카메라 위치 조정
        Quaternion targetRotation = Quaternion.LookRotation(newTarget.position - targetPosition);

        while (Time.time < startTime + cameraTransition.zoomDuration)
        {
            float t = (Time.time - startTime) / cameraTransition.zoomDuration;
            Camera.main.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            Camera.main.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
            yield return null;
        }

        Camera.main.transform.position = targetPosition;
        Camera.main.transform.rotation = targetRotation;
        cameraTransition.isZooming = false;
    }

    public void FollowObject()
    {
        if (currentTarget != null)
        {
            Vector3 targetPosition = currentTarget.position + offset;
            Camera.main.transform.position = targetPosition;

            // 카메라가 타겟을 계속 바라보도록 설정
            Camera.main.transform.LookAt(currentTarget);
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

