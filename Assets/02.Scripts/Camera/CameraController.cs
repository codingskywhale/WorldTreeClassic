using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class CameraController : MonoBehaviour
{
    public Transform target; // 나무의 Transform을 에디터에서 할당
    public float duration = 2.5f; // 애니메이션 시간
    public float rotationSpeed = 10f; // 회전 속도
    public float zoomDuration = 1.0f; // 줌 애니메이션 시간
    public Button toggleButton;

    private Vector3 initialPosition = new Vector3(0, 5, -10); // 카메라의 초기 위치
    private Quaternion initialRotation = Quaternion.Euler(-70, 0, 0); // 카메라의 초기 회전
    private Quaternion finalRotation = Quaternion.Euler(20, 0, 0); // 오프닝 애니메이션 후 카메라의 회전
    private Vector3 zoomInPosition = new Vector3(0, 4, -9); // 줌인 위치
    private Quaternion zoomInRotation = Quaternion.Euler(20, 0, 0); // 줌인 상태에서의 회전

    private bool animationCompleted = false;
    private bool isFreeCamera = false;
    private bool isZooming = false;
    private bool isDragging = false;
    private bool isAnimalTarget = false;

    private Transform currentTarget; // 현재 타겟

    private Vector3 animalOffset = new Vector3(0, 5f, -15);

    private void Start()
    {
        // 초기 위치와 회전 설정
        Camera.main.transform.position = initialPosition;
        Camera.main.transform.rotation = initialRotation;

        // 애니메이션 시작
        StartCoroutine(OpeningCamera());
    }

    private void Update()
    {
        if (animationCompleted)
        {
            if (isFreeCamera && !isAnimalTarget)
            {
                HandleFreeCamera();
            }
            else if (isAnimalTarget && currentTarget != null)
            {
                FollowAnimal();
            }
        }
    }

    private IEnumerator OpeningCamera()
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

    private void RotateCamera()
    {
        float horizontal = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        float vertical = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

        // 카메라가 현재 타겟 주변을 회전하도록 설정
        if (currentTarget != null)
        {
            Camera.main.transform.RotateAround(currentTarget.position, Vector3.up, horizontal);
            Camera.main.transform.RotateAround(currentTarget.position, Camera.main.transform.right, -vertical);

            // 타겟을 계속 바라보도록 카메라의 로컬 회전 조정
            Camera.main.transform.LookAt(currentTarget);
        }
    }

    private void HandleFreeCamera()
    {
        if (isZooming) return;

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
            HandleAnimalClick();
        }
    }

    private void HandleAnimalClick()
    {        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
                
        if (Physics.Raycast(ray, out hit)) 
        {           
            if (hit.transform.CompareTag("Animal")) // 동물 오브젝트에 "Animal" 태그를 설정해야 합니다.
            {                
                SetTarget(hit.transform);
            }
        }        
    }

    private void SetTarget(Transform newTarget)
    {
        currentTarget = newTarget;
        isAnimalTarget = true;                
        StartCoroutine(ZoomToTarget(newTarget));
    }

    private IEnumerator ZoomToTarget(Transform newTarget)
    {
        isZooming = true;
        float startTime = Time.time;
        Vector3 startPosition = Camera.main.transform.position;
        Quaternion startRotation = Camera.main.transform.rotation;

        Vector3 targetPosition = newTarget.position + animalOffset; // 동물의 위치를 기준으로 카메라 위치 조정
        Quaternion targetRotation = Quaternion.LookRotation(newTarget.position - targetPosition);

        while (Time.time < startTime + zoomDuration)
        {
            float t = (Time.time - startTime) / zoomDuration;
            Camera.main.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            Camera.main.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
            yield return null;
        }

        Camera.main.transform.position = targetPosition;
        Camera.main.transform.rotation = targetRotation;
        isZooming = false;
    }

    private void FollowAnimal()
    {
        if (currentTarget != null)
        {
            Vector3 targetPosition = currentTarget.position + animalOffset;
            Camera.main.transform.position = targetPosition;
            Camera.main.transform.LookAt(currentTarget);
        }
    }

    public void ToggleFreeCamera()
    {
        if (isZooming || !animationCompleted) return;

        StopAllCoroutines();
        if (isFreeCamera)
        {
            // 자유 시점 모드에서 고정 시점 모드로 전환
            currentTarget = target; // 다시 나무를 타겟으로 설정
            isAnimalTarget = false;
            StartCoroutine(ZoomCamera(initialPosition, finalRotation));
        }
        else
        {
            // 고정 시점 모드에서 자유 시점 모드로 전환
            currentTarget = target; // 나무를 타겟으로 설정
            isAnimalTarget = false;
            StartCoroutine(ZoomCamera(zoomInPosition, zoomInRotation));
        }

        // 모드 전환
        isFreeCamera = !isFreeCamera;

        // 1초 후 버튼 다시 활성화
        StartCoroutine(EnableButtonAfterDelay(1.0f));
    }

    private IEnumerator ZoomCamera(Vector3 targetPosition, Quaternion targetRotation)
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

    private IEnumerator EnableButtonAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        toggleButton.interactable = true;
    }
}