using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    [SerializeField] private GameObject MainMenu, Settings, Info, Shop, LeaderBoard, LogIn;
    GameManager gameManager;

    private void Start() {
        gameManager = GetComponent<GameManager>();
    }

    public void LoadMainMenu()
    {
        MainMenu.SetActive(true);
        Settings.SetActive(false);
        Info.SetActive(false);
        Shop.SetActive(false);
        LeaderBoard.SetActive(false);
        LogIn.SetActive(false);
    }

    public void LoadSettings()
    {
        if(gameManager.GetGameState() == GameState.MainMenu || gameManager.GetGameState() == GameState.InGame){
            Settings.SetActive(true);
            MainMenu.SetActive(false);
        }
    }

    public void LoadInfo()
    {
        if(gameManager.GetGameState() != GameState.Lose)
        {
        MainMenu.SetActive(false);
        Settings.SetActive(false);
        Info.SetActive(true);
        Shop.SetActive(false);
        LeaderBoard.SetActive(false);
        }
    }

    public void LoadShop()
    {
        if(gameManager.GetGameState() != GameState.Lose){
        MainMenu.SetActive(false);
        Settings.SetActive(false);
        Info.SetActive(false);
        Shop.SetActive(true);
        LeaderBoard.SetActive(false);
        }
    }

    public void LoadLeaderBoard()
    {
        if(gameManager.GetGameState() != GameState.Lose){
        MainMenu.SetActive(false);
        Settings.SetActive(false);
        Info.SetActive(false);
        Shop.SetActive(false);
        LeaderBoard.SetActive(true);
        }
    }
}
