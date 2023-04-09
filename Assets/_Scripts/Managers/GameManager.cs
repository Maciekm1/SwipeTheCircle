using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Services.CloudSave;
using System.IO;
using Unity.Services.Leaderboards;
using Newtonsoft.Json;
using Unity.Services.Leaderboards.Models;

public class GameManager : MonoBehaviour
{
    public static readonly string HsSaveKey = "HighScore";
    // References
    [SerializeField] CloudSaveManager cloudSaveManager;
    [SerializeField] UIManager uiManager;
    [SerializeField] SettingsManager settingsManager;

    [SerializeField] List<UIState> UIStateObjects;
    [SerializeField] GameObject MainMenuGO;

    public LeaderboardScoresPage LeaderboardData {get; private set;}

    [field:SerializeField] public Unlockables unlockables { get; set; }
    private string unlockablesPath;

    // Events
    private GameState _gameState;
    [field:SerializeField] public GameDifficultyEnum GameDifficulty { get; set; }
    public static event Action OnPointGain;
    public static event Action OnPointLose;

    public static event Action OnGameLose;

    private Vector3 mainmenuPos;

    // Score
    public int Score { get; set; }
    public int Stars{ get; set; }
    public int HighScore {get; set;}

    [field:SerializeField] public int Lives {get; set;}

    // Colors
    private readonly Color32[] ColorArray = new Color32[] { new Color32(79, 196, 207, 255), // Cyan
                                                   //new Color32(0, 0, 0, 255), // Black
                                                   new Color32(242, 238, 245, 255), // White
                                                   new Color32(251, 221, 116, 255), // Yellow
                                                   new Color32(153, 79, 243, 255), // Purple
    };
    public Color32 CurrentColor { get; private set; }
    public Color32 TargetColor { get; private set; }

    // Timers
    [SerializeField][Range(0.5f,5)] private float colorChangeSpeed = 2f;
    private float colorChangeSpeedInternal;
    [SerializeField] [Range(2, 10)] private float targetColorChangeSpeed = 5f;
    private float timer;
    public float targetTimer { get; set; }

    private void Awake() // TODO - init Stars
    {
        mainmenuPos = MainMenuGO.transform.position;
        unlockablesPath = $"{Application.persistentDataPath}/unlockables.json";
        Stars = 1000;
        ResetCurrentGameStats();
        GameDifficulty = (GameDifficultyEnum)Enum.Parse(typeof(GameDifficultyEnum), PlayerPrefs.GetString("GameDifficulty", "Medium"));
        uiManager.UpdateDifficultyText();

        Application.targetFrameRate = 120;
        ChangeGameState(GameState.LogIn);

        PlayButtonState.OnTapAction += CheckTap;
    }

    private void Start()
    {
        Debug.Log($"{Application.persistentDataPath}/unlockables.json");
        if(File.Exists(unlockablesPath))
        {
            loadUnlockables();
        }
        else
        {
            unlockables.currentCirclePattern = 0;
            unlockables.circlePatterns = new int[10] {1, 0, 0, 0, 0, 0, 0, 0, 0, 0};
            saveUnlockables();
        }
    }

    private void loadUnlockables()
    {
        string json = File.ReadAllText(unlockablesPath);
        unlockables = JsonUtility.FromJson<Unlockables>(json);
    }

    public void saveUnlockables()
    {
        string json = JsonUtility.ToJson(unlockables);
        File.WriteAllText(unlockablesPath, json);
    }

    public async void LoadHighScore(){
        var query = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string>{ GameManager.HsSaveKey });
        try{
            HighScore = Int32.Parse(query[GameManager.HsSaveKey]);
        }
        catch (KeyNotFoundException){
            HighScore = 0;
        }
        uiManager.UpdateHighScoreUI(HighScore.ToString());
    }

    public float GetTargetColourChangeSpeed()
    {
        return targetColorChangeSpeed;
    }

    public GameState GetGameState()
    {
        return _gameState;
    }

    private void ChangeUIState(State state)
    {
        foreach (UIState item in UIStateObjects)
        {
            item.changeState(state);
        }
    }

    private void Update()
    {
        ExecuteStateUpdate();
    }

    private void ExecuteStateUpdate(){
        if(_gameState == GameState.InGame){
                timer -= Time.deltaTime;
                targetTimer -= Time.deltaTime;

                if(targetTimer <= 0)
                {
                    TargetColor = ColorArray[UnityEngine.Random.Range(0, ColorArray.Length)];
                    targetTimer = targetColorChangeSpeed;
                    uiManager.UpdateTargetColor(TargetColor);
                }

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

    private void ExecuteState()
    {
        switch (_gameState)
        {
            case GameState.LogIn:
                break;
            case GameState.MainMenu:
                // load normal colour/no pattern
                uiManager.ChangeCirclePattern(0);

                MainMenuGO.transform.position = mainmenuPos;
                ChangeUIState(State.MainMenuIdle);
                uiManager.UpdateGameElementsToMainMenu();
                /*
                Lives = 3;
                Score = 0;
                colorChangeSpeedInternal = colorChangeSpeed;
                timer = colorChangeSpeed;
                */
                break;
            case GameState.InGame:
                uiManager.ChangeCirclePattern(unlockables.currentCirclePattern);
                ChangeUIState(State.InGameIdle);
                uiManager.UpdateGameElementsToInGame();
                break;
            case GameState.Lose:
                uiManager.DeactivatePlayButton();
                ChangeUIState(State.LoseIdle);
                ResetGame();
                break;
            default:
                break;
        }
    }

    private async void ResetGame()
    {
        uiManager.GameEndUI();
        await System.Threading.Tasks.Task.Delay(2000);
        ChangeGameState(GameState.MainMenu);
        ResetCurrentGameStats();
    }
    public void ChangeGameState(GameState n)
    {
        if(_gameState != n)
        {
            _gameState = n;
        }
        ExecuteState();
    }

    public void ChangeGameState(int n){
        if(_gameState != (GameState) n)
        {
            _gameState = (GameState) n;
        }
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
        if(_gameState == GameState.InGame){
            if (CurrentColor.ToString() == TargetColor.ToString())
            {
                Score++;
                Stars++;
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
                    if(Score > HighScore)
                    {
                        // Save highscore to the cloud
                        cloudSaveManager.SaveCloudData(Score, HsSaveKey);
                        HighScore = Score;
                        uiManager.UpdateHighScoreUI(HighScore.ToString());

                        // Add new highscore to the leaderboard
                        AddScoreToLB(HighScore);
                        GetScoresFromLB();

                    }
                    OnGameLose?.Invoke();
                    ChangeGameState(GameState.Lose);
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

    public async void GetScoresFromLB()
    {
        // Returns top x results (limit 10)
        LeaderboardData = await LeaderboardsService.Instance.GetScoresAsync("ColourSwipeLB");
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
