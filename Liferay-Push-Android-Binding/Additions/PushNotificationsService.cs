using Android.App;
using Android.Content;
using Org.Json;

namespace Com.Liferay.Mobile.Push.Service
{
    public class PushNotificationsService : IntentService, Push.IOnPushNotification
    {
        Util.GoogleServices GoogleServices = new Util.GoogleServices();

        public PushNotificationsService() { }

        protected override void OnHandleIntent(Intent intent)
        {
            try
            {
                JSONObject PushNotification = GoogleServices.GetPushNotification(this, intent);

                Bus.BusUtil.Post(PushNotification);
                OnPushNotification(PushNotification);
            }
            catch (Exception.PushNotificationReceiverException e)
            {
                Bus.BusUtil.Post(e);
            }
        }

        public virtual void OnPushNotification(JSONObject p0) { }

        public void SetGoogleServices(Util.GoogleServices GoogleServices)
        {
            this.GoogleServices = GoogleServices;
        }
    }
}