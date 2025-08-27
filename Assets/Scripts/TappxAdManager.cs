using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using TappxSDK;
using System.Xml.Serialization;

public class TappxAdManager : MonoBehaviour
{
    [Header("Tappx Configuration")]
    [SerializeField] private string tappxAppIdIOS = "";
    [SerializeField] private string tappxAppIdAndroid = "";
    [SerializeField] private bool testMode = true;
    [SerializeField] private TappxSettings.POSITION_BANNER bannerPosition = TappxSettings.POSITION_BANNER.BOTTOM;
    [SerializeField] private bool useMREC = false;
    [SerializeField] private bool showMRECInCenter = true;
    
    [Header("Auto Load Settings")]
    [SerializeField] private bool autoLoadBanner = true;
    [SerializeField] private bool autoLoadInterstitial = true;
    [SerializeField] private bool autoLoadRewarded = true;
    
    [Header("Auto Show Settings")]
    [SerializeField] private bool interstitialAutoShow = false;
    [SerializeField] private bool rewardedAutoShow = false;
    
    [Header("View Integration")]
    [SerializeField] private ViewManager viewManager;
    
    [Header("Status Display")]
    [SerializeField] private List<Text> statusTexts = new List<Text>();
    [SerializeField] private int maxStatusMessages = 10;
    [SerializeField] private bool showTimestamps = true;
    
    // Ad states
    private bool isBannerLoaded = false;
    private bool isInterstitialLoaded = false;
    private bool isRewardedLoaded = false;
    
    // Status tracking
    private List<string> statusMessages = new List<string>();
    
    // Events
    public Action OnBannerLoaded;
    public Action OnBannerFailed;
    public Action OnInterstitialLoaded;
    public Action OnInterstitialFailed;
    public Action OnInterstitialClosed;
    public Action OnRewardedLoaded;
    public Action OnRewardedFailed;
    public Action OnRewardedCompleted;
    public Action OnRewardedClosed;
    
    private void Start()
    {
        InitializeTappx();
        SetupViewManager();
        SetupTappxCallbacks();
        AddStatusMessage("TappxAdManager initialized");
    }
    
    private void InitializeTappx()
    {
        // Initialize Tappx SDK
        if (TappxManagerUnity.instance != null)
        {
            // Set Tappx App ID based on platform (required)
            string appId = GetPlatformAppId();
            if (!string.IsNullOrEmpty(appId))
            {
                TappxManagerUnity.instance.SetTappxId(appId);
                AddStatusMessage($"🔧 Tappx initialized with App ID: {appId} ({GetPlatformName()})");
            }
            else
            {
                AddStatusMessage($"⚠️ Tappx App ID not set for {GetPlatformName()}! Please set it in the inspector.");
            }
            
            // Set test mode
            TappxManagerUnity.instance.SetTestMode(testMode);
            AddStatusMessage($"🧪 Test mode: {testMode}");
            
            // Set privacy settings (optional)
            TappxManagerUnity.instance.AcceptGeolocate(true);
            TappxManagerUnity.instance.AcceptCoppa(true);
            AddStatusMessage("🔒 Privacy settings applied");
        }
        else
        {
            AddStatusMessage("❌ TappxManagerUnity instance not found!");
            return;
        }
        
        // Auto-load ads if enabled
        if (autoLoadBanner)
        {
            LoadBanner();
        }
        
        if (autoLoadInterstitial)
        {
            LoadInterstitial();
        }
        
        if (autoLoadRewarded)
        {
            LoadRewarded();
        }
    }
    
    private void SetupViewManager()
    {
        if (viewManager == null)
        {
            viewManager = FindObjectOfType<ViewManager>();
        }
        
        if (viewManager != null)
        {
            // Subscribe to view changes to show ads at appropriate times
            viewManager.OnViewChanged += OnViewChanged;
        }
    }
    
