using UnityEngine;

namespace GlyphaeScripts
{
    [CreateAssetMenu(fileName = "Need", menuName = "ScriptableObjects/Need")]
    public class Need : ScriptableObject
    {
        #region Serialized Fields

        [Header("Base Values")]
        [Tooltip("The type of need as an enum.")]
        [SerializeField] private NeedTypes type;

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
        /// The type of <see cref="Need"/> as an enum.
        /// </summary>
        public NeedTypes Type { get => type; }

        /// <summary>
        /// The current amount of this <see cref="Need"/>.
        /// </summary>
        public float Current { get => current; set => current = Mathf.Clamp(value, MIN, MAX); }
        
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

    #region Enums

    /// <summary>
    /// Marks the different needs a <see cref="Pet"/> has.
    /// </summary>
    public enum NeedTypes
    {
        /// <summary>
        /// Need is stilled when feeding the <see cref="Pet"/>.
        /// </summary>
        Hunger,

        /// <summary>
        /// Need is stilled when washing the <see cref="Pet"/>.
        /// </summary>
        Health,

        /// <summary>
        /// Need is stilled when playing with the <see cref="Pet"/>.
        /// </summary>
        Joy,

        /// <summary>
        /// Need is stilled when putting the <see cref="Pet"/> to sleep.
        /// </summary>
        Energy
    }

    #endregion
}