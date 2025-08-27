#import "TAPPXUnityUtils.h"

@implementation TAPPXUnityUtils

+ (void)setCoppaCompliance:(BOOL)accept{
    [TappxFramework setCoppaCompliance:accept];
}

- (void)dealloc {}

@end
extern "C" {
    void setCoppaCompliance( bool accept );
}

void setCoppaCompliance(bool accept){
    [TAPPXUnityUtils setCoppaCompliance:accept];
}