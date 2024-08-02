using System;
using UnityEngine;
using UnityEngine.UI;

namespace GlyphaeScripts
{
    public class GameButton : MonoBehaviour
    {
        #region Serialized Fields

        [Header("UI Values")]
        [Tooltip("The sound this plays when clicked.")]
        [SerializeField] protected AudioSource sound;

        [SerializeField] protected Button button;

        [Tooltip("The button's background.")]
        [SerializeField] protected Image back;

        [Tooltip("The button's actual display symbol.")]
        [SerializeField] protected Image icon;

        #endregion


        #region Fields

        GlyphData data;

        #endregion


        #region Events

        public static event Action<GlyphData> OnInput;

        #endregion


        #region GetSets / Properties

        public bool Switch
        {
            set
            {
                button.interactable = value;
                icon.enabled = value;
            }
        }

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


        #region Methods

        /// <summary>
        /// Sets up the buttons relevant data.
        /// </summary>
        /// <param name="gylph"><see cref="GlyphData"/> to compare against on click.</param>
        /// <param name="display">The <see cref="Sprite"/> to display on this input.</param>
        public void Setup(GlyphData gylph, Sprite display)
        {
            //sound.clip = gylph.Sound;
            icon.sprite = display;
            back.enabled = true;
            data = gylph;
        }

        /// <summary>
        /// Wrapper method to call <see cref="OnInput"/> event.
        /// </summary>
        public void Clicked()
        {
            OnInput?.Invoke(data);
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