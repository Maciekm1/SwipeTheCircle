using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject playButtonOuter;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private GameObject HS;
    [SerializeField] private GameObject prompt;
    [SerializeField] private GameObject targetColour;

    [SerializeField] private GameObject ColourSwitchBar;

    [SerializeField] private TextMeshProUGUI DiffSelectText;
    [SerializeField] private GameObject[] lives;

    // ShopUI
    [SerializeField] private TextMeshProUGUI starsText;

    [SerializeField] private Image equipButton;

    private void Awake()
    {
        PlayButtonState.OnTapAction += OnTap;
        GameManager.OnPointLose += loseLife;
    }

    private void loseLife()
    {
        if(gameManager.Lives >= 0)
        {
            LeanTween.scale(lives[gameManager.Lives], new Vector3(0, 0, 0), 0.7f).setEase(LeanTweenType.easeInOutElastic).setOnComplete(() => lives[gameManager.Lives+1].SetActive(false));
        }
    }

    public void setUpLife()
    {
        for (int i = 0; i <= gameManager.Lives; i++)
        {
            lives[i].SetActive(true);
            lives[i].transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    public void updateStarsUI() => starsText.text = gameManager.Stars.ToString();

    private void Update()
    {
        if(gameManager.GetGameState() == GameState.InGame){
            ColourSwitchBar.GetComponentInChildren<Image>().fillAmount = Mathf.Lerp(0, 1,gameManager.targetTimer / gameManager.GetTargetColourChangeSpeed());
        }
    }

    public void UpdateHighScoreUI(string score){
        HS.GetComponentInChildren<TextMeshProUGUI>().text = $"HighScore: {score}";
    }

    public void UpdateCurrentColor(Color32 color)
    {
        playButton.transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<Image>().color = color;
    }

    public void UpdateTargetColor(Color32 color)
    {
        targetColour.GetComponent<Image>().color = color;
    }

    private void OnTap()
    {
        if(gameManager.GetGameState() == GameState.InGame || gameManager.GetGameState() == GameState.Lose)
        {
            if(gameManager.CurrentColor.ToString() == gameManager.TargetColor.ToString())
            {
                playButton.transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<Image>().color = new Color(playButton.transform.GetChild(0).gameObject.GetComponent<Image>().color.r * 0.95f,
                                                                      playButton.transform.GetChild(0).gameObject.GetComponent<Image>().color.g * 0.95f,
                                                                      playButton.transform.GetChild(0).gameObject.GetComponent<Image>().color.b * 0.95f);
            }
            LeanTween.scale(playButtonOuter, new Vector3(1.1f, 1.1f, 1.1f), 0.2f).setEase(LeanTweenType.easeOutBack);
            LeanTween.scale(playButtonOuter, new Vector3(1f, 1f, 1f), 0.1f).setDelay(0.2f).setEase(LeanTweenType.easeOutCirc);
        }
    }

    public void UpdateScoreUI(String score)
    {
        titleText.text = score;
    }

    public void UpdateGameElementsToInGame()
    {
        
        titleText.text = "0";
        titleText.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 10, 0);
        prompt.SetActive(false);
        targetColour.SetActive(true);
        ColourSwitchBar.SetActive(true);

        LeanTween.alpha(targetColour.GetComponent<RectTransform>(), 1f, 0.1f);
        LeanTween.alpha(ColourSwitchBar.GetComponent<RectTransform>(), 1f, 0.1f);
        ColourSwitchBar.transform.GetChild(0).GetComponent<TextMeshProUGUI>().CrossFadeAlpha(1f, 0.1f, false);
    }

    public void UpdateGameElementsToMainMenu()
    {
        titleText.text = "Tap The Circle";
        titleText.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);
        prompt.SetActive(true);
        targetColour.SetActive(false);
        ColourSwitchBar.SetActive(false);

        LeanTween.alpha(targetColour.GetComponent<RectTransform>(), 0f, 0.1f);
        LeanTween.alpha(ColourSwitchBar.GetComponent<RectTransform>(), 0f, 0.1f);
        ColourSwitchBar.transform.GetChild(0).GetComponent<TextMeshProUGUI>().CrossFadeAlpha(0f, 0.1f, false);
    }

    public void GameEndUI(){
        LeanTween.alpha(targetColour.GetComponent<RectTransform>(), 0f, 1f).setDelay(0.5f);
        LeanTween.alpha(ColourSwitchBar.GetComponent<RectTransform>(), 0f, 1f).setDelay(0.5f);
        ColourSwitchBar.transform.GetChild(0).GetComponent<TextMeshProUGUI>().CrossFadeAlpha(0f, 1.5f, false);
    }

    public void UpdateDifficultyText()
    {
        DiffSelectText.text = gameManager.GameDifficulty.ToString();
        PlayerPrefs.SetString("GameDifficulty", gameManager.GameDifficulty.ToString());
        if(gameManager.GetGameState() == GameState.InGame)
        {
            gameManager.ResetCurrentGameStats();
        }
    }

    public void ActivatePlayButton(){
        playButton.GetComponent<Button>().interactable = true;

    }

    public void DeactivatePlayButton(){
        playButton.GetComponent<Button>().interactable = false;

    }

}
