using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script to display the Tappx SDK version in a UI Text component
/// </summary>
public class VersionDisplay : MonoBehaviour
{
    [Header("Version Display Settings")]
    [SerializeField] private Text versionText;
    [SerializeField] private string versionPrefix = "SDK Version: ";
    [SerializeField] private bool updateOnStart = true;
    
    private void Start()
    {
        if (updateOnStart)
        {
            UpdateVersionDisplay();
        }
    }
    
    /// <summary>
    /// Updates the version text with the current SDK version
    /// </summary>
    public void UpdateVersionDisplay()
    {
        if (versionText == null)
        {
            // Try to get the Text component from this GameObject if not assigned
            versionText = GetComponent<Text>();
        }
        
        if (versionText != null)
        {
            string sdkVersion = TappxManagerUnity.GetVersion();
            versionText.text = versionPrefix + sdkVersion;
            Debug.Log($"[VersionDisplay] Updated version display to: {versionText.text}");
        }
        else
        {
            Debug.LogError("[VersionDisplay] No Text component found to display version!");
        }
    }
    
    /// <summary>
    /// Manually refresh the version display
    /// </summary>
    [ContextMenu("Refresh Version")]
    public void RefreshVersion()
    {
        UpdateVersionDisplay();
    }
}
