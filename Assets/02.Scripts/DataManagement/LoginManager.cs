using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;
using PlayFab.ClientModels;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System.Collections.Generic;

public class LoginManager : MonoBehaviour
{
    public Button googleLoginButton;
    public Button guestLoginButton;
    public Button resetLoginButton; // 로그인 초기화 버튼 추가
    public TMP_Text loadingText;
    public GameObject loginPanel;

    private string webClientId = "41547311661-himu41jj8sm40obegnj3g60rualr4j57.apps.googleusercontent.com";

    private void Start()
    {
        PlayerPrefs.DeleteKey("GuestLoggedIn");
        PlayerPrefs.DeleteKey("GoogleLoggedIn");

        googleLoginButton.onClick.AddListener(OnGoogleLoginButtonClicked);
        guestLoginButton.onClick.AddListener(OnGuestLoginButtonClicked);        

        PlayFabManager.Instance.OnLoginSuccessEvent += OnLoginSuccess;
                
        // Google Play Games Services 초기화
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
            .RequestServerAuthCode(false) // 요청하지 않음
            .RequestIdToken()
            .Build();

        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();

        // 자동 로그인 시도
        if (PlayerPrefs.HasKey("GuestLoggedIn"))
        {
            PlayFabManager.Instance.AutoLogin();
        }
        else if (PlayerPrefs.HasKey("GoogleLoggedIn"))
        {
            OnGoogleLoginButtonClicked();
        }
    }

    private void OnGoogleLoginButtonClicked()
    {
        loadingText.text = "Google로 로그인 중...";

        Social.localUser.Authenticate((bool success) =>
        {
            if (success)
            {
                string idToken = PlayGamesPlatform.Instance.GetIdToken();
                Debug.Log("Google Play Games Sign-In successful, ID Token: " + idToken);
                PlayFabManager.Instance.LoginWithGoogle(idToken);
            }
            else
            {
                Debug.LogError("Google Play Games Sign-In Failed");

                // 로그인 실패 원인 출력
                PlayGamesPlatform.Instance.GetAnotherServerAuthCode(false, code =>
                {
                    Debug.LogError("Failed to retrieve server auth code, reason: " + code);
                });
            }
        });
    }

    private void OnGuestLoginButtonClicked()
    {
        loadingText.text = "Guest로 로그인 중...";
        PlayFabManager.Instance.LoginWithGuest();
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Login successful, starting data load and intro.");
        loginPanel.SetActive(false);
        StartCoroutine(StartIntroAndLoadData());

        // 로그인 성공 시 PlayerPrefs에 저장
        if (PlayGamesPlatform.Instance.IsAuthenticated())
        {
            PlayerPrefs.SetInt("GoogleLoggedIn", 1);
        }
        else
        {
            PlayerPrefs.SetInt("GuestLoggedIn", 1);
        }
        PlayerPrefs.Save();
    }

    private IEnumerator StartIntroAndLoadData()
    {
        var loadGameDataCoroutine = GameManager.Instance.saveDataManager.LoadGameDataCoroutine(GameManager.Instance.resourceManager, GameManager.Instance.skills, GameManager.Instance.artifacts, GameManager.Instance.worldTree);
        StartCoroutine(loadGameDataCoroutine);

        yield return StartCoroutine(GameManager.Instance.introManager.PlayIntro());

        yield return loadGameDataCoroutine;

        GameManager.Instance.OnIntroAndOpeningCompleted();
    }

    private void OnDestroy()
    {
        PlayFabManager.Instance.OnLoginSuccessEvent -= OnLoginSuccess;
    }
}
