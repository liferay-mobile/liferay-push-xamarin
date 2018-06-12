using System;
using Foundation;
using ObjCRuntime;

namespace LiferayPush
{
    // @interface LRBasicAuthentication : NSObject <LRAuthentication>
    [BaseType(typeof(NSObject))]
    interface LRBasicAuthentication : LRAuthentication
    {
        // @property (nonatomic, strong) NSString * password;
        [Export("password", ArgumentSemantic.Strong)]
        string Password { get; set; }

        // @property (nonatomic, strong) NSString * username;
        [Export("username", ArgumentSemantic.Strong)]
        string Username { get; set; }

        // -(id)initWithUsername:(NSString *)username password:(NSString *)password;
        [Export("initWithUsername:password:")]
        IntPtr Constructor(string username, string password);
    }
}
