using System.Collections;
using TMPro;
using UnityEngine;

public class GainIndicatorTextObject : MonoBehaviour
{
    private readonly float visibleTime = 0.75f;  // 텍스트가 보여지는 시간
    private TextMeshPro rewardText;
    private Camera cam;
    private Vector3 originPos;

    private void Awake()
    {
        cam = Camera.main;
    }

    public void ShowGainIndicator(Vector3 buttonPosition)
    {
        // 텍스트 오브젝트의 위치를 버튼 위치로 설정
        transform.position = buttonPosition;
        originPos = transform.position;

        // 텍스트 초기화 및 효과 실행
        rewardText = GetComponent<TextMeshPro>();
        rewardText.text = BigIntegerUtils.FormatBigInteger(DataManager.Instance.touchData.touchIncreaseAmount);
        rewardText.color = new Color(0, 0, 0, 1);

        gameObject.SetActive(true);  // 텍스트 오브젝트 활성화
        StartCoroutine(GainEffect());  // 효과 코루틴 시작
    }

    private IEnumerator GainEffect()
    {
        float elapsedTime = 0f;
        Color originalColor = rewardText.color;
        Vector3 targetPosition = originPos + Vector3.up * 1f;  // 위로 이동할 목표 위치 설정

        while (elapsedTime < visibleTime)
        {
            // 시간에 따른 위치 이동과 색상 변화
            transform.position = Vector3.Lerp(originPos, targetPosition, elapsedTime / visibleTime);
            rewardText.color = Color.Lerp(originalColor, new Color(0, 0, 0, 0), elapsedTime / visibleTime);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // 효과가 끝나면 텍스트 오브젝트를 비활성화
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (gameObject.activeSelf)
        {
            LookCamera();  // 카메라를 향해 텍스트가 회전
        }
    }

    private void LookCamera()
    {
        transform.LookAt(cam.transform);
        transform.rotation = Quaternion.Euler(-transform.rotation.eulerAngles.x,
                                              transform.rotation.eulerAngles.y + 180,
                                              transform.rotation.eulerAngles.z);
    }
}