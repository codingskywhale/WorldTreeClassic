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
using Unity.VisualScripting.FullSerializer;

public class LoginManager : MonoBehaviour
{
    public Button googleLoginButton;
    public Button guestLoginButton;
    public Button resetLoginButton; // 로그인 초기화 버튼 추가
    public TMP_Text loadingText;
    public GameObject loginPanel;
    public static string User_ID = null;
    private void Awake()
    {
        //PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
        //    .RequestServerAuthCode(false)
        //    .RequestIdToken()
        //    .Build();
        ////커스텀 된 정보로 GPGS 초기화
        ////PlayGamesPlatform.InitializeInstance(config);
        //PlayGamesPlatform.DebugLogEnabled = true;
        ////GPGS 시작.
        //PlayGamesPlatform.Activate();

        PlayFabManager.Instance.OnLoginSuccessEvent += OnLoginSuccess;

    }
    private void Start()
    {
        googleLoginButton.onClick.AddListener(OnGoogleLoginButtonClicked);
        guestLoginButton.onClick.AddListener(OnGuestLoginButtonClicked);

        //PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
        //    .RequestIdToken()
        //    .Build();
        //.RequestEmail()


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
    public void OnGoogleLoginButtonClicked()
    {
        PlayGamesPlatform.Instance.Authenticate(SignInInteractivity.CanPromptOnce, ProcessAuthentication);
    }
    internal void ProcessAuthentication(SignInStatus status)
    {
        if (status == SignInStatus.Success)
        {
            string idToken = PlayGamesPlatform.Instance.GetIdToken();
            string displayName = PlayGamesPlatform.Instance.GetUserDisplayName();
            string userId = PlayGamesPlatform.Instance.GetUserId();
            Debug.Log("Google 로그인 성공, ID Token: " + idToken);
            Debug.Log("Google 로그인 성공, user ID" + userId);

            // PlayFab 로그인 연결
            PlayFabClientAPI.LoginWithGooglePlayGamesServices(new LoginWithGooglePlayGamesServicesRequest()
            {
                TitleId = PlayFabSettings.TitleId,
                ServerAuthCode = idToken,
                CreateAccount = true
            },
            (result) =>
            {
                Debug.Log("PlayFab 로그인 성공!");
                User_ID = result.PlayFabId;
            },
            (error) =>
            {
                Debug.LogError($"PlayFab 로그인 실패: {error.GenerateErrorReport()}");
            });
        }
        else
        {
            Debug.LogError($"Google 로그인 실패! 상태: {status}");
        }
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

