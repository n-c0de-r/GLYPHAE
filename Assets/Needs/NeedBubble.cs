using System;
using System.Collections;
using UnityEngine;

namespace GlyphaeScripts
{
    public class NeedBubble : MonoBehaviour
    {
        #region Serialized Fields

        [Tooltip("The sound this bubble plays when satisfied.")]
        [SerializeField] private AudioSource sound;

        [Tooltip("The back image of the bubble.")]
        [SerializeField] private SpriteRenderer back;

        [Tooltip("The icon back shown inside the bubble.")]
        [SerializeField] private SpriteRenderer iconBack;

        [Tooltip("The icon outline shown inside the bubble.")]
        [SerializeField] private SpriteRenderer iconFill;

        [Tooltip("The outline of the bubble.")]
        [SerializeField] private SpriteRenderer outline;

        [Tooltip("The current Settings for display values.")]
        [SerializeField] private Settings settings;

        #endregion


        #region Fields

        private string methodAfter;

        #endregion


        #region Events

        public static event Action OnAnimationDone;

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


        #region Methods

        public void Setup(AudioClip sound, Sprite display)
        {
            //sound.clip = sound;
            Setup(display);
        }

        public void Setup(Sprite display)
        {
            if (iconFill == display) return;

            iconFill.sprite = display;
            gameObject.SetActive(true);
        }

        public void Show()
        {
            StartCoroutine(AnimateFade(0, 1, settings.SpeedFactor));
        }

        #endregion


        #region Helpers

        private IEnumerator AnimateFade(float start, float end, float speedFactor)
        {
            Color color;

            for (float i = start; i <= end; i += Time.deltaTime * speedFactor)
            {
                float value = Mathf.Abs(i);
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
                yield return new WaitForEndOfFrame();
            }

            if (end > 0)
            {
                yield return new WaitForSeconds(1f / speedFactor);
                yield return AnimateFade(-1, 0, settings.SpeedFactor * 2);
            }
            OnAnimationDone?.Invoke();
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