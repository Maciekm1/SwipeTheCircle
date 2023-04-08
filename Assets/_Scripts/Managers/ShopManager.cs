using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [Header("Circle")]
    [SerializeField] private ShopItem[] shopItems;
    [SerializeField] private GameObject[] panels;
    [SerializeField] private Image[] sprites;
    [SerializeField] private Button[] costButtons;

    [SerializeField] private Sprite tickSprite;

    private void Start()
    {
        updateShopButtons();
    }

    private void updateShopButtons()
    {
        for (int i = 0; i < gameManager.unlockables.circlePatterns.Length; i++)
        {
            // 1 - Purchased, 2 - Equipped
            if(gameManager.unlockables.circlePatterns[i] == 1)
            {
                costButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = "equip";
                costButtons[i].GetComponent<Image>().color = new Color32(0, 255, 0, 255);

                //Deactivate star icon
                costButtons[i].transform.GetChild(1).gameObject.SetActive(false);
            }
            else if(gameManager.unlockables.circlePatterns[i] == 2)
            {
                costButtons[i].GetComponent<Image>().color = new Color32(0, 90, 0, 255);
                costButtons[i].GetComponentInChildren<TextMeshProUGUI>().fontSize = 20;
                costButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = "equipped";
                costButtons[i].transform.GetChild(0).GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);

                //Disable Star
                costButtons[i].transform.GetChild(1).gameObject.SetActive(false);
            }
        }
    }
    public void enablePanels(){
        check_purchasable();
        for (int i = 0; i < shopItems.Length; i++)
        {
            panels[i].SetActive(true);
            sprites[i].sprite = shopItems[i].Sprite;
            Button costButton = costButtons[i];
            if(gameManager.unlockables.circlePatterns[i] == 0)
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
