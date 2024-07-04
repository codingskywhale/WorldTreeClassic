using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class CameraController : MonoBehaviour
{
    public Transform target; // 나무의 Transform을 에디터에서 할당
    public Button toggleButton;
    public float rotationSpeed = 10f; // 회전 속도

    private CameraTransition cameraTransition;
    private CameraTargetHandler cameraTargetHandler;

    private bool isFreeCamera = false;
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
            if (isFreeCamera && !cameraTargetHandler.isAnimalTarget)
            {
                HandleFreeCamera();
            }
            else if (cameraTargetHandler.isAnimalTarget && cameraTargetHandler.currentTarget != null)
            {
                cameraTargetHandler.FollowAnimal();
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
            cameraTargetHandler.HandleAnimalClick();
        }
    }

    private void RotateCamera()
    {
        float horizontal = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        float vertical = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

        // 카메라가 현재 타겟 주변을 회전하도록 설정
        if (cameraTargetHandler.currentTarget != null)
        {
            Camera.main.transform.RotateAround(cameraTargetHandler.currentTarget.position, Vector3.up, horizontal);
            Camera.main.transform.RotateAround(cameraTargetHandler.currentTarget.position, Camera.main.transform.right, -vertical);

            // 타겟을 계속 바라보도록 카메라의 로컬 회전 조정
            Camera.main.transform.LookAt(cameraTargetHandler.currentTarget);
        }
    }

    public void ToggleFreeCamera()
    {
        if (cameraTransition.isZooming || !cameraTransition.animationCompleted) return;

        StopAllCoroutines();
        if (isFreeCamera)
        {
            // 자유 시점 모드에서 고정 시점 모드로 전환
            cameraTargetHandler.SetTarget(target);
            cameraTargetHandler.isAnimalTarget = false;
            StartCoroutine(cameraTransition.ZoomCamera(cameraTransition.initialPosition, cameraTransition.finalRotation));
        }
        else
        {   
            // 고정 시점 모드에서 자유 시점 모드로 전환
            cameraTargetHandler.SetTarget(target);
            cameraTargetHandler.isAnimalTarget = false;
            StartCoroutine(cameraTransition.ZoomCamera(cameraTransition.zoomInPosition, cameraTransition.zoomInRotation));
        }

        // 모드 전환
        isFreeCamera = !isFreeCamera;

        // 1초 후 버튼 다시 활성화
        StartCoroutine(EnableButtonAfterDelay(1.0f));
    }

    private IEnumerator EnableButtonAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        toggleButton.interactable = true;
    }
}