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
    private bool isPanelOpen = false; // 패널이 닫혀 있는 상태로 시작

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

    public void OpenPanels()
    {
        StartCoroutine(OpenPanelsCoroutine());
    }

    private IEnumerator ClosePanelCoroutine(bool instant = false)
    {
        // 패널이 닫힐 때 카메라의 현재 위치를 기준으로 Y축을 이동
        Vector3 cameraTargetPosition = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y + cameraMoveAmount, mainCamera.transform.position.z);

        Vector2 targetPosition1 = panelOriginalPosition1 - new Vector2(0, bottomUIPanel1.rect.height);
        Vector2 targetPosition2 = panelOriginalPosition2 - new Vector2(0, bottomUIPanel2.rect.height);
        Vector2 targetPosition3 = panelOriginalPosition3 - new Vector2(0, bottomUIPanel3.rect.height);

        if (instant)
        {
            // 즉시 이동하는 경우
            mainCamera.transform.position = cameraTargetPosition;
            bottomUIPanel1.anchoredPosition = targetPosition1;
            bottomUIPanel2.anchoredPosition = targetPosition2;
            bottomUIPanel3.anchoredPosition = targetPosition3;
            AdjustOtherButtons(-bottomUIPanel1.rect.height);
        }
        else
        {
            // 애니메이션을 통한 이동
            yield return StartCoroutine(MoveUIPanelsAndCamera(cameraTargetPosition, targetPosition1, targetPosition2, targetPosition3));
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

        // 패널이 열릴 때 카메라의 현재 위치를 기준으로 Y축을 이동
        Vector3 cameraTargetPosition = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y - cameraMoveAmount, mainCamera.transform.position.z);

        Vector2 targetPosition1 = panelOriginalPosition1;
        Vector2 targetPosition2 = panelOriginalPosition2;
        Vector2 targetPosition3 = panelOriginalPosition3;

        yield return StartCoroutine(MoveUIPanelsAndCamera(cameraTargetPosition, targetPosition1, targetPosition2, targetPosition3));

        // 닫기 버튼 활성화
        closeButton.gameObject.SetActive(true);

        isPanelOpen = true;
    }

    private IEnumerator MoveUIPanelsAndCamera(Vector3 cameraTargetPosition, Vector2 targetPosition1, Vector2 targetPosition2, Vector2 targetPosition3)
    {
        float elapsedTime = 0f;
        Vector3 cameraStartingPosition = mainCamera.transform.position;

        Vector2 startingPosition1 = bottomUIPanel1.anchoredPosition;
        Vector2 startingPosition2 = bottomUIPanel2.anchoredPosition;
        Vector2 startingPosition3 = bottomUIPanel3.anchoredPosition;

        Vector2[] buttonStartingPositions = new Vector2[otherButtons.Length];
        for (int i = 0; i < otherButtons.Length; i++)
        {
            buttonStartingPositions[i] = otherButtons[i].anchoredPosition;
        }

        while (elapsedTime < animationDuration)
        {
            mainCamera.transform.position = Vector3.Lerp(cameraStartingPosition, cameraTargetPosition, elapsedTime / animationDuration);
            bottomUIPanel1.anchoredPosition = Vector2.Lerp(startingPosition1, targetPosition1, elapsedTime / animationDuration);
            bottomUIPanel2.anchoredPosition = Vector2.Lerp(startingPosition2, targetPosition2, elapsedTime / animationDuration);
            bottomUIPanel3.anchoredPosition = Vector2.Lerp(startingPosition3, targetPosition3, elapsedTime / animationDuration);

            for (int i = 0; i < otherButtons.Length; i++)
            {
                otherButtons[i].anchoredPosition = Vector2.Lerp(buttonStartingPositions[i], buttonStartingPositions[i] + new Vector2(0, targetPosition1.y - startingPosition1.y), elapsedTime / animationDuration);
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.position = cameraTargetPosition;
        bottomUIPanel1.anchoredPosition = targetPosition1;
        bottomUIPanel2.anchoredPosition = targetPosition2;
        bottomUIPanel3.anchoredPosition = targetPosition3;        
    }

    private void AdjustOtherButtons(float adjustment)
    {
        for (int i = 0; i < otherButtons.Length; i++)
        {
            otherButtons[i].anchoredPosition += new Vector2(0, adjustment);
        }
    }
}