using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace GlyphaeScripts
{
    /// <summary>
    /// Class controlling the color of circular meters UI.
    /// </summary>
    public class CircleMeter : MonoBehaviour
    {
        #region Serialized Fields

        [Header("Need Values")]
        [SerializeField] private Need need;

        [Space]
        [Header("Animation Values")]
        [Tooltip("The image to fill.")]
        [SerializeField] private Image slider;

        [Tooltip("The delay before the start of animation.")]
        [SerializeField][Range(0.1f, 1f)] private float delay = 0.25f;

        [Tooltip("The speed of animation and delay between frames.")]
        [SerializeField][Range(0.01f, 0.1f)] private float speed = 0.025f;

        #endregion


        #region Fields

        private Color fillColor = Color.green;
        private float _half = 50;

        #endregion


        #region GetSets / Properties



        #endregion


        #region Unity Built-Ins

        void Awake()
        {
            _half = Need.MAX / 2;
            slider.fillAmount = need.Current / Need.MAX;
            fillColor.r = (Need.MAX - need.Current) / _half;
            fillColor.g = need.Current / _half;
            slider.color = fillColor;
            Pet.OnNeedUpdate += UpdateValue;
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

        void OnDestroy()
        {
            Pet.OnNeedUpdate -= UpdateValue;
        }

        #endregion


        #region Events



        #endregion


        #region Methods


        #endregion


        #region Helpers

        //private void MessageNeed()
        //{
        //    Debug.Log("Need " + name);
        //}


        /// <summary>
        /// Updates the messaged need value by a given amount.
        /// </summary>
        /// <param name="type">The need type enum to update.</param>
        /// <param name="amount">The amount to update the need value by.</param>
        private void UpdateValue(Needs type, float amount)
        {
            if (need.Type != type) return;

            StartCoroutine(AnimateFill(need.Current, need.Current + amount, Mathf.Sign(amount)));
        }

        private IEnumerator AnimateFill(float start, float end, float inc)
        {
            yield return new WaitForSeconds(delay);
            Color color = Color.black;

            for (float i = start; i != end; i += inc)
            {
                slider.fillAmount = i / Need.MAX;
                color.r = (Need.MAX - i) / _half;
                color.g = i / _half;
                slider.color = color;
                yield return new WaitForSeconds(speed);
            }
        }

        #endregion
    }
}