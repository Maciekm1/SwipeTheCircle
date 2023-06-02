using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Newtonsoft.Json;
using System;
using TMPro;

public class CloudSaveManager : MonoBehaviour
{
    // Fields
    private UIManager uiManager;
    private ShopManager shopManager;
    private GameManager gameManager;
    public static event Action OnUserLogin;
    [SerializeField] private LeaderBoardUIManager leaderBoardUIManager;

    [SerializeField] TMP_InputField input_lb;

    private async void Start()
    {
        // Get the necessary components
        gameManager = FindObjectOfType<GameManager>();
        uiManager = FindObjectOfType<UIManager>();
        shopManager = FindObjectOfType<ShopManager>();

        //input_lb.characterLimit = 10;

        // Initialize Unity Services (cloud save and authentication)
        await UnityServices.InitializeAsync();
        AnonLoginButtonClicked();
    }

    public async void AnonLoginButtonClicked()
    {
        // Sign in anonymously using Unity Services Authentication
        await SignInAnon();

        //load the main menu
        OnUserLogin?.Invoke();
        LoadMainMenu();
    }

    private void LoadMainMenu()
    {
        // Load the main menu scene
        FindObjectOfType<SceneManager>().LoadMainMenu();

        // Update the game state and load high scores
        gameManager.ChangeGameState(1);
        gameManager.LoadHighScore();
    }

    public void ValidateName()
    {
        if(gameManager.Stars < 100 || input_lb.text.Contains(" ") || input_lb.text == ""){
            return;
        }
        gameManager.Stars -= 100;
        uiManager.updateStarsUI();
        // Add the name to the cloud save service
        AddNameToCloud(input_lb.text);
        shopManager.updateShopName(input_lb.text);
        input_lb.text = "";
        
        Debug.Log("Player name updated");
    }

    private async Task SignInAnon()
    {
        // Subscribe to events that occur after signing in
        AuthenticationService.Instance.SignedIn += async () =>
        {
            await AuthenticationService.Instance.GetPlayerNameAsync();
            Debug.Log("Signed in as" + AuthenticationService.Instance.PlayerName.Split("#")[0]);
            shopManager.updateShopName(AuthenticationService.Instance.PlayerName.Split("#")[0]);
            PlayerPrefs.SetString("playerName", AuthenticationService.Instance.PlayerName.Split("#")[0]);
            
        };
        AuthenticationService.Instance.SignInFailed += s =>
        {
            // Take some action here if sign-in fails
            Debug.Log(s);
        };

        // Sign in anonymously
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void SaveCloudData(object _data, string saveKey)
    {
        // Convert the data to a dictionary with a single key/value pair
        var data = new Dictionary<string, object>{ { saveKey, _data } };

        // Save the data to the cloud using the cloud save service
        await CloudSaveService.Instance.Data.ForceSaveAsync(data);
    }

    private async void AddNameToCloud(string s)
    {
        // Currently has a bug of not disposing a native array
        try
        {
        await AuthenticationService.Instance.UpdatePlayerNameAsync(s);
        await AuthenticationService.Instance.GetPlayerNameAsync();
        PlayerPrefs.SetString("playerName", AuthenticationService.Instance.PlayerName.Split("#")[0]);
        gameManager.saveUnlockables();
        shopManager.updateShopName();
        }
        catch (AuthenticationException e)
        {
            input_lb.text = e.ToString();
        }
        catch (RequestFailedException e)
        {
            input_lb.text = e.ToString();
        }
    }
}