    private void SetupTappxCallbacks()
    {
        // Subscribe to Tappx SDK events
        TappxManagerUnity.OnBannerLoaded += OnBannerLoadSuccess;
        TappxManagerUnity.OnBannerFailedToLoad += OnBannerLoadFailure;
        TappxManagerUnity.OnInterstitialLoaded += OnInterstitialLoadSuccess;
        TappxManagerUnity.OnInterstitialFailedToLoad += OnInterstitialLoadFailure;
        TappxManagerUnity.OnInterstitialClosed += OnInterstitialClosedCallback;
        TappxManagerUnity.OnRewardedLoaded += OnRewardedLoadSuccess;
        TappxManagerUnity.OnRewardedFailedToLoad += OnRewardedLoadFailure;
        TappxManagerUnity.OnRewardedVideoCompleted += OnRewardedCompletedCallback;
        TappxManagerUnity.OnRewardedVideoClosed += OnRewardedClosedCallback;
        
        AddStatusMessage("🔗 TappxAdManager ready - subscribed to SDK events");
    }
    
        private void OnViewChanged(Canvas newView)
    {
        // Auto-show ads based on view name
        switch (newView.name)
        {
            case "MainMenu":
                HideBanner();
                AddStatusMessage("🏠 MainMenu - Hiding ads");
                break;
            case "BannerView":
                ShowBanner();
                break;
            case "MRECView":
                ShowMREC();
                break;
        }
        
        AddStatusMessage($"📱 Switched to view: {newView.name}");
    }
    
    #region Banner Ads
    
    /// <summary>
    /// Load and show banner ad
    /// </summary>
    public void LoadBanner()
    {
        Debug.Log("Loading banner ad...");
        AddStatusMessage("🔄 Loading banner ad...");
        if (TappxManagerUnity.instance != null)
        {
            TappxManagerUnity.instance.show(bannerPosition, false); // false = normal banner, not MREC
            isBannerLoaded = true;
            OnBannerLoaded?.Invoke();
        }
        else
        {
            Debug.LogError("TappxManagerUnity instance not found!");
            OnBannerLoadFailure("TappxManagerUnity instance not found");
        }
    }
    
    /// <summary>
    /// Load and show MREC ad
    /// </summary>
    public void LoadMREC()
    {
        Debug.Log("Loading MREC ad...");
        AddStatusMessage("🔄 Loading MREC ad...");
        if (TappxManagerUnity.instance != null)
        {
            // MREC ads positioned at bottom
            TappxSettings.POSITION_BANNER mrecPosition = TappxSettings.POSITION_BANNER.BOTTOM;
            
            TappxManagerUnity.instance.show(mrecPosition, true); // true = MREC
            isBannerLoaded = true;
            OnBannerLoaded?.Invoke();
        }
        else
        {
            Debug.LogError("TappxManagerUnity instance not found!");
            OnBannerLoadFailure("TappxManagerUnity instance not found");
        }
    }
    
    /// <summary>
    /// Show banner ad
    /// </summary>
    public void ShowBanner()
    {
        Debug.Log("Showing banner ad...");
        AddStatusMessage("📺 Showing banner ad...");
        if (TappxManagerUnity.instance != null)
        {
            Debug.Log($"Calling TappxManagerUnity.show with position: {bannerPosition}, mrec: false");
            AddStatusMessage($"🔧 Calling SDK: show({bannerPosition}, false)");
            
            TappxManagerUnity.instance.show(bannerPosition, false); // false = normal banner, not MREC
            isBannerLoaded = true;
            
            AddStatusMessage("✅ Banner show command sent to SDK");
        }
        else
        {
            Debug.LogError("TappxManagerUnity instance not found!");
            AddStatusMessage("❌ TappxManagerUnity instance not found!");
        }
    }
    
    /// <summary>
    /// Hide banner ad
    /// </summary>
    public void HideBanner()
    {
        Debug.Log("Hiding banner ad...");
        AddStatusMessage("👻 Hiding banner ad...");
        if (TappxManagerUnity.instance != null)
        {
            TappxManagerUnity.instance.hide();
            isBannerLoaded = false;
        }
    }
    
