using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static Unity.Collections.AllocatorManager;

namespace GlyphaeScripts
{
    /// <summary>
    /// Represents a the Pet with all behaviors 
    /// and data in the game. Refe
    /// </summary>
    [RequireComponent(typeof(Animator), typeof(AudioSource))]
    [RequireComponent(typeof(BoxCollider2D), typeof(SpriteRenderer))]
    public class Pet : MonoBehaviour, IClickable
    {
        #region Serialized Fields

        [Header("Internal values")]
        [Tooltip("The prefab of this Pet")]
        [SerializeField] private GameObject prefab;

        [Tooltip("The sprites this Pet\r\ngoes through its evolutions")]
        [SerializeField] private Sprite[] levelSprites;

        // This can be removed
        [SerializeField] private InputActionReference click;

        [Header("Game related values")]
        [Tooltip("The list of Glyphs\r\nthis Pet needs to learn.")]
        [SerializeField] private List<Glyph> literals;

        [Tooltip("The value at which\r\na need sets a notification.")]
        [SerializeField][Range(0, 50)] private float critical = 10;

        [Tooltip("Gets the current state\r\nif the Pet is already unlocked.")]
        [SerializeField] private bool unlocked;

        #endregion


        #region Fields

        public const float MIN = 0, MAX = 100;

        private Animator _animator;
        private AudioSource _audioSource;
        private SpriteRenderer _spriteRenderer;
        private BoxCollider2D _boxCollider;

        private Dictionary<Need, float> needs = new() { { Need.Hunger, 0 }, { Need.Health, 100 }, { Need.Joy, 0 }, { Need.Energy, 100 } };
        public Evolution _petLevel = Evolution.Egg;
        private int _clickTimes;
        private bool _clicked = false;
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
        /// The current <see cref="Evolution"/> level
        /// enum of this <see cref="Pet"/>
        /// </summary>
        public Evolution PetLevel { get => _petLevel; }

        /// <summary>
        /// A Dictionary containing all basic <see cref="Need"/>s: Hunger, Health, Joy, Energy
        /// </summary>
        public Dictionary<Need, float> Needs { get => needs; set => needs = value; }

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

            click.action.performed += OnClick;
            Minigame.OnGameStart += (cost) => UpdateNeed(Need.Energy, cost);
            Minigame.OnGameWin += UpdateNeed;

            OnNeedUpdate?.Invoke(Need.Hunger, needs[Need.Hunger] - MAX);
            OnNeedUpdate?.Invoke(Need.Energy, needs[Need.Energy] - MAX);
            OnNeedUpdate?.Invoke(Need.Joy, needs[Need.Joy] - MAX);
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
                UpdateNeed(Need.Hunger, -1);
                UpdateNeed(Need.Energy, -1);
                UpdateNeed(Need.Joy, -1);
            }
        }

        void Update()
        {

        }

        void OnDestroy()
        {
            click.action.performed -= OnClick;
            Minigame.OnGameStart -= (cost) => UpdateNeed(Need.Hunger, cost);
            Minigame.OnGameWin += UpdateNeed;
        }

        #endregion


        #region Events

        public static event Action<Need, float> OnNeedUpdate;

        #endregion


        #region Methods

        public void OnClick(InputAction.CallbackContext context)
        {
            if (!_clicked)
            {
                switch (_petLevel)
                {
                    case Evolution.Egg:
                        SwitchSprite();
                        break;

                    case Evolution.Baby:
                        break;

                    default:
                        break;
                }
                _clicked = !_clicked;
            }
        }

        private void UpdateNeed(Need need, float amount)
        {
            float value = needs[need] + amount;
            needs[need] = Mathf.Clamp(value, MIN, MAX);
            OnNeedUpdate?.Invoke(need, amount);

            if (!hasCalled && value <= critical)
            {
                hasCalled = true;
                Debug.Log(need + " is low!");
                //MessageNeed();
            }
            else if (hasCalled && value >= MAX / 2)
            {
                hasCalled = false;
            }
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

        private void CrackEgg()
        {
            _clickTimes++;
            if(_clickTimes >= 3)
            {
                _animator.SetTrigger("OpenEgg");
                _clickTimes = 0;
                _petLevel++;
                _spriteRenderer.sprite = levelSprites[(int)_petLevel % levelSprites.Length];
                _boxCollider.size = _spriteRenderer.size;
            }
        }

        #endregion
    }
}