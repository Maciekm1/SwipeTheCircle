using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using System;

public class CloudSaveManager : MonoBehaviour
{
    GameManager gameManager;

    private void Start() {
        gameManager = FindObjectOfType<GameManager>();
    }
    public async void AnonLoginButtonClicked() {
        await UnityServices.InitializeAsync();
        await SignInAnon();
        gameManager.LoadHighScore();
        
    }

    private async Task SignInAnon(){
        AuthenticationService.Instance.SignedIn += () => {
            string playerId = AuthenticationService.Instance.PlayerId;
            Debug.Log("Signed in as: " + playerId);

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
