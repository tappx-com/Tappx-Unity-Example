using System.Collections;
using UnityEngine;
using System.Runtime.InteropServices;
using TappxSDK;
using System;

public class TappxManagerUnity : MonoBehaviour
{
    public enum Position
    {
        TOP = 0,
        BOTTOM = 1
    }

    // === EVENT CALLBACKS ===
    // Banner Events
    public static event Action OnBannerLoaded;
    public static event Action<string> OnBannerFailedToLoad;
    public static event Action OnBannerClicked;
    
    // Interstitial Events
    public static event Action OnInterstitialLoaded;
    public static event Action<string> OnInterstitialFailedToLoad;
    public static event Action OnInterstitialShown;
    public static event Action OnInterstitialClicked;
    public static event Action OnInterstitialDismissed;
    public static event Action OnInterstitialClosed;
    
    // Rewarded Events
    public static event Action OnRewardedLoaded;
    public static event Action<string> OnRewardedFailedToLoad;
    public static event Action OnRewardedVideoStarted;
    public static event Action OnRewardedVideoClicked;
    public static event Action OnRewardedVideoPlaybackFailed;
    public static event Action OnRewardedVideoClosed;
    public static event Action OnRewardedVideoCompleted;
    public static event Action OnRewardedUserEarnedReward;

	private static TappxManagerUnity mInstance = null;


#if UNITY_IPHONE
    [DllImport("__Internal")]
    private static extern void trackInstallIOS_(string tappxID, bool testMode);
	[DllImport("__Internal")]
    private static extern void setEndpointIOS_(string endpoint);
    //Banner
    [DllImport("__Internal")]
    private static extern void loadBannerIOS_(int positionBanner, bool mrec);
//	private static extern void createBannerIOS_(Position positionBanner, bool mrec);
	[DllImport("__Internal")]
    private static extern void hideAdIOS_();
//    [DllImport("__Internal")]
//	private static extern void showAdIOS_(Position positionBanner);
	[DllImport("__Internal")]
	private static extern void releaseTappxIOS_();

	//Interstitial
    [DllImport("__Internal")]
	private static extern void loadInterstitialIOS_(bool autoShow );
	[DllImport("__Internal")]
	private static extern void showInterstitialIOS_();
	[DllImport("__Internal")]
	private static extern void releaseInterstitialTappxIOS_();
    [DllImport("__Internal")]
    private static extern bool isInterstitialReady_ ();


    //CCPA
    [DllImport("__Internal")]
    private static extern void setUSPrivacy_(string p_us_privacy);

	//Rewarded
	[DllImport("__Internal")]
	private static extern bool isRewardedReady_();
	[DllImport("__Internal")]
	private static extern void releaseRewardedTappxIOS_();
	[DllImport("__Internal")]
	private static extern void showRewardedIOS_();
	[DllImport("__Internal")]
	private static extern void loadRewardedIOS_(bool autoShow);

	//Coppa
    [DllImport("__Internal")]
    private static extern void setCoppaCompliance(bool accept);

#elif UNITY_ANDROID
    private AndroidJavaObject bannerControl = null;
	private AndroidJavaObject interstitialControl = null;
	private AndroidJavaObject rewardedControl = null;
	
#endif

    private int rewardedIsLoaded = 0;

	public static TappxManagerUnity instance
    {
        get
        {
            return mInstance;
        }
    }

    private bool isTestMode = false;
    private string currentTappxId = "";

