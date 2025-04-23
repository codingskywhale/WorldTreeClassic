using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using System;
using UnityEngine;
public class PlayFabManager : MonoBehaviour
{
    public static PlayFabManager Instance { get; private set; }
    public event Action<LoginResult> OnLoginSuccessEvent;

    private string playFabTitleId = "14ACD0";
    private const string GameDataKey = "gameData";

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
        //PlayerPrefs.DeleteAll();
        //PlayerPrefs.Save();

        ResetAllGameData();
    }

    public void LoginWithGuest()
    {
        Debug.Log("Trying to login with guest ID...");
        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true
        };
        Debug.Log("Custom ID: " + request.CustomId);
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }


    public void LoginWithGoogle(string serverAuthCode)
    {
        var request = new LoginWithGoogleAccountRequest
        {
            ServerAuthCode = serverAuthCode,
            CreateAccount = true
        };

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
    public void DeleteGameData()
    {
        if (PlayFabClientAPI.IsClientLoggedIn())
        {
            var request = new UpdateUserDataRequest
            {
                KeysToRemove = new List<string> { "gameData" }
            };

            PlayFabClientAPI.UpdateUserData(request,
                result => Debug.Log("Game data deleted from PlayFab."),
                error => Debug.LogError("Failed to delete game data: " + error.GenerateErrorReport()));
        }
        else
        {
            Debug.LogWarning("Cannot delete data: Not logged in to PlayFab.");
        }
    }

    public void ResetAllGameData()
    {
        PlayerPrefs.DeleteAll();
        DeleteGameData();
    }


}