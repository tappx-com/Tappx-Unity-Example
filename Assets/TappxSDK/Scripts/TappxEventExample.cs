using UnityEngine;
using TappxSDK;

/// <summary>
/// Example script showing how to listen to Tappx SDK events
/// </summary>
public class TappxEventExample : MonoBehaviour
{
    void Start()
    {
        // Subscribe to banner events
        TappxManagerUnity.OnBannerLoaded += OnBannerLoaded;
        TappxManagerUnity.OnBannerFailedToLoad += OnBannerFailedToLoad;
        TappxManagerUnity.OnBannerClicked += OnBannerClicked;
        
        // Subscribe to interstitial events
        TappxManagerUnity.OnInterstitialLoaded += OnInterstitialLoaded;
        TappxManagerUnity.OnInterstitialFailedToLoad += OnInterstitialFailedToLoad;
        TappxManagerUnity.OnInterstitialShown += OnInterstitialShown;
        TappxManagerUnity.OnInterstitialClicked += OnInterstitialClicked;
        TappxManagerUnity.OnInterstitialDismissed += OnInterstitialDismissed;
        
        // Subscribe to rewarded events
        TappxManagerUnity.OnRewardedLoaded += OnRewardedLoaded;
        TappxManagerUnity.OnRewardedFailedToLoad += OnRewardedFailedToLoad;
        TappxManagerUnity.OnRewardedVideoStarted += OnRewardedVideoStarted;
        TappxManagerUnity.OnRewardedVideoClicked += OnRewardedVideoClicked;
        TappxManagerUnity.OnRewardedVideoPlaybackFailed += OnRewardedVideoPlaybackFailed;
        TappxManagerUnity.OnRewardedVideoClosed += OnRewardedVideoClosed;
        TappxManagerUnity.OnRewardedVideoCompleted += OnRewardedVideoCompleted;
        TappxManagerUnity.OnRewardedUserEarnedReward += OnRewardedUserEarnedReward;
    }
    
    void OnDestroy()
    {
        // Unsubscribe from all events to prevent memory leaks
        TappxManagerUnity.OnBannerLoaded -= OnBannerLoaded;
        TappxManagerUnity.OnBannerFailedToLoad -= OnBannerFailedToLoad;
        TappxManagerUnity.OnBannerClicked -= OnBannerClicked;
        
        TappxManagerUnity.OnInterstitialLoaded -= OnInterstitialLoaded;
        TappxManagerUnity.OnInterstitialFailedToLoad -= OnInterstitialFailedToLoad;
        TappxManagerUnity.OnInterstitialShown -= OnInterstitialShown;
        TappxManagerUnity.OnInterstitialClicked -= OnInterstitialClicked;
        TappxManagerUnity.OnInterstitialDismissed -= OnInterstitialDismissed;
        
        TappxManagerUnity.OnRewardedLoaded -= OnRewardedLoaded;
        TappxManagerUnity.OnRewardedFailedToLoad -= OnRewardedFailedToLoad;
        TappxManagerUnity.OnRewardedVideoStarted -= OnRewardedVideoStarted;
        TappxManagerUnity.OnRewardedVideoClicked -= OnRewardedVideoClicked;
        TappxManagerUnity.OnRewardedVideoPlaybackFailed -= OnRewardedVideoPlaybackFailed;
        TappxManagerUnity.OnRewardedVideoClosed -= OnRewardedVideoClosed;
        TappxManagerUnity.OnRewardedVideoCompleted -= OnRewardedVideoCompleted;
        TappxManagerUnity.OnRewardedUserEarnedReward -= OnRewardedUserEarnedReward;
    }
    
    // Banner Event Handlers
    void OnBannerLoaded()
    {
        Debug.Log("🎯 Banner loaded successfully!");
        // Add your banner loaded logic here
    }
    
    void OnBannerFailedToLoad(string error)
    {
        Debug.Log($"❌ Banner failed to load: {error}");
        // Add your banner failed logic here
    }
    
    void OnBannerClicked()
    {
        Debug.Log("👆 Banner was clicked!");
        // Add your banner click logic here
    }
    
    // Interstitial Event Handlers
    void OnInterstitialLoaded()
    {
        Debug.Log("🎯 Interstitial loaded successfully!");
        // Add your interstitial loaded logic here
    }
    
    void OnInterstitialFailedToLoad(string error)
    {
        Debug.Log($"❌ Interstitial failed to load: {error}");
        // Add your interstitial failed logic here
    }
    
    void OnInterstitialShown()
    {
        Debug.Log("📱 Interstitial shown!");
        // Add your interstitial shown logic here
    }
    
    void OnInterstitialClicked()
    {
        Debug.Log("👆 Interstitial was clicked!");
        // Add your interstitial click logic here
    }
    
    void OnInterstitialDismissed()
    {
        Debug.Log("❌ Interstitial dismissed!");
        // Add your interstitial dismissed logic here
    }
    
    // Rewarded Event Handlers
    void OnRewardedLoaded()
    {
        Debug.Log("🎯 Rewarded ad loaded successfully!");
        // Add your rewarded loaded logic here
    }
    
    void OnRewardedFailedToLoad(string error)
    {
        Debug.Log($"❌ Rewarded ad failed to load: {error}");
        // Add your rewarded failed logic here
    }
    
    void OnRewardedVideoStarted()
    {
        Debug.Log("▶️ Rewarded video started!");
        // Add your rewarded video start logic here
    }
    
    void OnRewardedVideoClicked()
    {
        Debug.Log("👆 Rewarded video was clicked!");
        // Add your rewarded video click logic here
    }
    
    void OnRewardedVideoPlaybackFailed()
    {
        Debug.Log("❌ Rewarded video playback failed!");
        // Add your rewarded video playback failed logic here
    }
    
    void OnRewardedVideoClosed()
    {
        Debug.Log("❌ Rewarded video closed!");
        // Add your rewarded video closed logic here
    }
    
    void OnRewardedVideoCompleted()
    {
        Debug.Log("✅ Rewarded video completed!");
        // Add your rewarded video completed logic here
    }
    
    void OnRewardedUserEarnedReward()
    {
        Debug.Log("🎁 User earned reward!");
        // Add your reward logic here
        // Example: Give player coins, unlock content, etc.
    }
} 