    public void Awake()
    {
        if (mInstance == null)
        {
            mInstance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OnDestroy()
    {
        if (Application.isEditor) return;

#if UNITY_IPHONE
        if (mInstance == this)
        {
            // Only release global ad resources
            releaseTappxIOS_();
            releaseInterstitialTappxIOS_();
            releaseRewardedTappxIOS_();
        }
#elif UNITY_ANDROID
        if (mInstance == this)
        {
            // Release Android ad resources
            if (bannerControl != null)
            {
                bannerControl.Call("hideBannerGONE");
                bannerControl = null;
            }
            if (interstitialControl != null)
            {
                interstitialControl = null;
            }
            if (rewardedControl != null)
            {
                rewardedControl = null;
            }

        }
#endif
    }

    public void hide()
    {
		#if UNITY_IPHONE
			hideAdIOS_();
		#elif UNITY_ANDROID
			bannerControl.Call("hideBannerGONE");
			bannerControl = null;
		#endif
    }

	
    public void setUSPrivacy(string p_us_privacy)
    {
        #if UNITY_IPHONE
			setUSPrivacy_(p_us_privacy);
        #elif UNITY_ANDROID
            new AndroidJavaObject("com.tappx.unity.ccpaTappx").Call("setUSPrivacy", p_us_privacy);
        #endif
    }
    


	public void AcceptGeolocate(bool p_accept)
	{
		new AndroidJavaObject("Tappx.setCollectLocationEnabled").Call("acceptGeolocate", p_accept);
	}

	public void AcceptCoppa(bool p_accept)
	{
#if UNITY_IPHONE
			setCoppaCompliance(p_accept);
#elif UNITY_ANDROID
			new AndroidJavaObject("com.tappx.unity.coppaTappx").Call("acceptCoppa", p_accept);
#endif
	}

	public void show( TappxSettings.POSITION_BANNER pos, bool mrec )
	{
		
#if UNITY_IPHONE
		loadBannerIOS_( pos == TappxSettings.POSITION_BANNER.TOP ? (int)Position.TOP : (int)Position.BOTTOM, mrec );
#elif UNITY_ANDROID
		
		// if (bannerControl != null)
		// {
		// 	bannerControl.Call("hideBannerGONE");
		// 	bannerControl = null;
		// }
		bool posit = pos != TappxSettings.POSITION_BANNER.BOTTOM;
		var androidAppId = TappxSettings.getAndroidAppId();
		bannerControl = new AndroidJavaObject("com.tappx.unity.bannerTappx", androidAppId, mrec, posit, TappxSettings.Instance.testEnabled, "TappxManagerUnity", TappxSettings.getEndpoint());
#endif	
		
		
	}
	
	public void show()
    {
        // No per-scene logic, just show default banner
        show(TappxSettings.POSITION_BANNER.BOTTOM, false);
    }

    public bool isBannerVisible()
    {
		#if UNITY_ANDROID
		if(bannerControl!=null){
        	return bannerControl.Call<bool>("isBannerVisible");
		}
		#endif
        return false;
    }

	public bool isInterstitialReady()
	{
		
#if UNITY_IPHONE

	return isInterstitialReady_(); 

#endif

#if UNITY_ANDROID
	if (  interstitialControl != null )
		return interstitialControl.Call<bool>("isReady");
	else
		return false;
#endif

        return false;		
	}

	public void loadInterstitial(bool _autoShow)
	{
        #if UNITY_IPHONE
			loadInterstitialIOS_( _autoShow );
	#elif UNITY_ANDROID
		if(interstitialControl!=null){
			interstitialControl = null;
		}
		interstitialControl = new AndroidJavaObject ("com.tappx.unity.interstitialTappx",TappxSettings.getAndroidAppId(), _autoShow, TappxSettings.Instance.testEnabled ,"TappxManagerUnity", TappxSettings.getEndpoint());

#endif

    }
	
	public void loadInterstitial(){

		loadInterstitial( false );

	}

	public void interstitialShow(){
		#if UNITY_IPHONE
			showInterstitialIOS_();
		#elif UNITY_ANDROID
		if(interstitialControl!=null){
		    interstitialControl.Call("show");
		}
		#endif
	}

	//Rewarded
	public bool isRewardedReady(){
#if UNITY_ANDROID
			if (  rewardedControl != null )
				return rewardedControl.Call<bool>("isReady");
			else
				return false;
#elif UNITY_IPHONE
		return isRewardedReady_();
#endif
		return false;
	}
	//Rewarded
	public void loadRewarded(bool autoShow = false)
    {
#if UNITY_ANDROID
        if (rewardedControl != null)
        {
            rewardedControl = null;
        }
        rewardedControl = new AndroidJavaObject("com.tappx.unity.rewardedTappx", TappxSettings.getAndroidAppId(), autoShow, TappxSettings.Instance.testEnabled, "TappxManagerUnity", TappxSettings.getEndpoint());
#elif UNITY_IPHONE
        loadRewardedIOS_(autoShow);
#endif
    }
	//Rewarded
	public void rewardedShow(){
#if UNITY_ANDROID
			if(rewardedControl!=null){
				rewardedControl.Call("show");
			}
#elif UNITY_IPHONE
		showRewardedIOS_();
#endif
	}

	//Rewarded
	public void releaseRewarded(){
#if UNITY_ANDROID
		if(rewardedControl!=null){
			rewardedControl = null;
		}
#elif UNITY_IPHONE
		releaseRewardedTappxIOS_();
#endif
	}

#if UNITY_IPHONE
	public void tappxBannerDidReceiveAd(){
		UnityEngine.Debug.Log("Banner Received");
		OnBannerLoaded?.Invoke();
	}
	
	public void tappxBannerFailedToLoad(string error){
		UnityEngine.Debug.Log("Banner Error " + error);
		OnBannerFailedToLoad?.Invoke(error);
	}
	
	public void tappxInterstitialDidReceiveAd(){
		UnityEngine.Debug.Log("Interstitial Load");
	}
	
	public void tappxInterstitialFailedToLoad(string error){
		UnityEngine.Debug.Log("Interstitial Error " + error);
	}

	public void tappxViewWillLeaveApplication() {
		UnityEngine.Debug.Log("Banner did click ");	
	}

	public void interstitialWillLeaveApplication() {
		UnityEngine.Debug.Log("Interstitial did click ");	
	}

	//Rewarded

	public void tappxRewardedViewControllerDidFinishLoad()
    {
		UnityEngine.Debug.Log("tappxRewardedViewControllerDidFinishLoad");
		OnRewardedLoaded?.Invoke();
	}

	public void tappxRewardedViewControllerDidFail(string error)
	{
		UnityEngine.Debug.Log("tappxRewardedViewControllerDidFail: " + error);
		OnRewardedFailedToLoad?.Invoke(error);
	}

	public void tappxRewardedViewControllerClicked()
	{
		UnityEngine.Debug.Log("Rewarded Clicked");
		OnRewardedVideoClicked?.Invoke();
	}

	public void tappxRewardedViewControllerPlaybackFailed()
	{
		UnityEngine.Debug.Log("Rewarded Playback Failed");
		OnRewardedVideoPlaybackFailed?.Invoke();
	}

	public void tappxRewardedViewControllerVideoClosed()
	{
		UnityEngine.Debug.Log("Rewarded Closed");
		OnRewardedVideoClosed?.Invoke();
	}

	public void tappxRewardedViewControllerVideoCompleted()
	{
		UnityEngine.Debug.Log("tappxRewardedViewControllerVideoCompleted");
		OnRewardedVideoCompleted?.Invoke();
	}

	public void tappxRewardedViewControllerDidAppear()
	{
		UnityEngine.Debug.Log("tappxRewardedViewControllerDidAppear");
		OnRewardedVideoStarted?.Invoke();
	}

	public void tappxRewardedViewControllerDismissed()
	{
		UnityEngine.Debug.Log("tappxRewardedViewControllerDismissed");
	}

	public void tappxRewardedViewControllerUserDidEarnReward()
	{
		UnityEngine.Debug.Log("Rewarded Complete");
		OnRewardedUserEarnedReward?.Invoke();
	}


#elif UNITY_ANDROID
	public void OnAdLoaded(){
		UnityEngine.Debug.Log("Banner Received");
		OnBannerLoaded?.Invoke();
	}

	public void OnAdFailedToLoad(string error){
		UnityEngine.Debug.Log("Banner Error " + error);
		OnBannerFailedToLoad?.Invoke(error);
	}

	public void InterstitialLoaded(){
		UnityEngine.Debug.Log("Interstitial Load");
		OnInterstitialLoaded?.Invoke();
	}
	
	public void InterstitialFailedToLoad(string error){
		UnityEngine.Debug.Log("Interstitial Error " + error);
		OnInterstitialFailedToLoad?.Invoke(error);
	}
	
	public void InterstitialLeftApplication(){
		UnityEngine.Debug.Log("Interstitial Cliked");
		OnInterstitialClicked?.Invoke();
	}
	
	public void onInterstitialDismissed(){
		UnityEngine.Debug.Log("Interstitial Dismissed");
		OnInterstitialDismissed?.Invoke();
	}

	public void onInterstitialShown(){
		UnityEngine.Debug.Log("Interstitial Shown");
		OnInterstitialShown?.Invoke();
	}
	//S>Rewarded
	public void ChangeStatusRewardedIsLoaded(int value)
    {
        rewardedIsLoaded = value;
#if UNITY_ANDROID
        // If you need to sync with AndroidJavaObject, do it here
#endif
    }
	public int RewardedIsLoadedValue()
    {
		return rewardedIsLoaded;
    }
	public void RewardedLoaded(){
		rewardedIsLoaded = 2;
		UnityEngine.Debug.Log("Rewarded Load");
		OnRewardedLoaded?.Invoke();
	}
	public void RewardedFailedToLoad(string error){
		rewardedIsLoaded = 0;
		UnityEngine.Debug.Log("Rewarded failed to Load " + error);
		OnRewardedFailedToLoad?.Invoke(error);
	}
	public void RewardedVideoStart(){
		UnityEngine.Debug.Log("Rewarded Start");
		OnRewardedVideoStarted?.Invoke();
	}
	public void RewardedVideoClicked(){
		UnityEngine.Debug.Log("Rewarded Clicked");
		OnRewardedVideoClicked?.Invoke();
	}
	public void RewardedVideoPlaybackFailed(){
		UnityEngine.Debug.Log("Rewarded Playback Failed");
		OnRewardedVideoPlaybackFailed?.Invoke();
	}
	public void RewardedVideoClosed(){
		rewardedIsLoaded = 0;
		UnityEngine.Debug.Log("Rewarded Closed");
		OnRewardedVideoClosed?.Invoke();
	}
	public void RewardedVideoCompleted(){
		UnityEngine.Debug.Log("Rewarded Complete");
		OnRewardedVideoCompleted?.Invoke();
	}
	//E>Rewarded
	
	public void OnAdLeftApplication()
	{
		UnityEngine.Debug.Log("Banner did click" );
	}


#endif

    public void SetTestMode(bool testMode)
    {
        isTestMode = testMode;
        if (!string.IsNullOrEmpty(currentTappxId))
        {
#if UNITY_IPHONE
            trackInstallIOS_(currentTappxId, isTestMode);
#elif UNITY_ANDROID
#endif
        }
    }

    public void SetTappxId(string tappxId)
    {
        if (string.IsNullOrEmpty(tappxId)) return;
        
        currentTappxId = tappxId;
#if UNITY_IPHONE
        trackInstallIOS_(currentTappxId, isTestMode);
#elif UNITY_ANDROID
#endif
    }

// === Interstitial Callbacks from iOS Bridge ===

public void tappxInterstitialDidAppear()
{
    UnityEngine.Debug.Log("tappxInterstitialDidAppear");
    OnInterstitialShown?.Invoke();
}

public void tappxInterstitialDidFail(string error)
{
    UnityEngine.Debug.Log("tappxInterstitialDidFail: " + error);
    OnInterstitialFailedToLoad?.Invoke(error);
}

public void interstitialDidPress()
{
    UnityEngine.Debug.Log("interstitialDidPress");
    OnInterstitialClicked?.Invoke();
}

public void tappxInterstitialDidFinishLoad()
{
    UnityEngine.Debug.Log("tappxInterstitialDidFinishLoad");
    OnInterstitialLoaded?.Invoke();
}

public void tappxInterstitialDismissed()
{
    UnityEngine.Debug.Log("tappxInterstitialDismissed");
    OnInterstitialDismissed?.Invoke();
}

}