    /// <summary>
    /// Show MREC ad
    /// </summary>
    public void ShowMREC()
    {
        Debug.Log("Showing MREC ad...");
        AddStatusMessage("📦 Showing MREC ad...");
        if (TappxManagerUnity.instance != null)
        {
            // MREC ads positioned at bottom
            TappxSettings.POSITION_BANNER mrecPosition = TappxSettings.POSITION_BANNER.BOTTOM;
            
            Debug.Log($"Calling TappxManagerUnity.show with position: {mrecPosition}, mrec: true");
            AddStatusMessage($"🔧 Calling SDK: show({mrecPosition}, true)");
            
            TappxManagerUnity.instance.show(mrecPosition, true); // true = MREC
            isBannerLoaded = true;
            
            AddStatusMessage("✅ MREC show command sent to SDK");
        }
        else
        {
            Debug.LogError("TappxManagerUnity instance not found!");
            AddStatusMessage("❌ TappxManagerUnity instance not found!");
        }
    }
    
    private void OnBannerLoadSuccess()
    {
        isBannerLoaded = true;
        Debug.Log("Banner loaded successfully!");
        AddStatusMessage("✅ Banner loaded successfully!");
        OnBannerLoaded?.Invoke();
    }
    
    private void OnBannerLoadFailure(string error)
    {
        isBannerLoaded = false;
        Debug.LogError($"Banner load failed: {error}");
        AddStatusMessage($"❌ Banner load failed: {error}");
        OnBannerFailed?.Invoke();
    }
    
    #endregion
    
    #region Interstitial Ads
    
    /// <summary>
    /// Load interstitial ad with current auto-show setting
    /// </summary>
    public void LoadInterstitial()
    {
        LoadInterstitialWithAutoShow(interstitialAutoShow);
    }
    
    /// <summary>
    /// Load interstitial ad with specified auto-show setting
    /// </summary>
    /// <param name="autoShow">Whether to auto-show the ad when loaded</param>
    public void LoadInterstitialWithAutoShow(bool autoShow)
    {
        Debug.Log($"Loading interstitial ad (AutoShow: {autoShow})...");
        AddStatusMessage($"🔄 Loading interstitial ad (AutoShow: {autoShow})...");
        if (TappxManagerUnity.instance != null)
        {
            TappxManagerUnity.instance.loadInterstitial(autoShow);
            // Don't set loaded immediately - wait for callback
        }
        else
        {
            Debug.LogError("TappxManagerUnity instance not found!");
            AddStatusMessage("❌ TappxManagerUnity instance not found!");
            OnInterstitialLoadFailure("TappxManagerUnity instance not found");
        }
    }
    
 
    
    private void OnInterstitialLoadSuccess()
    {
        isInterstitialLoaded = true;
        Debug.Log("Interstitial loaded successfully!");
        AddStatusMessage("✅ Interstitial loaded successfully!");
        OnInterstitialLoaded?.Invoke();
    }
    
    private void OnInterstitialLoadFailure(string error)
    {
        isInterstitialLoaded = false;
        Debug.LogError($"Interstitial load failed: {error}");
        AddStatusMessage($"❌ Interstitial load failed: {error}");
        OnInterstitialFailed?.Invoke();
    }
    
    private void OnInterstitialClosedCallback()
    {
        Debug.Log("Interstitial closed");
        AddStatusMessage("📱 Interstitial ad closed");
        OnInterstitialClosed?.Invoke();
    }


    public void IsInterstitialReady() 
    {
        if (TappxManagerUnity.instance != null)
        {
            if (TappxManagerUnity.instance.isInterstitialReady())
            {
                AddStatusMessage("📱 Interstitial ad ready");
            }
            else
            {
                AddStatusMessage("❌ Interstitial ad not ready");
            }
        }
    }


        
        #endregion
        
        #region Rewarded Ads
        
        /// <summary>
    /// Load rewarded ad with current auto-show setting
    /// </summary>
    public void LoadRewarded()
    {
        LoadRewardedWithAutoShow(rewardedAutoShow);
    }
    
