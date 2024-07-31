using System.Collections.Generic;
using UnityEngine;

namespace GlyphaeScripts
{
    /// <summary>
    /// Basic game to put the <see cref="Pet"/> to sleep, reduces <see cref="NeedData"/> loss and keeps it silent.
    /// </summary>
    public class LullabyChant : Minigame
    {
        #region Serialized Fields

        [Space]
        [Header("Game Specific")]
        [Tooltip("The container where the list of items to match will spawn.")]
        [SerializeField] private Transform container;

        [Tooltip("The icon template to spawn and setup.")]
        [SerializeField] private TimeIcon template;

        #endregion


        #region Fields

        private List<TimeIcon> _timeIcons;
        private List<int> _order;
        private int _orderIndex = 0;

        #endregion


        #region Events



        #endregion


        #region Events



        #endregion


        #region GetSets / Properties



        #endregion


        #region Unity Built-Ins

        void Awake()
        {
            
        }

        private void OnEnable()
        {
            TimeIcon.OnAnimationDone += AnimateNext;

            GameButton.OnInput += CheckInput;

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

        private void OnDisable()
        {
            TimeIcon.OnAnimationDone -= AnimateNext;

            GameButton.OnInput -= CheckInput;

        }

        #endregion

        #region Methods

        public override void SetupGame(List<GlyphData> glyphs, int baseLevel)
        {
            base.SetupGame(glyphs, baseLevel);
        }

        public override void NextRound()
        {
            _usedGlyphs = new();
            _timeIcons = new(new TimeIcon[_rounds]);

            _order = new();
            while (_order.Count < _rounds)
            {
                int rng = Random.Range(0, _rounds);
                if (_order.Contains(rng)) continue;
                _order.Add(rng);
            }

            for (int i = 0; i < _buttonCount; i++)
            {
                if (_allOtherGlyphs.Count > 0)
                {
                    _toMatch = _allOtherGlyphs[Random.Range(0, _allOtherGlyphs.Count)];
                    _allOtherGlyphs.Remove(_toMatch);
                }
                else
                {
                    _toMatch = _newGlyphs[Random.Range(0, _newGlyphs.Count)];
                    _newGlyphs.Remove(_toMatch);
                }
                
                gameInputs[i].Setup(_toMatch, _toMatch.Letter);
                _usedGlyphs.Add(_toMatch);
            }

            for (int i = 0; i < _rounds; i++)
            {
                GameObject instance = Instantiate(template.gameObject, container);
                instance.SetActive(true);
                TimeIcon timer = instance.GetComponent<TimeIcon>();

                int rng = Random.Range(0, _buttonCount);

                timer.Setup(_usedGlyphs[rng], _usedGlyphs[rng].Symbol);
                _timeIcons[_order[i]] = timer;
            }

            AnimateNext();
        }

        #endregion


        #region Helpers

        private void AnimateNext()
        {
            if (_orderIndex >= _order.Count)
            {
                ActivateButtons(true);
                _orderIndex = 0;
                return;
            }

            _timeIcons[_orderIndex].Animate();
            _orderIndex++;
        }

        #endregion
    }
}