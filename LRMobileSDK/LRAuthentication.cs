using Foundation;

namespace LiferayPush
{
    interface ILRAuthentication { }

    // @protocol LRAuthentication <NSObject>
    [BaseType(typeof(NSObject))]
    [Protocol, Model]
    interface LRAuthentication
    {
        // @required -(void)authenticate:(NSMutableURLRequest *)request;
        [Abstract]
        [Export("authenticate:")]
        void Authenticate(NSMutableUrlRequest request);
    }
}