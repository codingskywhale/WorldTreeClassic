using UnityEngine;

public class ZoomEffect : MonoBehaviour
{
    public Camera mainCamera;
    public float zoomDuration = 1f;
    public float zoomAmount = 30f; // 줄일 field of view 값
    private float initialFieldOfView;
    private bool isZooming = false;
    private float zoomTimer = 0f;
    private Vector3 initialPosition;
    private Transform targetTransform;

    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        initialFieldOfView = mainCamera.fieldOfView;
        initialPosition = mainCamera.transform.position;
    }

    void Update()
    {
        if (isZooming)
        {
            zoomTimer += Time.deltaTime;
            if (zoomTimer <= zoomDuration)
            {
                mainCamera.fieldOfView = Mathf.Lerp(initialFieldOfView, zoomAmount, zoomTimer / zoomDuration);
                if (targetTransform != null)
                {
                    mainCamera.transform.position = Vector3.Lerp(initialPosition, targetTransform.position + new Vector3(0, 0, -10), zoomTimer / zoomDuration);
                    mainCamera.transform.LookAt(targetTransform);
                }
            }
            else
            {
                mainCamera.fieldOfView = initialFieldOfView;
                mainCamera.transform.position = initialPosition;
                isZooming = false;
                zoomTimer = 0f;
                if (targetTransform != null)
                {
                    mainCamera.transform.LookAt(targetTransform);
                }
            }
        }
    }

    public void StartZoom(Transform target)
    {
        if (!isZooming)
        {
            isZooming = true;
            zoomTimer = 0f;
            targetTransform = target;
            initialPosition = mainCamera.transform.position;
        }
    }
}
