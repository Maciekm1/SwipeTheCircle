using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Threading.Tasks;

public class Disappear : MonoBehaviour
{

    public async void ChangeText(string t, int dur, Action callback)
    {
        GetComponent<TextMeshProUGUI>().text = t;
        await Task.Delay(dur);
        callback();
    }

    public void LeanDisappear()
    {
        LeanTween.alpha(this.gameObject, 0f, 0.5f).setOnComplete(() => gameObject.SetActive(false));
    }
}
