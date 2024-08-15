using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace GlyphaeScripts
{
    public class NeedBubble : MonoBehaviour
    {
        #region Serialized Fields

        [Tooltip("The current Settings for display values.")]
        [SerializeField] private Settings settings;

        [Header("UI Values")]
        [Tooltip("The sound this bubble plays when satisfied.")]
        [SerializeField] private AudioSource audioSource;

        [Tooltip("The back image of the bubble.")]
        [SerializeField] private SpriteRenderer back;

        [Tooltip("The icon back shown inside the bubble.")]
        [SerializeField] private SpriteRenderer iconBack;

        [Tooltip("The icon outline shown inside the bubble.")]
        [SerializeField] private SpriteRenderer iconFill;

        [Tooltip("The outline of the bubble.")]
        [SerializeField] private SpriteRenderer outline;

        [SerializeField] private TMP_Text valueLabel;
        [SerializeField] private TMP_Text waitLabel;
        [SerializeField] private TMP_Text deltaLabel;

        #endregion


        #region Fields

        private IEnumerator lastCoroutine;

        #endregion


        #region Events

        public static event Action OnFeedbackDone;

        #endregion


        #region Methods

        /// <summary>
        /// Sets up the bubble UI. 
        /// </summary>
        /// <param name="sound">The sound to play when displayed.</param>
        /// <param name="display">The sprite to show when displayed.</param>
        public void Setup(AudioClip sound, Sprite display)
        {
            audioSource.clip = sound;
            Setup(display);
        }

        /// <summary>
        /// Sets up the bubble UI. 
        /// </summary>
        /// <param name="sound">The sound to play when displayed.</param>
        /// <param name="display">The sprite to show when displayed.</param>
        public void Setup(Sprite display)
        {
            if (iconFill == display) return;
            iconFill.sprite = display;
        }

        /// <summary>
        /// Shows the feedback bubble.
        /// </summary>
        public IEnumerator ShowFeedback()
        {
            if (settings.DebugMode)
            {
                valueLabel.transform.parent.gameObject.SetActive(true);
                waitLabel.transform.parent.gameObject.SetActive(true);
                deltaLabel.transform.parent.gameObject.SetActive(true);
            }

            yield return lastCoroutine = AnimateFade(settings.AnimationSpeed);
            yield return new WaitForSeconds(1f / settings.AnimationSpeed);
            yield return lastCoroutine = AnimateFade(settings.AnimationSpeed * 2, -1);

            OnFeedbackDone?.Invoke();

            if (settings.DebugMode)
            {
                valueLabel.transform.parent.gameObject.SetActive(false);
                waitLabel.transform.parent.gameObject.SetActive(false);
                deltaLabel.transform.parent.gameObject.SetActive(false);
            }
        }

        public IEnumerator ShowCall()
        {
            if (settings.DebugMode)
            {
                valueLabel.transform.parent.gameObject.SetActive(true);
                waitLabel.transform.parent.gameObject.SetActive(true);
                deltaLabel.transform.parent.gameObject.SetActive(true);
            }

            audioSource.Play();
            yield return lastCoroutine = AnimateFade(settings.AnimationSpeed);
            yield return new WaitForSeconds(1f / settings.AnimationSpeed);
            yield return lastCoroutine = AnimateFade(settings.AnimationSpeed * 2, - 1);

            if (settings.DebugMode)
            {
                valueLabel.transform.parent.gameObject.SetActive(false);
                waitLabel.transform.parent.gameObject.SetActive(false);
                deltaLabel.transform.parent.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Stops the fading animation and hides this <see cref="NeedBubble"/>.
        /// </summary>
        public void Disable()
        {
            //TODO BUGS: Doesn't work on devices
            StopAllCoroutines();
            if (lastCoroutine != null) StopCoroutine(lastCoroutine);

            Color color;
            color = back.color;
            color.a = 0;
            back.color = color;

            color = iconBack.color;
            color.a = 0;
            iconBack.color = color;

            color = iconFill.color;
            color.a = 0;
            iconFill.color = color;

            color = outline.color;
            color.a = 0;
            outline.color = color;
        }

        #endregion


        #region Helpers

        private IEnumerator AnimateFade(float speedFactor, int direction = 1)
        {
            Color color;
            int increment = (int)speedFactor;

            for (int i = 0; i < 100; i += increment)
            {
                float value = i / 100.0f * direction;

                color = back.color;
                color.a = value;
                back.color = color;

                color = iconBack.color;
                color.a = value;
                iconBack.color = color;

                color = iconFill.color;
                color.a = value;
                iconFill.color = color;

                color = outline.color;
                color.a = value;
                outline.color = color;

                if (settings.DebugMode)
                {
                    valueLabel.text = ""+value;
                    waitLabel.text = "" + (0.01f / speedFactor);
                    deltaLabel.text = ""+Time.deltaTime;
                }

                yield return new WaitForSeconds(0.01f / speedFactor);
            }
        }

        #endregion
    }
}