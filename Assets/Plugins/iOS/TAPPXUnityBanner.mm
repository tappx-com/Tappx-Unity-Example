#import "TAPPXUnityBanner.h"

extern UIViewController* UnityGetGLViewController();
extern UIView* UnityGetGLView();

@implementation TAPPXUnityBanner

@synthesize bannerView;

static TAPPXUnityBanner *instance = nil;

+ (void) trackInstall:(NSString *)tappxID withTestMode:(BOOL)isTest{

    if ( isTest )
        [TappxFramework addTappxKey:tappxID testMode:YES];
    else
        [TappxFramework addTappxKey:tappxID fromNonNative:@"unity_ios"];
        
}

+ (void) setCustomEndpoint:(NSString *) customEndpoint{
    if(customEndpoint != nil){
        if(![customEndpoint isEqualToString:@""])
            [TappxFramework setEndpoint:customEndpoint];
    }
}


- (id)init {
    self = [super init];
    if (self != nil) {
        
    }
    return self;
}

- (void) loadBanner {
        
    self.bannerView = nil;
    
    
    self.bannerView = [[TappxBannerView alloc] initWithDelegate:instance andSize:(!instance.isMREC ? TappxBannerSmartBanner : TappxBannerSize300x250)];
    [self addBannerViewToView:self.bannerView position:NSLayoutAttributeTop];
    NSLayoutAttribute posAttribute = (self.position == 1) ? NSLayoutAttributeBottom : NSLayoutAttributeTop;
    [self addBannerViewToView:self.bannerView position:posAttribute];
    [self.bannerView load];
    
    
    
    
}


- (void)addBannerViewToView:(UIView *)bannerView position:(NSLayoutAttribute)pos {
    bannerView.translatesAutoresizingMaskIntoConstraints = NO;
    UIViewController* controller = UnityGetGLViewController();
    [controller.view addSubview:bannerView];
    [controller.view addConstraints:@[
        [NSLayoutConstraint constraintWithItem:bannerView
                                     attribute:pos
                                     relatedBy:NSLayoutRelationEqual
                                        toItem:controller.view.safeAreaLayoutGuide
                                     attribute:pos
                                    multiplier:1
                                      constant:0],
        [NSLayoutConstraint constraintWithItem:bannerView
                                     attribute:NSLayoutAttributeCenterX
                                     relatedBy:NSLayoutRelationEqual
                                        toItem:controller.view
                                     attribute:NSLayoutAttributeCenterX
                                    multiplier:1
                                      constant:0]
    ]];
}


- (void) hideAd{
    if(self.bannerView!=nil){
        [self.bannerView removeBanner];
        self.bannerView = nil;
    }
}


-(void) tappxBannerViewDidFinishLoad:(TappxBannerView*) vc{
    
    NSLog(@"BANNER: DIDAPPEAR");
    UnitySendMessage("TappxManagerUnity", "tappxBannerDidReceiveAd", "");
}

-(void) tappxBannerViewDidPress:(TappxBannerView*) vc{
    
    NSLog(@"BANNER: DIDPRESS");
    UnitySendMessage("TappxManagerUnity", "tappxViewWillLeaveApplication", "");
    
}
-(void) tappxBannerViewDidClose:(TappxBannerView*) vc{
    NSLog(@"BANNER: DIDCLOSE");
}

-(void) tappxBannerViewDidFail:(TappxBannerView*) vc withError:(TappxErrorAd*) error{
    NSLog(@"BANNER: DIDFAIL %@", error.descriptionError);
    UnitySendMessage("TappxManagerUnity", "tappxBannerFailedToLoad", [[NSString stringWithFormat:@"%@",error.descriptionError] UTF8String]);
}




- (void)dealloc {
    self.view = nil;
    [self.bannerView removeBanner];
    instance = nil;
    //  [super dealloc];
}

@end

extern "C" {
    void trackInstallIOS_(char *tappxID, bool isTest);
    void setEndpointIOS_(char *customEndpoint);
    void hideAdIOS_();
    void releaseTappxIOS_();
    void loadBannerIOS_(int positionBanner, bool mrec);
}

void trackInstallIOS_(char *tappxID, bool isTest){
    [TAPPXUnityBanner trackInstall:[NSString stringWithCString:tappxID encoding:NSASCIIStringEncoding] withTestMode:isTest];
}

void setEndpointIOS_(char *customEndpoint){
    [TAPPXUnityBanner setCustomEndpoint:[NSString stringWithCString:customEndpoint encoding:NSASCIIStringEncoding]];
}

void hideAdIOS_(){
    if(instance != nil){
        [instance hideAd];
    }
}

void loadBannerIOS_(int positionBanner, bool mrec) {
    

    // Init
    TAPPXUnityBanner *tappxUnityBanner = [[TAPPXUnityBanner alloc] init];
    instance = tappxUnityBanner;
    instance.position = positionBanner; // store top/bottom choice
    instance.isMREC = mrec;
    
    
    if ( instance != nil) {
        [instance loadBanner];
    }
    
    
}




void releaseTappxIOS_(){
    if(instance != nil){
        instance = nil;
    }
}
