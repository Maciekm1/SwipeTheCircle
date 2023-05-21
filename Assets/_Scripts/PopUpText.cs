using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class PopUpText : MonoBehaviour
{
    private float appearTimer;
    private bool active;
    private int innerCount = 1;
    public void Appear(){
        gameObject.SetActive(true);
        if(active){
            innerCount++;
        }
        else{
            innerCount = 1;
        }
        active = true;
        appearTimer = Time.time + 1f;
        LeanTween.scale(this.gameObject, new Vector3(1f, 1f, 1f), 0.2f).setEase(LeanTweenType.easeInOutSine);
        LeanTween.alpha(GetComponent<RectTransform>(), 1f, 0.2f);
    }

    private void Update() {
        if(appearTimer < Time.time && active)
        {
            Disappear();
        }
    }
    public void showText(){
        GetComponent<TextMeshProUGUI>().text = $"+{innerCount}";
    }

    public void Disappear()
    {
        LeanTween.scale(this.gameObject, new Vector3(0f, 0f, 0f), 0.5f).setEase(LeanTweenType.easeInOutSine);
        LeanTween.alpha(GetComponent<RectTransform>(), 0f, 0.5f).setEase(LeanTweenType.easeInElastic);
        active = false;
        //gameObject.SetActive(false);
    }

    private void OnEnable() 
    {

    }

    private void OnDisable() 
    {
        
    }
}
