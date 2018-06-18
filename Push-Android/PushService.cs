using System;
using Android.App;
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