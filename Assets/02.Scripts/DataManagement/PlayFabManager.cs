using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using System;
using UnityEngine;
public class PlayFabManager : MonoBehaviour
{
    public static PlayFabManager Instance { get; private set; }
    public event Action<LoginResult> OnLoginSuccessEvent;

    private string playFabTitleId = "2759E";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            PlayFabSettings.staticSettings.TitleId = playFabTitleId;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoginWithGuest()
    {
        var request = new LoginWithCustomIDRequest { CustomId = SystemInfo.deviceUniqueIdentifier, CreateAccount = true };
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }

    public void LoginWithGoogle(string idToken)
    {
        var request = new LoginWithGoogleAccountRequest { ServerAuthCode = idToken, CreateAccount = true };
        PlayFabClientAPI.LoginWithGoogleAccount(request, OnLoginSuccess, OnLoginFailure);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("PlayFab login successful");
        OnLoginSuccessEvent?.Invoke(result);
    }

    private void OnLoginFailure(PlayFabError error)
    {
        Debug.LogError("PlayFab login failed: " + error.GenerateErrorReport());
    }    

    public void SaveGameData(GameData gameData)
    {
        string json = JsonUtility.ToJson(gameData);
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                { "gameData", json }
            }
        };
        PlayFabClientAPI.UpdateUserData(request, OnDataSendSuccess, OnDataSendFailure);
    }

    public void LoadGameData(Action<GameData> onLoaded)
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), result =>
        {
            if (result.Data != null && result.Data.ContainsKey("gameData"))
            {
                string json = result.Data["gameData"].Value;
                GameData gameData = JsonUtility.FromJson<GameData>(json);
                onLoaded(gameData);
            }
            else
            {
                onLoaded(null);
            }
        }, error =>
        {
            onLoaded(null);
        });
    }

    private void OnDataSendSuccess(UpdateUserDataResult result)
    {
        Debug.Log("Game data saved to PlayFab successfully");
    }

    private void OnDataSendFailure(PlayFabError error)
    {
        Debug.LogError("Failed to save game data to PlayFab: " + error.GenerateErrorReport());
    }

    public void AutoLogin()
    {
        var request = new LoginWithCustomIDRequest { CustomId = SystemInfo.deviceUniqueIdentifier, CreateAccount = true };
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }

    public void DeleteUserData()
    {
        // 데이터를 삭제하여 계정을 초기화하는 효과를 만듭니다.
        var request = new UpdateUserDataRequest
        {
            KeysToRemove = new List<string> { "gameData" } // 여기에 삭제할 데이터의 키를 추가
        };
        PlayFabClientAPI.UpdateUserData(request, OnDataDeleteSuccess, OnDataDeleteFailure);
    }

    private void OnDataDeleteSuccess(UpdateUserDataResult result)
    {
        Debug.Log("User data deleted successfully.");
    }

    private void OnDataDeleteFailure(PlayFabError error)
    {
        Debug.LogError("Failed to delete user data: " + error.GenerateErrorReport());
    }

    public void ResetLogin()
    {
        PlayerPrefs.DeleteKey("GuestLoggedIn");
        PlayerPrefs.DeleteKey("GoogleLoggedIn");
        PlayFabClientAPI.ForgetAllCredentials();
        Debug.Log("Login reset complete. You can now log in again.");
    }
}