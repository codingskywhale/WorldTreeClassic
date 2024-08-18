using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Diagnostics;

public class CameraController : MonoBehaviour
{
    public Transform target; // 나무의 Transform을 에디터에서 할당
    public Button toggleButton;
    public float rotationSpeed = 2f; // 회전 속도
    public GameObject messagePrefab; // 메시지를 표시할 프리팹
    public Transform messageParent; // 메시지를 표시할 부모 객체
           
    private CameraTargetHandler cameraTargetHandler;
    private CameraTransition cameraTransition;
    private GameObject currentMessage;

    public float zoomSpeed = 0.5f; // 줌 속도
    public float minZoomDistance = 2f; // 줌인의 최소 거리
    public float maxZoomDistance = 10f;

    public bool isFreeCamera = false;
    private bool isDragging = false;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;

        cameraTargetHandler = GetComponent<CameraTargetHandler>();
        cameraTransition = GetComponent<CameraTransition>();

        // 메시지 표시 등 초기화
        messageParent.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (CameraSettings.Instance.animationCompleted)
        {
            if (isFreeCamera)
            {
                HandleFreeCamera();
                HandlePinchZoom();
            }
            else if (cameraTargetHandler.isObjectTarget && cameraTargetHandler.currentTarget != null)
            {
                cameraTargetHandler.FollowObject();
            }

            if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
            {
                HandleClick();
            }
        }
    }

    private void HandleFreeCamera()
    {
        if (CameraSettings.Instance.isZooming) return;

        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            isDragging = true; // 드래그 시작
        }
        else if (Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended))
        {
            isDragging = false; // 드래그 종료
        }

        if (isDragging)
        {
            RotateCamera();
        }
        else if (cameraTargetHandler.currentTarget != null)
        {
            Camera.main.transform.LookAt(cameraTargetHandler.currentTarget);
        }
    }

    private void HandlePinchZoom()
    {
        // 터치가 두 개 이상일 때 핀치 제스처 처리
        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // 두 터치의 이전 위치와 현재 위치 사이의 거리 계산
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float currentTouchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // 이전 거리와 현재 거리의 차이
            float deltaMagnitudeDiff = prevTouchDeltaMag - currentTouchDeltaMag;

            // 카메라 줌을 적용
            ZoomCamera(deltaMagnitudeDiff * zoomSpeed);
        }
    }

    private void ZoomCamera(float increment)
    {
        // 카메라의 현재 거리에서 줌을 계산
        float currentDistance = Vector3.Distance(mainCamera.transform.position, cameraTargetHandler.currentTarget.position);

        // 새로운 줌 거리를 계산하고 제한 범위를 적용
        float newDistance = Mathf.Clamp(currentDistance + increment, minZoomDistance, maxZoomDistance);

        // 카메라의 위치 업데이트 (타겟을 기준으로)
        mainCamera.transform.position = cameraTargetHandler.currentTarget.position + (mainCamera.transform.position - cameraTargetHandler.currentTarget.position).normalized * newDistance;
    }

    private void RotateCamera()
    {
        float horizontal = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        float vertical = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

        if (cameraTargetHandler.currentTarget != null)
        {
            // 카메라가 회전하려고 하는 각도를 저장
            float desiredXRotation = Camera.main.transform.eulerAngles.x - vertical;

            // 수직 각도가 제한 내에 있는 경우에만 회전 적용
            if (desiredXRotation >= CameraSettings.Instance.minVerticalAngle && desiredXRotation <= CameraSettings.Instance.maxVerticalAngle)
            {
                Camera.main.transform.RotateAround(cameraTargetHandler.currentTarget.position, Camera.main.transform.right, -vertical);
            }

            // 수평 회전은 항상 허용
            Camera.main.transform.RotateAround(cameraTargetHandler.currentTarget.position, Vector3.up, horizontal);

            // 각도 제한: X축 각도 제한 적용
            Vector3 angles = Camera.main.transform.eulerAngles;
            angles.x = Mathf.Clamp(angles.x, CameraSettings.Instance.minVerticalAngle, CameraSettings.Instance.maxVerticalAngle);

            // 제한된 각도를 적용
            Camera.main.transform.eulerAngles = angles;

            // 카메라가 타겟을 계속 바라보도록 설정
            Camera.main.transform.LookAt(cameraTargetHandler.currentTarget);
        }
    }

    private void HandleClick()
    {
        if (!isFreeCamera) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            var clickable = hit.transform.GetComponent<IClickableObject>();
            clickable?.OnPointerClick(new PointerEventData(EventSystem.current));
        }
    }

    public void ToggleCameraMode()
    {
        if (CameraSettings.Instance.isZooming || !CameraSettings.Instance.animationCompleted) return;

        if (isFreeCamera)
        {
            SwitchToFixedCameraMode();
        }
        else
        {
            SwitchToFreeCameraMode();
        }
    }

    public void SwitchToFixedCameraMode()
    {
        if (CameraSettings.Instance.isZooming) return;

        // 나무 레벨에 따른 고정된 위치 계산
        int treeLevel = DataManager.Instance.touchData.touchIncreaseLevel;
        Vector3 fixedPosition = CameraSettings.Instance.GetInitialPosition(treeLevel);

        // 고정된 회전 값 사용
        Quaternion fixedRotation = CameraSettings.Instance.finalRotation;

        // 카메라 위치와 회전 값을 명확하게 초기화
        Camera.main.transform.position = fixedPosition;
        Camera.main.transform.rotation = fixedRotation;

        // 나무를 정확히 바라보게 설정
        Camera.main.transform.LookAt(CameraSettings.Instance.worldTree.transform);

        // 현재 상태 업데이트
        CameraSettings.Instance.currentCameraPosition = fixedPosition;
        CameraSettings.Instance.currentCameraRotation = Camera.main.transform.rotation;

        // 모드 전환
        isFreeCamera = false;
        cameraTargetHandler.SetFreeCameraMode(isFreeCamera);

        ShowMessage("카메라가 나무에 고정됩니다.");

        // 버튼 다시 활성화
        StartCoroutine(EnableButtonAfterDelay(1.0f));
    }

    public void SwitchToFreeCameraMode()
    {
        // 자유시점 모드로 전환되기 전에 모든 코루틴 중지
        StopAllCoroutines();

        // 자유시점 모드에서는 현재의 카메라 위치와 회전 값을 유지
        Vector3 currentPosition = Camera.main.transform.position;
        Quaternion currentRotation = Camera.main.transform.rotation;

        // CameraSettings에 상태를 업데이트 (자유시점 모드)
        CameraSettings.Instance.currentCameraPosition = currentPosition;
        CameraSettings.Instance.currentCameraRotation = currentRotation;

        // 자유시점 모드로 설정
        isFreeCamera = true;
        cameraTargetHandler.SetFreeCameraMode(isFreeCamera);

        // 자유시점 모드 설정 후 메시지 표시
        ShowMessage("카메라 자유 조작이 활성화됩니다.");

        // 1초 후 버튼 다시 활성화
        StartCoroutine(EnableButtonAfterDelay(1.0f));
    }

    private IEnumerator EnableButtonAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        toggleButton.interactable = true;
    }

    private void ShowMessage(string message)
    {
        // 기존 메시지가 있다면 제거
        if (currentMessage != null)
        {
            Destroy(currentMessage);
        }

        messageParent.gameObject.SetActive(true);

        // 메시지 생성
        currentMessage = Instantiate(messagePrefab, messageParent);
        currentMessage.GetComponent<TMP_Text>().text = message;

        // 메시지 애니메이션 시작
        StartCoroutine(FadeAndMoveMessage(currentMessage));
    }

    private IEnumerator FadeAndMoveMessage(GameObject message)
    {
        TMP_Text messageText = message.GetComponent<TMP_Text>();
        Color originalColor = messageText.color;
        Vector3 originalPosition = message.transform.position;

        float duration = 2f;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1.0f, 0.0f, elapsed / duration);
            messageText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            message.transform.position = originalPosition + new Vector3(0, elapsed * 10, 0); // 조금씩 위로 이동

            yield return null;
        }

        Destroy(message);
        messageParent.gameObject.SetActive(false);
    }
}
