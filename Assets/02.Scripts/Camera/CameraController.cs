using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using static UnityEngine.GraphicsBuffer;

public class CameraController : MonoBehaviour
{
    public Transform target; // 나무의 Transform을 에디터에서 할당
    public Button toggleButton;
    public float rotationSpeed = 10f; // 회전 속도
    public GameObject messagePrefab; // 메시지를 표시할 프리팹
    public Transform messageParent; // 메시지를 표시할 부모 객체

    private CameraTransition cameraTransition;
    private CameraTargetHandler cameraTargetHandler;
    private GameObject currentMessage;
    private WorldTree worldTree;

    public bool isFreeCamera = false;
    private bool isDragging = false;

    private void Start()
    {
        cameraTransition = GetComponent<CameraTransition>();
        cameraTargetHandler = GetComponent<CameraTargetHandler>();
        worldTree = FindObjectOfType<WorldTree>();

        // 초기 위치와 회전 설정
        Camera.main.transform.position = cameraTransition.initialPosition;
        cameraTransition.initialRotation = Quaternion.Euler(-100, -116, 0);
        cameraTransition.finalRotation = Quaternion.Euler(30, -116, 0);
        cameraTransition.zoomInRotation = Quaternion.Euler(25, -116, 0);        
        Camera.main.transform.rotation = cameraTransition.initialRotation;       

        // 애니메이션 시작
        StartCoroutine(cameraTransition.OpeningCamera());

        messageParent.gameObject.SetActive(false);
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

            // WorldTree의 위치 오프셋을 적용하여 새로운 위치 계산
            Vector3 newPosition = cameraTransition.initialPosition + worldTree.GetPositionOffset();
            
            StartCoroutine(cameraTransition.ZoomCamera(newPosition, cameraTransition.finalRotation));
            ShowMessage("카메라가 나무에 고정됩니다.");
        }
        else
        {
            // 고정 시점 모드에서 자유 시점 모드로 전환
            cameraTargetHandler.SetTarget(target);
            cameraTargetHandler.isObjectTarget = false;

            // WorldTree의 위치 오프셋을 적용하여 새로운 위치 계산
            Vector3 newPosition = cameraTransition.zoomInPosition + worldTree.GetPositionOffset();
            

            StartCoroutine(cameraTransition.ZoomCamera(newPosition, cameraTransition.zoomInRotation));
            ShowMessage("카메라 자유 조작이 활성화됩니다.");
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