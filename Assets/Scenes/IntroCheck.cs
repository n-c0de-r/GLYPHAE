using UnityEngine;

namespace GlyphaeScripts
{
    public class IntroCheck : MonoBehaviour
    {
        #region Serialized Fields
        
        [SerializeField] private Settings settings;
        [SerializeField] private SceneSwitch sceneSwitch;

        #endregion


        #region Fields



        #endregion


        #region GetSets / Properties



        #endregion


        #region Unity Built-Ins

        void Awake()
        {
            settings.LoadSettings();

            if (!settings.FirstRun) sceneSwitch.Next();
        }

        void Start()
        {
            
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