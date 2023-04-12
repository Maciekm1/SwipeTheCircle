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

    [SerializeField] InputObject input_lb;

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
        // Add the name to the cloud save service
        AddNameToCloud(input_lb.GetNameInput());

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
            Debug.Log("Signed in as" + AuthenticationService.Instance.GetPlayerNameAsync().ToString());
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
        await AuthenticationService.Instance.UpdatePlayerNameAsync(s);
        FindObjectOfType<Disappear>().ChangeText(AuthenticationService.Instance.PlayerInfo.ToString(), 2);
        // SaveCloudData(PlayerName, "player-name")
    }
}