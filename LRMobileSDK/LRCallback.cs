using Foundation;

namespace LiferayPush
{
    // @protocol LRCallback <NSObject>
    [Protocol, Model]
    [BaseType(typeof(NSObject))]
    interface LRCallback
    {
        // @required -(void)onFailure:(NSError *)error;
        [Abstract]
        [Export("onFailure:")]
        void OnFailure(NSError error);

        // @required -(void)onSuccess:(id)result;
        [Abstract]
        [Export("onSuccess:")]
        void OnSuccess(NSObject result);
    }

    // typedef void (^LRFailureBlock)(NSError *);
    delegate void LRFailureBlock(NSError arg0);

    // typedef void (^LRSuccessBlock)(id);
    delegate void LRSuccessBlock(NSObject arg0);
}