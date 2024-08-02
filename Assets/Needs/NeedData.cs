using System;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;

namespace GlyphaeScripts
{
    /// <summary>
    /// A scriptable object container holding all relevant data of a specific <see cref="Pet"/>'s need.
    /// </summary>
    [CreateAssetMenu(fileName = "Need", menuName = "ScriptableObjects/Need")]
    public class NeedData : ScriptableObject
    {
        #region Serialized Fields

        [Header("Base Values")]
        [Tooltip("The initial value of this need.")]
        [SerializeField][Range(0, 100)] private float initial;

        [Tooltip("The current value of this need.")]
        [SerializeField][Range(0, 100)] private float current;

        [Tooltip("The value at which\r\na need sets a notification.")]
        [SerializeField][Range(10, 50)] private float critical = 20;

        [Space]
        [Header("Feedback Icons")]
        [Tooltip("The icon of the critical call.")]
        [SerializeField] private Sprite alarm;

        [Tooltip("The icon of the positive feedback.")]
        [SerializeField] private Sprite positive;

        [Tooltip("The icon of the negative feedback.")]
        [SerializeField] private Sprite negative;

        #endregion


        #region Fields

        public const int MIN = 0, MAX = 100;
        private float _upFactor, _downFactor, _randomOffset;

        #endregion


        #region GetSets / Properties

        /// <summary>
        /// The current amount of this <see cref="NeedData"/>.
        /// </summary>
        public float Current { get => current; }
        
        /// <summary>
        /// The amount limit where a care call is isued by the <see cref="Pet"/>.
        /// </summary>
        public float Critical { get => critical; }

        /// <summary>
        /// The icon of the critical call.
        /// </summary>
        public Sprite Alarm { get => alarm; }

        /// <summary>
        /// The icon of the positive feedback.
        /// </summary>
        public Sprite Positive { get => positive; }

        /// <summary>
        /// The icon of the negative feedback.
        /// </summary>
        public Sprite Negative { get => negative; }

        #endregion


        #region Events

        public static event Action<NeedData, int> OnNeedUpdate;

        #endregion


        #region Unity Built-Ins

        void Awake()
        {
            
        }

        private void OnEnable()
        {
            
        }

        void Start()
        {
            OnNeedUpdate?.Invoke(this, (int)Mathf.Sign(current));
        }

        void FixedUpdate()
        {
            
        }

        void Update()
        {
            
        }

        private void OnDisable()
        {
            
        }

        private void OnDestroy()
        {
            
        }

        #endregion


        #region Methods

        /// <summary>
        /// Increases the current <see cref="NeedData"/> value with a specific calculation.
        /// </summary>
        /// <param name="value">The base value to add.</param>
        public void Increase(float value)
        {
            if (value == 0) return;
            
            if (current < MIN || current > MAX) return;
            value = value * (_downFactor + _randomOffset);
            current = Mathf.Clamp(current + value, MIN, MAX);
            OnNeedUpdate?.Invoke(this, (int)Mathf.Sign(value));
        }

        /// <summary>
        /// Decreases the current <see cref="NeedData"/> value with a specific calculation.
        /// </summary>
        /// <param name="value">The base value to subtract.</param>
        public void Decrease(float value)
        {
            if (value == 0) return;

            if (current < MIN || current > MAX) return;
            value = -value * (_downFactor + _randomOffset);
            current = Mathf.Clamp(current + value, MIN, MAX);
            OnNeedUpdate?.Invoke(this, (int)Mathf.Sign(value));
        }

        /// <summary>
        /// Sets up the relevant factors for this <see cref="NeedData"/>
        /// </summary>
        /// <param name="upFactor">The factor that is applied when satisfying needs.</param>
        /// <param name="downFactor">The factor that is applied when reducing needs.</param>
        public void SetupFactors(int upFactor, int downFactor)
        {
            _upFactor = upFactor;
            _downFactor = downFactor;
        }


        public void Reset()
        {
            initial = current;
        }

        /// <summary>
        /// Adds a random offset to factors.
        /// They get updated after calls, level-ups and day changes.
        /// </summary>
        /// <param name="calls">The number of calls of a <see cref="Pet"/> satisfied so far. Ramps up randomness.</param>
        public void Randomize(int calls)
        {
            _randomOffset = UnityEngine.Random.Range(-0.13f, +0.13f) * calls;
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