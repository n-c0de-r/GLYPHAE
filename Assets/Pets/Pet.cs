using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GlyphaeScripts
{
    /// <summary>
    /// Represents a the Pet with all behaviors 
    /// and data in the game.
    /// </summary>
    [RequireComponent(typeof(AudioSource), typeof(BoxCollider2D), typeof(SpriteRenderer))]
    public class Pet : MonoBehaviour
    {
        #region Serialized Fields
        #if UNITY_ANDROID
        [SerializeField] private NotificationsAndroid notifications;
        #endif
        [SerializeField] private Settings settings;

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
        [SerializeField][TextArea(3, 10)] private string description;

        [Tooltip("The Pet's icon for the menu.")]
        [SerializeField] private Sprite symbol;

        #endregion


        #region Fields

        private Animator _animator;
        private AudioSource _audioSource;
        private SpriteRenderer _spriteRenderer;
        private BoxCollider2D _boxCollider;

        private Evolutions _level = Evolutions.Egg;
        private HashSet<NeedData> _criticals = new();
        private DateTime _birthTime, _previousTimeStamp;


        private const char ITEM_SPLIT = ';', VALUE_SPLIT = ':', PART_SPLIT = '~';
        private const float INCREMENT_MIN = 0.23f, INCREMENT_MAX = 0.31f;
        private float _needTimer = 60;
        private int _evolutionCalls, _sicknessChanceFactor, _sickCount;
        private float _sleepynessFactor = 1f;

        private bool _isSleeping = false, _isEvolving = false;

        #endregion


        #region Events

        public static event Action OnEvolve, OnWakeUp;

        #endregion


        #region GetSets / Properties

        /// <summary>
        /// The current Hunger <see cref="NeedData"/> container.
        /// </summary>
        public NeedData[] Needs { get => needs; }

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

        /// <summary>
        /// The <see cref="Pet"/>'s birth time.
        /// </summary>
        public DateTime BirthTime { get => _birthTime; }

        /// <summary>
        /// The <see cref="Pet"/>'s age.
        /// </summary>
        public int Age { get => Mathf.Abs((DateTime.Now - settings.SelectedPet.BirthTime).Days); }


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
        /// Sets the hidden evolution calls number value.
        /// Only for debugging on hardware.
        /// </summary>
        public int EvolutionCalls { get => _evolutionCalls; set => _evolutionCalls = value; }

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

            CalculateNeedFactors();
        }

        private void OnEnable()
        {
            LullabyChant.OnSleep += SleepFactors;

            Minigame.OnNextRound += Call;
            Minigame.OnCorrectGuess += Feedback;
            Minigame.OnWrongGuess += Feedback;

            NeedData.OnNeedCritical += SetCiticals;
            #if UNITY_ANDROID
            notifications.ClearAllNotifications();
            #endif
            LoadPrefs();

            ChangeSprite((int)_level);
            CalculateNeedFactors();
            RecalculateNeeds();
            CheckEvolution();
            if (_isSleeping && Energy.Current > Energy.SatisfiedLimit)
                OnWakeUp?.Invoke();
        }

        void FixedUpdate()
        {
            _needTimer -= Time.fixedDeltaTime * settings.GameSpeed;

            if (_needTimer <= 0)
            {
                _needTimer = 60;
                DecreaseNeeds();
                CheckEvolution();
                if (_isSleeping && Energy.Current > Energy.SatisfiedLimit)
                    OnWakeUp?.Invoke();
            }
        }


        private void OnDisable()
        {
            Minigame.OnNextRound -= Call;
            Minigame.OnCorrectGuess -= Feedback;
            Minigame.OnWrongGuess -= Feedback;

            NeedData.OnNeedCritical -= SetCiticals;

            #if UNITY_ANDROID
            notifications.ClearAllNotifications();
            #endif
            CalculateNotifications();
            _previousTimeStamp = DateTime.Now;
            if (_level != Evolutions.Egg) SavePrefs();
        }

        // Apparently needed on mobile. Better safe than sorry?
        private void OnApplicationPause(bool isPaused)
        {
            #if UNITY_ANDROID
            notifications.ClearAllNotifications();
            #endif
            if (isPaused)
            {
                CalculateNotifications();
                _previousTimeStamp = DateTime.Now;

                if (_level != Evolutions.Egg) SavePrefs();
            }
            else
            {
                ChangeSprite((int)_level);
                CalculateNeedFactors();
                RecalculateNeeds();
                CheckEvolution();
                if (_isSleeping && Energy.Current > Energy.SatisfiedLimit)
                    OnWakeUp?.Invoke();
            }
        }

        private void OnApplicationFocus(bool focus)
        {
            #if UNITY_ANDROID
            notifications.ClearAllNotifications();
            #endif
            if (focus)
            {
                ChangeSprite((int)_level);
                CalculateNeedFactors();
                RecalculateNeeds();
                CheckEvolution();
                if (_isSleeping && Energy.Current > Energy.SatisfiedLimit)
                    OnWakeUp?.Invoke();
            }
            else
            {
                CalculateNotifications();
                _previousTimeStamp = DateTime.Now;
                if (_level != Evolutions.Egg) SavePrefs();
            }
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
            _isEvolving = false;
            _evolutionCalls = 0;

            if (_level == Evolutions.Egg)
                _birthTime = DateTime.Now;

            if (_level != Evolutions.Egg) OnEvolve?.Invoke();
            _level++;

            ChangeSprite((int)_level);
            CalculateNeedFactors();
            _criticals = new();

            foreach (NeedData item in needs)
                item.Initialize();
        }

        /// <summary>
        /// Changes the <see cref="Pet"/>'s sprite according to its <see cref="Evolutions"/>.
        /// </summary>
        /// <param name="spriteNumber">The sprite index to pick from, corresponds to the current <see cref="Evolutions"/> level.</param>
        public void ChangeSprite(int spriteNumber)
        {
            _spriteRenderer.sprite = levelSprites[spriteNumber];
        }

        /// <summary>
        /// Resets the factors back to their originals.
        /// </summary>
        public void WakeUp()
        {
            if (_isEvolving && Energy.Current > Energy.SatisfiedLimit)
            {
                IncreaseLevel();
            }
            else
            {
                CalculateNeedFactors();
            }
            _isSleeping = false;
            
        }

        public void ResetPet()
        {
            _level = Evolutions.Egg;
            foreach (GlyphData item in literals)
                item.ResetLevel();

            foreach (NeedData item in needs)
                item.Initialize();

            foreach (Pet item in settings.Pets)
                if (gameObject.name.Contains(item.name))
                    settings.SelectedPet = item;
        }

        #region Persistence

        /// <summary>
        /// Load <see cref="Pet"/>'s values.
        /// </summary>
        public void LoadPrefs()
        {
            string prefix = petName + "_";

            if (PlayerPrefs.HasKey(petName + nameof(_level)))
            {
                Enum.TryParse(PlayerPrefs.GetString(petName + nameof(_level)), out Evolutions lvl);
                _level = lvl;
            }


            if (PlayerPrefs.HasKey(petName + nameof(_isSleeping)))
            {
                _isSleeping = PlayerPrefs.GetString(petName + nameof(_isSleeping)).Equals(true.ToString());
                if (_isSleeping) SleepFactors();
            }


            if (PlayerPrefs.HasKey(petName + nameof(_previousTimeStamp)))
            {
                DateTime.TryParse(PlayerPrefs.GetString(petName + nameof(_previousTimeStamp)), out DateTime timeStamp);
                _previousTimeStamp = timeStamp;
            }

            if (PlayerPrefs.HasKey(petName + nameof(_birthTime)))
            {
                DateTime.TryParse(PlayerPrefs.GetString(petName + nameof(_birthTime)), out DateTime timeStamp);
                _birthTime = timeStamp;
            }


            if (PlayerPrefs.HasKey(petName + nameof(_evolutionCalls)))
                _evolutionCalls = PlayerPrefs.GetInt(petName + nameof(_evolutionCalls));


            if (PlayerPrefs.HasKey(prefix + nameof(unlocked)))
                unlocked = PlayerPrefs.GetString(prefix + nameof(unlocked)).Equals(true.ToString());


            if (PlayerPrefs.HasKey(prefix + nameof(needs)))
            {
                foreach (string item in PlayerPrefs.GetString(prefix + nameof(needs)).Split(ITEM_SPLIT))
                {
                    if (item == "") continue;

                    string[] needsData = item.Split(VALUE_SPLIT);
                    int.TryParse(needsData[0][..1], out int index);
                    if (index == 0) continue;
                    string[] values = needsData[1].Split(PART_SPLIT);
                    float.TryParse(values[0], out float current);
                    float.TryParse(values[1], out float randomness);

                    needs[index-1].SetValue(current, CalculateNeedIncrement(), randomness);
                }
            }


            if (PlayerPrefs.HasKey(prefix + nameof(literals)))
            {
                foreach (string item in PlayerPrefs.GetString(prefix + nameof(literals)).Split(ITEM_SPLIT))
                {
                    if (item == "") continue;

                    string[] glyphData = item.Split(VALUE_SPLIT);
                    string[] values = glyphData[1].Split(PART_SPLIT);
                    if (Enum.TryParse(values[0], out MemoryLevels level))
                    {
                        int.TryParse(glyphData[0].Substring(3,2), out int index);
                        if (index == 0) continue;
                        int.TryParse(values[1], out int correct);
                        int.TryParse(values[1], out int wrong);
                        Literals[index - 1].SetupData(level, correct, wrong);
                    }
                }
            }
        }

        /// <summary>
        /// Saves <see cref="Pet"/>'s values.
        /// </summary>
        public void SavePrefs()
        {
            string prefix = petName + "_";

            PlayerPrefs.SetString(prefix + nameof(unlocked), unlocked.ToString());

            PlayerPrefs.SetString(petName + nameof(_isSleeping), _isSleeping.ToString());

            PlayerPrefs.SetString(petName + nameof(_level), _level.ToString());

            PlayerPrefs.SetInt(petName + nameof(_evolutionCalls), _evolutionCalls);

            PlayerPrefs.SetString(petName + nameof(_previousTimeStamp), DateTime.Now.ToString());

            PlayerPrefs.SetString(petName + nameof(_birthTime), _birthTime.ToString());


            string needValues = "";
            foreach (NeedData item in needs)
            {
                needValues += item.name + VALUE_SPLIT + item.Current + PART_SPLIT + item.RandomOffset + ITEM_SPLIT;
            }
            PlayerPrefs.SetString(prefix + nameof(needs), needValues);


            //string needCriticals = "";
            //foreach (NeedData item in _criticals)
            //{
                
            //}


            string glyphs = "";
            foreach (GlyphData item in literals)
            {
                glyphs += item.name + VALUE_SPLIT + item.MemoryLevel.ToString() + PART_SPLIT + item.CorrectGuesses + PART_SPLIT + item.WrongGuesses + ITEM_SPLIT;
            }

            PlayerPrefs.SetString(prefix + nameof(literals), glyphs);
        }

        #endregion Persistence

        #endregion


        #region Helpers

        /// <summary>
        /// Reduces the needs when the internal timer is up.
        /// <param name="minutesPassed">The amount of minutes that have passed.</param>
        /// </summary>
        private void DecreaseNeeds(int minutesPassed = 1)
        {
            if (_level == Evolutions.Egg) return;

            float factor = 1f * _sickCount + 1;

            Hunger.Decrease(minutesPassed);
            Health.Decrease(minutesPassed);
            Joy.Decrease(minutesPassed);

            if (!_isSleeping)
            {
                if (DateTime.Now.Hour >= settings.SilenceStart
                    || DateTime.Now.Hour < settings.SilenceEnd) _sleepynessFactor *= 1.10f;
                Energy.Decrease(minutesPassed * _sleepynessFactor);
            } else
            {
                _sleepynessFactor = 1f;
                Energy.Increase(minutesPassed * _sleepynessFactor);
            }

            //CheckSickness();
        }

        /// <summary>
        /// Recalculates needs after a reload.
        /// </summary>
        private void RecalculateNeeds()
        {
            if (_previousTimeStamp == null) return;

            float difference = Mathf.Abs((float)(_previousTimeStamp - DateTime.Now).TotalMinutes);
            int minutes = (int)Mathf.Round(difference);
            if(minutes > 1) DecreaseNeeds(minutes);
        }

        /// <summary>
        /// Checks if the <see cref="Pet"/> is ready to evolve to the next <see cref="Evolutions"/> level.
        /// </summary>
        private void CheckEvolution()
        {
            if (_level == Evolutions.God) return;

            if (!_isEvolving && _evolutionCalls >= Enum.GetValues(typeof(Evolutions)).Length)
            {
                _isEvolving = true;

                Energy.Decrease(Energy.Current - 10);
                EvolutionFactors();
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

            int rng = UnityEngine.Random.Range(3, 11);
            float sicknessChance = (hungerFactor + healthFactor + energyFactor + joyFactor + rng) * _sicknessChanceFactor;
            rng = UnityEngine.Random.Range(NeedData.MAX / (10 / _sicknessChanceFactor), NeedData.MAX * NeedData.MAX / _sicknessChanceFactor);
                //TODO: Add poop?

            if (sicknessChance > rng)
            {
                _sickCount = Mathf.Clamp(_sickCount+1, 0, 3);
                Health.Decrease(_sicknessChanceFactor * 10);
                needCall.Setup(Health.AlarmSound, Health.Alarm);
            }
        }



        /// <summary>
        /// Displays a need <see cref="NeedBubble"/>.
        /// </summary>
        /// <param name="sprite">The icon to show, taken from <see cref="NeedData"/>. Either positive or negative.</param>
        private void Call(AudioClip clip, Sprite sprite)
        {
            needCall.Setup(clip, sprite);
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

        /// <summary>
        /// Sets up ctitical values.
        /// </summary>
        /// <param name="data">The data to set critical.</param>
        /// <param name="state">The state to set to.</param>
        private void SetCiticals(NeedData data, bool state)
        {
            if (state)
            {
                _criticals.Add(data);
                needFeedback.Setup(data.AlarmSound, data.Alarm);
                StartCoroutine(needFeedback.ShowFeedback());
            }
            else if (!_isEvolving && _criticals.Remove(data))
            {
                _evolutionCalls++;
                CheckEvolution();
            }

            // FIX BUGS
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
            
            Hunger.SetupValues(CalculateReverseCurve(), CalculateReverseLine(), CalculateNeedIncrement(), _evolutionCalls);
            Health.SetupValues(1, 1, CalculateNeedIncrement() / 3.0f, _evolutionCalls);
            Joy.SetupValues(CalculateReverseLine(), CalculateCurve(), CalculateNeedIncrement(), _evolutionCalls);
            Energy.SetupValues(CalculateReverseLine(), CalculateReverseCurve(), CalculateNeedIncrement(), _evolutionCalls);

            _sicknessChanceFactor = CalculateReverseCurve();
        }

        /// <summary>
        /// Sets all factors to 1 while sleeping
        /// </summary>
        private void SleepFactors()
        {
            if (_level == Evolutions.Egg) return;

            Hunger.SetupValues(0, 1, CalculateNeedIncrement(), _evolutionCalls);
            Health.SetupValues(0, 1, CalculateNeedIncrement(), _evolutionCalls);
            Joy.SetupValues(0, 1, CalculateNeedIncrement(), _evolutionCalls);
            Energy.SetupValues(Energy.UpFactor, 0, CalculateNeedIncrement(), _evolutionCalls);

            _sicknessChanceFactor = 1;

            _isSleeping = true;
        }

        /// <summary>
        /// Sets all factors to 0 while evolving
        /// </summary>
        private void EvolutionFactors()
        {
            if (_level == Evolutions.Egg) return;

            Hunger.SetupValues(0, 0, 0, 0);
            Health.SetupValues(0, 0, 0, 0);
            Joy.SetupValues(0, 0, 0, 0);
            Energy.SetupValues(Energy.UpFactor * 2, Energy.DownFactor * 2, 1, 0);

            _sicknessChanceFactor = 0;
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

        /// <summary>
        /// Calculate the notification calls when game is closed.
        /// </summary>
        private void CalculateNotifications()
        {
            if (_level == Evolutions.Egg) return;
            if (_isSleeping) return;
            
            Hunger.CalculateNotification();
            Health.CalculateNotification();
            Joy.CalculateNotification();
            Energy.CalculateNotification();
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