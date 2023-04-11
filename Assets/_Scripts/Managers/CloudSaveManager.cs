using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Newtonsoft.Json;
using TMPro;

public class CloudSaveManager : MonoBehaviour
{
    // Fields
    private UIManager uiManager;
    private GameManager gameManager;

    // Properties
    public string PlayerName { get; set; }

    private async void Start()
    {
        // Get the necessary components
        gameManager = FindObjectOfType<GameManager>();
        uiManager = FindObjectOfType<UIManager>();

        // Initialize Unity Services (cloud save and authentication)
        await UnityServices.InitializeAsync();
    }

    public async void AnonLoginButtonClicked()
    {
        // Sign in anonymously using Unity Services Authentication
        await SignInAnon();

        // If the app has been run before, load the main menu
        if (PlayerPrefs.GetInt("first-run", 0) != 0)
        {
            LoadMainMenu();
        }
        // Otherwise, prompt the user for their name
        else
        {
            uiManager.NameInputAppear();
        }
    }

    private void LoadMainMenu()
    {
        // Load the main menu scene
        FindObjectOfType<SceneManager>().LoadMainMenu();

        // Update the game state and load high scores
        gameManager.ChangeGameState(1);
        gameManager.LoadHighScore();

        // Load leaderboard scores
        gameManager.GetScoresFromLB();
    }

    public void ValidateName()
    {
        // Get the player's name from the UI
        PlayerName = uiManager.GetNameInput();

        // Add the name to the cloud save service
        AddNameToCloud();

        // Set a flag to indicate that the app has been run before
        PlayerPrefs.SetInt("first-run", 1);

        // Load the main menu
        LoadMainMenu();
    }

    private async Task SignInAnon()
    {
        // Subscribe to events that occur after signing in
        AuthenticationService.Instance.SignedIn += () =>
        {
            AuthenticationService.Instance.UpdatePlayerNameAsync(PlayerName);
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

    private void AddNameToCloud()
    {
        // Create a dictionary with the player's name as the value
        var data = new Dictionary<string, object>{ { "player_name", PlayerName } };

        // Save the data to the cloud using the cloud save service
        CloudSaveService.Instance.Data.ForceSaveAsync(data);
    }
}