using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GainIndicatorTextObject : MonoBehaviour
{
    private readonly float visibleTime = 0.75f;
    public Transform originTr;
    TextMeshPro rewardText;
    Camera cam;

    private void Awake()
    {
        cam = Camera.main;
        rewardText = GetComponent<TextMeshPro>();
    }
    private void OnEnable()
    {
        rewardText.text = BigIntegerUtils.FormatBigInteger(DataManager.Instance.touchData.touchIncreaseAmount);
        rewardText.color = new Color (0,0,0,255);
        StartCoroutine(gainEffect());
    }

    private void Update()
    {
        LookCamera();
    }

    private void LookCamera()
    {
        transform.LookAt(cam.transform);
        transform.rotation = Quaternion.Euler(-transform.rotation.eulerAngles.x,
                                              transform.rotation.eulerAngles.y + 180, transform.rotation.eulerAngles.z);
    }

    // 1초 정도 보임.
    IEnumerator gainEffect()
    {
        // y값의 이동이 없는 오리지널 포지션
        Vector3 originPos = transform.position;
        Color32 origin = rewardText.color;
        Color32 c = new Color32(0, 0, 0, 255 / 75);
        float elapsedTime = 0f;

        while (true)
        {
            this.transform.position += (Vector3)Vector2.up / 75;
            origin.a -= c.a;
            rewardText.color = origin; // 변경된 Color 값을 다시 할당

            yield return new WaitForSeconds(0.01f);
            elapsedTime += 0.01f; // Time.deltaTime을 사용할 필요 없이 WaitForSeconds의 값으로 시간 계산

            if (elapsedTime > visibleTime)
            {
                gameObject.SetActive(false);
                elapsedTime = 0f;
                break;
            }
        }
        // 최종 x,y,z 가 변경된 상태이지만 y값의 변화만 초기화 시켜주면 되는 부분이다.
        Vector3 movedVector = new Vector3(transform.position.x, originPos.y, transform.position.z);
        transform.position = movedVector;
    }
}
