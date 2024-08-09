using System;
using UnityEngine;
using UnityEngine.UI;

namespace GlyphaeScripts
{
    public class GameButton : MonoBehaviour
    {
        #region Serialized Fields

        [Tooltip("The current Settings for display values.")]
        [SerializeField] protected Settings settings;

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

        private GlyphData data;

        #endregion


        #region Events

        public static event Action<GlyphData> OnInput;
        public static event Action<GlyphData, GameButton> OnMatch;

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


        #region Methods

        /// <summary>
        /// Sets up the buttons relevant data.
        /// </summary>
        /// <param name="gylph"><see cref="GlyphData"/> to compare against on click.</param>
        /// <param name="display">The <see cref="Sprite"/> to display on this input.</param>
        public void Setup(GlyphData gylph, Sprite display)
        {
            sound.clip = gylph.Sound;
            icon.sprite = display;
            data = gylph;
        }

        /// <summary>
        /// Wrapper method to call <see cref="OnInput"/> event.
        /// </summary>
        public void Clicked()
        {
            OnInput?.Invoke(data);
            OnMatch?.Invoke(data, this);
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