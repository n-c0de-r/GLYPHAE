using System;
using UnityEngine;
#if UNITY_ANDROID
using UnityEngine.Android;
using Unity.Notifications.Android;


namespace GlyphaeScripts
{
    /// <summary>
    /// Issues notifications to the mobile device.
    /// Source: https://www.youtube.com/watch?v=ulCH0Cd4b_I
    /// https://docs.unity3d.com/ScriptReference/Android.Permission.RequestUserPermission.html
    /// </summary>
    public class NotificationsAndroid : MonoBehaviour
    {
        #region Fields

        private const string REQUEST_MESSAGE = "android.permission.POST_NOTIFICATIONS";

        private const string CHANEL_ID = "Needs";
        private const string CHANEL_NAME = "Need Notifications";
        private const string CHANEL_DESCRIPTION = "Notifications to send when a need is critically low.";

        #endregion


        #region Methods

        public bool RequestNotificationPermission()
        {
            if (!Permission.HasUserAuthorizedPermission(REQUEST_MESSAGE))
            {
                Permission.RequestUserPermission(REQUEST_MESSAGE);
                return true;
            }
            return false;

            // This didn't work well
            // https://docs.unity3d.com/Packages/com.unity.mobile.notifications@2.3/manual/Android.html
            //var request = new PermissionRequest();
            //while (request.Status == PermissionStatus.Allowed)
            //{
            //    Permission.RequestUserPermission(And)

            //}

            // here use request.Status to determine users response
        }

        public void RegisterNotificationChanel()
        {
            AndroidNotificationChannel channel = new()
            {
                Id = CHANEL_ID,
                Name = CHANEL_NAME,
                Importance = Importance.Default,
                Description = CHANEL_DESCRIPTION,
            };
            AndroidNotificationCenter.RegisterNotificationChannel(channel);
        }

        public void SendNotification(string title, string text, DateTime fireAt)
        {
            AndroidNotification notification = new()
            {
                Title = title,
                Text = text,
                FireTime = fireAt,
                SmallIcon = "small",
                LargeIcon = "large",
                ShowTimestamp = true,
            };

            AndroidNotificationCenter.SendNotification(notification, CHANEL_ID);
        }

        public void ClearAllNotifications()
        {
            AndroidNotificationCenter.CancelAllScheduledNotifications();
            AndroidNotificationCenter.CancelAllNotifications();
        }

        #endregion
    }
}
#endif