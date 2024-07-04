using System.Collections;
using UnityEngine;

public class CameraTargetHandler : MonoBehaviour
{
    public Transform currentTarget; // 현재 타겟
    public bool isAnimalTarget = false;
    private Vector3 animalOffset = new Vector3(0, 5f, -15); // 동물 타겟에 대한 카메라 오프셋
    private CameraTransition cameraTransition;

    private void Start()
    {
        cameraTransition = GetComponent<CameraTransition>();
    }

    public void HandleAnimalClick()
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

    public void SetTarget(Transform newTarget)
    {
        currentTarget = newTarget;
        isAnimalTarget = true;
        StartCoroutine(ZoomToTarget(newTarget));
    }

    private IEnumerator ZoomToTarget(Transform newTarget)
    {
        cameraTransition.isZooming = true;
        float startTime = Time.time;
        Vector3 startPosition = Camera.main.transform.position;
        Quaternion startRotation = Camera.main.transform.rotation;

        Vector3 targetPosition = newTarget.position + animalOffset; // 동물의 위치를 기준으로 카메라 위치 조정
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

    public void FollowAnimal()
    {
        if (currentTarget != null)
        {
            Vector3 targetPosition = currentTarget.position + animalOffset;
            Camera.main.transform.position = targetPosition;
            Camera.main.transform.LookAt(currentTarget);
        }
    }
}

