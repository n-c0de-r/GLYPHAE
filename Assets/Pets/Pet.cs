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

        [SerializeField] private Sprite[] levelSprites;
        [SerializeField] private List<Glyph> literals;
        [SerializeField] private InputActionReference click;
        [SerializeField][Range(0, 50)] private float critical = 10;
        [SerializeField] private bool unlocked;

        #endregion


        #region Fields

        public const float MIN = 0, MAX = 100;

        private Animator _animator;
        private AudioSource _audioSource;
        private SpriteRenderer _spriteRenderer;
        private BoxCollider2D _boxCollider;

        private Dictionary<Need, float> needs = new() { { Need.Hunger, 100 }, { Need.Health, 100 }, { Need.Joy, 100 }, { Need.Energy, 100 } };
        private Evolution _currentLevel;
        private int _clickTimes;
        private bool _clicked = false;
        private bool hasCalled = false;


        #endregion


        #region GetSets / Properties

        #endregion


        #region Unity Built-Ins

        void Awake()
        {
            if (_animator == null) TryGetComponent(out _animator);
            if (_audioSource == null) TryGetComponent(out _audioSource);
            if (_spriteRenderer == null) TryGetComponent(out _spriteRenderer);
            if (_boxCollider == null) TryGetComponent(out _boxCollider);

            _spriteRenderer.sprite = levelSprites[(int)_currentLevel];

            click.action.performed += OnClick;
            Settings.OnNeedUpdate += UpdateNeed;
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

        void OnDestroy()
        {
            click.action.performed -= OnClick;
            Settings.OnNeedUpdate -= UpdateNeed;
        }

        #endregion


        #region Events

        #endregion


        #region Methods

        public void OnClick(InputAction.CallbackContext context)
        {
            if (!_clicked)
            {
                switch (_currentLevel)
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

        #endregion


        #region Helpers

        private void UpdateNeed(Need need, float amount)
        {
            float value = needs[need] + amount;
            needs[need] = Mathf.Clamp(value, MIN, MAX);

            if (!hasCalled && value <= critical)
            {
                hasCalled = true;
                Debug.Log(need + " is low!");
                //MessageNeed();
            }
            else if (hasCalled && value >= MAX/2)
            {
                hasCalled = false;
            }
        }

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
                _currentLevel++;
                _spriteRenderer.sprite = levelSprites[(int)_currentLevel % levelSprites.Length];
                _boxCollider.size = _spriteRenderer.size;
            }
        }

        #endregion
    }
}