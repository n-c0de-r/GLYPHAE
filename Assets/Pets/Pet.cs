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

        [SerializeField] private NotificationsAndroid notifications;

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
        private static HashSet<NeedData> _criticals = new();
        private DateTime _previousTimeStamp;

        private int _evolutionCalls = 0;
        private int _sicknessChanceFactor, _sickCount;

        private const char ITEM_SPLIT = ';', VALUE_SPLIT = ':';
        private const float INCREMENT_MIN = 0.13f, INCREMENT_MAX = 0.23f;
        private float _hungerIncrement, _healthIncrement, _joyIncrement, _energyIncrement;
        private float _needTimer = 60;
        public long[] _randomSeeds = { -1, -1, -1, -1, -1 }; // Invalid seeds
        public int minutes = 0;
        public int _debugTimeFactor = 1;

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
        public List<GlyphData> Literals { get => literals; }

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

            LoadPrefs();

            CalculateNeedFactors();
        }

        private void OnEnable()
        {
            Minigame.OnNextRound += Call;
            Minigame.OnCorrectGuess += Feedback;
            Minigame.OnWrongGuess += Feedback;

            NeedData.OnNeedCritical += SetCiticals;
            NeedData.OnNeedSatisfied += SatisfyCriticals;

            notifications.ClearAllNotifications();

            ChangeSprite((int)_level);
            CalculateNeedFactors();
            RecalculateNeeds();
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

            NeedData.OnNeedCritical -= SetCiticals;
            NeedData.OnNeedSatisfied -= SatisfyCriticals;

            CalculateNotifications();

            if (_level != Evolutions.Egg) SavePrefs();
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
            _criticals = new();
        }

        /// <summary>
        /// Changes the <see cref="Pet"/>'s sprite according to its <see cref="Evolutions"/>.
        /// </summary>
        /// <param name="spriteNumber">The sprite index to pick from, corresponds to the current <see cref="Evolutions"/> level.</param>
        public void ChangeSprite(int spriteNumber)
        {
            _spriteRenderer.sprite = levelSprites[spriteNumber];
        }

        #region Persistence

        public void LoadPrefs()
        {
            string prefix = petName + "_";

            if (PlayerPrefs.HasKey(petName + nameof(_level)))
            {
                Enum.TryParse(PlayerPrefs.GetString(petName + nameof(_level)), out Evolutions lvl);
                _level = lvl;
            }


            if (PlayerPrefs.HasKey(petName + nameof(_evolutionCalls))) _evolutionCalls = PlayerPrefs.GetInt(petName + nameof(_evolutionCalls));


            if (PlayerPrefs.HasKey(prefix + nameof(unlocked)))
                unlocked = PlayerPrefs.GetString(prefix + nameof(unlocked)).Equals("True");


            if (PlayerPrefs.HasKey(prefix + nameof(needs)))
            {
                foreach (string item in PlayerPrefs.GetString(prefix + nameof(needs)).Split(ITEM_SPLIT))
                {
                    if (item == "") continue;

                    string[] needsData = item.Split(VALUE_SPLIT);
                    int.TryParse(needsData[0][..1], out int index);
                    float.TryParse(needsData[1], out float value);
                        Debug.Log(index);
                    needs[index-1].Current = value;
                }
            }


            if (PlayerPrefs.HasKey(prefix + nameof(literals)))
            {
                foreach (string item in PlayerPrefs.GetString(prefix + nameof(literals)).Split(ITEM_SPLIT))
                {
                    if (item == "") continue;

                    string[] glyphData = item.Split(VALUE_SPLIT);
                    if (Enum.TryParse(glyphData[1], out MemoryLevels level))
                    {
                        int.TryParse(glyphData[0].Substring(3,2), out int index);
                        Debug.Log(index);
                        Debug.Log(glyphData[0].Substring(3, 2));
                        Literals[index-1].MemoryLevel = level;
                    }
                }
            }

            if (PlayerPrefs.HasKey(petName + nameof(_randomSeeds)))
            {
                string[] values = PlayerPrefs.GetString(petName + nameof(_randomSeeds)).Split(ITEM_SPLIT);
                for (int i = 0; i < values.Length; i++)
                {
                    long.TryParse(values[i], out long value);
                    _randomSeeds[0] = value;
                }
            }

            if (PlayerPrefs.HasKey(petName + nameof(_previousTimeStamp)))
            {
                DateTime.TryParse(PlayerPrefs.GetString(petName + nameof(_previousTimeStamp)), out DateTime timeStamp);
                _previousTimeStamp = timeStamp;
            }

            PlayerPrefs.SetString(petName + nameof(_previousTimeStamp), DateTime.Now.ToString());

        }

        public void SavePrefs()
        {
            string prefix = petName + "_";

            PlayerPrefs.SetString(prefix + nameof(unlocked), prefix + unlocked.ToString());


            PlayerPrefs.SetString(petName + nameof(_level), _level.ToString());


            PlayerPrefs.SetInt(petName + nameof(_evolutionCalls), _evolutionCalls);


            string needValues = "";
            foreach (NeedData item in needs)
            {
                needValues += item.name + VALUE_SPLIT + item.Current + ITEM_SPLIT;
            }
            PlayerPrefs.SetString(prefix + nameof(needs), needValues);


            string glyphs = "";
            foreach (GlyphData item in literals)
            {
                glyphs += item.name + VALUE_SPLIT + item.MemoryLevel.ToString() + ITEM_SPLIT;
            }

            PlayerPrefs.SetString(prefix + nameof(literals), glyphs);

            string seeds = "";
            foreach (long item in _randomSeeds)
            {
                seeds += item.ToString() + ITEM_SPLIT;
            }
            PlayerPrefs.SetString(petName + nameof(_randomSeeds), seeds);

            PlayerPrefs.SetString(petName + nameof(_previousTimeStamp), DateTime.Now.ToString());
        }

        #endregion Persistence

        #endregion


        #region Helpers

        /// <summary>
        /// Reduces the needs when the internal timer is up.
        /// </summary>
        private void DecreaseNeeds()
        {
            if (_level == Evolutions.Egg) return;

            Hunger.Decrease(_hungerIncrement);
            Health.Decrease(_healthIncrement * _sickCount + 1);
            Joy.Decrease(_joyIncrement * _sickCount + 1);
            Energy.Decrease(_energyIncrement * _sickCount + 1);

            CheckSickness();
            CheckEvolution();
        }

        private void RecalculateNeeds()
        {
            if (_previousTimeStamp == null) return;

            Debug.Log("then: " + _previousTimeStamp.ToShortTimeString());
            Debug.Log("now: " + DateTime.Now.ToShortTimeString());

            var minutes = (_previousTimeStamp - DateTime.Now).TotalMinutes;
            Debug.Log("diff: " + minutes);

            for (int i = 0; i < minutes; i++) DecreaseNeeds();
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

            if (_randomSeeds[4] != -1)
            {
                UnityEngine.Random.InitState((int)_randomSeeds[4]);
            }
            else
            {
                UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
                _randomSeeds[4] = DateTime.Now.Ticks;
            }

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

        private void SatisfyCriticals(NeedData data)
        {
            if (_criticals.Remove(data))
                _evolutionCalls++;
        }

        #region Math

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
            _randomSeeds[0] = Hunger.Randomize(_evolutionCalls);
            Hunger.Initialize();

            _healthIncrement = CalculateNeedIncrement() / 3.0f;
            Health.SetupFactors(1, 1);
            _randomSeeds[1] = Health.Randomize(_evolutionCalls);
            Health.Initialize();

            _joyIncrement = CalculateNeedIncrement();
            Joy.SetupFactors(CalculateReverseLine(), CalculateCurve());
            _randomSeeds[2] = Joy.Randomize(_evolutionCalls);
            Joy.Initialize();

            _energyIncrement = CalculateNeedIncrement();
            Energy.SetupFactors(CalculateReverseLine(), CalculateReverseCurve());
            _randomSeeds[3] = Energy.Randomize(_evolutionCalls);
            Energy.Initialize();

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
            notifications.ClearAllNotifications();
            if (_level == Evolutions.Egg) return;
            
            Hunger.CalculateNotification(_hungerIncrement);
            Health.CalculateNotification(_healthIncrement);
            Joy.CalculateNotification(_joyIncrement);
            Energy.CalculateNotification(_energyIncrement);

            //_randomSeed = Mathf.Abs((int)DateTime.Now.Ticks);
        }

        #endregion Math

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