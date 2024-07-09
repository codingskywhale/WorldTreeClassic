using UnityEngine;
using UnityEngine.EventSystems;

public class ClickableObject : MonoBehaviour, IPointerClickHandler
{
    private CameraController cameraController;

    private void Start()
    {
        cameraController = Camera.main.GetComponent<CameraController>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("OnPointerClick called");

        if (!cameraController.isFreeCamera) // 자유시점 모드가 아닌 경우 이벤트를 무시
        {
            Debug.Log("Click ignored: Not in free camera mode");
            return;
        }

        if (CameraTargetHandler.Instance.currentTarget == transform) // 이미 타겟이 설정된 경우 이벤트를 무시
        {
            Debug.Log("Click ignored: Target is already set to this object");
            return;
        }

        Debug.Log("Target set to " + transform.name);
        CameraTargetHandler.Instance.SetTarget(transform);
    }
}