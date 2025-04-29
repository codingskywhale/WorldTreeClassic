using UnityEngine;
using TMPro;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System.Collections;

public class LoginManager : MonoBehaviour
{
    public Button googleLoginButton;
    public Button guestLoginButton;
    public TMP_Text loadingText;
    public GameObject loginPanel;

    public static string User_ID = null;

    private void Awake()
    {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
            .RequestIdToken()
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
        loadingText.text = "Google로 로그인 중...";
        PlayGamesPlatform.Instance.Authenticate(SignInInteractivity.CanPromptOnce, ProcessAuthentication);
    }

    private void ProcessAuthentication(SignInStatus status)
    {
        if (status == SignInStatus.Success)
        {
            string serverAuthCode = PlayGamesPlatform.Instance.GetServerAuthCode();
            Debug.Log("Google 로그인 성공. ServerAuthCode: " + serverAuthCode);

            if (string.IsNullOrEmpty(serverAuthCode))
            {
                Debug.LogError("ServerAuthCode가 null이거나 비어있습니다.");
                return;
            }

            var request = new LoginWithGooglePlayGamesServicesRequest
            {
                TitleId = PlayFabSettings.TitleId,
                ServerAuthCode = serverAuthCode,  // idToken 아니라 ServerAuthCode 사용!!
                CreateAccount = true
            };

            PlayFabClientAPI.LoginWithGooglePlayGamesServices(request,
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
            Debug.LogError("Google 로그인 실패: " + status);
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

        // 로그인 성공 기록
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
        var loadGameDataCoroutine = GameManager.Instance.saveDataManager.LoadGameDataCoroutine(
            GameManager.Instance.skills,
            GameManager.Instance.artifacts,
            GameManager.Instance.worldTree
        );
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
