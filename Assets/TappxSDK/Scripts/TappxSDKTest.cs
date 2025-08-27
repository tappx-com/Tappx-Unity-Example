using UnityEngine;
using TappxSDK;

public class TappxSDKTest : MonoBehaviour
{
    private Vector2 scrollPosition;
    private string logText = "";
    private bool testMode = true;
    private string tappxId = "";
    
    // Banner settings
    private TappxSettings.POSITION_BANNER bannerPosition = TappxSettings.POSITION_BANNER.BOTTOM;
    private bool mrecBanner = false;
    
    // Auto-show settings
    private bool interstitialAutoShow = false;
    private bool rewardedAutoShow = false;

    // UI Layout constants
    private const int BUTTON_HEIGHT = 90;
    private const int BUTTON_WIDTH = 400;
    private const int SPACING = 30;
    private const int SECTION_HEIGHT = 50;
    private const int LABEL_HEIGHT = 50;
    private const int TEXT_FIELD_HEIGHT = 50;

    void Start()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        try {
            using (var clazz = new AndroidJavaClass("com.tappx.sdk.android.TappxInterstitial")) {
                Debug.Log("TappxInterstitial Java class found.");
            }
        } catch (System.Exception e) {
            Debug.Log("TappxInterstitial Java class NOT found: " + e.Message);
        }
#endif

        // Load Tappx ID from settings
        LoadTappxIdFromSettings();
        
        AddLog("Tappx SDK Test Script Started");
        AddLog("Tappx ID loaded from settings: " + tappxId);
        