    /// <summary>
    /// Load rewarded ad with specified auto-show setting
    /// </summary>
    /// <param name="autoShow">Whether to auto-show the ad when loaded</param>
    public void LoadRewardedWithAutoShow(bool autoShow)
    {
        Debug.Log($"Loading rewarded ad (AutoShow: {autoShow})...");
        AddStatusMessage($"🔄 Loading rewarded ad (AutoShow: {autoShow})...");
        if (TappxManagerUnity.instance != null)
        {
            TappxManagerUnity.instance.loadRewarded(autoShow);
            // Don't set loaded immediately - wait for callback
        }
        else
        {
            Debug.LogError("TappxManagerUnity instance not found!");
            AddStatusMessage("❌ TappxManagerUnity instance not found!");
            OnRewardedLoadFailure("TappxManagerUnity instance not found");
        }
    }
    

    
    private void OnRewardedLoadSuccess()
    {
        isRewardedLoaded = true;
        Debug.Log("Rewarded ad loaded successfully!");
        AddStatusMessage("✅ Rewarded ad loaded successfully!");
        OnRewardedLoaded?.Invoke();
    }
    
    private void OnRewardedLoadFailure(string error)
    {
        isRewardedLoaded = false;
        Debug.LogError($"Rewarded ad load failed: {error}");
        AddStatusMessage($"❌ Rewarded ad load failed: {error}");
        OnRewardedFailed?.Invoke();
    }
    
    private void OnRewardedCompletedCallback()
    {
        Debug.Log("Rewarded ad completed - give reward!");
        AddStatusMessage("🎁 Rewarded ad completed - give reward!");
        OnRewardedCompleted?.Invoke();
    }
    
    private void OnRewardedClosedCallback()
    {
        Debug.Log("Rewarded ad closed");
        AddStatusMessage("📱 Rewarded ad closed");
        OnRewardedClosed?.Invoke();
    }

    public void IsRewardedReady()
    {
        if (TappxManagerUnity.instance != null)
        {
            if (TappxManagerUnity.instance.isRewardedReady())
            {
                AddStatusMessage("📱 Rewarded ad ready");
            }
            else
            {
                AddStatusMessage("❌ Rewarded ad not ready");
            }
        }
    }
    
    #endregion
    
    #region Tappx SDK Callbacks
    
    /// <summary>
    /// Called by Tappx SDK when interstitial is loaded
    /// </summary>
    public void TappxInterstitialLoaded()
    {
        isInterstitialLoaded = true;
        OnInterstitialLoadSuccess();
    }
    
    /// <summary>
    /// Called by Tappx SDK when interstitial fails to load
    /// </summary>
    public void TappxInterstitialFailed(string error)
    {
        isInterstitialLoaded = false;
        OnInterstitialLoadFailure(error);
    }
    
    /// <summary>
    /// Called by Tappx SDK when rewarded ad is loaded
    /// </summary>
    public void TappxRewardedLoaded()
    {
        isRewardedLoaded = true;
        OnRewardedLoadSuccess();
    }
    
    /// <summary>
    /// Called by Tappx SDK when rewarded ad fails to load
    /// </summary>
    public void TappxRewardedFailed(string error)
    {
        isRewardedLoaded = false;
        OnRewardedLoadFailure(error);
    }
    
    /// <summary>
    /// Called by Tappx SDK when rewarded ad is completed
    /// </summary>
    public void TappxRewardedCompleted()
    {
        OnRewardedCompletedCallback();
    }
    
    /// <summary>
    /// Called by Tappx SDK when rewarded ad is closed
    /// </summary>
    public void TappxRewardedClosed()
    {
        OnRewardedClosedCallback();
    }
    
    /// <summary>
    /// Called by Tappx SDK when interstitial is closed
    /// </summary>
    public void TappxInterstitialClosed()
    {
        OnInterstitialClosedCallback();
    }
    
    #endregion
    
    #region Additional Tappx Methods
    
    /// <summary>
    /// Set US Privacy string
    /// </summary>
    public void SetUSPrivacy(string privacyString)
    {
        if (TappxManagerUnity.instance != null)
        {
            TappxManagerUnity.instance.setUSPrivacy(privacyString);
        }
    }
    
    /// <summary>
    /// Accept geolocation
    /// </summary>
    public void AcceptGeolocation(bool accept)
    {
        if (TappxManagerUnity.instance != null)
        {
            TappxManagerUnity.instance.AcceptGeolocate(accept);
        }
    }
    
