using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class LeaderBoardUIManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject leaderBoardEntryTemplate;
    [SerializeField] private Transform content;

    public void GenerateEntries()
    {
        ClearEntries();
        for (int i = 0; i < gameManager.LeaderboardData.Results.Count; i++)
        {
            GameObject newEntry = Instantiate(leaderBoardEntryTemplate);
            newEntry.transform.SetParent(content, false);
            newEntry.SetActive(true);

            newEntry.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = (gameManager.LeaderboardData.Results[i].Rank + 1).ToString();
            newEntry.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = gameManager.LeaderboardData.Results[i].Tier;
            newEntry.transform.GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>().text = gameManager.LeaderboardData.Results[i].PlayerName.Split("#")[0];
            newEntry.transform.GetChild(0).GetChild(3).GetComponent<TextMeshProUGUI>().text = (gameManager.LeaderboardData.Results[i].Score).ToString();
        }
    }

    public void ClearEntries()
    {
        for (int i = 1; i < content.childCount; i++)
        {
            Destroy(content.GetChild(i).gameObject);
        }
    }
}
