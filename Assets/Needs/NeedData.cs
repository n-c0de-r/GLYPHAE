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
        [Tooltip("The icon of the need.")]
        [SerializeField] private Sprite icon;

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
        [SerializeField][TextArea(1, 3)] private string description;

        #endregion


        #region Fields

        private DateTime _callTime;
        public const int MIN = 0, MAX = 100;
        public const float RANDOM_MIN = -0.13f, RANDOM_MAX = 0.13f;
        private float _upFactor, _downFactor, _incrementValue, _randomOffset;
        private bool _isCritical = false;

        #endregion


        #region GetSets / Properties

        /// <summary>
        /// The current amount of this <see cref="NeedData"/>.
        /// </summary>
        public float Current { get => current; }
        
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
        /// Gets the calculated increment value.
        /// </summary>
        public float Increment { get => _incrementValue; }

        /// <summary>
        /// Gets the hidden random offset value.
        /// </summary>
        public float RandomOffset { get => _randomOffset; }

        /// <summary>
        /// The icon of the need.
        /// </summary>
        public Sprite Icon { get => icon; }

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
        }

        /// <summary>
        /// Sets up the relevant factors for this <see cref="NeedData"/>
        /// </summary>
        /// <param name="upFactor">The factor that is applied when satisfying needs.</param>
        /// <param name="downFactor">The factor that is applied when reducing needs.</param>
        /// <param name="increment">The increment this need rises or falls by.</param>
        /// <param name="randomFactor">The factor to calculate randomness offset by.</param>
        public void SetupValues(int upFactor, int downFactor, float increment, int randomFactor)
        {
            _upFactor = upFactor;
            _downFactor = downFactor;
            _incrementValue = increment;
            Randomize(randomFactor);
        }

        /// <summary>
        /// Resets this <see cref="NeedData"/> back to its initial values.
        /// </summary>
        /// <param name="value">The value to set.</param>
        /// <param name="randomness">The randomness offset to set.</param>
        public void SetValue(float value, float increment, float randomness)
        {
            current = value;
            _incrementValue = increment;
            _randomOffset = randomness;

            //OnNeedUpdate?.Invoke(this, (int)Mathf.Sign(current));

            if (!_isCritical && current < criticalLimit)
                _isCritical = true;

            if (_isCritical && current > criticalLimit)
                _isCritical = false;
            
            OnNeedCritical?.Invoke(this, _isCritical);
        }

        /// <summary>
        /// Increases the current <see cref="NeedData"/> value with a specific calculation.
        /// </summary>
        /// <param name="factor">The factor to influence increment by additionally.</param>
        public void Increase(float factor = 1)
        {
            if (factor == 0) return;
            
            if (current < MIN || current > MAX) return;
            factor = _incrementValue * factor * 5 * (_upFactor + _randomOffset);
            current = Mathf.Clamp(current + factor, MIN, MAX);
            OnNeedUpdate?.Invoke(this, (int)Mathf.Sign(factor));

            if (_isCritical && current > criticalLimit)
            {
                _isCritical = false;
                OnNeedCritical?.Invoke(this, _isCritical);
            }
        }

        /// <summary>
        /// Decreases the current <see cref="NeedData"/> value with a specific calculation.
        /// </summary>
        /// <param name="value">The factor to influence increment by additionally.</param>
        public void Decrease(float factor = 1f)
        {
            if (factor == 0) return;

            if (current < MIN || current > MAX) return;
            factor = -_incrementValue * factor * (_downFactor + _randomOffset) / 5;
            current = Mathf.Clamp(current + factor, MIN, MAX);
            OnNeedUpdate?.Invoke(this, (int)Mathf.Sign(factor));

            if (!_isCritical && current < criticalLimit)
            {
                _isCritical = true;
                OnNeedCritical?.Invoke(this, _isCritical);
            }
        }

        #endregion


        #region Helpers

        /// <summary>
        /// Adds a random offset to factors.
        /// They get updated after calls, level-ups and day changes.
        /// </summary>
        /// <param name="calls">The number of calls of a <see cref="Pet"/> satisfied so far. Ramps up randomness.</param>
        /// <returns>The offset to apply to all calculations of this need.</returns>
        public float Randomize(int calls)
        {
            _randomOffset = UnityEngine.Random.Range(RANDOM_MIN, RANDOM_MAX) * calls;
            return _randomOffset;
        }

        /// <summary>
        /// Sets up a mobile notification based on <see cref="NeedData"/> values.
        /// <param name="value">The base value to subtract.</param>
        /// </summary>
        public void CalculateNotification()
        {
            _incrementValue *= (_downFactor + _randomOffset);
            int minutes = (int)(current / _incrementValue);
            if (minutes <= 0) return;

            DateTime now = DateTime.Now;
            _callTime = now.AddMinutes(minutes);

            if ((_callTime.Day > DateTime.Now.Day && _callTime.Hour < settings.SilenceEnd) || _callTime.Hour > settings.SilenceStart)
                _callTime = new DateTime(_callTime.Year, _callTime.Month, _callTime.Day, settings.SilenceEnd, _callTime.Minute, _callTime.Second);
            notifications.SendNotification(title, description, minutes);
        }

        #endregion
    }
}