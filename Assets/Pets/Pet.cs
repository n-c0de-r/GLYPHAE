using System;
using System.Collections.Generic;
using UnityEngine;

namespace GlyphaeScripts
{
    /// <summary>
    /// Represents a the Pet with all behaviors 
    /// and data in the game. Refe
    /// </summary>
    [RequireComponent(typeof(AudioSource), typeof(BoxCollider2D), typeof(SpriteRenderer))]
    public class Pet : MonoBehaviour
    {
        #region Serialized Fields

        [Header("Need values")]
        [Tooltip("Basic needs of the pet:\r\nHunger, Health, Joy, Energy.")]
        [SerializeField] private NeedData[] needs;

        [Space]
        [Tooltip("Displays the Pet's need.")]
        [SerializeField] private NeedBubble needCall;

        [Tooltip("Displays the Pet's feedback on actions.")]
        [SerializeField] private NeedBubble needFeedback;

        [Header("Game related values")]
        [Tooltip("The sprites this Pet\r\ngoes through its evolutions")]
        [SerializeField] private Sprite[] levelSprites;

        [Tooltip("The list of Glyphs\r\nthis Pet needs to learn.")]
        [SerializeField] private List<GlyphData> literals;

        [Tooltip("Gets the current state\r\nif the Pet is already unlocked.")]
        [SerializeField] private bool unlocked;

        [Header("GUI Values")]
        [Tooltip("The name of this Pet.")]
        [SerializeField] private string petName;

        [Tooltip("The category of literals this Pet holds.")]
        [SerializeField] private string category;

        [Tooltip("A short description of this Pet.")]
        [SerializeField][TextArea(3, 10)] protected string description;

        [Tooltip("The Pet's icon for the menu.")]
        [SerializeField] private Sprite symbol;

        #endregion


        #region Fields

        private Animator _animator;
        private AudioSource _audioSource;
        private SpriteRenderer _spriteRenderer;
        private BoxCollider2D _boxCollider;

        public Evolutions _level = Evolutions.Egg;
        private float _hungerIncrement, _healthIncrement, _joyIncrement, _energyIncrement;
        private int _evolutionCalls = 3;
        private int _sicknessChance, _sicknessChanceFactor, _sickCount;
        private bool hasCalled = false;

        #endregion


        #region Events

        public static event Action<Sprite, List<GlyphData>> OnNeedCall;
        public static event Action<bool> OnNeedSatisfied;
        float timer = 60;
        public int _debugTimeFactor = 1;
        public int minutes = 0;

        #endregion


        #region GetSets / Properties

        /// <summary>
        /// The current Hunger <see cref="NeedData"/> container.
        /// </summary>
        public NeedData Hunger { get => needs[0]; }

        /// <summary>
        /// The current Health <see cref="NeedData"/> container.
        /// </summary>
        public NeedData Health { get => needs[1]; }

        /// <summary>
        /// The current Joy <see cref="NeedData"/> container.
        /// </summary>
        public NeedData Joy { get => needs[2]; }

        /// <summary>
        /// The current Energy <see cref="NeedData"/> container.
        /// </summary>
        public NeedData Energy { get => needs[3]; }

        /// <summary>
        /// The list of all <see cref="GlyphData"/>s
        /// this <see cref="Pet"/> needs to learn.
        /// </summary>
        public List<GlyphData> Literals { get => literals; set => literals = value; }

        /// <summary>
        /// The current state if
        /// the <see cref="Pet"/> is already unlocked.
        /// </summary>
        public bool Unlocked { get => unlocked; }



        /// <summary>
        /// The name of this <see cref="Pet"/>.
        /// </summary>
        public string Name { get => petName; }

        /// <summary>
        /// The category of <see cref="GlyphData"/>s this <see cref="Pet"/> holds.
        /// </summary>
        public string Category { get => category; }

        /// <summary>
        /// A short description of this <see cref="Pet"/>.
        /// </summary>
        public string Description { get => description; }

        /// <summary>
        /// The <see cref="Pet"/>'s icon for the menu.
        /// </summary>
        public Sprite Symbol { get => symbol; }

        /// <summary>
        /// The current <see cref="Evolutions"/> level
        /// enum of this <see cref="Pet"/>
        /// </summary>
        public Evolutions Level { get => _level; }

        /// <summary>
        /// Set the current <see cref="Evolutions"/> level.
        /// Only for debugging on hardware.
        /// </summary>
        public int LevelValue { set => _level = (Evolutions)value; }

        /// <summary>
        /// Sets the time factor value to speed up display.
        /// Only for debugging on hardware.
        /// </summary>
        public float TimeFactor { set => _debugTimeFactor = (int)value; }

        #endregion


        #region Unity Built-Ins