    /// <summary>
    /// Accept COPPA compliance
    /// </summary>
    public void AcceptCOPPA(bool accept)
    {
        if (TappxManagerUnity.instance != null)
        {
            TappxManagerUnity.instance.AcceptCoppa(accept);
        }
    }
    
    #endregion
    
    #region Public Methods for UI Buttons
    
    /// <summary>
    /// Load all ads - Use this for Unity Inspector events
    /// </summary>
    public void LoadAllAds()
    {
        LoadBanner();
        LoadInterstitial();
        LoadRewarded();
    }
    
    /// <summary>
    /// Toggle interstitial auto-show setting - Use this for Unity Inspector events
    /// </summary>
    public void ToggleInterstitialAutoShow()
    {

        interstitialAutoShow = !interstitialAutoShow;
        AddStatusMessage($"🔄 Interstitial AutoShow: {interstitialAutoShow}");
    }
    
    /// <summary>
    /// Set interstitial auto-show setting - Use this for Unity Inspector events
    /// </summary>
    public void SetInterstitialAutoShow(bool autoShow)
    {
        interstitialAutoShow = autoShow;
        AddStatusMessage($"🔄 Interstitial AutoShow set to: {interstitialAutoShow}");
    }
    
    /// <summary>
    /// Toggle rewarded auto-show setting - Use this for Unity Inspector events
    /// </summary>
    public void ToggleRewardedAutoShow()
    {
        rewardedAutoShow = !rewardedAutoShow;
        AddStatusMessage($"🔄 Rewarded AutoShow: {rewardedAutoShow}");
    }
    
    /// <summary>
    /// Set rewarded auto-show setting - Use this for Unity Inspector events
    /// </summary>
    public void SetRewardedAutoShow(bool autoShow)
    {
        rewardedAutoShow = autoShow;
        AddStatusMessage($"🔄 Rewarded AutoShow set to: {rewardedAutoShow}");
    }
    
    /// <summary>
    /// Manually trigger interstitial loaded success - Use this for testing
    /// </summary>
    public void TriggerInterstitialLoaded()
    {
        TappxInterstitialLoaded();
    }
    
    /// <summary>
    /// Manually trigger interstitial failed - Use this for testing
    /// </summary>
    public void TriggerInterstitialFailed()
    {
        TappxInterstitialFailed("Manual trigger - test error");
    }
    
    /// <summary>
    /// Manually trigger rewarded loaded success - Use this for testing
    /// </summary>
    public void TriggerRewardedLoaded()
    {
        TappxRewardedLoaded();
    }
    
    /// <summary>
    /// Manually trigger rewarded failed - Use this for testing
    /// </summary>
    public void TriggerRewardedFailed()
    {
        TappxRewardedFailed("Manual trigger - test error");
    }
    
    /// <summary>
    /// Manually trigger rewarded completed - Use this for testing
    /// </summary>
    public void TriggerRewardedCompleted()
    {
        TappxRewardedCompleted();
    }
    
    /// <summary>
    /// Manually trigger interstitial closed - Use this for testing
    /// </summary>
    public void TriggerInterstitialClosed()
    {
        TappxInterstitialClosed();
    }
    
    /// <summary>
    /// Show banner ad - Use this for Unity Inspector events
    /// </summary>
    public void ShowBannerAd()
    {
        ShowBanner();
    }
    
    /// <summary>
    /// Show MREC ad - Use this for Unity Inspector events
    /// </summary>
    public void ShowMRECAd()
    {
        ShowMREC();
    }
    
