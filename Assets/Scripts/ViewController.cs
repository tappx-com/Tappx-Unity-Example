using UnityEngine;
using UnityEngine.UI;

public class ViewController : MonoBehaviour
{
    [Header("View Settings")]
    [SerializeField] private string viewName;
    [SerializeField] private bool isRootView = false;
    
    [Header("Navigation")]
    [SerializeField] private Button[] navigationButtons;
    [SerializeField] private string[] targetViewNames;
    
    [Header("Back Button")]
    [SerializeField] private Button backButton;
    [SerializeField] private bool showBackButton = true;
    
    private ViewManager viewManager;
    private Canvas canvas;
    
    // Events
    public System.Action OnViewWillAppear;
    public System.Action OnViewDidAppear;
    public System.Action OnViewWillDisappear;
    public System.Action OnViewDidDisappear;
    
    private void Awake()
    {
        canvas = GetComponent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError($"ViewController requires a Canvas component on {gameObject.name}!");
        }
        
        if (string.IsNullOrEmpty(viewName))
        {
            viewName = gameObject.name;
        }
    }
    
    private void Start()
    {
        // Find ViewManager in the scene
        viewManager = FindObjectOfType<ViewManager>();
        if (viewManager == null)
        {
            Debug.LogError("ViewManager not found in the scene!");
            return;
        }
        
        SetupNavigationButtons();
        SetupBackButton();
        
        // Subscribe to view manager events
        viewManager.OnViewChanged += OnViewManagerViewChanged;
    }
    
    private void SetupNavigationButtons()
    {
        if (navigationButtons == null || targetViewNames == null) return;
        
        for (int i = 0; i < navigationButtons.Length && i < targetViewNames.Length; i++)
        {
            if (navigationButtons[i] != null)
            {
                string targetView = targetViewNames[i];
                navigationButtons[i].onClick.AddListener(() => NavigateToView(targetView));
            }
        }
    }
    
    private void SetupBackButton()
    {
        if (backButton != null)
        {
            backButton.onClick.AddListener(() => {
                if (viewManager != null)
                {
                    viewManager.GoBack();
                }
            });
        }
    }
    
    private void OnViewManagerViewChanged(Canvas activeCanvas)
    {
        if (activeCanvas == canvas)
        {
            OnViewDidAppear?.Invoke();
        }
        else if (activeCanvas != canvas && canvas.gameObject.activeInHierarchy)
        {
            OnViewWillDisappear?.Invoke();
            OnViewDidDisappear?.Invoke();
        }
    }
    
    /// <summary>
    /// Navigate to a specific view by name
    /// </summary>
    /// <param name="viewName">Name of the target view</param>
    public void NavigateToView(string viewName)
    {
        if (viewManager != null)
        {
            viewManager.ShowView(viewName);
        }
    }
    
    /// <summary>
    /// Navigate to a view by index
    /// </summary>
    /// <param name="viewIndex">Index of the target view</param>
    public void NavigateToView(int viewIndex)
    {
        if (viewManager != null)
        {
            viewManager.ShowView(viewIndex);
        }
    }
    
    /// <summary>
    /// Go back to the previous view
    /// </summary>
    public void GoBack()
    {
        if (viewManager != null)
        {
            viewManager.GoBack();
        }
    }
    
    /// <summary>
    /// Go to the root view
    /// </summary>
    public void GoToRoot()
    {
        if (viewManager != null)
        {
            viewManager.GoToRoot();
        }
    }
    
    /// <summary>
    /// Get the name of this view
    /// </summary>
    public string GetViewName()
    {
        return viewName;
    }
    
    /// <summary>
    /// Check if this is the root view
    /// </summary>
    public bool IsRootView()
    {
        return isRootView;
    }
    
    /// <summary>
    /// Set the back button for this view
    /// </summary>
    public void SetBackButton(Button button)
    {
        if (backButton != null)
        {
            backButton.onClick.RemoveAllListeners();
        }
        
        backButton = button;
        SetupBackButton();
    }
    
    /// <summary>
    /// Add a navigation button with target view
    /// </summary>
    public void AddNavigationButton(Button button, string targetViewName)
    {
        if (button != null)
        {
            button.onClick.AddListener(() => NavigateToView(targetViewName));
        }
    }
    
    private void OnDestroy()
    {
        if (viewManager != null)
        {
            viewManager.OnViewChanged -= OnViewManagerViewChanged;
        }
        
        // Clean up button listeners
        if (navigationButtons != null)
        {
            foreach (var button in navigationButtons)
            {
                if (button != null)
                {
                    button.onClick.RemoveAllListeners();
                }
            }
        }
        
        if (backButton != null)
        {
            backButton.onClick.RemoveAllListeners();
        }
    }
} 