        void Awake()
        {
            if (_animator == null) TryGetComponent(out _animator);
            if (_audioSource == null) TryGetComponent(out _audioSource);
            if (_spriteRenderer == null) TryGetComponent(out _spriteRenderer);
            if (_boxCollider == null) TryGetComponent(out _boxCollider);

            if (PlayerPrefs.HasKey(nameof(Evolutions))) _level = (Evolutions)PlayerPrefs.GetInt(nameof(Evolutions));
            if (PlayerPrefs.HasKey(nameof(_evolutionCalls))) _evolutionCalls = PlayerPrefs.GetInt(nameof(_evolutionCalls));

            CalculateNeedFactors();
        }

        private void OnEnable()
        {
            _spriteRenderer.sprite = levelSprites[(int)_level];

            GlyphData.OnCorrectGuess += Feedback;
            GlyphData.OnWrongGuess += Feedback;
            Minigame.OnNextRound += Call;

            CheckEvolution();
        }

        void Start()
        {
            
        }

        void FixedUpdate()
        {
            timer -= Time.fixedDeltaTime * _debugTimeFactor;

            if (timer <= 0)
            {
                UpdateNeeds();
                timer = 60;
                minutes++;
            }
        }

        void Update()
        {

        }

        private void OnDisable()
        {
            GlyphData.OnCorrectGuess -= Feedback;
            GlyphData.OnWrongGuess -= Feedback;
            Minigame.OnNextRound -= Call;
        }

        #endregion


        #region Methods

        public void IncreaseLevel()
        {
            if ((int)_level >= levelSprites.Length) return;
            _level++;
            _spriteRenderer.sprite = levelSprites[(int)_level];
            CalculateNeedFactors();
        }

        #endregion


        #region Helpers

        private void UpdateNeeds()
        {
            Hunger.Decrease(_hungerIncrement);
            Health.Decrease(_healthIncrement * _sickCount);
            Joy.Decrease(_joyIncrement);
            Energy.Decrease(_energyIncrement);

            _sicknessChance = (int)(NeedData.MAX - Health.Current) * _sicknessChanceFactor;
        }

        private void CheckEvolution()
        {
            if (_evolutionCalls >= Enum.GetValues(typeof(Evolutions)).Length)
            {
                IncreaseLevel();
            }
        }

        private void Call(Sprite sprite)
        {
            needCall.Setup(sprite);
            StartCoroutine(needCall.ShowCall());
        }

        private void Feedback(Sprite sprite)
        {
            needFeedback.Setup(sprite);
            StartCoroutine(needFeedback.ShowFeedback());
        }

        private void CalculateNeedFactors()
        {
            _hungerIncrement = CalculateNeedIncrement();
            Hunger.SetupFactors(CalculateReverseCurve(), CalculateReverseLine());
            Hunger.Randomize(_evolutionCalls);

            _healthIncrement = CalculateNeedIncrement();
            Health.SetupFactors(0, 0);
            Health.Randomize(_evolutionCalls);

            _joyIncrement = CalculateNeedIncrement();
            Joy.SetupFactors(CalculateReverseLine(), CalculateCurve());
            Joy.Randomize(_evolutionCalls);

            _energyIncrement = CalculateNeedIncrement();
            Energy.SetupFactors(CalculateReverseLine(), CalculateReverseCurve());
            Energy.Randomize(_evolutionCalls);

            _sicknessChanceFactor = CalculateReverseCurve();
        }

        private int CalculateCurve()
        {
            return (int)Evolutions.Teen - Mathf.Abs((int)_level - (int)Evolutions.Teen);
        }

        private int CalculateReverseCurve()
        {
            return Mathf.Abs((int)_level - (int)Evolutions.Teen) + 1;
        }

        private int CalculateReverseLine()
        {
            return (int)Enum.GetValues(typeof(Evolutions)).Length - (int)_level;
        }

        private float CalculateNeedIncrement()
        {
            return UnityEngine.Random.Range(0.07f, 0.13f) * CalculateReverseLine();
        }

        #endregion
    }


    #region Enums

    /// <summary>
    /// The <see cref="Evolutions"/> levels a <see cref="Pet"/> goes through.
    /// </summary>
    public enum Evolutions
    {
        /// <summary>
        /// Initial starting form. Has no interactions.
        /// </summary>
        Egg,

        /// <summary>
        /// 
        /// </summary>
        Baby,

        /// <summary>
        /// 
        /// </summary>
        Kid,

        /// <summary>
        /// 
        /// </summary>
        Teen,

        /// <summary>
        /// 
        /// </summary>
        Adult,

        /// <summary>
        /// Final form. Can play any game.
        /// </summary>
        God
    }

    #endregion
}