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
        if (!cameraController.isFreeCamera) // 자유시점 모드가 아닌 경우 이벤트를 무시
        {            
            return;
        }

        if (CameraTargetHandler.Instance.currentTarget == transform) // 이미 타겟이 설정된 경우 이벤트를 무시
        {            
            return;
        }                
        CameraTargetHandler.Instance.SetTarget(transform);
    }
}