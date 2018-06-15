using System;
using Foundation;

namespace LiferayPush
{
    // @interface LRPush : NSObject
    [BaseType(typeof(NSObject))]
    interface LRPush
    {
        // @property (readonly, copy, nonatomic, class) NSString * _Nonnull PAYLOAD;
        [Static]
        [Export("PAYLOAD")]
        string PAYLOAD { get; }

        // +(LRPush * _Nonnull)withSession:(LRSession * _Nonnull)session __attribute__((warn_unused_result));
        [Static]
        [Export("withSession:")]
        LRPush WithSession(LRSession session);

        // -(void)didReceiveRemoteNotification:(NSDictionary<NSString *,id> * _Nonnull)pushNotification;
        [Export("didReceiveRemoteNotification:")]
        void DidReceiveRemoteNotification(NSDictionary<NSString, NSObject> pushNotification);

        // -(instancetype _Nonnull)onFailure:(id)failure __attribute__((warn_unused_result));
        [Export("onFailure:")]
        LRPush OnFailure(Action<NSError> failure);

        // -(instancetype _Nonnull)onPushNotification:(void (^ _Nonnull)(NSDictionary<NSString *,id> * _Nonnull))pushNotification __attribute__((warn_unused_result));
        [Export("onPushNotification:")]
        LRPush OnPushNotification(Action<NSDictionary<NSString, NSObject>> pushNotification);

        // -(instancetype _Nonnull)onSuccess:(void (^ _Nonnull)(NSDictionary<NSString *,id> * _Nullable))success __attribute__((warn_unused_result));
        [Export("onSuccess:")]
        LRPush OnSuccess(Action<NSDictionary<NSString, NSObject>> success);

        // -(void)registerDevice;
        [Export("registerDevice")]
        void RegisterDevice();

        // -(void)registerDeviceTokenData:(NSData * _Nonnull)deviceTokenData;
        [Export("registerDeviceTokenData:")]
        void RegisterDeviceTokenData(NSData deviceTokenData);

        // -(void)registerDeviceToken:(NSString * _Nonnull)deviceToken;
        [Export("registerDeviceToken:")]
        void RegisterDeviceToken(string deviceToken);

        // -(void)sendToUserId:(NSInteger)userId notification:(NSDictionary<NSString *,id> * _Nonnull)notification;
        [Export("sendToUserId:notification:")]
        void SendToUserId(nint userId, NSDictionary<NSString, NSObject> notification);

        // -(void)sendToUserIds:(NSArray<NSNumber *> * _Nonnull)userIds notification:(NSDictionary<NSString *,id> * _Nonnull)notification;
        [Export("sendToUserIds:notification:")]
        void SendToUserIds(NSNumber[] userIds, NSDictionary<NSString, NSObject> notification);

        // -(void)unregisterDeviceToken:(NSString * _Nonnull)deviceToken;
        [Export("unregisterDeviceToken:")]
        void UnregisterDeviceToken(string deviceToken);

        // -(instancetype _Nonnull)withPortalVersion:(NSInteger)portalVersion __attribute__((warn_unused_result));
        [Export("withPortalVersion:")]
        LRPush WithPortalVersion(nint portalVersion);
    }
}
