using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Threading.Tasks;

public class Disappear : MonoBehaviour
{

    public async void ChangeText(string t, int dur)
    {
        GetComponent<TextMeshProUGUI>().text = t;
        await Task.Delay(dur);
        LeanDisappear();
    }

    public void LeanDisappear()
    {
        LeanTween.alpha(this.gameObject, 0f, 0.5f).setOnComplete(() => gameObject.SetActive(false));
    }
}
