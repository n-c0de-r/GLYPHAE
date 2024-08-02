using System.Collections;
using TMPro;
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
        [Tooltip("The data container holding the actual need values.")]
        [SerializeField] private NeedData need;

        [Tooltip("The UI representation of the name of this need.")]
        [SerializeField] private TMP_Text nameTag;

        [Tooltip("The UI representation of the value of this need.")]
        [SerializeField] private TMP_Text valueTag;

        [Space]
        [Header("Animation Values")]
        [Tooltip("The image to fill.")]
        [SerializeField] private Image slider;

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
            NeedData.OnNeedUpdate += UpdateValue;

            _current = need.Current;
            nameTag.text = gameObject.name;
            valueTag.text = "" + (int)_current;

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
            NeedData.OnNeedUpdate += UpdateValue;
        }

        #endregion


        #region Methods

        /// <summary>
        /// Updates the messaged need value by a given amount.
        /// </summary>
        /// <param name="amount">The amount to update the need value by.</param>
        public void UpdateValue(NeedData incoming, int direction)
        {
            if (incoming == need)
            {
                if (isActiveAndEnabled) StartCoroutine(Animate(direction));
            }
        }

        #endregion


        #region Helpers

        private IEnumerator Animate(int direction)
        {
            yield return AnimateFill(_current, need.Current, direction);
            _current = need.Current;
        }

        /// <summary>
        /// Animates filling the need circle.
        /// </summary>
        private IEnumerator AnimateFill(float start, float end, int inc)
        {
            Color color = Color.black;

            for (int i = (int)(start * NeedData.MAX); i != (int)(end * NeedData.MAX); i += inc)
            {
                float value = (float)i / NeedData.MAX;
                slider.fillAmount = value / NeedData.MAX;
                color.r = (NeedData.MAX - value) / _half;
                color.g = value / _half;
                slider.color = color;
            }
            yield return new WaitForSeconds(0.5f / settings.AnimationSpeed);
        }

        #endregion
    }
}