using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GainIndicatorTextObject : MonoBehaviour
{
    private readonly float visibleTime = 0.75f;
    public Transform originTr;
    TextMeshPro rewardText;

    private void OnEnable()
    {
        rewardText = GetComponent<TextMeshPro>();
        rewardText.color = new Color (0,0,0,255);
        this.transform.position = originTr.position;
        StartCoroutine(gainEffect());
    }

    // 1초 정도 보임.
    IEnumerator gainEffect()
    {
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
    }
}
