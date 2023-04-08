using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private UIManager uIManager;
    [Header("Circle")]
    [SerializeField] private ShopItem[] shopItems;
    [SerializeField] private GameObject[] panels;
    [SerializeField] private Image[] sprites;
    [SerializeField] private Button[] costButtons;

    [SerializeField] private Sprite tickSprite;

    private void Start()
    {
        //UpdateShopButtons();
    }

    public void UpdateShopButtons()
    {
        for (int i = 0; i < gameManager.unlockables.circlePatterns.Length; i++)
        {
            if(gameManager.unlockables.currentCirclePattern == i)
            {
                costButtons[i].GetComponent<Image>().color = new Color32(0, 160, 0, 255);
                costButtons[i].GetComponentInChildren<TextMeshProUGUI>().fontSize = 20;
                costButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = "equipped";
                costButtons[i].interactable = false;

                int x = i;
                costButtons[i].onClick.RemoveAllListeners();

                costButtons[i].transform.GetChild(0).GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);

                //Deactivate star icon
                costButtons[i].transform.GetChild(1).gameObject.SetActive(false);
            }
            else if(gameManager.unlockables.circlePatterns[i] == 1)
            {
                costButtons[i].interactable = true;
                costButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = "equip";
                costButtons[i].GetComponent<Image>().color = new Color32(0, 255, 0, 255);

                int x = i;
                costButtons[i].onClick.RemoveAllListeners();
                costButtons[i].onClick.AddListener(delegate {equipCirclePattern(x);});
                costButtons[i].transform.GetChild(0).GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);

                //Deactivate star icon
                costButtons[i].transform.GetChild(1).gameObject.SetActive(false);
            }
            else if(gameManager.unlockables.circlePatterns[i] == 0)
            {
                int x = i;
                costButtons[i].onClick.RemoveAllListeners();
                costButtons[i].onClick.AddListener(delegate {PurchaseItem(x);});
            }

            // Update circle pattern
            if(gameManager.GetGameState() == GameState.InGame){
                uIManager.ChangeCirclePattern(gameManager.unlockables.currentCirclePattern);
            }
        }
    }

    public ShopItem GetShopItem(int i){
        return shopItems[i];
    }

    private void equipCirclePattern(int i)
    {
        gameManager.unlockables.currentCirclePattern = i;
        gameManager.saveUnlockables();

        UpdateShopButtons();
    }

    private void PurchaseItem(int i)
    {
        if(gameManager.Stars >= shopItems[i].Cost){

            gameManager.Stars -= shopItems[i].Cost;
            uIManager.updateStarsUI();
            check_purchasable();

            gameManager.unlockables.circlePatterns[i] = 1;
            gameManager.saveUnlockables();

            Debug.Log($"Purchase successful for {shopItems[i].Cost}");
            UpdateShopButtons();
        }
    }
    public void enablePanels(){
        check_purchasable();
        for (int i = 0; i < shopItems.Length; i++)
        {
            panels[i].SetActive(true);
            sprites[i].sprite = shopItems[i].Sprite;
            Button costButton = costButtons[i];
            if(gameManager.unlockables.circlePatterns[i] == 0 && gameManager.unlockables.currentCirclePattern != i)
            {
                costButton.GetComponentInChildren<TextMeshProUGUI>().text = shopItems[i].Cost.ToString();
            }
            /*
            else{
                costButton.GetComponentInChildren<TextMeshProUGUI>().text = "equip";
                costButton.GetComponent<Image>().color = new Color(0, 255, 0);
                costButton.transform.GetChild(1).gameObject.SetActive(false);
            }
            */
            
        }
    }

    public void disablePanels(){
        for (int i = 0; i < shopItems.Length; i++)
        {
            panels[i].SetActive(false);
        }
    }

    private void check_purchasable()
    {
        for (int i = 0; i < shopItems.Length; i++)
        {
            if(gameManager.Stars >= shopItems[i].Cost)
            {
                costButtons[i].interactable = true;
            }
            else
            {
                costButtons[i].interactable = false;
            }
        }
    }
}
