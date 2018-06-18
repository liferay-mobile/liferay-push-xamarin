using System;
using CoreFoundation;
using Foundation;
using LiferayPush;
using UIKit;
using UserNotifications;

namespace PushiOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate, IUNUserNotificationCenterDelegate
    {
        public override UIWindow Window
        {
            get;
            set;
        }

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            var Center = UNUserNotificationCenter.Current;
            Center.Delegate = this;

            Center.RequestAuthorization(UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound, (approved, error) =>
            {
                if (approved)
                {
                    DispatchQueue.MainQueue.DispatchAsync(UIApplication.SharedApplication.RegisterForRemoteNotifications);
                    Console.WriteLine("Authorization approved: " + approved);
                }
                else
                {
                    Console.WriteLine("Error: " + error);
                }
            });

            return true;
        }

        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            LRBasicAuthentication BasicAuthentication = new LRBasicAuthentication("test@liferay.com", "test");
            LRSession Session = new LRSession("http://localhost:8080/", BasicAuthentication);
            LiferayPush.LRPush.WithSession(Session)
                  .WithPortalVersion(70)
                  .OnSuccess((obj) => Console.WriteLine("Device registered successfully!"))
                  .OnFailure((err) => Console.WriteLine($"Device registered failed: {err.LocalizedDescription}"))
                  .RegisterDeviceTokenData(deviceToken);
        }

        public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
        {
            UIAlertView Alert = new UIAlertView()
            {
                Title = "Error registering push notifications",
                Message = error.LocalizedDescription
            };
            Alert.AddButton("OK");
            Alert.Show();
        }
    }
}