![Liferay Mobile SDK logo](https://github.com/liferay/liferay-mobile-sdk/raw/master/logo.png)

# Liferay Push for Xamarin

* [Setup](#setup)
* [Android](#android)
	* [Registering a device](#registering-a-device)
	* [Receiving push notifications](#receiving-push-notifications)
	* [Sending push notifications](#sending-push-notifications)
	* [Unregistering a device](#unregistering-a-device)
* [iOS](#ios)
	* [Registering a device](#registering-a-device-1)
	* [Sending push notifications](#sending-push-notifications-1)
	* [Unregistering a device](#unregistering-a-device-1)

## Setup

Add the NuGet package as a dependency in your solution:

<!-- Dialog in Visual Studio to add NuGet package -->

## Android

### Registering a device

To receive push notifications, your app must register itself to the Liferay instance first. On the Liferay instance side, each device is tied to a user. Each user can have multiple registered devices. A device is represented by a device token string. Google calls this the `registrationId`.

To register a device, we need a `SENDER_ID`, the id of our project in firebase. Read [Firebase's documentation](https://firebase.google.com/docs/cloud-messaging/) to learn how to get the `SENDER_ID`.

The `SENDER_ID` is available, after creating a firebase project, in the *Cloud Messaging* tab under the project settings:

<img src="docs/images/Firebase Console Sender Id.png">

After obtaining the `SENDER_ID` it's easy to register a device with *Liferay Push for Xamarin (Android)*, you just have to call to the following method:

```
using Com.Liferay.Mobile.Android.Auth.Basic;
using Com.Liferay.Mobile.Android.Service;
using Com.Liferay.Mobile.Push;

var Session = new SessionImpl("http://localhost:8080", new BasicAuthentication("test@liferay.com", "test"));

Push.With(Session)
	.OnSuccess(this)
	.OnFailure(this)
	.Register(this, SENDER_ID);
```

**If you want to use Liferay 7.x you should manually specify the version** with a call like this:

```
Push.WithPortalVersion(70)
```

Since all operations are asynchronous, you can set callbacks to check if the registration succeeded or an error occurred on the server side:

```
public void OnSuccess(JSONObject json)
{
    Console.WriteLine($"Device registered succesfully: {json}");
}

public void OnFailure(Java.Lang.Exception e)
{
    Console.WriteLine($"Device register failed: {e.Message}");
}
```

The `OnSuccess` and `OnFailure` callbacks are optional, but it's good practice to implement both. By doing so, your app can persist the `registrationId` device token or tell the user that an error occurred.

*Liferay Push for Xamarin (Android)* is calling the *GCM server*, retrieving the results and storing your `registrationId` in the Liferay instance for later use.

Don't forget to add in your `AndroidManifest.xml` the *Internet* permission if you haven't done it already:

```
<uses-permission android:name="android.permission.INTERNET" />
```
		
And, if you are using Liferay 7, you will have to add permissions to be able to register the device in the Liferay instance:

<img src="docs/images/Liferay Permissions.png">

All set! If everything went well, you should see a new device registered under the *Push Notifications* menu in *Configuration*.

#### Using the `registrationId` directly without registering against Liferay instance

If you obtain the token manually, you can register the device to the Liferay instance by calling the following method:

```
Push.With(Session).Register("SENDER_ID");
```

Now each time the Liferay instance wants to send a push notification to the user `test@liferay.com`, it looks up all registered devices for the user (including the one just registered) and sends the push notification for each `registrationId` found.

You should note that the [Push](https://github.com/liferay-mobile/liferay-push-android/blob/master/library/src/main/java/com/liferay/mobile/push/Push.java) class is a wrapper for the Mobile SDK generated services. Internally, it calls the Mobile SDK's `PushNotificationsDeviceService` class. While you can still use `PushNotificationsDeviceService` directly, using the wrapper class is easier.

### Receiving push notifications

Once your device is registered, you have to configure both the server and the client to be able to receive push messages.

To send notifications from Liferay you should configure the `API_KEY` inside:

* Liferay 6.2: *System Settings*, *Other* and *Android Push Notifications Sender*. 
* Liferay 7.0: *Configuration*, *System Settings* and *Android Push Notifications Sender*. 
* Liferay 7.1: *Configuration*, *System Settings*, *Notifications* and *Android*. 

To obtain the `API_KEY` you should, again, access your Firebase project settings and under *Cloud Messaging*, use the **Legacy Server Key**. 

<img src="docs/images/Firebase Console Server Key.png">
 
Then you have to configure your project to be able to listen for notifications:

* You should implement a `BroadcastReceiver` instance in your app. [Android's developer documentation](http://developer.android.com/google/gcm/client.html#sample-receive) shows you how to do this. Specifically, you should:

	* Create a `PushReceiver` class (our `BroadcastReceiver`) with `com.google.android.c2dm.permission.SEND` permission and an `IntentFilter` with the action `com.google.android.c2dm.intent.RECEIVE`:

		```
		using System;
		using Android.App;
		using Android.Content;
		using Com.Liferay.Mobile.Push;
		
		namespace PushAndroid
		{
		    [BroadcastReceiver(Permission = "com.google.android.c2dm.permission.SEND")]
		    [IntentFilter(new String[]{"com.google.android.c2dm.intent.RECEIVE"})]
		    public class PushReceiver : PushNotificationsReceiver
		    {
		        public PushReceiver() { }
		
		        public override string ServiceClassName => Java.Lang.Class.FromType(typeof(PushService)).Name;
		    }
		}
		```
	
	* Register the `INTERNET` and `WAKE_LOCK` permission if you had not used those permissions before:

		```
		<uses-permission android:name="android.permission.INTERNET" />
		<uses-permission android:name="android.permission.WAKE_LOCK" />
		```
	
	* Create an `IntentService`:

		```
		using System;
		using Android.App;
		using Com.Liferay.Mobile.Push.Service;
		using Org.Json;
		
		namespace PushAndroid
		{
			[Service]
			public class PushService : PushNotificationsService
		    {
		        public PushService() { }
		
		        public override void OnPushNotification(JSONObject json)
		        {
		        	//This json contains the push notification
		        }
		    }
		}
		```

* If you want to execute an action or show a notification only if the application is active, you could register a callback:

```
Push.With(session).OnPushNotification(this);

public void OnPushNotification(JSONObject p0)
{
	//This json contains the push notification
}
```
	
This method only works if you have already registered against Liferay instance using the previous instructions.

### Sending push notifications

You can send push notifications from Liferay instance but also, you can send push notifications from your Android app. Just make sure the user has the proper permissions in the Liferay instance to send push notifications.

```
JSONObject Notification = new JSONObject();
Notification.Put("body", "Hello!");
Push.With(Session).Send(USER_ID, Notification);
```

In this code, the push notification is sent to the user specified by `USER_ID`. Upon receiving the notification, the Liferay instance looks up all the user's registered devices (both Android and iOS devices) and sends `Notification` as the body of the push notification.

### Unregistering a device

If you want to stop receiving push notifications on a device, you can unregister it from from the Liferay instance with the following code:

```
Push.With(Session).Unregister(registrationId);
```

Users can only unregister devices they own.

## iOS

### Registering a device

To receive push notifications, your app must register itself to the Liferay instance first. On the instance side, each device is tied to a user. Each user can have multiple registered devices. A device is represented by a device token string.

Read [Apple's documentation](https://developer.apple.com/library/archive/documentation/NetworkingInternet/Conceptual/RemoteNotificationsPG/HandlingRemoteNotifications.html) to learn how to get the device token. This [tutorial](http://www.raywenderlich.com/32960/apple-push-notification-services-in-ios-6-tutorial-part-1) is also useful to learn how Apple Push Notification works.

Once you have the device token, you can register the device in your `AppDelegate.cs` implementing `RegisteredForRemoteNotifications` method:

```
using LiferayPush;

public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
{
    LRBasicAuthentication BasicAuthentication = new LRBasicAuthentication("test@liferay.com", "test");
    LRSession Session = new LRSession("http://localhost:8080/", BasicAuthentication);
    LiferayPush.LRPush.WithSession(Session)
          .OnSuccess((obj) => Console.WriteLine("Device registered successfully!"))
          .OnFailure((err) => Console.WriteLine($"Device registered failed: {err.LocalizedDescription}"))
          .RegisterDeviceTokenData(deviceToken);
}
```

**If you want to use Liferay 7.x you should manually specify the version** with a call like this:

```
Push.WithPortalVersion(70)
```

Now each time the instance wants to send a push notification to the user `test@liferay.com`, it looks up all registered devices for the user (including the one just registered) and sends the push notification for each `deviceToken` found.

Since all operations are asynchronous, you can set callbacks to check if the registration succeeded or an error occurred on the server side. The `OnSuccess` and `OnFailure` blocks are optional, but it's good practice to implement both. By doing your app can persist the device token or tell the user that an error ocurred.

You should note that the [`LRPush`](https://github.com/liferay-mobile/liferay-push-ios/blob/master/Core/LRPush.swift) class is a wrapper for the Mobile SDK generated services. Internally, it calls the Mobile SDK's `LRPushNotificationsDeviceService` class. While you can still use `LRPushNotificationsDeviceService` directly, using the wrapper class is easier.

Once your device is registered, your app must be able to listen for notifications. [Apple's developer documentation](https://developer.apple.com/library/archive/documentation/NetworkingInternet/Conceptual/RemoteNotificationsPG/HandlingRemoteNotifications.html#//apple_ref/doc/uid/TP40008194-CH6-SW1) shows how to implement this in your app.

### Sending push notifications

You can send push notifications from Liferay instance but also, you can send push notifications from your iOS app. Just make sure the user has the proper permissions in the instance to send push notifications. Just make sure the user has the proper permissions in the instance to send push notifications.

```
NSDictionary Notification = new NSDictionary("body", "Hello!");
LiferayPush.LRPush.WithSession(Session).SendToUserId(USER_ID, Notification);
```

In this code, the push notification is sent to the user specified by `USER_ID`. Upon receiving the notification, the instance looks up all the user's registered devices (both Android and iOS devices) and sends `Notification` as the body of the push notification.

### Unregistering a device

If you want to stop receiving push notifications on a device, you can unregister it from from the instance with the following code:

```
LiferayPush.LRPush.WithSession(Session).UnregisterDeviceToken(deviceToken);
```
    
Users can only unregister devices they own.