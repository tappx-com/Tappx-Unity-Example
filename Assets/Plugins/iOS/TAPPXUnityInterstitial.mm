#import "TAPPXUnityInterstitial.h"

extern UIViewController* UnityGetGLViewController();
extern UIView* UnityGetGLView();

@implementation TAPPXUnityInterstitial

@synthesize interstitialView;

static TAPPXUnityInterstitial *instance = nil;

- (id)init {
    self = [super init];
    if (self != nil) {
        
    }
    return self;
}

+ (void) createInterstitial:(BOOL)autoShow{
    
    if(instance != nil) return;
    
    
    TAPPXUnityInterstitial* inte = [[TAPPXUnityInterstitial alloc]init];
    instance = inte;
    TappxInterstitialAd *interstitial = [[TappxInterstitialAd alloc] initWithDelegate:inte];
    inte.interstitialView = interstitial;
    [inte.interstitialView setAutoShowWhenReady:autoShow];
    [inte.interstitialView load];
    
}

- (void) loadInterstitial:(BOOL)autoShow{
    self.interstitialView = [[TappxInterstitialAd alloc] initWithDelegate:self];
    [self.interstitialView setAutoShowWhenReady:autoShow];
    [self.interstitialView load];
    
}

- (void) showInterstitial{
    [self.interstitialView showFrom:UnityGetGLViewController()];
}

-(BOOL) isInterstitalReady {
    return [self.interstitialView isReady];
}

- (UIViewController*)presentViewController{
    return UnityGetGLViewController();
}

- (void)dealloc {
    self.view = nil;
    instance = nil;
}






-(void) tappxInterstitialAdDidAppear:(TappxInterstitialAd*) viewController{
    
    NSLog(@"INTERSTITIAL: DIDAPPEAR");
    UnitySendMessage("TappxManagerUnity", "tappxInterstitialDidAppear", "");

}

-(void) tappxInterstitialAdDidFail:(TappxInterstitialAd*) viewController withError:(TappxErrorAd*) error{
    
    NSLog(@"INTERSTITIAL: DIDFAIL %@", error.descriptionError);
    UnitySendMessage("TappxManagerUnity", "tappxInterstitialDidFail", [[NSString stringWithFormat:@"%@",error.descriptionError] UTF8String]);
    
}

-(void) tappxInterstitialAdDidClose:(TappxInterstitialAd*) viewController{
    NSLog(@"INTERSTITIAL: DIDCLOSE");
}

-(void) tappxInterstitialAdDidPress:(TappxInterstitialAd*) viewController{
    NSLog(@"INTERSTITIAL: DIDPRESS");
    UnitySendMessage("TappxManagerUnity", "interstitialDidPress", "");
}

-(void) tappxInterstitialAdDidFinishLoad:(TappxInterstitialAd*) viewController
{
    NSLog(@"INTERSTITIAL: DIDFINISHLOAD");
    UnitySendMessage("TappxManagerUnity", "tappxInterstitialDidFinishLoad", "");
}

-(void) tappxInterstitialAdDismissed:(TappxInterstitialAd*) viewController
{
    NSLog(@"INTERSTITIAL: DISMISSED");
    UnitySendMessage("TappxManagerUnity", "tappxInterstitialDismissed", "");
}






@end
extern "C" {
    void loadInterstitialIOS_( bool autoShow );
    void showInterstitialIOS_();
    void releaseInterstitialTappxIOS_();
    bool isInterstitialReady_ ();
}

bool isInterstitialReady_ () {
    return [ instance isInterstitalReady ];
}


void loadInterstitialIOS_(bool autoShow){
    if(instance != nil){
        [instance loadInterstitial:autoShow];
    }else{
        [TAPPXUnityInterstitial createInterstitial:autoShow];
    }
}

void showInterstitialIOS_(){
    if(instance != nil){
        if ( [instance isInterstitalReady] )
            [instance showInterstitial];
    }
}

void releaseInterstitialTappxIOS_(){
    if(instance != nil){
        instance = nil;
    }
}
