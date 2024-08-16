using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Intro : MonoBehaviour
{
    public TextMeshProUGUI introText; // 인트로 글귀를 표시할 텍스트
    public Image backgroundPanel; // 인트로 배경 패널    
    public float introDisplayDuration = 2f; // 인트로 글귀를 표시할 시간
    public GameObject introCanvas;
    public float fadeDuration = 2.5f;

    private List<string> messages = new List<string>
    {
        "오늘도 힘내세요! \n당신은 할 수 있습니다.",
        "작은 노력들이 모여 \n큰 성과를 이룹니다.",
        "행복은 멀리 있는 것이 아니라 \n바로 당신 곁에 있습니다.",
        "어려움은 극복하는 자의 것입니다. \n오늘도 최선을 다하세요.",
        "매일 조금씩 더 나아가는 \n당신을 응원합니다.",
        "당신의 꿈을 향해 한 걸음씩 나아가세요. \n멋진 미래가 기다리고 있습니다.",
        "지금의 노력은 \n내일의 당신을 빛나게 합니다.",
        "포기하지 말고 끝까지 도전하세요. \n성공은 곧 다가옵니다.",
        "오늘의 작은 성취가 \n내일의 큰 행복을 만듭니다.",
        "긍정적인 생각은 \n긍정적인 결과를 가져옵니다. \n힘내세요!"
    };

    public IEnumerator PlayIntro()
    {
        introCanvas.SetActive(true);

        // 메시지를 랜덤으로 선택
        string randomMessage = messages[Random.Range(0, messages.Count)];
        introText.text = randomMessage;

        // 4초 동안 글귀와 배경 패널을 그대로 유지합니다.
        yield return new WaitForSeconds(introDisplayDuration);

        // 배경 패널을 서서히 사라지게 합니다.
        yield return StartCoroutine(FadeOutImage(backgroundPanel, fadeDuration -1f));

        // 카메라 전환 코루틴 실행
        GameManager.Instance.cameraTransition.StartCoroutine(GameManager.Instance.cameraTransition.OpeningCamera());

        yield return new WaitForSeconds(1.5f);

        // 배경 패널이 사라진 후 텍스트를 위로 올리면서 서서히 사라지게 합니다.
        yield return StartCoroutine(MoveAndFadeText(introText, fadeDuration));
        
        // 모든 애니메이션이 끝난 후 캔버스를 비활성화합니다.
        introCanvas.SetActive(false);
    }


    private IEnumerator FadeOutImage(Image image, float duration)
    {
        Color startColor = image.color;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, time / duration);  // 투명해지는 속도 조절
            image.color = new Color(startColor.r, startColor.g, startColor.b, Mathf.Lerp(startColor.a, 0, t));
            yield return null;
        }

        image.color = new Color(startColor.r, startColor.g, startColor.b, 0);
    }

    private IEnumerator MoveAndFadeText(TextMeshProUGUI text, float duration)
    {
        Vector3 startPosition = text.rectTransform.localPosition;
        Vector3 endPosition = startPosition + Vector3.up * 50f; // 50 유닛 위로 이동
        Color startColor = text.color;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            text.rectTransform.localPosition = Vector3.Lerp(startPosition, endPosition, time / duration);
            text.color = new Color(startColor.r, startColor.g, startColor.b, Mathf.Lerp(startColor.a, 0, time / duration));
            yield return null;
        }

        text.rectTransform.localPosition = endPosition;
        text.color = new Color(startColor.r, startColor.g, startColor.b, 0);
    }
}