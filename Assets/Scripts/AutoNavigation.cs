using UnityEngine;
using System.Collections;

public class AutoNavigation : MonoBehaviour
{
    [Header("Auto Navigation")]
    [SerializeField] private float waitTime = 2f;
    [SerializeField] private int targetViewIndex = 1;
    [SerializeField] private bool autoStart = true;
    
    private ViewManager viewManager;
    
    private void Start()
    {
        viewManager = FindObjectOfType<ViewManager>();
        
        if (autoStart)
        {
            StartAutoNavigation();
        }
    }
    
    public void StartAutoNavigation()
    {
        StartCoroutine(AutoNavigate());
    }
    
    private IEnumerator AutoNavigate()
    {
        // Wait for the specified time
        yield return new WaitForSeconds(waitTime);
        
        // Navigate to the target view
        if (viewManager != null)
        {
            viewManager.ShowView(targetViewIndex);
        }
    }
    
    /// <summary>
    /// Set the target view index
    /// </summary>
    public void SetTargetView(int viewIndex)
    {
        targetViewIndex = viewIndex;
    }
    
    /// <summary>
    /// Set the wait time
    /// </summary>
    public void SetWaitTime(float time)
    {
        waitTime = time;
    }
} 