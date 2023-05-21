using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelPlayAds : MonoBehaviour
{

    void Start(){
        IronSource.Agent.init("1a06bd54d");
        IronSource.Agent.validateIntegration();
    }

    private void OnEnable() {
        IronSourceEvents.onSdkInitializationCompletedEvent += SdkInitializationCompletedEvent;

        //Add AdInfo Interstitial Events
        IronSourceInterstitialEvents.onAdReadyEvent += InterstitialOnAdReadyEvent;
        IronSourceInterstitialEvents.onAdLoadFailedEvent += InterstitialOnAdLoadFailed;
        IronSourceInterstitialEvents.onAdOpenedEvent += InterstitialOnAdOpenedEvent;
        IronSourceInterstitialEvents.onAdClickedEvent += InterstitialOnAdClickedEvent;
        IronSourceInterstitialEvents.onAdShowSucceededEvent += InterstitialOnAdShowSucceededEvent;
        IronSourceInterstitialEvents.onAdShowFailedEvent += InterstitialOnAdShowFailedEvent;
        IronSourceInterstitialEvents.onAdClosedEvent += InterstitialOnAdClosedEvent;
    }
    void OnApplicationPause(bool isPaused) {            
        IronSource.Agent.onApplicationPause(isPaused);
    }

    private void SdkInitializationCompletedEvent(){

    }

    public void loadFullSizeAdd(){
        IronSource.Agent.loadInterstitial();
    }

    public void showFullSizeAdd(){
        if(IronSource.Agent.isInterstitialReady()){
            IronSource.Agent.showInterstitial();
        }
        else{
            Debug.Log("FULL SIZE ADD NOT READY");
        }
    }

    // Banner Callbacks

    // RewardedAd Callbacks

    // Full Size Ad Callbacks

    /************* Interstitial AdInfo Delegates *************/
// Invoked when the interstitial ad was loaded succesfully.
void InterstitialOnAdReadyEvent(IronSourceAdInfo adInfo) {
}
// Invoked when the initialization process has failed.
void InterstitialOnAdLoadFailed(IronSourceError ironSourceError) {
}
// Invoked when the Interstitial Ad Unit has opened. This is the impression indication. 
void InterstitialOnAdOpenedEvent(IronSourceAdInfo adInfo) {
}
// Invoked when end user clicked on the interstitial ad
void InterstitialOnAdClickedEvent(IronSourceAdInfo adInfo) {
}
// Invoked when the ad failed to show.
void InterstitialOnAdShowFailedEvent(IronSourceError ironSourceError, IronSourceAdInfo adInfo) {
}
// Invoked when the interstitial ad closed and the user went back to the application screen.
void InterstitialOnAdClosedEvent(IronSourceAdInfo adInfo) {
    FindObjectOfType<UIManager>().UpdateGameElementsToMainMenu();
    FindObjectOfType<GameManager>().ChangeGameState(GameState.MainMenu);
}
// Invoked before the interstitial ad was opened, and before the InterstitialOnAdOpenedEvent is reported.
// This callback is not supported by all networks, and we recommend using it only if  
// it's supported by all networks you included in your build. 
void InterstitialOnAdShowSucceededEvent(IronSourceAdInfo adInfo) {
    FindObjectOfType<UIManager>().UpdateGameElementsToMainMenu();
}

}
