![Liferay Mobile SDK logo](https://github.com/liferay/liferay-mobile-sdk/raw/master/logo.png)

# Liferay Push for Xamarin

* [Setup](#setup)
* [Android](#android)
	* [Registering a device](#registering-a-device)
	* [Receiving push notifications](#receiving-push-notifications)
	* [Sending push notifications](#sending-push-notifications)
	* [Unregistering a device](#unregistering-a-device)
* [iOS](#ios)
	* [Registering a device](#registering-a-device)
	* [Receiving push notifications](#receiving-push-notifications)
	* [Sending push notifications](#sending-push-notifications)
	* [Unregistering a device](#unregistering-a-device)

## Setup

Add the NuGet package as a dependency in your solution:

<!-- Dialog in Visual Studio to add NuGet package -->

## Android

### Registering a device

To receive push notifications, your app must register itself to the portal first. On the portal side, each
device is tied to a user. Each user can have multiple registered devices. A device is represented by a device token string. Google calls this the `registrationId`.

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

*Liferay Push for Xamarin (Android)* is calling the *GCM server*, retrieving the results and storing your `registrationId` in the Liferay Portal instance for later use.

Don't forget to add in your `AndroidManifest.xml` the *Internet* permission if you haven't done it already:

```
<uses-permission android:name="android.permission.INTERNET" />
```
		
And, if you are using Liferay 7, you will have to add permissions to be able to register the device in the portal:

<img src="docs/images/Liferay Permissions.png">

All set! If everything went well, you should see a new device registered under the *Push Notifications* menu in *Configuration*.

#### Using the `registrationId` directly without registering against Liferay Portal

If you obtain the token manually, you can register the device to the portal by calling the following method:

```
Push.With(Session).Register("SENDER_ID");
```

Now each time the portal wants to send a push notification to the user `test@liferay.com`, it looks up all registered devices for the user (including the one just registered) and sends the push notification for each `registrationId` found.

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
	
This method only works if you have already registered against Liferay Portal using the previous instructions.

### Sending push notifications

You can send push notifications from Liferay Portal but also, you can send push notifications from your Android app. Just make sure the user has the proper permissions in the portal to send push notifications.

```
JSONObject Notification = new JSONObject();
Notification.Put("body", "Hello!");
Push.With(Session).Send(USER_ID, Notification);
```

In this code, the push notification is sent to the user specified by `USER_ID`. Upon receiving the notification, the portal looks up all the user's registered devices (both Android and iOS devices) and sends `Notification` as the body of the push notification.

### Unregistering a device

If you want to stop receiving push notifications on a device, you can unregister it from from the portal with the following code:

```
Push.With(Session).Unregister(registrationId);
```

Users can only unregister devices they own.

## iOS

### Registering a device

### Receiving push notifications

### Sending push notifications

### Unregistering a device