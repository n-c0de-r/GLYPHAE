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
        private HashSet<NeedData> _criticals = new();
        private int _evolutionCalls = 0;
        private int _sicknessChanceFactor, _sickCount;

        private const float INCREMENT_MIN = 0.13f, INCREMENT_MAX = 0.23f;
        private float _hungerIncrement, _healthIncrement, _joyIncrement, _energyIncrement;
        private float _needTimer = 60;
        public int minutes = 0;
        public int _debugTimeFactor = 1;

        public int _randomSeed = -1; // invalid seed

        #endregion


        #region Events



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


        #region Debug 

        /// <summary>
        /// Set the current <see cref="Evolutions"/> level.
        /// Only for debugging on hardware.
        /// </summary>
        public int LevelValue
        {
            set
            {
                _level = (Evolutions)value;
                ChangeSprite(value);
                CalculateNeedFactors();
            }
        }

        /// <summary>
        /// Sets the time factor value to speed up display.
        /// Only for debugging on hardware.
        /// </summary>
        public float TimeFactor { set => _debugTimeFactor = (int)value; }

        /// <summary>
        /// Sets the hidden hunger increment value.
        /// Only for debugging on hardware.
        /// </summary>
        public float HungerIncrement { get => _hungerIncrement; }

        /// <summary>
        /// Sets the hidden health increment value.
        /// Only for debugging on hardware.
        /// </summary>
        public float HealthIncrement { get => _healthIncrement; }

        /// <summary>
        /// Sets the hidden joy increment value.
        /// Only for debugging on hardware.
        /// </summary>
        public float JoyIncrement { get => _joyIncrement; }

        /// <summary>
        /// Sets the hidden energy increment value.
        /// Only for debugging on hardware.
        /// </summary>
        public float EnergyIncrement { get => _energyIncrement; }

        /// <summary>
        /// Sets the hidden evolution calls number value.
        /// Only for debugging on hardware.
        /// </summary>
        public int EvolutionCalls { get => _evolutionCalls; }

        /// <summary>
        /// Sets the hidden sickness chance factor value.
        /// Only for debugging on hardware.
        /// </summary>
        public int SicknessChanceFactor { get => _sicknessChanceFactor; }

        #endregion Debug

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
            Minigame.OnNextRound += Call;
            Minigame.OnCorrectGuess += Feedback;
            Minigame.OnWrongGuess += Feedback;

            NeedData.OnNeedCritical += SetCiticals;
            NeedData.OnNeedSatisfied += (need) => Debug.Log(need);

            ChangeSprite((int)_level);
            CalculateNeedFactors();
            CheckEvolution();
        }

        void Start()
        {
            
        }

        void FixedUpdate()
        {
            _needTimer -= Time.fixedDeltaTime * _debugTimeFactor;

            if (_needTimer <= 0)
            {
                DecreaseNeeds();
                _needTimer = 60;
                minutes++;
            }
        }

        void Update()
        {

        }

        private void OnApplicationPause(bool isPaused)
        {

        }

        private void OnDisable()
        {
            Minigame.OnNextRound -= Call;
            Minigame.OnCorrectGuess -= Feedback;
            Minigame.OnWrongGuess -= Feedback;
            
            CalculateNotifications();
        }

        #endregion


        #region Methods

        /// <summary>
        /// Increases the <see cref="Pet"/>'s <see cref="Evolutions"/> level and
        /// does all the relevant background calculations for the next round.
        /// </summary>
        public void IncreaseLevel()
        {
            if ((int)_level >= levelSprites.Length) return;
            _level++;
            ChangeSprite((int)_level);
            CalculateNeedFactors();
        }

        /// <summary>
        /// Changes the <see cref="Pet"/>'s sprite according to its <see cref="Evolutions"/>.
        /// </summary>
        /// <param name="spriteNumber">The sprite index to pick from, corresponds to the current <see cref="Evolutions"/> level.</param>
        public void ChangeSprite(int spriteNumber)
        {
            _spriteRenderer.sprite = levelSprites[spriteNumber];
        }

        #endregion


        #region Helpers

        /// <summary>
        /// Reduces the needs when the internal timer is up.
        /// </summary>
        private void DecreaseNeeds()
        {
            Hunger.Decrease(_hungerIncrement);
            Health.Decrease(_healthIncrement * _sickCount + 1);
            Joy.Decrease(_joyIncrement * _sickCount + 1);
            Energy.Decrease(_energyIncrement * _sickCount + 1);

            CheckSickness();
            CheckEvolution();
        }

        /// <summary>
        /// Checks if the <see cref="Pet"/> is ready to evolve to the next <see cref="Evolutions"/> level.
        /// </summary>
        private void CheckEvolution()
        {
            if (_evolutionCalls > Enum.GetValues(typeof(Evolutions)).Length)
            {
                IncreaseLevel();
                //TODO: Animation
                _evolutionCalls = 0;
            }
        }

        /// <summary>
        /// Checks if the <see cref="Pet"/> gets sick.
        /// The chance depends on overall health, hunger, mood, some level related factors and some randomness.
        /// </summary>
        private void CheckSickness()
        {
            float hungerFactor = Hunger.Critical / (Hunger.Current + 1) / 3;
            float healthFactor = Health.Critical / (Health.Current + 1);
            float joyFactor = Joy.Critical / (Joy.Current + 1) / 2;
            float energyFactor = Energy.Critical / (Energy.Current + 1) / 4;

            if (_randomSeed != -1) UnityEngine.Random.InitState(_randomSeed);
            int rng = UnityEngine.Random.Range(3, 11);
            float sicknessChance = (hungerFactor + healthFactor + energyFactor + joyFactor + rng) * _sicknessChanceFactor;
            rng = UnityEngine.Random.Range(NeedData.MAX / (10 / _sicknessChanceFactor), NeedData.MAX * NeedData.MAX / _sicknessChanceFactor);
                //TODO: Add poop?

            if (sicknessChance > rng)
            {
                _sickCount = Mathf.Clamp(_sickCount+1, 0, 3);
                Health.Decrease(_healthIncrement * _sicknessChanceFactor * 10);
                needCall.Setup(Health.Alarm);
            }
        }

        /// <summary>
        /// Displays a need <see cref="NeedBubble"/>.
        /// </summary>
        /// <param name="sprite">The icon to show, taken from <see cref="NeedData"/>. Either positive or negative.</param>
        private void Call(Sprite sprite)
        {
            needCall.Setup(sprite);
            StartCoroutine(needCall.ShowCall());
        }

        /// <summary>
        /// Displays a feedback <see cref="NeedBubble"/>.
        /// </summary>
        /// <param name="sprite">The icon to show, taken from <see cref="NeedData"/>. Either positive or negative.</param>
        private void Feedback(Sprite sprite)
        {
            needCall.Disable();
            needFeedback.Setup(sprite);
            StartCoroutine(needFeedback.ShowFeedback());
        }

        private void SetCiticals(NeedData data)
        {
            _criticals.Add(data);
            needCall.Setup(data.Alarm);
        }

        private void RemoveCriticals(NeedData data)
        {
            _criticals.Remove(data);
            _evolutionCalls++;
        }

        // MATH

        /// <summary>
        /// Calculate all the relevant <see cref="NeedData"/> change factors.
        /// Correlation with the <see cref="Pet"/>'s <see cref="Evolutions"/> levels and <see cref="EvolutionCalls"/>.
        /// </summary>
        private void CalculateNeedFactors()
        {
            if (_level == Evolutions.Egg) return;
            
            _hungerIncrement = CalculateNeedIncrement();
            Hunger.SetupFactors(CalculateReverseCurve(), CalculateReverseLine());
            Hunger.Randomize(_evolutionCalls);

            _healthIncrement = CalculateNeedIncrement() / 3.0f;
            Health.SetupFactors(1, 1);
            Health.Randomize(_evolutionCalls);

            _joyIncrement = CalculateNeedIncrement();
            Joy.SetupFactors(CalculateReverseLine(), CalculateCurve());
            Joy.Randomize(_evolutionCalls);

            _energyIncrement = CalculateNeedIncrement();
            Energy.SetupFactors(CalculateReverseLine(), CalculateReverseCurve());
            Energy.Randomize(_evolutionCalls);

            _sicknessChanceFactor = CalculateReverseCurve();
        }

        /// <summary>
        /// Calculates the <see cref="NeedData"/> change factors that resemble a <see href="https://en.wikipedia.org/wiki/Normal_distribution">Gaussian Normal Distribution</see>.
        /// lowest at the edges, highest in the middle.
        /// This is proportional to the <see cref="Pet"/>'s <see cref="Evolutions"/> level.
        /// </summary>
        /// <returns>A factor affecting the <see cref="NeedData"/> it is passed onto.</returns>
        private int CalculateCurve()
        {
            return (int)Evolutions.Teen - Mathf.Abs((int)_level - (int)Evolutions.Teen);
        }

        /// <summary>
        /// Calculates the <see cref="NeedData"/> change factors that resemble a reversed <see href="https://en.wikipedia.org/wiki/Normal_distribution">Gaussian Normal Distribution</see> curve.
        /// Highest at the edges, lowest in the middle.
        /// This is inversly proportional to the <see cref="Pet"/>'s <see cref="Evolutions"/> level.
        /// </summary>
        /// <returns>A factor affecting the <see cref="NeedData"/> it is passed onto.</returns>
        private int CalculateReverseCurve()
        {
            return Mathf.Abs((int)_level - (int)Evolutions.Teen) + 1;
        }

        /// <summary>
        /// Calculates the <see cref="NeedData"/> change factors.
        /// Highest at the beginning, lowest at the end.
        /// This is inversely proportional to the <see cref="Pet"/>'s <see cref="Evolutions"/> level.
        /// </summary>
        /// <returns>A factor affecting the <see cref="NeedData"/> it is passed onto.</returns>
        private int CalculateReverseLine()
        {
            return (int)Enum.GetValues(typeof(Evolutions)).Length - (int)_level;
        }

        /// <summary>
        /// The value each <see cref="NeedData"/> is reduced by.
        /// Correlates with the <see cref="Pet"/>'s <see cref="Evolutions"/> level.
        /// Changes after a leve-up [<see cref="IncreaseLevel"/>].
        /// </summary>
        /// <returns>A random float increment value.</returns>
        private float CalculateNeedIncrement()
        {
            return UnityEngine.Random.Range(INCREMENT_MIN, INCREMENT_MAX) * CalculateReverseLine();
        }

        // ENDGAME
        private void CalculateNotifications()
        {
            NotificationsAndroid.ClearAllNotifications();
            if (_level == Evolutions.Egg) return;
            
            Hunger.CalculateNotification(_hungerIncrement);
            Health.CalculateNotification(_healthIncrement);
            Joy.CalculateNotification(_joyIncrement);
            Energy.CalculateNotification(_energyIncrement);

            //_randomSeed = Mathf.Abs((int)DateTime.Now.Ticks);
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