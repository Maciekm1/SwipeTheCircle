using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Tutorial : MonoBehaviour
{
    public static readonly string firstTimeKey = "firstTimeKey";
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] private TextMeshProUGUI counterText;
    [SerializeField] private Button button;

    [SerializeField] private Sprite[] imageSprites;
    [SerializeField] private string[] tutorialtexts;

    private short currentImage = 0;
    private short currentText = 0;
    private short currentCount = 1;

    private void Awake(){
        Application.targetFrameRate = Screen.currentResolution.refreshRate;
        if(PlayerPrefs.GetInt(firstTimeKey, 0) == 1){
            UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
            return;
        }
        PlayerPrefs.SetInt(firstTimeKey, 1);
    }

    private void Start(){
        image.sprite = imageSprites[currentImage];
        tutorialText.text = tutorialtexts[currentText];
        counterText.text = $"{currentCount}/{imageSprites.Length}";
    }

    public void AdvanceTutorial(){

        if(currentCount == imageSprites.Length) { return; }

        image.sprite = imageSprites[++currentImage];
        tutorialText.text = tutorialtexts[++currentText];
        counterText.text = $"{++currentCount}/{imageSprites.Length}";

        if(currentCount == imageSprites.Length){
            button.transform.GetComponentInChildren<TextMeshProUGUI>().text = "Close"; 
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(LoadNextScene);
        }

    }

    private void LoadNextScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
    }
}