    /// <summary>
    /// Force MREC ad with explicit parameters - Use this for testing
    /// </summary>
    public void ForceMREC()
    {
        Debug.Log("Force MREC ad...");
        AddStatusMessage("🔧 Force MREC ad...");
        if (TappxManagerUnity.instance != null)
        {
            // Force MREC with explicit parameters
            int position = 1; // BOTTOM
            bool mrec = true;
            
            Debug.Log($"Force calling loadBannerIOS_ with position: {position}, mrec: {mrec}");
            AddStatusMessage($"🔧 Force SDK call: loadBannerIOS_({position}, {mrec})");
            
            // Call the iOS native method directly
#if UNITY_IOS
            // This would require the method to be public, but let's try the show method
            TappxManagerUnity.instance.show(TappxSettings.POSITION_BANNER.BOTTOM, true);
#else
            TappxManagerUnity.instance.show(TappxSettings.POSITION_BANNER.BOTTOM, true);
#endif
            
            AddStatusMessage("✅ Force MREC command sent");
        }
        else
        {
            Debug.LogError("TappxManagerUnity instance not found!");
            AddStatusMessage("❌ TappxManagerUnity instance not found!");
        }
    }
    
    /// <summary>
    /// Debug current banner state - Use this for testing
    /// </summary>
    public void DebugBannerState()
    {
        Debug.Log("=== BANNER DEBUG INFO ===");
        AddStatusMessage("🔍 === BANNER DEBUG INFO ===");
        
        if (TappxManagerUnity.instance != null)
        {
            bool isVisible = TappxManagerUnity.instance.isBannerVisible();
            Debug.Log($"Banner visible: {isVisible}");
            AddStatusMessage($"👁️ Banner visible: {isVisible}");
            
            Debug.Log($"Current banner position: {bannerPosition}");
            AddStatusMessage($"📍 Current banner position: {bannerPosition}");
            
            Debug.Log($"useMREC setting: {useMREC}");
            AddStatusMessage($"🔧 useMREC setting: {useMREC}");
            
            Debug.Log($"showMRECInCenter setting: {showMRECInCenter}");
            AddStatusMessage($"🎯 showMRECInCenter setting: {showMRECInCenter}");
        }
        else
        {
            Debug.LogError("TappxManagerUnity instance not found!");
            AddStatusMessage("❌ TappxManagerUnity instance not found!");
        }
        
        AddStatusMessage("🔍 === END DEBUG INFO ===");
    }
    
    /// <summary>
    /// Show interstitial ad
    /// </summary>
    public void ShowInterstitial()
    {
        Debug.Log("Showing interstitial ad...");
        AddStatusMessage("📱 Showing interstitial ad...");
        if (TappxManagerUnity.instance != null)
        {
            if (TappxManagerUnity.instance.isInterstitialReady())
            {
                TappxManagerUnity.instance.interstitialShow();
                isInterstitialLoaded = false; // Reset after showing
                AddStatusMessage("✅ Interstitial ad shown successfully!");
                
                // Auto-reload after showing
                LoadInterstitial();
            }
            else
            {
                Debug.LogWarning("Interstitial not ready. Loading now...");
                AddStatusMessage("⚠️ Interstitial not ready. Loading now...");
                LoadInterstitial();
            }
        }
        else
        {
            Debug.LogError("TappxManagerUnity instance not found!");
            AddStatusMessage("❌ TappxManagerUnity instance not found!");
        }
    }
    
    /// <summary>
    /// Show rewarded ad
    /// </summary>
    public void ShowRewarded()
    {
        Debug.Log("Showing rewarded ad...");
        AddStatusMessage("🎁 Showing rewarded ad...");
        if (TappxManagerUnity.instance != null)
        {
            if (TappxManagerUnity.instance.isRewardedReady())
            {
                TappxManagerUnity.instance.rewardedShow();
                isRewardedLoaded = false; // Reset after showing
                AddStatusMessage("✅ Rewarded ad shown successfully!");
                
                // Auto-reload after showing
                LoadRewarded();
            }
            else
            {
                Debug.LogWarning("Rewarded ad not ready. Loading now...");
                AddStatusMessage("⚠️ Rewarded ad not ready. Loading now...");
                LoadRewarded();
            }
        }
        else
        {
            Debug.LogError("TappxManagerUnity instance not found!");
            AddStatusMessage("❌ TappxManagerUnity instance not found!");
        }
    }
    
    /// <summary>
    /// Show interstitial ad - Use this for Unity Inspector events
    /// </summary>
    public void ShowInterstitialAd()
    {
        ShowInterstitial();
    }
    
    /// <summary>
    /// Show rewarded ad - Use this for Unity Inspector events
    /// </summary>
    public void ShowRewardedAd()
    {
        ShowRewarded();
    }
    
