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
    GameManager gameManager;

    public string PlayerName {get; set;}

    private void Start() {
        gameManager = FindObjectOfType<GameManager>();
    }
    public async void AnonLoginButtonClicked() {
        await UnityServices.InitializeAsync();
        await SignInAnon();

        gameManager.LoadHighScore();

        // Load LB scores
        gameManager.GetScoresFromLB();
        
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
