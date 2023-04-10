using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using System;
using Newtonsoft.Json;

public class CloudSaveManager : MonoBehaviour
{
    UIManager uIManager;
    GameManager gameManager;

    public string PlayerName {get; set;}

    private void Start() {
        gameManager = FindObjectOfType<GameManager>();
        uIManager = FindObjectOfType<UIManager>();
    }
    public async void AnonLoginButtonClicked() {
        await UnityServices.InitializeAsync();
        await SignInAnon();

        if(!(PlayerPrefs.GetInt("first-run", 0) == 0))
        {
            LoadMainMenu();
        }
        else
        {
            uIManager.NameInputAppear();
        }
        
    }

    private void LoadMainMenu()
    {
        FindObjectOfType<SceneManager>().LoadMainMenu();
        gameManager.ChangeGameState(1);
        gameManager.LoadHighScore();
        // Load LB scores
        gameManager.GetScoresFromLB();
    }

    public void ValidateName()
    {
        string s = uIManager.GetNameInput();
        addNameToCloud(s);
    }

    private void addNameToCloud(string name)
    {
        AuthenticationService.Instance.UpdatePlayerNameAsync(name);
        PlayerPrefs.SetInt("first-run", 1);
        LoadMainMenu();
    }

    // TODO - implement first time login and name set using this PlayerPrefs key - "unity.player_sessionid", if its not there -> first time login, otherwise login normally.
    private async Task SignInAnon(){
        AuthenticationService.Instance.SignedIn += () => {
            string playerId = AuthenticationService.Instance.PlayerId;
            PlayerName = AuthenticationService.Instance.PlayerName;

            Debug.Log("Signed in as: " + PlayerName);

        };
        AuthenticationService.Instance.SignInFailed += s =>
        {
            // Take some action here...
            Debug.Log(s);
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void SaveCloudData(object _data, string saveKey){
        var data = new Dictionary<string, object>{ { saveKey, _data } };
        await CloudSaveService.Instance.Data.ForceSaveAsync(data);
    }
}
