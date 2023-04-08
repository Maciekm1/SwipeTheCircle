using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeanScale : MonoBehaviour
{
    private void OnEnable()
    {
        GameManager.OnPointGain += ScaleUp;
        GameManager.OnPointLose += ScaleDown;
    }

    private void OnDisable()
    {
        GameManager.OnPointGain -= ScaleUp;
        GameManager.OnPointLose -= ScaleDown;
    }

    private void ScaleUp()
    {
        LeanTween.scale(this.gameObject, new Vector3(1.25f, 1.25f, 1.25f), 0.2f).setEase(LeanTweenType.easeOutQuint);
        LeanTween.scale(this.gameObject, new Vector3(1f, 1f, 1f), 0.1f).setDelay(0.3f).setEase(LeanTweenType.easeInSine);
    }

    private void ScaleDown()
    {
        LeanTween.scale(this.gameObject, new Vector3(0.75f, 0.75f, 0.75f), 0.2f).setEase(LeanTweenType.easeOutQuint);
        LeanTween.scale(this.gameObject, new Vector3(1f, 1f, 1f), 0.1f).setDelay(0.3f).setEase(LeanTweenType.easeInSine);
    }
}
