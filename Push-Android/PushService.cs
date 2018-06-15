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
            Console.WriteLine(json);
        }
    }
}