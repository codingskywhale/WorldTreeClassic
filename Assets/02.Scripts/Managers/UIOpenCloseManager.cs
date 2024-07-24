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

    public Camera mainCamera; // 메인 카메라
    public float cameraMoveAmount = 0.6f; // 카메라 이동 거리

    private Vector2 panelOriginalPosition1;
    private Vector2 panelOriginalPosition2;
    private Vector2 panelOriginalPosition3;
    private Vector2[] buttonOriginalPositions;
    private Vector3 cameraOriginalPosition;
    private bool isPanelOpen = false; // 패널이 닫혀 있는 상태로 시작

    public bool IsPanelOpen => isPanelOpen;

    private void Start()
    {
        panelOriginalPosition1 = bottomUIPanel1.anchoredPosition;
        panelOriginalPosition2 = bottomUIPanel2.anchoredPosition;
        panelOriginalPosition3 = bottomUIPanel3.anchoredPosition;
        cameraOriginalPosition = mainCamera.transform.position;

        // 각 버튼의 원래 위치를 저장
        buttonOriginalPositions = new Vector2[otherButtons.Length];
        for (int i = 0; i < otherButtons.Length; i++)
        {
            buttonOriginalPositions[i] = otherButtons[i].anchoredPosition;
        }

        // 초기에는 닫기 버튼 비활성화
        closeButton.gameObject.SetActive(false);

        // 버튼 클릭 이벤트 등록
        closeButton.onClick.AddListener(ClosePanel);
        openButton.onClick.AddListener(OpenPanels);

        // 패널을 닫힌 상태로 초기화
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
        Vector2 targetPosition3 = panelOriginalPosition3 - new Vector2(0, bottomUIPanel3.rect.height);
        Vector3 cameraTargetPosition = new Vector3(cameraOriginalPosition.x, cameraOriginalPosition.y + cameraMoveAmount, cameraOriginalPosition.z); // 카메라를 위로 이동

        if (instant)
        {
            bottomUIPanel1.anchoredPosition = targetPosition1;
            bottomUIPanel2.anchoredPosition = targetPosition2;
            bottomUIPanel3.anchoredPosition = targetPosition3;
            mainCamera.transform.position = cameraTargetPosition;
            foreach (var button in otherButtons)
            {
                button.anchoredPosition -= new Vector2(0, bottomUIPanel1.rect.height);
            }
        }
        else
        {
            yield return StartCoroutine(MoveUIPanelsAndButtons(targetPosition1, targetPosition2, targetPosition3, cameraTargetPosition, true));
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

        Vector2 targetPosition1 = panelOriginalPosition1;
        Vector2 targetPosition2 = panelOriginalPosition2;
        Vector2 targetPosition3 = panelOriginalPosition3;
        Vector3 cameraTargetPosition = new Vector3(cameraOriginalPosition.x, cameraOriginalPosition.y - cameraMoveAmount, cameraOriginalPosition.z); // 카메라를 아래로 이동

        yield return StartCoroutine(MoveUIPanelsAndButtons(targetPosition1, targetPosition2, targetPosition3, cameraTargetPosition, false));

        // 닫기 버튼 활성화
        closeButton.gameObject.SetActive(true);

        isPanelOpen = true;
    }

    private IEnumerator MoveUIPanelsAndButtons(Vector2 targetPosition1, Vector2 targetPosition2, Vector2 targetPosition3, Vector3 cameraTargetPosition, bool closing)
    {
        float elapsedTime = 0f;
        Vector2 startingPosition1 = bottomUIPanel1.anchoredPosition;
        Vector2 startingPosition2 = bottomUIPanel2.anchoredPosition;
        Vector2 startingPosition3 = bottomUIPanel3.anchoredPosition;
        Vector3 cameraStartingPosition = mainCamera.transform.position;
        Vector2[] startingPositions = new Vector2[otherButtons.Length];

        for (int i = 0; i < otherButtons.Length; i++)
        {
            startingPositions[i] = otherButtons[i].anchoredPosition;
        }

        while (elapsedTime < animationDuration)
        {
            bottomUIPanel1.anchoredPosition = Vector2.Lerp(startingPosition1, targetPosition1, elapsedTime / animationDuration);
            bottomUIPanel2.anchoredPosition = Vector2.Lerp(startingPosition2, targetPosition2, elapsedTime / animationDuration);
            bottomUIPanel3.anchoredPosition = Vector2.Lerp(startingPosition3, targetPosition3, elapsedTime / animationDuration);
            mainCamera.transform.position = new Vector3(
                cameraStartingPosition.x,
                Mathf.Lerp(cameraStartingPosition.y, cameraTargetPosition.y, elapsedTime / animationDuration),
                cameraOriginalPosition.z); // Z 값을 원래 값으로 고정

            for (int i = 0; i < otherButtons.Length; i++)
            {
                otherButtons[i].anchoredPosition = Vector2.Lerp(startingPositions[i], closing ? startingPositions[i] - new Vector2(0, bottomUIPanel1.rect.height) : buttonOriginalPositions[i], elapsedTime / animationDuration);
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        bottomUIPanel1.anchoredPosition = targetPosition1;
        bottomUIPanel2.anchoredPosition = targetPosition2;
        bottomUIPanel3.anchoredPosition = targetPosition3;
        mainCamera.transform.position = new Vector3(cameraTargetPosition.x, cameraTargetPosition.y, cameraOriginalPosition.z); // Z 값을 원래 값으로 고정

        for (int i = 0; i < otherButtons.Length; i++)
        {
            otherButtons[i].anchoredPosition = closing ? startingPositions[i] - new Vector2(0, bottomUIPanel1.rect.height) : buttonOriginalPositions[i];
        }
    }
}
