using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIOpenCloseManager : MonoBehaviour
{
    public RectTransform bottomUIPanel1; // 강화창
    public RectTransform bottomUIPanel2; // 버튼부분
    public RectTransform bottomUIPanel3; // 자원창
    public RectTransform[] otherButtons; // 화면에 남아있는 버튼들
    public Button closeButton; // 닫기 버튼
    public Button openButton; // 열기 버튼
    public float animationDuration = 0.5f; // 애니메이션 시간

    private Vector2 panelOriginalPosition1;
    private Vector2 panelOriginalPosition2;
    private Vector2 panelOriginalPosition3;
    private Vector2[] buttonOriginalPositions;
    private bool isPanelOpen = true; // 패널이 열려 있는지 여부

    public bool IsPanelOpen => isPanelOpen;

    private void Start()
    {
        panelOriginalPosition1 = bottomUIPanel1.anchoredPosition;
        panelOriginalPosition2 = bottomUIPanel2.anchoredPosition;
        panelOriginalPosition3 = bottomUIPanel3.anchoredPosition;

        // 각 버튼의 원래 위치를 저장
        buttonOriginalPositions = new Vector2[otherButtons.Length];
        for (int i = 0; i < otherButtons.Length; i++)
        {
            buttonOriginalPositions[i] = otherButtons[i].anchoredPosition;
        }

        // 초기에는 열기 버튼 비활성화
        openButton.gameObject.SetActive(false);

        // 버튼 클릭 이벤트 등록
        closeButton.onClick.AddListener(ClosePanel);
        openButton.onClick.AddListener(OpenPanels);

        StartCoroutine(ClosePanelCoroutine(true));
    }

    private void ClosePanel()
    {
        StartCoroutine(ClosePanelCoroutine());
    }

    private void OpenPanels()
    {
        StartCoroutine(OpenPanelsCoroutine());
    }

    private IEnumerator ClosePanelCoroutine(bool instant = false)
    {
        Vector2 targetPosition1 = panelOriginalPosition1 - new Vector2(0, bottomUIPanel1.rect.height);
        Vector2 targetPosition2 = panelOriginalPosition2 - new Vector2(0, bottomUIPanel2.rect.height);
        Vector2 targetPosition3 = panelOriginalPosition3 - new Vector2(0, bottomUIPanel2.rect.height);

        if (instant)
        {
            bottomUIPanel1.anchoredPosition = targetPosition1;
            bottomUIPanel2.anchoredPosition = targetPosition2;
            bottomUIPanel3.anchoredPosition = targetPosition3;
            foreach (var button in otherButtons)
            {
                button.anchoredPosition -= new Vector2(0, bottomUIPanel1.rect.height);
            }
        }
        else
        {
            StartCoroutine(MoveUIPanel(bottomUIPanel1, targetPosition1));
            StartCoroutine(MoveUIPanel(bottomUIPanel2, targetPosition2));
            StartCoroutine(MoveUIPanel(bottomUIPanel3, targetPosition3));

            foreach (var button in otherButtons)
            {
                Vector2 targetPosition = button.anchoredPosition - new Vector2(0, bottomUIPanel1.rect.height);
                StartCoroutine(MoveUIPanel(button, targetPosition));
            }

            yield return new WaitForSeconds(animationDuration);
        }

        // 열기 버튼 활성화
        openButton.gameObject.SetActive(true);
        // 닫기 버튼 비활성화
        closeButton.gameObject.SetActive(false);

        isPanelOpen = false;
    }

    private IEnumerator OpenPanelsCoroutine()
    {
        openButton.gameObject.SetActive(false); 

        StartCoroutine(MoveUIPanel(bottomUIPanel1, panelOriginalPosition1));
        StartCoroutine(MoveUIPanel(bottomUIPanel2, panelOriginalPosition2));
        StartCoroutine(MoveUIPanel(bottomUIPanel3, panelOriginalPosition3));
                
        for (int i = 0; i < otherButtons.Length; i++)
        {
            StartCoroutine(MoveUIPanel(otherButtons[i], buttonOriginalPositions[i]));
        }

        yield return new WaitForSeconds(animationDuration); 

        // 닫기 버튼 활성화
        closeButton.gameObject.SetActive(true);

        isPanelOpen = true;
    }

    private IEnumerator MoveUIPanel(RectTransform panel, Vector2 targetPosition)
    {
        float elapsedTime = 0f;
        Vector2 startingPosition = panel.anchoredPosition;

        while (elapsedTime < animationDuration)
        {
            panel.anchoredPosition = Vector2.Lerp(startingPosition, targetPosition, elapsedTime / animationDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        panel.anchoredPosition = targetPosition;
    }
}
