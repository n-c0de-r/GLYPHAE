using System;
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

        public static event Action<string> OnInputCheck;

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
            // TODO: Play audio
            //SendMessageUpwards("InputCheck", toCheck);
            OnInputCheck?.Invoke(icon.sprite.name);

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