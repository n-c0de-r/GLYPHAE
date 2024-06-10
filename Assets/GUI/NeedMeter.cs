using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts
{
    /// <summary>
    /// Class controlling the UI representation of a Pet's need.
    /// </summary>
    public class NeedMeter : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField] private Image slider;
        [SerializeField][Range(0, 100)] private float value;

        #endregion


        #region Fields

        private const float MIN = 0, MAX = 100,  HALF = 50, CRITICAL = 10;
        private bool hasCalled = false;

        #endregion


        #region GetSets / Properties



        #endregion


        #region Unity Built-Ins

        void Awake()
        {
            slider.fillAmount = value / MAX;
            slider.color = new Color((MAX - value) / HALF, value / HALF, 0, 1);
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


        #region Events
        
        

        #endregion


        #region Methods
        
        /// <summary>
        /// Updates the need value by a given amount.
        /// </summary>
        /// <param name="amount">The amount to change the meter.</param>
        public void UpdateValue(float amount)
        {
            StartCoroutine(AnimateFill(value, value+amount, Mathf.Sign(amount)));

            value = Mathf.Clamp(value+amount, MIN, MAX);

            if (!hasCalled && value <= CRITICAL)
            {
                hasCalled = true;
                MessageNeed();
            } 
            else if (hasCalled && value >= MAX-CRITICAL)
            {
                hasCalled = false;
            }
        }

        #endregion


        #region Helpers
        
        private void MessageNeed()
        {
            Debug.Log("Need " + name);
        }

        private IEnumerator AnimateFill(float start, float end, float inc)
        {
            for (float i = start; i != end; i += inc)
            {
                slider.fillAmount = i / MAX;
                slider.color = new Color((MAX - i) / HALF, i / HALF, 0, 1);
                yield return new WaitForEndOfFrame();
            }
        }

        #endregion
    }
}