using Unity.Notifications.Android;
using UnityEngine.Android;
using System;

namespace GlyphaeScripts
{
    /// <summary>
    /// Issues notifications to the mobile device.
    /// Source: https://www.youtube.com/watch?v=ulCH0Cd4b_I
    /// https://docs.unity3d.com/ScriptReference/Android.Permission.RequestUserPermission.html
    /// </summary>
    public static class NotificationsAndroid
    {
        #region Fields

        private const string REQUEST_MESSAGE = "android.permission.POST_NOTIFICATIONS";
        private const string CHANEL_GROUP_ID = "GLYPHAE Needs";
        private const string CHANEL_GROUP_NAME = "GLYPHAE Pet's Need Calls";

        private const string CHANEL_ID = "Needs";
        private const string CHANEL_NAME = "Need Notifications";
        private const string CHANEL_DESCRIPTION = "Notifications to send when a need is critically low.";

        #endregion


        #region Methods

        public static void RequestNotificationPermission()
        {

            if (!Permission.HasUserAuthorizedPermission(REQUEST_MESSAGE))
            {
                Permission.RequestUserPermission(REQUEST_MESSAGE );
            }

            // This didn't work well
            // https://docs.unity3d.com/Packages/com.unity.mobile.notifications@2.3/manual/Android.html
            //var request = new PermissionRequest();
            //while (request.Status == PermissionStatus.Allowed)
            //{
            //    Permission.RequestUserPermission(And)

            //}

            // here use request.Status to determine users response
        }

        public static void RegisterNotificationChanel()
        {
            AndroidNotificationChannelGroup group = new()
            {
                Id = CHANEL_GROUP_ID,
                Name = CHANEL_GROUP_NAME,
            };

            AndroidNotificationCenter.RegisterNotificationChannelGroup(group);

            AndroidNotificationChannel channel = new()
            {
                Id = CHANEL_ID,
                Name = CHANEL_NAME,
                Importance = Importance.Default,
                Description = CHANEL_DESCRIPTION,
                Group = CHANEL_GROUP_ID,
            };
            AndroidNotificationCenter.RegisterNotificationChannel(channel);
        }

        public static void SendNotification(string title, string text, int fireTimeInMinutes)
        {
            AndroidNotification notification = new()
            {
                Title = title,
                Text = text,
                FireTime = DateTime.Now.AddMinutes(fireTimeInMinutes),
                SmallIcon = "small",
                LargeIcon = "large"
            };

            AndroidNotificationCenter.SendNotification(notification, CHANEL_ID);
        }

        public static void ClearAllNotifications()
        {
            AndroidNotificationCenter.CancelAllScheduledNotifications();
        }

        #endregion
    }
}