        // Apply configuration automatically
        ApplyConfiguration();
    }

    private void LoadTappxIdFromSettings()
    {
        // Try to load from TappxSettings asset
        TappxSettings settings = TappxSettings.Instance;
        if (settings != null)
        {
            // Use platform-specific Tappx ID
#if UNITY_IOS
            tappxId = settings.iOSTappxID;
            if (string.IsNullOrEmpty(tappxId))
            {
                tappxId = settings.androidTappxID; // Fallback to Android ID
            }
            AddLog("Platform: iOS - Using iOS Tappx ID: " + tappxId);
#elif UNITY_ANDROID
            tappxId = settings.androidTappxID;
            if (string.IsNullOrEmpty(tappxId))
            {
                tappxId = settings.iOSTappxID; // Fallback to iOS ID
            }
            AddLog("Platform: Android - Using Android Tappx ID: " + tappxId);
#else
            // Editor or other platform - try iOS first, then Android
            tappxId = settings.iOSTappxID;
            if (string.IsNullOrEmpty(tappxId))
            {
                tappxId = settings.androidTappxID;
            }
            AddLog("Platform: Editor/Other - Using Tappx ID: " + tappxId);
#endif
            
            // Also set test mode from settings
            testMode = settings.testEnabled;
        }
        
        // Fallback if settings not found
        if (string.IsNullOrEmpty(tappxId))
        {
            tappxId = "No Tappx ID found in settings";
        }
    }

    void OnGUI()
    {
        // Calculate center positions
        float screenCenterX = Screen.width / 2f;
        float screenCenterY = Screen.height / 2f;
        float contentWidth = 800f; // Wider content width for bigger buttons
        float startX = screenCenterX - contentWidth / 2f;

        GUI.Box(new Rect(10, 10, Screen.width - 20, Screen.height - 20), "Tappx SDK Test Interface");
        
        scrollPosition = GUI.BeginScrollView(new Rect(20, 40, Screen.width - 40, Screen.height - 60), scrollPosition, 
            new Rect(0, 0, contentWidth, 700));

        int yPos = 20;

        // Initialization Section
        GUI.Label(new Rect(startX, yPos, contentWidth, SECTION_HEIGHT), "=== INITIALIZATION ===", GUI.skin.box);
        yPos += 50;

        float buttonStartX = startX + (contentWidth - BUTTON_WIDTH) / 2f;
        if (GUI.Button(new Rect(buttonStartX, yPos, BUTTON_WIDTH, BUTTON_HEIGHT), "Initialize SDK"))
        {
            InitializeSDK();
        }
        yPos += BUTTON_HEIGHT + SPACING;

        // Banner Ads Section
        GUI.Label(new Rect(startX, yPos, contentWidth, SECTION_HEIGHT), "=== BANNER ADS ===", GUI.skin.box);
        yPos += 50;

        // Banner Position - centered
        GUI.Label(new Rect(startX, yPos, 120, LABEL_HEIGHT), "Position:");
        if (GUI.Button(new Rect(startX + 150, yPos, 120, LABEL_HEIGHT), bannerPosition == TappxSettings.POSITION_BANNER.TOP ? "TOP" : "BOTTOM"))
        {
            bannerPosition = bannerPosition == TappxSettings.POSITION_BANNER.TOP ? 
                TappxSettings.POSITION_BANNER.BOTTOM : TappxSettings.POSITION_BANNER.TOP;
        }
        yPos += 50;

        // MREC Banner - centered
        float toggleStartX = startX + (contentWidth - 250) / 2f;
        mrecBanner = GUI.Toggle(new Rect(toggleStartX, yPos, 250, LABEL_HEIGHT), mrecBanner, "MREC Banner");
        yPos += 50;

        if (GUI.Button(new Rect(buttonStartX, yPos, BUTTON_WIDTH, BUTTON_HEIGHT), "Load & Show Banner"))
        {
            LoadAndShowBanner();
        }
        yPos += BUTTON_HEIGHT + SPACING;

        if (GUI.Button(new Rect(buttonStartX, yPos, BUTTON_WIDTH, BUTTON_HEIGHT), "Hide Banner"))
        {
            HideBanner();
        }
        yPos += BUTTON_HEIGHT + SPACING;

        if (GUI.Button(new Rect(buttonStartX, yPos, BUTTON_WIDTH, BUTTON_HEIGHT), "Is Banner Visible"))
        {
            CheckBannerVisibility();
        }
        yPos += BUTTON_HEIGHT + SPACING;

        // Interstitial Ads Section
        GUI.Label(new Rect(startX, yPos, contentWidth, SECTION_HEIGHT), "=== INTERSTITIAL ADS ===", GUI.skin.box);
        yPos += 50;

        interstitialAutoShow = GUI.Toggle(new Rect(toggleStartX, yPos, 250, LABEL_HEIGHT), interstitialAutoShow, "Auto Show Interstitial");
        yPos += 50;

        if (GUI.Button(new Rect(buttonStartX, yPos, BUTTON_WIDTH, BUTTON_HEIGHT), "Load Interstitial"))
        {
            LoadInterstitial();
        }
        yPos += BUTTON_HEIGHT + SPACING;

        if (GUI.Button(new Rect(buttonStartX, yPos, BUTTON_WIDTH, BUTTON_HEIGHT), "Show Interstitial"))
        {
            ShowInterstitial();
        }
        yPos += BUTTON_HEIGHT + SPACING;

        if (GUI.Button(new Rect(buttonStartX, yPos, BUTTON_WIDTH, BUTTON_HEIGHT), "Is Interstitial Ready"))
        {
            CheckInterstitialReady();
        }
        yPos += BUTTON_HEIGHT + SPACING;

        // Rewarded Ads Section
        GUI.Label(new Rect(startX, yPos, contentWidth, SECTION_HEIGHT), "=== REWARDED ADS ===", GUI.skin.box);
        yPos += 50;

        rewardedAutoShow = GUI.Toggle(new Rect(toggleStartX, yPos, 250, LABEL_HEIGHT), rewardedAutoShow, "Auto Show Rewarded");
        yPos += 50;

        if (GUI.Button(new Rect(buttonStartX, yPos, BUTTON_WIDTH, BUTTON_HEIGHT), "Load Rewarded"))
        {
            LoadRewarded();
        }
        yPos += BUTTON_HEIGHT + SPACING;

        if (GUI.Button(new Rect(buttonStartX, yPos, BUTTON_WIDTH, BUTTON_HEIGHT), "Show Rewarded"))
        {
            ShowRewarded();
        }
        yPos += BUTTON_HEIGHT + SPACING;

        if (GUI.Button(new Rect(buttonStartX, yPos, BUTTON_WIDTH, BUTTON_HEIGHT), "Is Rewarded Ready"))
        {
            CheckRewardedReady();
        }
        yPos += BUTTON_HEIGHT + SPACING;

        // Utility Section
        GUI.Label(new Rect(startX, yPos, contentWidth, SECTION_HEIGHT), "=== UTILITIES ===", GUI.skin.box);
        yPos += 50;

        if (GUI.Button(new Rect(buttonStartX, yPos, BUTTON_WIDTH, BUTTON_HEIGHT), "Clear Log"))
        {
            ClearLog();
        }
        yPos += BUTTON_HEIGHT + SPACING;

        // Log Display
        GUI.Label(new Rect(startX, yPos, contentWidth, SECTION_HEIGHT), "=== LOG ===", GUI.skin.box);
        yPos += 50;

        GUI.TextArea(new Rect(startX, yPos, contentWidth, 250), logText);

        GUI.EndScrollView();
    }

    private void ApplyConfiguration()
    {
        try
        {
            if (TappxManagerUnity.instance != null)
            {
                TappxManagerUnity.instance.SetTestMode(testMode);
                TappxManagerUnity.instance.SetTappxId(tappxId);
                
                AddLog("Configuration applied automatically");
                AddLog("Test Mode: " + testMode);
                AddLog("Tappx ID: " + tappxId);
            }
            else
            {
                AddLog("ERROR: TappxManagerUnity instance not found");
            }
        }
        catch (System.Exception e)
        {
            AddLog("ERROR applying configuration: " + e.Message);
        }
    }

    private void InitializeSDK()
    {
        try
        {
            AddLog("Initializing Tappx SDK...");
            // The SDK is typically initialized automatically when the TappxManagerUnity prefab is instantiated
            AddLog("SDK initialization completed (check if TappxManagerUnity prefab is in scene)");
        }
        catch (System.Exception e)
        {
            AddLog("ERROR initializing SDK: " + e.Message);
        }
    }

    private void LoadAndShowBanner()
    {
        try
        {
            if (TappxManagerUnity.instance != null)
            {
                AddLog("Loading and showing banner at position: " + bannerPosition + ", MREC: " + mrecBanner);
                TappxManagerUnity.instance.show(bannerPosition, mrecBanner);
            }
            else
            {
                AddLog("ERROR: TappxManagerUnity instance not found");
            }
        }
        catch (System.Exception e)
        {
            AddLog("ERROR loading banner: " + e.Message);
        }
    }

    private void HideBanner()
    {
        try
        {
            if (TappxManagerUnity.instance != null)
            {
                AddLog("Hiding banner");
                TappxManagerUnity.instance.hide();
            }
            else
            {
                AddLog("ERROR: TappxManagerUnity instance not found");
            }
        }
        catch (System.Exception e)
        {
            AddLog("ERROR hiding banner: " + e.Message);
        }
    }

    private void CheckBannerVisibility()
    {
        try
        {
            if (TappxManagerUnity.instance != null)
            {
                bool isVisible = TappxManagerUnity.instance.isBannerVisible();
                AddLog("Banner visibility: " + isVisible);
            }
            else
            {
                AddLog("ERROR: TappxManagerUnity instance not found");
            }
        }
        catch (System.Exception e)
        {
            AddLog("ERROR checking banner visibility: " + e.Message);
        }
    }

    private void LoadInterstitial()
    {
        try
        {
            if (TappxManagerUnity.instance != null)
            {
                AddLog("Loading interstitial (AutoShow: " + interstitialAutoShow + ")");
                TappxManagerUnity.instance.loadInterstitial(interstitialAutoShow);
            }
            else
            {
                AddLog("ERROR: TappxManagerUnity instance not found");
            }
        }
        catch (System.Exception e)
        {
            AddLog("ERROR loading interstitial: " + e.Message);
        }
    }

    private void ShowInterstitial()
    {
        try
        {
            if (TappxManagerUnity.instance != null)
            {
                AddLog("Showing interstitial");
                TappxManagerUnity.instance.interstitialShow();
            }
            else
            {
                AddLog("ERROR: TappxManagerUnity instance not found");
            }
        }
        catch (System.Exception e)
        {
            AddLog("ERROR showing interstitial: " + e.Message);
        }
    }

    private void CheckInterstitialReady()
    {
        try
        {
            if (TappxManagerUnity.instance != null)
            {
                bool isReady = TappxManagerUnity.instance.isInterstitialReady();
                AddLog("Interstitial ready: " + isReady);
            }
            else
            {
                AddLog("ERROR: TappxManagerUnity instance not found");
            }
        }
        catch (System.Exception e)
        {
            AddLog("ERROR checking interstitial ready: " + e.Message);
        }
    }

    private void LoadRewarded()
    {
        try
        {
            if (TappxManagerUnity.instance != null)
            {
                AddLog("Loading rewarded ad (AutoShow: " + rewardedAutoShow + ")");
                TappxManagerUnity.instance.loadRewarded();
            }
            else
            {
                AddLog("ERROR: TappxManagerUnity instance not found");
            }
        }
        catch (System.Exception e)
        {
            AddLog("ERROR loading rewarded: " + e.Message);
        }
    }

    private void ShowRewarded()
    {
        try
        {
            if (TappxManagerUnity.instance != null)
            {
                AddLog("Showing rewarded ad");
                TappxManagerUnity.instance.rewardedShow();
            }
            else
            {
                AddLog("ERROR: TappxManagerUnity instance not found");
            }
        }
        catch (System.Exception e)
        {
            AddLog("ERROR showing rewarded: " + e.Message);
        }
    }

    private void CheckRewardedReady()
    {
        try
        {
            if (TappxManagerUnity.instance != null)
            {
                bool isReady = TappxManagerUnity.instance.isRewardedReady();
                AddLog("Rewarded ready: " + isReady);
            }
            else
            {
                AddLog("ERROR: TappxManagerUnity instance not found");
            }
        }
        catch (System.Exception e)
        {
            AddLog("ERROR checking rewarded ready: " + e.Message);
        }
    }

    private void AddLog(string message)
    {
        string timestamp = System.DateTime.Now.ToString("HH:mm:ss");
        logText = "[" + timestamp + "] " + message + "\n" + logText;
        
        // Limit log length to prevent memory issues
        if (logText.Length > 5000)
        {
            logText = logText.Substring(0, 4000);
        }
        
        Debug.Log("[TappxSDKTest] " + message);
    }

    private void ClearLog()
    {
        logText = "";
        AddLog("Log cleared");
    }
} 