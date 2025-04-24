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
    private void Awake()
    {
        

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

        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
            .RequestServerAuthCode(false)
            .Build();
        //.RequestEmail()
        //커스텀 된 정보로 GPGS 초기화
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = true;
        //GPGS 시작.
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
        Social.localUser.Authenticate((bool success) => {

            if (success)
            {
                var serverAuthCode = PlayGamesPlatform.Instance.GetServerAuthCode();

                //디버깅용 로그
                if (string.IsNullOrEmpty(serverAuthCode))
                {
                    Debug.LogError(" ServerAuthCode가 null 또는 비어있습니다!");
                    return;
                }
                else
                {
                    Debug.Log($"ServerAuthCode 받음: {serverAuthCode}");
                }

                PlayFabClientAPI.LoginWithGoogleAccount(new LoginWithGoogleAccountRequest()
                {
                    TitleId = PlayFabSettings.TitleId,
                    ServerAuthCode = serverAuthCode,
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
                Debug.LogError("Google 로그인 실패!");
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