    #endregion
    
    #region Platform Helper Methods
    
    /// <summary>
    /// Get the appropriate App ID for the current platform
    /// </summary>
    private string GetPlatformAppId()
    {
#if UNITY_IOS
        return tappxAppIdIOS;
#elif UNITY_ANDROID
        return tappxAppIdAndroid;
#else
        return tappxAppIdIOS; // Default to iOS for editor
#endif
    }
    
    /// <summary>
    /// Get the current platform name
    /// </summary>
    private string GetPlatformName()
    {
#if UNITY_IOS
        return "iOS";
#elif UNITY_ANDROID
        return "Android";
#else
        return "Editor";
#endif
    }
    
    #endregion
    
    #region Status Methods
    
    /// <summary>
    /// Check if banner is loaded
    /// </summary>
    public bool IsBannerLoaded()
    {
        return isBannerLoaded;
    }
    
    /// <summary>
    /// Check if banner is visible
    /// </summary>
    public bool IsBannerVisible()
    {
        if (TappxManagerUnity.instance != null)
        {
            return TappxManagerUnity.instance.isBannerVisible();
        }
        return false;
    }
    
    /// <summary>
    /// Check if interstitial is loaded
    /// </summary>
    public bool IsInterstitialLoaded()
    {
        if (TappxManagerUnity.instance != null)
        {
            return TappxManagerUnity.instance.isInterstitialReady();
        }
        return isInterstitialLoaded;
    }
    
    /// <summary>
    /// Check if rewarded ad is loaded
    /// </summary>
    public bool IsRewardedLoaded()
    {
        if (TappxManagerUnity.instance != null)
        {
            return TappxManagerUnity.instance.isRewardedReady();
        }
        return isRewardedLoaded;
    }
    
    #endregion
    
    #region Status Display Methods
    
    /// <summary>
    /// Add a status message to all status texts
    /// </summary>
    /// <param name="message">Message to display</param>
    public void AddStatusMessage(string message)
    {
        string timestamp = showTimestamps ? $"[{System.DateTime.Now:HH:mm:ss}] " : "";
        string fullMessage = $"{timestamp}{message}";
        
        statusMessages.Add(fullMessage);
        
        // Keep only the last maxStatusMessages
        if (statusMessages.Count > maxStatusMessages)
        {
            statusMessages.RemoveAt(0);
        }
        
        // Update all status text components
        UpdateStatusTexts();
    }
    
    /// <summary>
    /// Clear all status messages
    /// </summary>
    public void ClearStatusMessages()
    {
        statusMessages.Clear();
        UpdateStatusTexts();
    }
    
    /// <summary>
    /// Update all status text components with current messages
    /// </summary>
    private void UpdateStatusTexts()
    {
        string combinedMessage = string.Join("\n", statusMessages);
        
        foreach (var statusText in statusTexts)
        {
            if (statusText != null)
            {
                statusText.text = combinedMessage;
            }
        }
    }
    
    #endregion
    
    private void OnDestroy()
    {
        if (viewManager != null)
        {
            viewManager.OnViewChanged -= OnViewChanged;
        }
        
        // Unsubscribe from Tappx SDK events
        TappxManagerUnity.OnBannerLoaded -= OnBannerLoadSuccess;
        TappxManagerUnity.OnBannerFailedToLoad -= OnBannerLoadFailure;
        TappxManagerUnity.OnInterstitialLoaded -= OnInterstitialLoadSuccess;
        TappxManagerUnity.OnInterstitialFailedToLoad -= OnInterstitialLoadFailure;
        TappxManagerUnity.OnInterstitialClosed -= OnInterstitialClosedCallback;
        TappxManagerUnity.OnRewardedLoaded -= OnRewardedLoadSuccess;
        TappxManagerUnity.OnRewardedFailedToLoad -= OnRewardedLoadFailure;
        TappxManagerUnity.OnRewardedVideoCompleted -= OnRewardedCompletedCallback;
        TappxManagerUnity.OnRewardedVideoClosed -= OnRewardedClosedCallback;
    }
} 