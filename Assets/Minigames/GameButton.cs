using System;
using UnityEngine;
using UnityEngine.UI;

namespace GlyphaeScripts
{
    public class GameButton : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField] protected AudioSource sound;
        [SerializeField] protected Button button;
        [SerializeField] protected Image back;
        [SerializeField] protected Image icon;

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

        public static event Action<string> OnInputCheck;

        #endregion


        #region Methods

        public void Setup(AudioClip sound, Sprite display)
        {
            //sound.clip = sound;
            icon.sprite = display;
            back.enabled = true;
        }

        public void Clicked()
        {
            // TODO: Play audio
            OnInputCheck?.Invoke(icon.sprite.name);

        }

        public virtual void SetupDrag(Transform rect) { }

        public virtual void SetSColor(Color color) { }

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