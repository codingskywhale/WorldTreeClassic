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

    public bool isFreeCamera = false;
    private bool isDragging = false;

    private void Start()
    {        
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

    private void RotateCamera()
    {
        float horizontal = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        float vertical = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

        if (cameraTargetHandler.currentTarget != null)
        {
            // 카메라의 기존 위치에서 회전 시도
            Camera.main.transform.RotateAround(cameraTargetHandler.currentTarget.position, Vector3.up, horizontal);
            Camera.main.transform.RotateAround(cameraTargetHandler.currentTarget.position, Camera.main.transform.right, -vertical);

            // 각도 제한: 위쪽 회전을 제한 (예: -85도에서 85도 사이)
            Vector3 angles = Camera.main.transform.eulerAngles;

            // X축 각도 제한 (위아래 회전)
            angles.x = Mathf.Clamp(angles.x, CameraSettings.Instance.minVerticalAngle, CameraSettings.Instance.maxVerticalAngle);

            // 제한된 각도를 적용
            Camera.main.transform.eulerAngles = angles;

            // 타겟을 바라보도록 설정
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
        // 코루틴 중지
        StopAllCoroutines();

        // 고정된 위치와 회전 설정
        Vector3 fixedPosition = CameraSettings.Instance.GetInitialPosition(DataManager.Instance.touchData.touchIncreaseLevel);
        Quaternion fixedRotation = CameraSettings.Instance.GetFinalRotation();

        // 타겟을 고정시점모드의 타겟으로 설정
        cameraTargetHandler.SetTarget(target);
        cameraTargetHandler.isObjectTarget = false;

        // 강제 위치와 회전 설정 (자유시점모드에서의 영향 무시)
        Camera.main.transform.position = fixedPosition;
        Camera.main.transform.rotation = fixedRotation;

        // CameraSettings에 상태를 갱신
        CameraSettings.Instance.currentCameraPosition = fixedPosition;
        CameraSettings.Instance.currentCameraRotation = fixedRotation;

        // 디버그 로그 추가 (테스트용)
        UnityEngine.Debug.Log($"[SwitchToFixedCameraMode] Position: {fixedPosition}, Rotation: {fixedRotation}");

        ShowMessage("카메라가 나무에 고정됩니다.");

        // 모드 플래그 설정
        isFreeCamera = false;
        cameraTargetHandler.SetFreeCameraMode(isFreeCamera);

        // 버튼 다시 활성화
        StartCoroutine(EnableButtonAfterDelay(1.0f));
    }

    public void SwitchToFreeCameraMode()
    {
        // 타겟을 자유시점모드의 타겟으로 설정 (필요시)
        cameraTargetHandler.SetTarget(target);
        cameraTargetHandler.isObjectTarget = false;

        // 자유시점모드로 전환 시 현재 위치와 회전 유지
        Vector3 currentPosition = Camera.main.transform.position;
        Quaternion currentRotation = Camera.main.transform.rotation;

        // Camera.main.transform.position = currentPosition; // 줌 코루틴 제거
        // Camera.main.transform.rotation = currentRotation; // 줌 코루틴 제거

        ShowMessage("카메라 자유 조작이 활성화됩니다.");

        // 모드 플래그 설정
        isFreeCamera = true;
        cameraTargetHandler.SetFreeCameraMode(isFreeCamera);

        // 버튼 다시 활성화
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
