using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Services.CloudSave;
using System.IO;
using Unity.Services.Leaderboards;
using Newtonsoft.Json;
using Unity.Services.Leaderboards.Models;
using System.Threading.Tasks;

public class GameManager : MonoBehaviour
{
    // The key for the high score value in the cloud save service
    public static readonly string HsSaveKey = "HighScore";

    // References to other managers and UI components
    [SerializeField] CloudSaveManager cloudSaveManager;
    [SerializeField] UIManager uiManager;
    [SerializeField] SettingsManager settingsManager;

    [SerializeField] LevelPlayAds levelPlayAds;

    // A list of UI states and a game object for the main menu
    [SerializeField] List<UIState> UIStateObjects;
    [SerializeField] GameObject MainMenuGO;

    // Unlockable items and their path for persistence
    [field:SerializeField] public Unlockables unlockables { get; set; }
    private string unlockablesPath;

    // Events for points gained, points lost, and game loss
    public static event Action OnPointGain;
    public static event Action OnPointLose;
    public static Action OnGameLose;
    public static event Action OnAdShow;
    private int _addCounter = 0;

    // The current game state and game difficulty
    private GameState _gameState;
    [field:SerializeField] public GameDifficultyEnum GameDifficulty { get; set; }

    // Variables for the score, stars, lives, and high score
    public int Score { get; set; }
    public int Stars{ get; set; }
    public int HighScore {get; set;}
    [field:SerializeField] public int Lives {get; set;}

    // An array of colors to cycle through, and the current and target colors
    private readonly Color32[] ColorArray = new Color32[] { 
        new Color32(79, 196, 207, 255), // Cyan
        new Color32(242, 238, 245, 255), // White
        new Color32(251, 221, 116, 255), // Yellow
        new Color32(153, 79, 243, 255), // Purple
    };
    public Color32 CurrentColor { get; private set; }
    public Color32 TargetColor { get; private set; }

    // Variables for color change and target change speed, and timers
    [SerializeField][Range(0.5f,5)] private float colorChangeSpeed = 2f;
    private float colorChangeSpeedInternal;
    [SerializeField] [Range(2, 10)] private float targetColorChangeSpeed = 5f;
    private float timer;
    public float targetTimer { get; set; }

    [SerializeField] private PopUpText starPopup;

    private Vector3 mainmenuPos;

    // Initializes the game manager
    private void Awake()
    {
        // Store the position of the main menu game object
        mainmenuPos = MainMenuGO.transform.position;
        // Set the path for the unlockables file
        unlockablesPath = $"{Application.persistentDataPath}/unlockables.json";

        ResetCurrentGameStats();
        // Set the game difficulty to the saved value or medium by default, and update the UI
        GameDifficulty = (GameDifficultyEnum)Enum.Parse(typeof(GameDifficultyEnum), PlayerPrefs.GetString("GameDifficulty", "Medium"));
        uiManager.UpdateDifficultyText();

        // Set the target frame rate to 120 and change the game state to login
        Application.targetFrameRate = 120;
        ChangeGameState(GameState.LogIn);

        // Register a callback for when the play button is tapped
        PlayButtonState.OnTapAction += CheckTap;
    }

// Print the path to the persistent data folder and the file name of the unlockables JSON file to the console
private void Start()
{
    //Debug.Log($"{Application.persistentDataPath}/unlockables.json");
    // If the unlockables file exists, load it. Otherwise, create a new unlockables object and save it.
    if(File.Exists(unlockablesPath))
    {
        loadUnlockables();
    }
    else
    {
        unlockables.currentCirclePattern = 0;
        unlockables.circlePatterns = new int[10] {1, 0, 0, 0, 0, 0, 0, 0, 0, 0};
        unlockables.stars = 0;
        saveUnlockables();
    }
}

// Load the unlockables from the JSON file
private void loadUnlockables()
{
    string json = File.ReadAllText(unlockablesPath);
    unlockables = JsonUtility.FromJson<Unlockables>(json);
    Stars = unlockables.stars;
}

// Save the current unlockables to the JSON file
public void saveUnlockables()
{
    string json = JsonUtility.ToJson(unlockables);
    File.WriteAllText(unlockablesPath, json);
}

// Asynchronously load the high score from the cloud
public async void LoadHighScore(){
    // Use CloudSaveService to load the high score data
    var query = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string>{ GameManager.HsSaveKey });
    try{
        // Try to parse the high score value from the loaded data
        HighScore = Int32.Parse(query[GameManager.HsSaveKey]);
    }
    // If the high score data is not found, set the high score to 0
    catch (KeyNotFoundException){
        HighScore = 0;
    }
    // Update the UI with the loaded high score
    uiManager.UpdateHighScoreUI(HighScore.ToString());
}

