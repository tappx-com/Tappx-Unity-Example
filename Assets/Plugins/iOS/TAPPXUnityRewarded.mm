#import "TAPPXUnityRewarded.h"

extern UIViewController* UnityGetGLViewController();
extern UIView* UnityGetGLView();

@implementation TAPPXUnityRewarded

@synthesize rewardedView;

static TAPPXUnityRewarded *instance = nil;

- (id)init {
    self = [super init];
    if (self != nil) {
        
    }
    return self;
}

+ (void) createRewarded:(BOOL)autoShow{
    if(instance != nil) return;
    
    TAPPXUnityRewarded* inte = [[TAPPXUnityRewarded alloc]init];
    instance = inte;
    TappxRewardedAd *rewarded = [[TappxRewardedAd alloc] initWithDelegate:inte];
    inte.rewardedView = rewarded;
    [inte.rewardedView setAutoShowWhenReady:autoShow];
    [inte.rewardedView load];
    
}

- (void) loadRewarded:(BOOL)autoShow{
    self.rewardedView = [[TappxRewardedAd alloc] initWithDelegate:self];
    [self.rewardedView setAutoShowWhenReady:autoShow];
    [self.rewardedView load];
    
}

- (void) showRewarded{
    [self.rewardedView showFrom:UnityGetGLViewController()];
}

-(BOOL) isRewardedReady {
    return [self.rewardedView isReady];
}

- (nonnull UIViewController *)presentViewController {
    return UnityGetGLViewController();
}

- (void)dealloc {
    self.view = nil;
    instance = nil;
}

- (void)present:(nonnull UIViewController*)viewController withCompletions:(void (^__nonnull)(void))completion{
    UIViewController *rootVC = UnityGetGLViewController();
    [rootVC presentViewController:viewController animated:false completion:completion];
}

- (void) tappxRewardedAdDidFinishLoad:(nonnull TappxRewardedAd*) viewController {
    NSLog(@"REWARDED: FINISHLOAD");
    UnitySendMessage("TappxManagerUnity", "tappxRewardedViewControllerDidFinishLoad", "");
}
- (void) tappxRewardedAdDidFail:(nonnull TappxRewardedAd*) viewController withError:(nonnull TappxErrorAd*) error {
    NSLog(@"REWARDED: DIDFAIL %@", error.descriptionError);
    UnitySendMessage("TappxManagerUnity", "tappxRewardedViewControllerDidFail", [[NSString stringWithFormat:@"%@",error.descriptionError] UTF8String]);
}
- (void) tappxRewardedAdClicked:(nonnull TappxRewardedAd*) viewController {
    NSLog(@"REWARDED: DIDPRESS");
    UnitySendMessage("TappxManagerUnity", "tappxRewardedViewControllerClicked", "");
}
- (void) tappxRewardedAdPlaybackFailed:(nonnull TappxRewardedAd*) viewController {
    NSLog(@"REWARDED: PLAYBACK FAILED");
    UnitySendMessage("TappxManagerUnity", "tappxRewardedViewControllerPlaybackFailed", "");
}
- (void) tappxRewardedAdVideoClosed:(nonnull TappxRewardedAd*) viewController {
    NSLog(@"REWARDED: DIDCLOSE");
    UnitySendMessage("TappxManagerUnity", "tappxRewardedViewControllerVideoClosed", "");
}
- (void) tappxRewardedAdVideoCompleted:(nonnull TappxRewardedAd*) viewController {
    NSLog(@"REWARDED: COMPLETED");
    UnitySendMessage("TappxManagerUnity", "tappxRewardedViewControllerVideoCompleted", "");
}
- (void) tappxRewardedAdDidAppear:(nonnull TappxRewardedAd *)viewController {
    NSLog(@"REWARDED: DIDAPPEAR");
    UnitySendMessage("TappxManagerUnity", "tappxRewardedViewControllerDidAppear", "");
}
- (void) tappxRewardedViewControllerDismissed:(nonnull TappxRewardedAd *) viewController {
    NSLog(@"REWARDED: DISMISSED");
    UnitySendMessage("TappxManagerUnity", "tappxRewardedViewControllerDismissed", "");
}
- (void) tappxRewardedViewControllerUserDidEarnReward:(nonnull TappxRewardedAd*) viewController {
    NSLog(@"REWARDED: REWARDED EARNED");
    UnitySendMessage("TappxManagerUnity", "tappxRewardedViewControllerUserDidEarnReward", "");
}

@end
extern "C" {
    void loadRewardedIOS_( bool autoShow );
    void showRewardedIOS_();
    void releaseRewardedTappxIOS_();
    bool isRewardedReady_ ();
}

bool isRewardedReady_ () {
    return [ instance isRewardedReady ];
}


void loadRewardedIOS_(bool autoShow){
    if(instance != nil){
        [instance loadRewarded:autoShow];
    }else{
        [TAPPXUnityRewarded createRewarded:autoShow];
    }
}

void showRewardedIOS_(){
    if(instance != nil){
        if ( [instance isRewardedReady] )
            [instance showRewarded];
    }
}

void releaseRewardedTappxIOS_(){
    if(instance != nil){
        instance = nil;
    }
}
