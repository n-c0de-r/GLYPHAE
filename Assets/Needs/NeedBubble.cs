using System;
using System.Collections;
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
        [SerializeField] private AudioSource audio;

        [Tooltip("The back image of the bubble.")]
        [SerializeField] private SpriteRenderer back;

        [Tooltip("The icon back shown inside the bubble.")]
        [SerializeField] private SpriteRenderer iconBack;

        [Tooltip("The icon outline shown inside the bubble.")]
        [SerializeField] private SpriteRenderer iconFill;

        [Tooltip("The outline of the bubble.")]
        [SerializeField] private SpriteRenderer outline;

        #endregion


        #region Fields

        private string methodAfter;

        #endregion


        #region Events

        public static event Action OnFeedbackDone;

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

        private void OnDestroy()
        {

        }

        #endregion


        #region Methods

        /// <summary>
        /// Sets up the bubble UI. 
        /// </summary>
        /// <param name="sound">The sound to play when displayed.</param>
        /// <param name="display">The sprite to show when displayed.</param>
        public void Setup(AudioClip sound, Sprite display)
        {
            audio.clip = sound;
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
            yield return AnimateFade(settings.AnimationSpeed/2);
            yield return new WaitForSeconds(1f / settings.AnimationSpeed);
            yield return AnimateFade(settings.AnimationSpeed, -1);

            OnFeedbackDone?.Invoke();
        }

        public IEnumerator ShowCall()
        {
            yield return AnimateFade(settings.AnimationSpeed/2);
            yield return new WaitForSeconds(1f / settings.AnimationSpeed);
            yield return AnimateFade(settings.AnimationSpeed, - 1);
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
                Debug.Log(value);

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
                yield return new WaitForSeconds(0.5f / speedFactor);
            }
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