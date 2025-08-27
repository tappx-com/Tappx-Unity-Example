# Unity View Management System

This system provides a native-like navigation experience for Unity applications, similar to iOS/Android navigation patterns. It manages canvas switching with back button functionality and smooth transitions.

## Components

### 1. ViewManager
The main controller that manages all views and navigation history.

**Features:**
- Stack-based navigation history
- Smooth fade transitions between views
- Back button functionality
- Event system for view changes
- Support for navigation by name, index, or direct reference

### 2. ViewController
Individual controller for each canvas/view that handles view-specific logic.

**Features:**
- View-specific navigation buttons
- Lifecycle events (OnViewWillAppear, OnViewDidAppear, etc.)
- Automatic back button setup
- View-specific settings

### 3. ViewNavigationExample
Example implementation showing how to use the system.

## Setup Instructions

### Step 1: Create Your Views
1. Create Canvas GameObjects for each screen/view in your app
2. Each Canvas should represent a different screen (e.g., Home, Settings, Profile)
3. Make sure each Canvas has a unique name

### Step 2: Setup ViewManager
1. Create an empty GameObject in your scene
2. Add the `ViewManager` script to it
3. In the inspector, configure:
   - **Views Array**: Drag all your Canvas GameObjects here
   - **Default View**: Set which view should be shown first
   - **Back Button**: Assign a UI Button for the back functionality
   - **Animation Settings**: Configure transition duration and curve

### Step 3: Setup Individual Views (Optional)
1. Add the `ViewController` script to each Canvas GameObject
2. Configure the view settings:
   - **View Name**: Unique name for this view
   - **Is Root View**: Check if this is the main/home view
   - **Navigation Buttons**: Assign buttons that navigate to other views
   - **Target View Names**: Names of the views to navigate to
   - **Back Button**: Assign a back button for this specific view

## Usage Examples

### Basic Navigation
```csharp
// Get reference to ViewManager
ViewManager viewManager = FindObjectOfType<ViewManager>();

// Navigate to a view by name
viewManager.ShowView("SettingsView");

// Navigate to a view by index
viewManager.ShowView(1);

// Go back to previous view
viewManager.GoBack();

// Go to root view
viewManager.GoToRoot();
```

### Using ViewController
```csharp
// Get reference to ViewController
ViewController viewController = GetComponent<ViewController>();

// Navigate from this view
viewController.NavigateToView("ProfileView");

// Go back
viewController.GoBack();
```

### Event Handling
```csharp
// Subscribe to view manager events
viewManager.OnViewChanged += (Canvas newView) => {
    Debug.Log($"Current view: {newView.name}");
};

viewManager.OnViewPushed += (Canvas pushedView) => {
    Debug.Log($"Pushed view: {pushedView.name}");
};

viewManager.OnViewPopped += (Canvas poppedView) => {
    Debug.Log($"Popped view: {poppedView.name}");
};
```

### ViewController Events
```csharp
ViewController viewController = GetComponent<ViewController>();

viewController.OnViewWillAppear += () => {
    Debug.Log("View will appear");
};

viewController.OnViewDidAppear += () => {
    Debug.Log("View did appear");
};

viewController.OnViewWillDisappear += () => {
    Debug.Log("View will disappear");
};

viewController.OnViewDidDisappear += () => {
    Debug.Log("View did disappear");
};
```

## Configuration Options

### ViewManager Settings
- **Views**: Array of all Canvas GameObjects
- **Default View**: The view to show when the app starts
- **Back Button**: Global back button reference
- **Enable Back Button**: Toggle back button functionality
- **Use Animations**: Enable/disable transition animations
- **Transition Duration**: How long transitions take
- **Transition Curve**: Animation curve for transitions

### ViewController Settings
- **View Name**: Unique identifier for this view
- **Is Root View**: Whether this is the main view
- **Navigation Buttons**: Buttons that navigate to other views
- **Target View Names**: Names of target views for navigation
- **Back Button**: View-specific back button
- **Show Back Button**: Whether to show back button on this view

## Best Practices

1. **Naming Convention**: Use descriptive names for your views (e.g., "HomeView", "SettingsView")
2. **Hierarchy**: Keep all views at the same level in the hierarchy
3. **Canvas Groups**: The system automatically adds CanvasGroup components for animations
4. **Event Cleanup**: Always unsubscribe from events in OnDestroy
5. **Button References**: Use the inspector to assign button references for better organization

## Troubleshooting

### Common Issues

1. **Views not switching**: Check that all Canvas GameObjects are assigned to the Views array
2. **Back button not working**: Ensure the back button is assigned and enableBackButton is true
3. **Animations not working**: Check that useAnimations is enabled and transitionDuration > 0
4. **Buttons not responding**: Verify that button references are assigned in the inspector

### Debug Tips

- Use the ViewManager's `GetCurrentView()` method to check the active view
- Use `GetHistoryCount()` to see how many views are in the navigation stack
- Use `CanGoBack()` to check if back navigation is available
- Subscribe to events to track navigation flow

## Advanced Features

### Custom Transitions
You can extend the system by creating custom transition effects by modifying the `TransitionToView` method in ViewManager.

### Multiple ViewManagers
For complex apps, you can have multiple ViewManagers for different sections of your app.

### Persistent Navigation
The navigation history is maintained during the session. You can clear it using `ClearHistory()` if needed. 