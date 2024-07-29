using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace GlyphaeScripts
{
    /// <summary>
    /// Class controlling the display of circular meters UI for <see cref="NeedData"/>.
    /// </summary>
    public class CircleMeter : MonoBehaviour
    {
        #region Serialized Fields

        [Tooltip("The current Settings for display values.")]
        [SerializeField] private Settings settings;

        [Header("Need Values")]
        [SerializeField] private NeedData need;

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
        private float _current;

        #endregion


        #region Events



        #endregion


        #region GetSets / Properties

        public NeedData Need { get => need; }

        #endregion


        #region Unity Built-Ins

        void Awake()
        {
            _current = 0;
            _half = NeedData.MAX / 2;
            slider.fillAmount = need.Current / NeedData.MAX;
            fillColor.r = (NeedData.MAX - need.Current) / _half;
            fillColor.g = need.Current / _half;
            slider.color = fillColor;
        }

        private void OnEnable()
        {
            //Pet.OnNeedUpdate += UpdateValue;
            UpdateValue(need.Current - _current);
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

        private void OnDisable()
        {
            //Pet.OnNeedUpdate -= UpdateValue;
        }

        #endregion


        #region Methods

        /// <summary>
        /// Updates the messaged need value by a given amount.
        /// </summary>
        /// <param name="amount">The amount to update the need value by.</param>
        public void UpdateValue(float amount)
        {
            StartCoroutine(AnimateFill(need.Current, need.Current + amount, Mathf.Sign(amount)));
        }

        #endregion


        #region Helpers

        //private void MessageNeed()
        //{
        //    Debug.Log("Need " + name);
        //}

        private IEnumerator AnimateFill(float start, float end, float inc)
        {
            yield return new WaitForSeconds(delay);
            Color color = Color.black;

            for (float i = start; i != end; i += inc)
            {
                slider.fillAmount = i / NeedData.MAX;
                color.r = (NeedData.MAX - i) / _half;
                color.g = i / _half;
                slider.color = color;
                yield return new WaitForSeconds(speed / settings.SpeedFactor);
            }
        }

        #endregion
    }
}