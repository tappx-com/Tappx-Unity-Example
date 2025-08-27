using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewManager : MonoBehaviour
{
    [Header("View Management")]
    [SerializeField] private Canvas[] views;
    [SerializeField] private Canvas defaultView;
    
    [Header("Back Button")]
    [SerializeField] private Button backButton;
    [SerializeField] private bool enableBackButton = true;
    
    [Header("Animation Settings")]
    [SerializeField] private bool useAnimations = true;
    [SerializeField] private float transitionDuration = 0.3f;
    [SerializeField] private AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    private Stack<Canvas> viewHistory = new Stack<Canvas>();
    private Canvas currentView;
    private bool isTransitioning = false;
    
    // Events
    public System.Action<Canvas> OnViewChanged;
    public System.Action<Canvas> OnViewPushed;
    public System.Action<Canvas> OnViewPopped;
    
    private void Start()
    {
        InitializeViews();
        SetupBackButton();
        
        if (defaultView != null)
        {
            ShowView(defaultView, false);
        }
    }
    
    private void InitializeViews()
    {
        // Hide all views initially
        foreach (var view in views)
        {
            if (view != null)
            {
                view.gameObject.SetActive(false);
            }
        }
    }
    
    private void SetupBackButton()
    {
        if (backButton != null)
        {
            backButton.onClick.AddListener(GoBack);
            backButton.gameObject.SetActive(false); // Initially hidden
        }
    }
    
    /// <summary>
    /// Shows a specific view and adds it to the navigation history
    /// </summary>
    /// <param name="view">The canvas to show</param>
    /// <param name="addToHistory">Whether to add this view to the navigation history</param>
    public void ShowView(Canvas view, bool addToHistory = true)
    {
        if (view == null || isTransitioning) return;
        
        if (addToHistory && currentView != null)
        {
            viewHistory.Push(currentView);
        }
        
        if (useAnimations)
        {
            StartCoroutine(TransitionToView(view));
        }
        else
        {
            SwitchToView(view);
        }
        
        UpdateBackButtonVisibility();
    }
    
    /// <summary>
    /// Shows a view by index
    /// </summary>
    /// <param name="viewIndex">Index of the view in the views array</param>
    /// <param name="addToHistory">Whether to add this view to the navigation history</param>
    public void ShowView(int viewIndex, bool addToHistory = true)
    {
        if (viewIndex >= 0 && viewIndex < views.Length)
        {
            ShowView(views[viewIndex], addToHistory);
        }
        else
        {
            Debug.LogError($"View index {viewIndex} is out of range!");
        }
    }
    
    /// <summary>
    /// Shows a view by name - Use this method for Unity Inspector events
    /// </summary>
    /// <param name="viewName">Name of the view GameObject</param>
    public void ShowViewByName(string viewName)
    {
        Canvas targetView = System.Array.Find(views, v => v != null && v.name == viewName);
        if (targetView != null)
        {
            ShowView(targetView, true);
        }
        else
        {
            Debug.LogError($"View with name '{viewName}' not found! Available views: {GetAvailableViewNames()}");
        }
    }
    
    /// <summary>
    /// Gets a list of all available view names for debugging
    /// </summary>
    private string GetAvailableViewNames()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        foreach (var view in views)
        {
            if (view != null)
            {
                sb.Append(view.name).Append(", ");
            }
        }
        return sb.ToString().TrimEnd(',', ' ');
    }
    
    /// <summary>
    /// Shows a view by name
    /// </summary>
    /// <param name="viewName">Name of the view GameObject</param>
    /// <param name="addToHistory">Whether to add this view to the navigation history</param>
    public void ShowView(string viewName, bool addToHistory = true)
    {
        Canvas targetView = System.Array.Find(views, v => v != null && v.name == viewName);
        if (targetView != null)
        {
            ShowView(targetView, addToHistory);
        }
        else
        {
            Debug.LogError($"View with name '{viewName}' not found!");
        }
    }
    
    /// <summary>
    /// Goes back to the previous view in the navigation history
    /// </summary>
    public void GoBack()
    {
        if (viewHistory.Count > 0 && !isTransitioning)
        {
            Canvas previousView = viewHistory.Pop();
            
            if (useAnimations)
            {
                StartCoroutine(TransitionToView(previousView, false));
            }
            else
            {
                SwitchToView(previousView, false);
            }
            
            UpdateBackButtonVisibility();
        }
    }
    
    /// <summary>
    /// Clears the navigation history
    /// </summary>
    public void ClearHistory()
    {
        viewHistory.Clear();
        UpdateBackButtonVisibility();
    }
    
    /// <summary>
    /// Returns to the root view (first view in the array)
    /// </summary>
    public void GoToRoot()
    {
        if (views.Length > 0 && views[0] != null)
        {
            ClearHistory();
            ShowView(views[0], false);
        }
    }
    
    private System.Collections.IEnumerator TransitionToView(Canvas targetView, bool addToHistory = true)
    {
        isTransitioning = true;
        
        Canvas fromView = currentView;
        Canvas toView = targetView;
        CanvasGroup toGroup = null;
        
        // Show the target view
        if (toView != null)
        {
            toView.gameObject.SetActive(true);
            toGroup = GetOrAddCanvasGroup(toView);
            toGroup.alpha = 0f;
        }
        
        // Hide the current view with fade out
        if (fromView != null)
        {
            CanvasGroup fromGroup = GetOrAddCanvasGroup(fromView);
            float elapsed = 0f;
            
            while (elapsed < transitionDuration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / transitionDuration;
                float curveValue = transitionCurve.Evaluate(progress);
                
                fromGroup.alpha = 1f - curveValue;
                if (toGroup != null)
                {
                    toGroup.alpha = curveValue;
                }
                
                yield return null;
            }
            
            fromView.gameObject.SetActive(false);
            fromGroup.alpha = 1f; // Reset for next use
        }
        else
        {
            // No previous view, just fade in the target
            if (toGroup != null)
            {
                float elapsed = 0f;
                while (elapsed < transitionDuration)
                {
                    elapsed += Time.deltaTime;
                    float progress = elapsed / transitionDuration;
                    float curveValue = transitionCurve.Evaluate(progress);
                    
                    toGroup.alpha = curveValue;
                    yield return null;
                }
                toGroup.alpha = 1f;
            }
        }
        
        currentView = targetView;
        isTransitioning = false;
        
        OnViewChanged?.Invoke(currentView);
        if (addToHistory)
        {
            OnViewPushed?.Invoke(currentView);
        }
        else
        {
            OnViewPopped?.Invoke(currentView);
        }
    }
    
    private void SwitchToView(Canvas targetView, bool addToHistory = true)
    {
        // Hide current view
        if (currentView != null)
        {
            currentView.gameObject.SetActive(false);
        }
        
        // Show target view
        if (targetView != null)
        {
            targetView.gameObject.SetActive(true);
        }
        
        currentView = targetView;
        
        OnViewChanged?.Invoke(currentView);
        if (addToHistory)
        {
            OnViewPushed?.Invoke(currentView);
        }
        else
        {
            OnViewPopped?.Invoke(currentView);
        }
    }
    
    private CanvasGroup GetOrAddCanvasGroup(Canvas canvas)
    {
        CanvasGroup group = canvas.GetComponent<CanvasGroup>();
        if (group == null)
        {
            group = canvas.gameObject.AddComponent<CanvasGroup>();
        }
        return group;
    }
    
    private void UpdateBackButtonVisibility()
    {
        if (backButton != null)
        {
            backButton.gameObject.SetActive(enableBackButton && viewHistory.Count > 0);
        }
    }
    
    /// <summary>
    /// Gets the current active view
    /// </summary>
    public Canvas GetCurrentView()
    {
        return currentView;
    }
    
    /// <summary>
    /// Gets the number of views in the navigation history
    /// </summary>
    public int GetHistoryCount()
    {
        return viewHistory.Count;
    }
    
    /// <summary>
    /// Checks if there's a previous view to go back to
    /// </summary>
    public bool CanGoBack()
    {
        return viewHistory.Count > 0;
    }
    
    /// <summary>
    /// Enables or disables the back button functionality
    /// </summary>
    public void SetBackButtonEnabled(bool enabled)
    {
        enableBackButton = enabled;
        UpdateBackButtonVisibility();
    }
    
    /// <summary>
    /// Sets the back button reference
    /// </summary>
    public void SetBackButton(Button button)
    {
        if (backButton != null)
        {
            backButton.onClick.RemoveListener(GoBack);
        }
        
        backButton = button;
        SetupBackButton();
    }
    
    private void OnDestroy()
    {
        if (backButton != null)
        {
            backButton.onClick.RemoveListener(GoBack);
        }
    }
} 