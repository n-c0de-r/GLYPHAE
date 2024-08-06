using System;
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

        [Tooltip("The current Settings for display values.")]
        [SerializeField] private Settings settings;

        [Header("Base Values")]
        [Tooltip("The initial value of this need.")]
        [SerializeField][Range(0, 100)] private float initial;

        [Tooltip("The current value of this need.")]
        [SerializeField][Range(0, 100)] private float current;

        [Tooltip("The lower limit value at\r\nwhich a need sets a notification.")]
        [SerializeField][Range(10, 30)] private float criticalLimit = 20;

        [Tooltip("The upper limit value at\r\nwhicha need call is satisfied.")]
        [SerializeField][Range(70, 90)] private float satisfiedLimit = 80;

        [Space]
        [Header("Feedback Icons")]
        [Tooltip("The icon of the critical call.")]
        [SerializeField] private Sprite alarm;

        [Tooltip("The icon of the positive feedback.")]
        [SerializeField] private Sprite positive;

        [Tooltip("The icon of the negative feedback.")]
        [SerializeField] private Sprite negative;

        [Space]
        [Header("Notification Values")]
        [Tooltip("The notification managment system.")]
        [SerializeField] private NotificationsAndroid notifications;
        [Tooltip("The title text to show for the notification.")]
        [SerializeField] private string title;
        [Tooltip("The Notification text message.")]
        [SerializeField][TextArea(1, 3)] protected string description;

        #endregion


        #region Fields

        public const int MIN = 0, MAX = 100;
        public const float RANDOM_MIN = -0.13f, RANDOM_MAX = 0.13f;
        private float _upFactor, _downFactor, _randomOffset;
        [SerializeField] private bool _isCritical = false;

        #endregion


        #region GetSets / Properties

        /// <summary>
        /// The current amount of this <see cref="NeedData"/>.
        /// </summary>
        public float Current { get => current; set => current = value; }
        
        /// <summary>
        /// The amount limit where a care call is issued by the <see cref="Pet"/>.
        /// </summary>
        public float Critical { get => criticalLimit; }

        /// <summary>
        /// The upper limit value at
        /// whicha need call is satisfied.
        /// </summary>
        public float SatisfiedLimit { get => satisfiedLimit; }

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


        #region Debug

        /// <summary>
        /// Gets the hidden up factor value.
        /// Only for debugging on hardware.
        /// </summary>
        public float UpFactor { get => _upFactor; }

        /// <summary>
        /// Gets the hidden down factor value.
        /// Only for debugging on hardware.
        /// </summary>
        public float DownFactor { get => _downFactor; }

        /// <summary>
        /// Gets the hidden random offset value.
        /// Only for debugging on hardware.
        /// </summary>
        public float RandomOffset { get => _randomOffset; }

        #endregion Debug

        #endregion


        #region Events

        public static event Action<NeedData, int> OnNeedUpdate;
        public static event Action<NeedData, bool> OnNeedCritical;

        #endregion


        #region Methods

        /// <summary>
        /// Resets this <see cref="NeedData"/> back to its initial values.
        /// </summary>
        public void Initialize()
        {
            current = initial;
            OnNeedUpdate?.Invoke(this, (int)Mathf.Sign(current));

            if (!_isCritical && current <= criticalLimit)
            {
                OnNeedCritical?.Invoke(this, true);
                _isCritical = true;
            }

            if (_isCritical && current > criticalLimit)
            {
                OnNeedCritical?.Invoke(this, false);
            }

            if (_isCritical && current > satisfiedLimit)
            {
                _isCritical = false;
            }
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

        /// <summary>
        /// Adds a random offset to factors.
        /// They get updated after calls, level-ups and day changes.
        /// </summary>
        /// <param name="calls">The number of calls of a <see cref="Pet"/> satisfied so far. Ramps up randomness.</param>
        /// <param name="seed">The random seed.</param>
        public long Randomize(int calls, long seed = -1)
        {
            if (seed != -1) UnityEngine.Random.InitState((int)seed);
            else seed = DateTime.Now.Ticks;
            UnityEngine.Random.InitState((int)seed);
            _randomOffset = UnityEngine.Random.Range(RANDOM_MIN, RANDOM_MAX) * calls;
            return seed;
        }

        /// <summary>
        /// Increases the current <see cref="NeedData"/> value with a specific calculation.
        /// </summary>
        /// <param name="value">The base value to add.</param>
        public void Increase(float value)
        {
            if (value == 0) return;
            
            if (current < MIN || current > MAX) return;
            value = value * (_upFactor + _randomOffset);
            current = Mathf.Clamp(current + value, MIN, MAX);
            OnNeedUpdate?.Invoke(this, (int)Mathf.Sign(value));

            if (_isCritical && current > criticalLimit)
            {
                OnNeedCritical?.Invoke(this, false);
            }

            if (_isCritical && current > satisfiedLimit)
            {
                _isCritical = false;
            }
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

            if (!_isCritical && current <= criticalLimit)
            {
                OnNeedCritical?.Invoke(this, true);
                _isCritical = true;
            }
        }

        #endregion


        #region Helpers

        /// <summary>
        /// Sets up a mobile notification based on <see cref="NeedData"/> values.
        /// <param name="value">The base value to subtract.</param>
        /// </summary>
        public void CalculateNotification(float value)
        {
            value *= (_downFactor + _randomOffset);
            int minutes = (int)(current / value);
            if (minutes <= 0) return;
            DateTime now = DateTime.Now;
            DateTime later = now.AddMinutes(minutes);
            if (later.Hour < settings.SilenceEnd || later.Hour > settings.SilenceStart) return;
            notifications.SendNotification(title, description, minutes);
        }

        #endregion
    }
}