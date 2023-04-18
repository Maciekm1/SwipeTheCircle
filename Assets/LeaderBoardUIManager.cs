using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Newtonsoft.Json;
using Unity.Services.Leaderboards;
using System.Threading.Tasks;

public class LeaderBoardUIManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject leaderBoardEntryTemplate;
    [SerializeField] private Transform content;
    List<LeaderBoardEntry> results;

    private void OnEnable() 
    {
        CloudSaveManager.OnUserLogin += GenerateEntriesAsync;    
    }

    private void OnDisable() 
    {
        CloudSaveManager.OnUserLogin -= GenerateEntriesAsync;
    }
    public async void GenerateEntriesAsync()
    {
        await GetScoresFromLB();
        
        ClearEntries();
        for (int i = 0; i < results.Count; i++)
        {
            if(i + 1< content.childCount && content.GetChild(i + 1) != null)
            {
                content.GetChild(i + 1).gameObject.SetActive(true);

                content.GetChild(i + 1).transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = (Int32.Parse(results[i].Rank) + 1).ToString();
                content.GetChild(i + 1).transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = results[i].Tier;
                content.GetChild(i + 1).transform.GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>().text = results[i].PlayerName.Split("#")[0];
                content.GetChild(i + 1).transform.GetChild(0).GetChild(3).GetComponent<TextMeshProUGUI>().text = Int32.Parse(results[i].Score.Split(".")[0]).ToString();

                if(results[i].PlayerName.Split("#")[0] == PlayerPrefs.GetString("playerName"))
                {
                    content.GetChild(i + 1).transform.GetChild(0).GetComponent<Image>().color = new Color32(255, 242, 102, 255);
                }
            }
            else
            {
                GameObject newEntry = Instantiate(leaderBoardEntryTemplate);
                newEntry.transform.SetParent(content, false);
                newEntry.gameObject.SetActive(true);

                newEntry.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = (Int32.Parse(results[i].Rank) + 1).ToString();
                newEntry.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = results[i].Tier;
                newEntry.transform.GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>().text = results[i].PlayerName.Split("#")[0];
                newEntry.transform.GetChild(0).GetChild(3).GetComponent<TextMeshProUGUI>().text = Int32.Parse(results[i].Score.Split(".")[0]).ToString();
            }
        }
        Debug.Log("LB UI updated.");
    }

    public void ClearEntries()
    {
        for (int i = 1; i < content.childCount; i++)
        {
            content.GetChild(i).gameObject.SetActive(false);
        }
        Debug.Log("Cleared Entries...");
    }

    public async Task GetScoresFromLB()
    {
        // Returns top x results (limit 50)
        var scoresResponse = await LeaderboardsService.Instance.GetScoresAsync("ColourSwipeLB", new GetScoresOptions{ Limit = 50 });
        results = JsonConvert.DeserializeObject<List<LeaderBoardEntry>>(JsonConvert.SerializeObject(scoresResponse.Results));
    }
}
