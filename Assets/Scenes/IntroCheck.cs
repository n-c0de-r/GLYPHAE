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


        #region Fields



        #endregion


        #region GetSets / Properties



        #endregion


        #region Unity Built-Ins

        void Awake()
        {
            
        }

        void Start()
        {
            settings.NotificationPermission = notifications.RequestNotificationPermission();
            if (settings.NotificationPermission) notifications.RegisterNotificationChanel();
            
            settings.LoadSettings();
            if (settings.SelectedPet != null && !settings.FirstRun) sceneSwitch.Load("2_Game");
            else sceneSwitch.Next();
        }

        void FixedUpdate()
        {
            
        }

        void Update()
        {
            
        }

        #endregion


        #region Events



        #endregion


        #region Methods



        public void TemplateMethod(bool param)
        {
            
        }

        #endregion


        #region Helpers
        
        

        private void TemplateHelper(bool param)
        {
            
        }

        #endregion


        #region Gizmos

        private void OnDrawGizmos()
        {
            
        }

        private void OnDrawGizmosSelected()
        {
             
        }

        #endregion
    }
}