using UnityEngine;

namespace GlyphaeScripts
{
    public class IntroCheck : MonoBehaviour
    {
        #region Serialized Fields
        
        [SerializeField] private Settings settings;
        [SerializeField] private SceneSwitch sceneSwitch;
        #if UNITY_ANDROID
        [SerializeField] private NotificationsAndroid notifications;
        #endif
        #endregion


        #region Unity Built-Ins

        void Start()
        {
            #if UNITY_ANDROID
            settings.NotificationPermission = notifications.RequestNotificationPermission();
            if (settings.NotificationPermission) notifications.RegisterNotificationChanel();
            #endif
            settings.LoadSettings();
            if (settings.SelectedPet != null && !settings.FirstRun) sceneSwitch.Load("2_Game");
            else sceneSwitch.Next();
        }

        #endregion
    }
}