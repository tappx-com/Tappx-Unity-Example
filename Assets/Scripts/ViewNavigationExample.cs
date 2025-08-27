using UnityEngine;
using UnityEngine.UI;

public class ViewNavigationExample : MonoBehaviour
{
    [Header("View Manager Reference")]
    [SerializeField] private ViewManager viewManager;
    
    [Header("Example Buttons")]
    [SerializeField] private Button goToHomeButton;
    [SerializeField] private Button goToSettingsButton;
    [SerializeField] private Button goToProfileButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Button rootButton;
    
    private void Start()
    {
        SetupButtons();
        
        // Subscribe to view manager events
        if (viewManager != null)
        {
            viewManager.OnViewChanged += OnViewChanged;
            viewManager.OnViewPushed += OnViewPushed;
            viewManager.OnViewPopped += OnViewPopped;
        }
    }
    
    private void SetupButtons()
    {
        if (goToHomeButton != null)
        {
            goToHomeButton.onClick.AddListener(() => {
                if (viewManager != null)
                {
                    viewManager.ShowView("HomeView");
                }
            });
        }
        
        if (goToSettingsButton != null)
        {
            goToSettingsButton.onClick.AddListener(() => {
                if (viewManager != null)
                {
                    viewManager.ShowView("SettingsView");
                }
            });
        }
        
        if (goToProfileButton != null)
        {
            goToProfileButton.onClick.AddListener(() => {
                if (viewManager != null)
                {
                    viewManager.ShowView("ProfileView");
                }
            });
        }
        
        if (backButton != null)
        {
            backButton.onClick.AddListener(() => {
                if (viewManager != null)
                {
                    viewManager.GoBack();
                }
            });
        }
        
        if (rootButton != null)
        {
            rootButton.onClick.AddListener(() => {
                if (viewManager != null)
                {
                    viewManager.GoToRoot();
                }
            });
        }
    }
    
    private void OnViewChanged(Canvas newView)
    {
        Debug.Log($"View changed to: {newView.name}");
        
        // Update UI based on current view
        UpdateUIForCurrentView(newView);
    }
    
    private void OnViewPushed(Canvas pushedView)
    {
        Debug.Log($"View pushed: {pushedView.name}");
    }
    
    private void OnViewPopped(Canvas poppedView)
    {
        Debug.Log($"View popped: {poppedView.name}");
    }
    
    private void UpdateUIForCurrentView(Canvas currentView)
    {
        // Example: Update button visibility based on current view
        if (backButton != null)
        {
            backButton.gameObject.SetActive(viewManager.CanGoBack());
        }
        
        if (rootButton != null)
        {
            rootButton.gameObject.SetActive(viewManager.GetHistoryCount() > 0);
        }
    }
    
    /// <summary>
    /// Example method to programmatically navigate
    /// </summary>
    public void NavigateToView(string viewName)
    {
        if (viewManager != null)
        {
            viewManager.ShowView(viewName);
        }
    }
    
    /// <summary>
    /// Example method to navigate by index
    /// </summary>
    public void NavigateToView(int viewIndex)
    {
        if (viewManager != null)
        {
            viewManager.ShowView(viewIndex);
        }
    }
    
    /// <summary>
    /// Example method to go back
    /// </summary>
    public void GoBack()
    {
        if (viewManager != null)
        {
            viewManager.GoBack();
        }
    }
    
    /// <summary>
    /// Example method to go to root
    /// </summary>
    public void GoToRoot()
    {
        if (viewManager != null)
        {
            viewManager.GoToRoot();
        }
    }
    
    private void OnDestroy()
    {
        if (viewManager != null)
        {
            viewManager.OnViewChanged -= OnViewChanged;
            viewManager.OnViewPushed -= OnViewPushed;
            viewManager.OnViewPopped -= OnViewPopped;
        }
        
        // Clean up button listeners
        if (goToHomeButton != null)
        {
            goToHomeButton.onClick.RemoveAllListeners();
        }
        
        if (goToSettingsButton != null)
        {
            goToSettingsButton.onClick.RemoveAllListeners();
        }
        
        if (goToProfileButton != null)
        {
            goToProfileButton.onClick.RemoveAllListeners();
        }
        
        if (backButton != null)
        {
            backButton.onClick.RemoveAllListeners();
        }
        
        if (rootButton != null)
        {
            rootButton.onClick.RemoveAllListeners();
        }
    }
} 