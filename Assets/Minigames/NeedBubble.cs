using UnityEngine;
using UnityEngine.UI;

namespace GlyphaeScripts
{
    public class NeedBubble : MonoBehaviour
    {
        #region Serialized Fields

        [Tooltip("The sound this bubble plays when satisfied.")]
        [SerializeField] private AudioSource sound;

        [Tooltip("The back image of the bubble.")]
        [SerializeField] private Image back;

        [Tooltip("The icon shown inside the bubble.")]
        [SerializeField] private Image icon;

        [Tooltip("The outline of the bubble.")]
        [SerializeField] private Image outline;

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

        public void Setup(AudioClip sound, Sprite display)
        {
            //sound.clip = sound;
            icon.sprite = display;
            gameObject.gameObject.SetActive(true);
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