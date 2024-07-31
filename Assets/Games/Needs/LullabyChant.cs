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

        private new void OnEnable()
        {
            base.OnEnable();
            TimeIcon.OnAnimationDone += AnimateNext;
            NeedBubble.OnFeedbackDone += NextRound;

        }

        private new void OnDisable()
        {
            base.OnDisable();
            TimeIcon.OnAnimationDone -= AnimateNext;
            NeedBubble.OnFeedbackDone -= NextRound;
        }

        #endregion

        #region Methods

        public override void SetupGame(List<GlyphData> glyphs, int baseLevel)
        {
            base.SetupGame(glyphs, baseLevel);

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
                instance.name = _order[i] + "_" + _usedGlyphs[rng].name;
                _timeIcons[i] = timer;
            }

            AnimateNext();
        }

        public override void NextRound()
        {

        }

        #endregion


        #region Helpers

        protected override void CheckInput(GlyphData input)
        {
            if (_toMatch == input)
            {
                _toMatch.CorrectlyGuessed(primaryNeed.Positive);
                Success();

                _timeIcons[_order[_orderIndex]].Disable();
                _orderIndex++;
                if (_orderIndex >= _order.Count) return;
                _toMatch = _timeIcons[_order[_orderIndex]].Data;
            }
            else
            {
                _toMatch.WronglyGuessed(primaryNeed.Negative);
                Fail();
            }
        }

        private void AnimateNext()
        {
            if (_orderIndex >= _order.Count)
            {
                ActivateButtons(true);
                _orderIndex = 0;
                _toMatch = _timeIcons[_order[_orderIndex]].Data;
                return;
            }

            _timeIcons[_order[_orderIndex]].Animate();
            _orderIndex++;
        }

        #endregion
    }
}