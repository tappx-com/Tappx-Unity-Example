#import <UIKit/UIKit.h>
extern "C" {
#import <TappxFramework/TappxAds.h>
}

@interface TAPPXUnityInterstitial : UIViewController <TappxInterstitialAdDelegate>{
    TappxInterstitialAd* interstitialView;
    BOOL isAutoShow;
}

+ (void) createInterstitial:(BOOL)autoShow;
- (void) showInterstitial;
- (void) loadInterstitial:(BOOL)autoShow;

@property(nonatomic, strong) TappxInterstitialAd *interstitialView;

@end
