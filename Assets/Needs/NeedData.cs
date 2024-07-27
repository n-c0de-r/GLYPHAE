using System;
using UnityEngine;

namespace GlyphaeScripts
{
    /// <summary>
    /// A scriptable object container holding all relevant data of a specific <see cref="Pet"/>'s <see cref="NeedTypes"/>
    /// </summary>
    [CreateAssetMenu(fileName = "Need", menuName = "ScriptableObjects/Need")]
    public class NeedData : ScriptableObject
    {
        #region Serialized Fields

        [Header("Base Values")]
        //[Tooltip("The type of need as an enum.")]
        //[SerializeField] private NeedTypes type;

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

        #endregion


        #region GetSets / Properties

        /// <summary>
        /// The type of <see cref="NeedData"/> as an enum.
        /// </summary>
        //public NeedTypes Type { get => type; }

        /// <summary>
        /// The current amount of this <see cref="NeedData"/>.
        /// </summary>
        public float Current { get => current; set => current = Mathf.Clamp(value, MIN, MAX); }
        
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


        #region Events

        public static event Action<float> OnNeedUpdate;

        #endregion


        #region Methods

        public void Reset()
        {
            initial = current;
        }

        #endregion


        #region Helpers
        
        

        private void TemplateHelper(bool param)
        {
            
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