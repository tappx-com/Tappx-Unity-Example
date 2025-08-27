#import <UIKit/UIKit.h>
extern "C" {
    #import <TappxFramework/TappxAds.h>
}

@interface TAPPXUnityRewarded : UIViewController <TappxRewardedAdDelegate>{
    TappxRewardedAd* rewardedView;
    BOOL isAutoShow;
}

+ (void) createRewarded:(BOOL)autoShow;
- (void) showRewarded;
- (void) loadRewarded:(BOOL)autoShow;

@property(nonatomic, strong) TappxRewardedAd *rewardedView;

@end
