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

        private Queue<Glyph> toMatch;
        public Evolutions _petLevel = Evolutions.Egg;
        private Needs feedbackType;
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
        public List<Glyph> Literals { get => literals; }

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
        public Need Hunger {  get => needs[(int)Needs.Hunger]; }

        /// <summary>
        /// The current Health need container.
        /// </summary>
        public Need Health { get => needs[(int)Needs.Health]; }

        /// <summary>
        /// The current Joy need container.
        /// </summary>
        public Need Joy { get => needs[(int)Needs.Joy]; }

        /// <summary>
        /// The current Energy need container.
        /// </summary>
        public Need Energy { get => needs[(int)Needs.Energy]; }

        float timer = 60;

        #endregion


        #region Unity Built-Ins

        void Awake()
        {
            if (_animator == null) TryGetComponent(out _animator);
            if (_audioSource == null) TryGetComponent(out _audioSource);
            if (_spriteRenderer == null) TryGetComponent(out _spriteRenderer);
            if (_boxCollider == null) TryGetComponent(out _boxCollider);

            _spriteRenderer.sprite = levelSprites[(int)_petLevel];

            GameInput.OnInputCheck += InputCheck;

            Minigame.OnGameStart += (need, cost) => UpdateNeed(Needs.Energy, cost);
            Minigame.OnGameStart += (need, cost) => feedbackType = need;
            Minigame.OnGameWin += UpdateNeed;
            Minigame.OnGameInit += SetNeeds;
            UpdateNeed(Needs.Hunger, needs[(int)Needs.Hunger].Current - Need.MAX);
            UpdateNeed(Needs.Joy, needs[(int)Needs.Joy].Current - Need.MAX);
            UpdateNeed(Needs.Energy, needs[(int)Needs.Energy].Current - Need.MAX);
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
                UpdateNeed(Needs.Hunger, -1);
                UpdateNeed(Needs.Joy, -1);
                UpdateNeed(Needs.Energy, -1);
            }
        }

        void Update()
        {

        }

        void OnDestroy()
        {
            GameInput.OnInputCheck -= InputCheck;

            Minigame.OnGameStart -= (need, cost) => UpdateNeed(Needs.Energy, cost);
            Minigame.OnGameStart -= (need, cost) => feedbackType = need;
            Minigame.OnGameWin -= UpdateNeed;
            Minigame.OnGameInit -= SetNeeds;
        }

        #endregion


        #region Events

        public static event Action<Glyph, Glyph[]> OnNeedCall;
        public static event Action<Needs, float> OnNeedUpdate;
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
            Glyph glyph = toMatch.Dequeue();

            if (glyph.Symbol.name == message || glyph.Character.name == message)
            {
                needFeedback.Setup(glyph.Sound, needs[(int)feedbackType].Positive);
                needFeedback.Show();
                OnNeedSatisfied?.Invoke(true);
            }
            else
            {
                needFeedback.Setup(glyph.Sound, needs[(int)feedbackType].Negative);
                needFeedback.Show();
                OnNeedSatisfied?.Invoke(false);

                toMatch.Enqueue(glyph);
            }
        }

        private void SetNeeds(int rounds)
        {
            if (rounds <= 0) return;

            Glyph[] currentGlyphs = literals.ToArray();
            toMatch = new();

            while (toMatch.Count < rounds)
            {
                int rand = UnityEngine.Random.Range(0, currentGlyphs.Length);
                Glyph glyph = currentGlyphs[rand];
                if (toMatch.Contains(glyph)) continue;

                toMatch.Enqueue(glyph);
            }

            GetNextNeed();
        }

        private void UpdateNeed(Needs need, float amount)
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

        private void GetNextNeed()
        {
            Glyph glyph = toMatch.Peek();
            needCall.Setup(glyph.Sound, glyph.Character);
            needCall.Show();
            OnNeedCall?.Invoke(glyph, literals.ToArray());
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
        /// Same as null.
        /// </summary>
        None,

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