using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace GlyphaeScripts
{
    public class TimeIcon : MonoBehaviour
    {
        #region Serialized Fields

        [Tooltip("The current Settings for display values.")]
        [SerializeField] private Settings settings;

        [Header("Animation Values")]
        [Tooltip("The image to fill.")]
        [SerializeField] private Image timerCircle;

        [Tooltip("The delay before the start of animation.")]
        [SerializeField][Range(0.1f, 1f)] private float delay = 0.25f;

        [SerializeField] private Image icon;

        #endregion


        #region Events

        public static event Action OnAnimationDone;

        #endregion


        #region Methods

        /// <summary>
        /// Starts the circle-fill animation
        /// </summary>
        public void Animate()
        {
            StartCoroutine(WrapMethod());
        }

        /// <summary>
        /// Sets up this icon for display.
        /// </summary>
        /// <param name="display">The sprite to show.</param>
        public void Setup(Sprite display)
        {
            icon.sprite = display;
        }

        public void Disable()
        {
            icon.enabled = false;
        }

        #endregion


        #region Helpers

        /// <summary>
        /// Needed to delay after-animation instructions.
        /// </summary>
        /// <returns></returns>
        private IEnumerator WrapMethod()
        {
            yield return StartCoroutine(AnimateFill());
            timerCircle.fillAmount = 0;
            OnAnimationDone?.Invoke();
        }

        /// <summary>
        /// Animates filling the circle around an icon.
        /// </summary>
        private IEnumerator AnimateFill()
        {
            yield return new WaitForSeconds(delay);

            while (timerCircle.fillAmount < 1)
            {
                timerCircle.fillAmount += Time.deltaTime * (settings.AnimationSpeed) / 3;
                yield return new WaitForEndOfFrame();
            }
        }

        #endregion
    }
}