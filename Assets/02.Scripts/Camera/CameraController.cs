using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class CameraController : MonoBehaviour
{
    public Transform target; // 나무의 Transform을 에디터에서 할당
    public Button toggleButton;
    public float rotationSpeed = 10f; // 회전 속도

    private CameraTransition cameraTransition;
    private CameraTargetHandler cameraTargetHandler;

    public bool isFreeCamera = false;
    private bool isDragging = false;

    private void Start()
    {
        cameraTransition = GetComponent<CameraTransition>();
        cameraTargetHandler = GetComponent<CameraTargetHandler>();

        // 초기 위치와 회전 설정
        Camera.main.transform.position = cameraTransition.initialPosition;
        Camera.main.transform.rotation = cameraTransition.initialRotation;

        // 애니메이션 시작
        StartCoroutine(cameraTransition.OpeningCamera());
    }

    private void Update()
    {
        if (cameraTransition.animationCompleted)
        {
            if (isFreeCamera)
            {
                HandleFreeCamera();
            }
            else if (cameraTargetHandler.isObjectTarget && cameraTargetHandler.currentTarget != null)
            {
                cameraTargetHandler.FollowObject();
            }

            if (Input.GetMouseButtonDown(0))
            {
                HandleClick();
            }
        }
    }


    private void HandleFreeCamera()
    {
        if (cameraTransition.isZooming) return;

        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true; // 드래그 시작
        }
        else if (Input.GetMouseButtonUp(0))
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
            // 기존 위치와 회전을 백업
            Vector3 originalPosition = Camera.main.transform.position;
            Quaternion originalRotation = Camera.main.transform.rotation;

            // 카메라의 위치 이동
            Camera.main.transform.RotateAround(cameraTargetHandler.currentTarget.position, Vector3.up, horizontal);
            Camera.main.transform.RotateAround(cameraTargetHandler.currentTarget.position, Camera.main.transform.right, -vertical);

            // 카메라의 각도 제한 적용
            Vector3 angles = Camera.main.transform.eulerAngles;
            angles.x = Mathf.Clamp(angles.x, cameraTargetHandler.minVerticalAngle, cameraTargetHandler.maxVerticalAngle);

            // 카메라의 높이 제한 적용
            Vector3 position = Camera.main.transform.position;
            position.y = Mathf.Clamp(position.y, cameraTargetHandler.minHeight, cameraTargetHandler.maxHeight);

            // 제한된 각도 및 높이로 카메라 설정
            Camera.main.transform.eulerAngles = angles;
            Camera.main.transform.position = position;

            // 각도나 높이가 제한을 벗어나면 회전을 취소하고 원래 상태로 되돌리기
            if (position.y != Camera.main.transform.position.y || angles.x != Camera.main.transform.eulerAngles.x)
            {
                Camera.main.transform.position = originalPosition;
                Camera.main.transform.rotation = originalRotation;
            }

            // 카메라가 타겟을 계속 바라보도록 설정
            Camera.main.transform.LookAt(cameraTargetHandler.currentTarget);
        }
    }

    private void HandleClick()
    {       
        if (!isFreeCamera) // 자유시점 모드가 아닌 경우 클릭 이벤트 무시
        {            
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            var clickable = hit.transform.GetComponent<IClickableObject>();
            if (clickable != null)
            {
                Debug.Log("HandleClick: Processing click on " + hit.transform.name);
                clickable.OnPointerClick(new PointerEventData(EventSystem.current));
            }
        }
    }

    public void ToggleCameraMode()
    {
        if (cameraTransition.isZooming || !cameraTransition.animationCompleted) return;

        StopAllCoroutines();
        if (isFreeCamera)
        {
            // 자유 시점 모드에서 고정 시점 모드로 전환
            cameraTargetHandler.SetTarget(target);
            cameraTargetHandler.isObjectTarget = false;
            StartCoroutine(cameraTransition.ZoomCamera(cameraTransition.initialPosition, cameraTransition.finalRotation));
        }
        else
        {
            // 고정 시점 모드에서 자유 시점 모드로 전환
            cameraTargetHandler.SetTarget(target);
            cameraTargetHandler.isObjectTarget = false;
            StartCoroutine(cameraTransition.ZoomCamera(cameraTransition.zoomInPosition, cameraTransition.zoomInRotation));
        }

        // 모드 전환
        isFreeCamera = !isFreeCamera;

        cameraTargetHandler.SetFreeCameraMode(isFreeCamera); // 자유시점 모드 설정
                
        // 1초 후 버튼 다시 활성화
        StartCoroutine(EnableButtonAfterDelay(1.0f));
    }

    private IEnumerator EnableButtonAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        toggleButton.interactable = true;
    }
}