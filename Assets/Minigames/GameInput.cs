using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GlyphaeScripts
{
    public class GameInput : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField] private AudioSource sound;
        [SerializeField] private Button button;
        [SerializeField] private Image back;
        [SerializeField] private Image icon;

        #endregion


        #region Fields
        
        private string toCheck;

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
            toCheck = display.name;
            back.enabled = true;
        }

        public void Clicked()
        {
            SendMessageUpwards("InputCheck", toCheck);
        }

        #endregion


        #region Helpers

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