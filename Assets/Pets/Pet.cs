using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore;

namespace GlyphaeScripts
{
    /// <summary>
    /// Represents a the Pet with all behaviors 
    /// and data in the game. Refe
    /// </summary>
    [RequireComponent(typeof(Animator), typeof(AudioSource))]
    [RequireComponent(typeof(BoxCollider2D), typeof(SpriteRenderer))]
    public class Pet : MonoBehaviour
    {
        #region Serialized Fields

        [Header("Need values")]
        [Tooltip("Basic needs of the pet:\r\nHunger, Health, Joy, Energy.")]
        [SerializeField] private Need[] needs;

        [Tooltip("Displays the Pet's need.")]
        [SerializeField] private PetMessage needCall;

        [Tooltip("Displays the Pet's feedback of actions.")]
        [SerializeField] private PetMessage needFeedback;

        [Header("Internal values")]
        [Tooltip("The prefab of this Pet")]
        [SerializeField] private GameObject prefab;

        [Tooltip("The sprites this Pet\r\ngoes through its evolutions")]
        [SerializeField] private Sprite[] levelSprites;

        [Header("Game related values")]
        [Tooltip("The list of Glyphs\r\nthis Pet needs to learn.")]
        [SerializeField] private List<Glyph> literals;

        [Tooltip("Gets the current state\r\nif the Pet is already unlocked.")]
        [SerializeField] private bool unlocked;

        #endregion


        #region Fields

        private Animator _animator;
        private AudioSource _audioSource;
        private SpriteRenderer _spriteRenderer;
        private BoxCollider2D _boxCollider;

        private Queue<Glyph> _toMatch;
        public Evolutions _petLevel = Evolutions.Egg;
        private NeedTypes _feedbackType;
        private int _evolutionCalls;
        private bool hasCalled = false;

        #endregion


        #region GetSets / Properties

        /// <summary>
        /// The prefab of this <see cref="Pet"/>.
        /// </summary>
        public GameObject Prefab { get => prefab; }

        /// <summary>
        /// The list of <see cref="Glyph"/>s
        /// this <see cref="Pet"/> needs to learn.
        /// </summary>
        public List<Glyph> Literals { get => literals; set => literals = value; }

        /// <summary>
        /// The current state if
        /// the <see cref="Pet"/> is already unlocked.
        /// </summary>
        public bool Unlocked { get => unlocked; }

        /// <summary>
        /// The current <see cref="Evolutions"/> level
        /// enum of this <see cref="Pet"/>
        /// </summary>
        public Evolutions PetLevel { get => _petLevel; }

        /// <summary>
        /// The current Hunger need container.
        /// </summary>
        public Need Hunger {  get => needs[(int)NeedTypes.Hunger]; }

        /// <summary>
        /// The current Health need container.
        /// </summary>
        public Need Health { get => needs[(int)NeedTypes.Health]; }

        /// <summary>
        /// The current Joy need container.
        /// </summary>
        public Need Joy { get => needs[(int)NeedTypes.Joy]; }

        /// <summary>
        /// The current Energy need container.
        /// </summary>
        public Need Energy { get => needs[(int)NeedTypes.Energy]; }

        float timer = 60;

        #endregion


        #region Unity Built-Ins

        void Awake()
        {
            if (_animator == null) TryGetComponent(out _animator);
            if (_audioSource == null) TryGetComponent(out _audioSource);
            if (_spriteRenderer == null) TryGetComponent(out _spriteRenderer);
            if (_boxCollider == null) TryGetComponent(out _boxCollider);

            if (PlayerPrefs.HasKey(nameof(Evolutions))) _petLevel = (Evolutions)PlayerPrefs.GetInt(nameof(Evolutions));
            if (PlayerPrefs.HasKey(nameof(_evolutionCalls))) _evolutionCalls = PlayerPrefs.GetInt(nameof(_evolutionCalls));
        }

        private void OnEnable()
        {
            _spriteRenderer.sprite = levelSprites[(int)_petLevel];

            GameButton.OnInputCheck += InputCheck;

            Minigame.OnGameStart += (need, cost) => UpdateNeed(NeedTypes.Energy, cost);
            Minigame.OnGameStart += (need, cost) => _feedbackType = need;
            Minigame.OnGameWin += UpdateNeed;
            Minigame.OnGameInit += SetNeeds;

            UpdateNeed(NeedTypes.Hunger, needs[(int)NeedTypes.Hunger].Current - Need.MAX);
            UpdateNeed(NeedTypes.Joy, needs[(int)NeedTypes.Joy].Current - Need.MAX);
            UpdateNeed(NeedTypes.Energy, needs[(int)NeedTypes.Energy].Current - Need.MAX);

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

            Minigame.OnGameStart -= (need, cost) => UpdateNeed(NeedTypes.Energy, cost);
            Minigame.OnGameStart -= (need, cost) => _feedbackType = need;
            Minigame.OnGameWin -= UpdateNeed;
            Minigame.OnGameInit -= SetNeeds;
        }

        #endregion


        #region Events

        public static event Action<Glyph, Sprite, Sprite, List<Glyph>> OnNeedCall;
        public static event Action<NeedTypes, float> OnNeedUpdate;
        public static event Action<bool> OnNeedSatisfied;

        #endregion


        #region Methods

        

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
            Glyph glyph = _toMatch.Dequeue();

            if (glyph.Symbol.name == message || glyph.Letter.name == message)
            {
                needFeedback.Setup(glyph.Sound, needs[(int)_feedbackType].Positive);
                OnNeedSatisfied?.Invoke(true);
            }
            else
            {
                needFeedback.Setup(glyph.Sound, needs[(int)_feedbackType].Negative);
                OnNeedSatisfied?.Invoke(false);

                _toMatch.Enqueue(glyph);
            }
            
            needFeedback.Show();
            GetNextNeed();
        }



        private void SetNeeds(int rounds, Minigame.Type type)
        {
            if (rounds <= 0 || type == Minigame.Type.None) return;

            Glyph[] currentGlyphs = literals.ToArray();
            _toMatch = new();

            while (_toMatch.Count < rounds)
            {
                int rand = UnityEngine.Random.Range(0, currentGlyphs.Length);
                Glyph glyph = currentGlyphs[rand];
                if (_toMatch.Contains(glyph)) continue;

                _toMatch.Enqueue(glyph);
            }

            GetNextNeed();
        }



        private void GetNextNeed()
        {
            if (_toMatch.Count == 0) return;
            Glyph glyph = _toMatch.Peek();

            int rnd = UnityEngine.Random.Range(0, 2);
            Sprite correct, wrong;

            correct = rnd == 0 ? glyph.Letter : glyph.Symbol;
            wrong = rnd == 0 ? glyph.Symbol : glyph.Letter;

            needCall.Setup(glyph.Sound, correct);
            needCall.Show();
            OnNeedCall?.Invoke(glyph, correct, wrong, new List<Glyph>(literals));
        }



        private void UpdateNeed(NeedTypes need, float amount)
        {
            float value = needs[(int)need].Current + amount;
            needs[(int)need].Current = value;
            OnNeedUpdate?.Invoke(need, amount);

            if (!hasCalled && value <= needs[(int)need].Critical)
            {
                hasCalled = true;
                Debug.Log(need + " is low!");
                //MessageNeed();
            }
            else if (hasCalled && value >= Need.MAX / 2)
            {
                hasCalled = false;
            }
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