// Return the target color change speed
public float GetTargetColourChangeSpeed()
{
    return targetColorChangeSpeed;
}

// Return the current game state
public GameState GetGameState()
{
    return _gameState;
}

// Change the state of all UI objects to the specified state
public void ChangeUIState(State state)
{
    foreach (UIState item in UIStateObjects)
    {
        item.changeState(state);
    }
}

// Update the game state every frame
private void Update()
{
    ExecuteStateUpdate();
}

// Update the game state based on the current state
private void ExecuteStateUpdate(){
    // If the game state is InGame, update the timer and target timer
    if(_gameState == GameState.InGame){
            timer -= Time.deltaTime;
            targetTimer -= Time.deltaTime;

            // If the target timer reaches 0, update the target color and reset the timer
            if(targetTimer <= 0)
            {
                TargetColor = ColorArray[UnityEngine.Random.Range(0, ColorArray.Length)];
                targetTimer = targetColorChangeSpeed;
                uiManager.UpdateTargetColor(TargetColor);
            }

            // If the color change timer reaches 0, update the current color and reset the timer
            if(timer <= 0)
            {
                List<Color32> colorList = new(ColorArray);
                colorList.Remove(CurrentColor);
                CurrentColor = colorList[UnityEngine.Random.Range(0, ColorArray.Length - 1)];
                timer = colorChangeSpeedInternal;
                uiManager.UpdateCurrentColor(CurrentColor);
            }
    }
}


// Method that executes a state based on the current game state
private void ExecuteState()
{
    switch (_gameState)
    {
        case GameState.LogIn: // Do nothing if game state is "LogIn"
            break;
        case GameState.MainMenu: // If game state is "MainMenu", execute the following code
            // Change circle pattern in UI manager to normal color with no pattern
            uiManager.ChangeCirclePattern(0);

            // Set the position of the main menu game object
            MainMenuGO.transform.position = mainmenuPos;

            // Change the UI state to "MainMenuIdle"
            ChangeUIState(State.MainMenuIdle);

            // Update game elements in the UI manager to reflect the main menu state
            uiManager.UpdateGameElementsToMainMenu();
            
            break;
        case GameState.InGame: // If game state is "InGame", execute the following code
            // Change circle pattern in UI manager to the current unlockable circle pattern
            uiManager.ChangeCirclePattern(unlockables.currentCirclePattern);

            // Change the UI state to "InGameIdle"
            ChangeUIState(State.InGameIdle);

            // Update game elements in the UI manager to reflect the in-game state
            uiManager.UpdateGameElementsToInGame();
            break;
        case GameState.Lose: // If game state is "Lose", execute the following code
            // Deactivate the play button in the UI manager
            uiManager.DeactivatePlayButton();

            // Change the UI state to "LoseIdle"
            ChangeUIState(State.LoseIdle);

            // Reset the game
            ResetGame();
            break;
        default: // Do nothing if game state is unknown
            break;
    }
}

