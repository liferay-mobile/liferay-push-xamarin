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