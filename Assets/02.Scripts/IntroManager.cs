using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class IntroManager : MonoBehaviour
{
    public TextMeshProUGUI introText; // 인트로 글귀를 표시할 텍스트
    public Image backgroundPanel; // 인트로 배경 패널    
    public float introDisplayDuration = 4f; // 인트로 글귀를 표시할 시간
    public GameObject introCanvas;

    private List<string> messages = new List<string>
    {
        "오늘도 힘내세요! 당신은 할 수 있습니다.",
        "작은 노력들이 모여 큰 성과를 이룹니다.",
        "행복은 멀리 있는 것이 아니라 바로 당신 곁에 있습니다.",
        "어려움은 극복하는 자의 것입니다. 오늘도 최선을 다하세요.",
        "매일 조금씩 더 나아가는 당신을 응원합니다.",
        "당신의 꿈을 향해 한 걸음씩 나아가세요. 멋진 미래가 기다리고 있습니다.",
        "지금의 노력은 내일의 당신을 빛나게 합니다.",
        "포기하지 말고 끝까지 도전하세요. 성공은 곧 다가옵니다.",
        "오늘의 작은 성취가 내일의 큰 행복을 만듭니다.",
        "긍정적인 생각은 긍정적인 결과를 가져옵니다. 힘내세요!"
    };

    public Material backgroundMaterial; // 그라데이션 Shader가 적용된 Material

    private void Start()
    {
        introCanvas.SetActive(true);
        backgroundPanel.material = backgroundMaterial; // 배경 패널에 Material 적용
    }

    public IEnumerator PlayIntro()
    {
        introCanvas.SetActive(true);

        // 메시지를 랜덤으로 선택
        string randomMessage = messages[Random.Range(0, messages.Count)];
        introText.text = randomMessage;

        // 글귀와 배경 패널을 즉시 나타나게 합니다.
        introText.gameObject.SetActive(true);
        backgroundPanel.gameObject.SetActive(true);

        // 4초 동안 글귀와 배경 패널을 그대로 유지합니다.
        yield return new WaitForSeconds(introDisplayDuration);

        // 인트로 애니메이션 완료 후 배경 패널과 텍스트 비활성화
        introText.gameObject.SetActive(false);
        backgroundPanel.gameObject.SetActive(false);
        GameManager.Instance.cameraTransition.StartCoroutine(GameManager.Instance.cameraTransition.OpeningCamera());

        introCanvas.SetActive(false);

    }
}