// Method that resets the game
private async void ResetGame()
{
    // Display the game end UI in the UI manager
    uiManager.GameEndUI();

    // Wait for 2 seconds
    await System.Threading.Tasks.Task.Delay(2000);

    // Change the game state to "MainMenu"
    ChangeGameState(GameState.MainMenu);

    // Reset current game stats
    ResetCurrentGameStats();
}

// Method that changes the game state to the given state
public void ChangeGameState(GameState n)
{
    // If the new game state is different from the current game state, change the game state
    if(_gameState != n)
    {
        _gameState = n;
    }

    // Execute the current state
    ExecuteState();
}

// Method that changes the game state to the given state (as an integer)
public void ChangeGameState(int n){
    // If the new game state (as a GameState) is different from the current game state, change the game state
    if(_gameState != (GameState) n)
    {
        _gameState = (GameState) n;
    }

    // Execute the current state
    ExecuteState();
    }

    public void ResetCurrentGameStats()
    {
        uiManager.ActivatePlayButton();
        colorChangeSpeedInternal = colorChangeSpeed;
        timer = colorChangeSpeed;
        targetTimer = targetColorChangeSpeed;
        CurrentColor = ColorArray[0];
        TargetColor = ColorArray[0];
        uiManager.UpdateCurrentColor(CurrentColor);
        uiManager.UpdateTargetColor(TargetColor);

        Lives = 2;
        uiManager.setUpLife();
        Score = 0;
        if(_gameState == GameState.InGame){
            UpdateScore(0);
        }
    }

    private void CheckTap()
    {
        //Debug.LogError("!!! CURRENT STATE: " + _gameState + " !!!");
        if(_gameState == GameState.InGame){
            if (CurrentColor.ToString() == TargetColor.ToString())
            {
                Score++;
                Stars++;

                // Manage star popup
                starPopup.Appear();
                starPopup.showText();
                unlockables.stars = Stars;
                saveUnlockables();
                OnPointGain?.Invoke();
            }
            else
            {
                OnPointLose?.Invoke();
                if (settingsManager.Vib)
                {
                    Handheld.Vibrate();
                    LeanTween.moveX(MainMenuGO, MainMenuGO.transform.position.x + 10f, 0.2f).setEase(LeanTweenType.easeShake).setLoopOnce();
                }

                Lives--;
                if(Lives < 0)
                {
                    _addCounter++;
                    if(_addCounter == 2){
                        levelPlayAds.loadFullSizeAdd();
                    }
                    if(Score > HighScore)
                    {
                        // Save highscore to the cloud
                        cloudSaveManager.SaveCloudData(Score, HsSaveKey);
                        HighScore = Score;
                        uiManager.UpdateHighScoreUI(HighScore.ToString());

                        // Add new highscore to the leaderboard
                        AddScoreToLB(HighScore);

                    }
                    OnGameLose?.Invoke();
                    ChangeGameState(GameState.Lose);
                    if(_addCounter == 4){
                        levelPlayAds.showFullSizeAdd();
                        OnAdShow?.Invoke();
                        _addCounter = 0;
                    }
                }
            }
            float scoreMult = 1f - (Score * (int) GameDifficulty/ 50f);
            colorChangeSpeedInternal = Mathf.Clamp(colorChangeSpeed * scoreMult, 0.6f - (0.1f * (int)GameDifficulty), colorChangeSpeed);
            UpdateScore(Score);
        }
    }

    public async void AddScoreToLB(int score)
    {
        var playerEntry = await LeaderboardsService.Instance.AddPlayerScoreAsync("ColourSwipeLB", score);
        //Debug.Log(JsonConvert.SerializeObject(playerEntry));
    }
       

    public void UpdateScore(int newScore)
    {
        Score = newScore;
        uiManager.UpdateScoreUI(newScore.ToString());
    }
}


public enum GameState
{
    LogIn,
    MainMenu,
    TransitionToMain,
    InGame,
    Lose
}

public enum GameDifficultyEnum
{
    Easy,
    Medium,
    Hard,
    Impossible
}
