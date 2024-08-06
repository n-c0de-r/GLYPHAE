using UnityEngine;

namespace GlyphaeScripts
{
    public class IntroCheck : MonoBehaviour
    {
        #region Serialized Fields
        
        [SerializeField] private Settings settings;
        [SerializeField] private SceneSwitch sceneSwitch;

        [SerializeField] private NotificationsAndroid notifications;

        #endregion


        #region Unity Built-Ins

        void Start()
        {
            settings.NotificationPermission = notifications.RequestNotificationPermission();
            if (settings.NotificationPermission) notifications.RegisterNotificationChanel();
            
            settings.LoadSettings();
            if (settings.SelectedPet != null && !settings.FirstRun) sceneSwitch.Load("2_Game");
            else sceneSwitch.Next();
        }

        #endregion
    }
}