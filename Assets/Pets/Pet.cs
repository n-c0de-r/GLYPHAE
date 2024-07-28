using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
        [SerializeField] private NeedData hunger;
        [SerializeField] private NeedData health;
        [SerializeField] private NeedData joy;
        [SerializeField] private NeedData energy;

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

        private Queue<GlyphData> _toMatch;
        public Evolutions _level = Evolutions.Egg;
        private int _evolutionCalls;
        private bool hasCalled = false;

        #endregion


        #region Events

        //public static event Action<Glyph, Sprite, Sprite, List<Glyph>> OnNeedCall;
        public static event Action<Sprite, List<GlyphData>> OnNeedCall;
        public static event Action<NeedTypes, float> OnNeedUpdate;
        public static event Action<bool> OnNeedSatisfied;
        float timer = 60;

        #endregion


        #region GetSets / Properties

        /// <summary>
        /// The list of <see cref="GlyphData"/>s
        /// this <see cref="Pet"/> needs to learn.
        /// </summary>
        public List<GlyphData> Literals { get => literals; set => literals = value; }

        /// <summary>
        /// The current state if
        /// the <see cref="Pet"/> is already unlocked.
        /// </summary>
        public bool Unlocked { get => unlocked; }

        /// <summary>
        /// The current <see cref="Evolutions"/> level
        /// enum of this <see cref="Pet"/>
        /// </summary>
        public Evolutions Level { get => _level; }

        /// <summary>
        /// The current Hunger need container.
        /// </summary>
        public NeedData Hunger {  get => hunger; }

        /// <summary>
        /// The current Health need container.
        /// </summary>
        public NeedData Health { get => health; }

        /// <summary>
        /// The current Joy need container.
        /// </summary>
        public NeedData Joy { get => joy; }

        /// <summary>
        /// The current Energy need container.
        /// </summary>
        public NeedData Energy { get => energy; }

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
        }

        private void OnEnable()
        {
            _spriteRenderer.sprite = levelSprites[(int)_level];

            GameButton.OnInputCheck += InputCheck;

            Minigame.OnGameInit += SetNeeds;

            //PetMessage.OnAnimationDone += () => Debug.Log("Done");

            UpdateNeed(NeedTypes.Hunger, hunger.Current - NeedData.MAX);
            UpdateNeed(NeedTypes.Joy, joy.Current - NeedData.MAX);
            UpdateNeed(NeedTypes.Energy, energy.Current - NeedData.MAX);

            CheckEvolution();

        }

        void Start()
        {
            
        }

        void FixedUpdate()
        {
            timer -= Time.fixedDeltaTime;

            if (timer <= 0)
            {
                timer = 60;
                UpdateNeed(NeedTypes.Hunger, -1);
                UpdateNeed(NeedTypes.Joy, -1);
                UpdateNeed(NeedTypes.Energy, -1);
            }
        }

        void Update()
        {

        }

        private void OnDisable()
        {
            GameButton.OnInputCheck -= InputCheck;

            Minigame.OnGameInit -= SetNeeds;

            NeedBubble.OnAnimationDone -= GetNextNeed;
        }

        #endregion


        #region Methods

        public void IncreaseLevel()
        {
            if ((int)_level >= levelSprites.Length) return;
            _level++;
            _spriteRenderer.sprite = levelSprites[(int)_level];
        }

        #endregion


        #region Helpers

        private void SwitchSprite()
        {
            Ray ray = Camera.main.ScreenPointToRay(Pointer.current.position.ReadValue());
            RaycastHit2D hit2D = Physics2D.GetRayIntersection(ray);
            if (hit2D.collider != null)
            {
                //_currentLevel++;
                //_spriteRenderer.sprite = levelSprites[_currentLevel % levelSprites.Length];
                //_boxCollider.size = _spriteRenderer.size;
                _animator.SetTrigger("Wiggle");
            }
        }



        private void InputCheck(string message)
        {
            if (_toMatch.Count == 0) return;
            GlyphData glyph = _toMatch.Dequeue();

            if (glyph.Symbol.name == message || glyph.Letter.name == message)
            {
                //needFeedback.Setup(glyph.Sound, needs[(int)_feedbackType].Positive);
                OnNeedSatisfied?.Invoke(true);
            }
            else
            {
                //needFeedback.Setup(glyph.Sound, needs[(int)_feedbackType].Negative);
                OnNeedSatisfied?.Invoke(false);

                _toMatch.Enqueue(glyph);
            }
            
            needFeedback.Show();
            GetNextNeed();
        }



        private void SetNeeds(GameType gameType, int gameRounds)
        {
            if (gameRounds <= 0) return;

            GlyphData[] currentGlyphs = literals.ToArray();
            _toMatch = new();

            while (_toMatch.Count < gameRounds)
            {
                int rand = UnityEngine.Random.Range(0, currentGlyphs.Length);
                GlyphData glyph = currentGlyphs[rand];
                if (_toMatch.Contains(glyph)) continue;

                _toMatch.Enqueue(glyph);
            }

            GetNextNeed();
        }



        private void GetNextNeed()
        {
            if (_toMatch.Count == 0) return;
            GlyphData glyph = _toMatch.Peek();
            List<GlyphData> copy = new(literals);
            copy.Remove(glyph);

            int rnd = UnityEngine.Random.Range(0, 2);
            Sprite correct = rnd == 0 ? glyph.Letter : glyph.Symbol;

            needCall.Setup(glyph.Sound, correct);
            needCall.Show();
            OnNeedCall?.Invoke(correct, copy);
        }



        private void UpdateNeed(NeedTypes need, float amount)
        {
            //float value = needs[(int)need].Current + amount;
            //needs[(int)need].UpdateData(value);
            //OnNeedUpdate?.Invoke(need, amount);

            //if (!hasCalled && value <= needs[(int)need].Critical)
            //{
            //    hasCalled = true;
            //    Debug.Log(need + " is low!");
            //    //MessageNeed();
            //}
            //else if (hasCalled && value >= NeedData.MAX / 2)
            //{
            //    hasCalled = false;
            //}
        }



        private void CheckEvolution()
        {
            if (_evolutionCalls >= Enum.GetValues(typeof(Evolutions)).Length - 1)
            {
                Debug.Log("Level Up");
            }
        }

        #endregion
    }
}