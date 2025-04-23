using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;
using PlayFab.ClientModels;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using PlayFab;

public class LoginManager : MonoBehaviour
{
    public Button googleLoginButton;
    public Button guestLoginButton;
    public Button resetLoginButton; // 로그인 초기화 버튼 추가
    public TMP_Text loadingText;
    public GameObject loginPanel;
    public static string User_ID = null;
    private string webClientId = "1094607343649-se0b7ljkebsobsbf03gv0d6tn48kpd91.apps.googleusercontent.com";
    private void Awake()
    {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
            .AddOauthScope("profile")
            .RequestServerAuthCode(false)
            .Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();

        PlayFabManager.Instance.OnLoginSuccessEvent += OnLoginSuccess;
    }
    private void Start()
    { 
        googleLoginButton.onClick.AddListener(OnGoogleLoginButtonClicked);
        guestLoginButton.onClick.AddListener(OnGuestLoginButtonClicked);        

        
                
        // Google Play Games Services 초기화
        //PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
        //    .RequestServerAuthCode(false) // 요청하지 않음
        //    .RequestIdToken()
        //    .Build();



        //PlayGamesPlatform.InitializeInstance(config);
        //PlayGamesPlatform.Activate();

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
        Social.localUser.Authenticate((bool success) => {

            if (success)
            {
                var serverAuthCode = PlayGamesPlatform.Instance.GetServerAuthCode();
                Debug.Log("Server Auth Code: " + serverAuthCode);

                PlayFabClientAPI.LoginWithGoogleAccount(new LoginWithGoogleAccountRequest()
                {
                    TitleId = PlayFabSettings.TitleId,
                    ServerAuthCode = serverAuthCode,
                    CreateAccount = true
                }, (result) =>
                {
                    User_ID = result.PlayFabId;
                    //SceneManager.LoadScene("LobbyScene");

                }, (error) =>
                {
                    Debug.Log(error);
                    return;
                }
                );
            }
            else
            {
                Debug.Log("Login Failed!");
            }

        });

        //Social.localUser.Authenticate((bool success) =>
        //{
        //    if (success)
        //    {
        //        Debug.Log("Google 로그인 성공");
        //        //string serverAuthCode = PlayGamesPlatform.Instance.GetServerAuthCode();
        //        var serverAuthCode = PlayGamesPlatform.Instance.GetServerAuthCode();
        //        if (!string.IsNullOrEmpty(serverAuthCode))
        //        {
        //            Debug.Log("Auth Code 받음: " + serverAuthCode);
        //            PlayFabManager.Instance.LoginWithGoogle(serverAuthCode);
        //        }
        //        else
        //        {
        //            Debug.LogError("ServerAuthCode 받아오기 실패");
        //        }
        //    }
        //    else
        //    {
        //        Debug.LogError("Google 로그인 실패");
        //        Debug.Log("LocalUser.authenticated: " + Social.localUser.authenticated);
        //        Debug.Log("LocalUser.userName: " + Social.localUser.userName);
        //    }
        //});
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
        var loadGameDataCoroutine = GameManager.Instance.saveDataManager.LoadGameDataCoroutine(GameManager.Instance.skills, GameManager.Instance.artifacts, GameManager.Instance.worldTree);
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
