using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static Unity.Collections.AllocatorManager;

namespace GlyphaeScripts
{
    /// <summary>
    /// Class controlling the color of circular meters UI.
    /// </summary>
    public class CircleMeter : MonoBehaviour
    {
        #region Serialized Fields

        [Header("Need Values")]
        [SerializeField] private Need needType;
        [SerializeField][Range(0, 100)] private float value;

        [Space]
        [Header("Animation Values")]
        [SerializeField] private Image slider;
        [SerializeField][Range(0.1f, 1f)] private float delay = 0.25f;
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
            _half = Pet.MAX / 2;
            slider.fillAmount = value / Pet.MAX;
            fillColor.r = (Pet.MAX - value) / _half;
            fillColor.g = value / _half;
            slider.color = fillColor;
            Settings.OnNeedUpdate += UpdateValue;
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
            Settings.OnNeedUpdate -= UpdateValue;
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
        /// <param name="need">The need enum to update.</param>
        /// <param name="amount">The amount to update the need value by.</param>
        private void UpdateValue(Need need, float amount)
        {
            if (needType == need)
            {
                StartCoroutine(AnimateFill(value, value + amount, Mathf.Sign(amount)));

                value = Mathf.Clamp(value + amount, Pet.MIN, Pet.MAX);
            }
        }

        private IEnumerator AnimateFill(float start, float end, float inc)
        {
            yield return new WaitForSeconds(delay);
            Color color = Color.black;

            for (float i = start; i != end; i += inc)
            {
                slider.fillAmount = i / Pet.MAX;
                color.r = (Pet.MAX - i) / _half;
                color.g = i / _half;
                slider.color = color;
                yield return new WaitForSeconds(speed);
            }
        }

        #endregion
    }
}