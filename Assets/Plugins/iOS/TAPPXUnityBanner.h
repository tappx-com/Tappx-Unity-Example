#import <UIKit/UIKit.h>
extern "C" {
#import <TappxFramework/TappxAds.h>
}
@interface TAPPXUnityBanner : UIViewController  <TappxBannerViewDelegate>{
    TappxBannerView *bannerView;
    BOOL position;
    BOOL isBannerVisible;
}

+ (void) trackInstall:(NSString *) tappxID withTestMode:(BOOL)isTest;
+ (void) setCustomEndpoint:(NSString *) customEndpoint;
- (void) hideAd;
- (void) loadBanner;

@property(nonatomic, strong) TappxBannerView *bannerView;
@property(assign) NSInteger position; // 0=Top, 1=Bottom
@property BOOL isMREC;

@end
