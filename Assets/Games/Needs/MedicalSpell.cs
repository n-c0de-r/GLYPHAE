using System.Collections.Generic;
using UnityEngine;

namespace GlyphaeScripts
{
    /// <summary>
    /// Basic game to heal and clean the <see cref="Pet"/>, removes sicknesses and restores health <see cref="NeedData"/>.
    /// </summary>
    public class MedicalSpell : Minigame
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
        private List<Sprite> _sprites;
        private int _orderIndex = 0;

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

        public override void SetupGame(bool isTeaching, List<GlyphData> glyphs, int baseLevel)
        {
            base.SetupGame(isTeaching, glyphs, baseLevel);

            List<GlyphData> temp = new(SelectGlyphs());

            _sprites = new();
            string type = Random.Range(0, 2) == 0 ? "letter" : "symbol";

            for (int i = 0; i < _buttonCount; i++)
            {
                GlyphData glyph = temp[Random.Range(0, temp.Count)];

                if (_toLearn != null && _toLearn == glyph) glyph = _toLearn;
                temp.Remove(glyph);

                Sprite sprite;
                sprite = type.Contains("letter") ? glyph.Symbol : glyph.Letter;
                type = type.Contains("letter") ? glyph.Symbol.name : glyph.Letter.name;

                _gameInputs[i].Setup(glyph, sprite);
                _sprites.Add(sprite);
            }

            _order = new();
            while (_order.Count < _rounds)
            {
                int rng = Random.Range(0, _rounds);
                if (_order.Contains(rng)) continue;
                _order.Add(rng);
            }

            _timeIcons = new();
            for (int i = 0; i < _rounds; i++)
            {
                GlyphData glyph = _usedGlyphs[i %  _usedGlyphs.Count];
                Sprite sprite = _sprites[i % _sprites.Count].name.Contains("letter") ? glyph.Symbol : glyph.Letter;

                GameObject instance = Instantiate(template.gameObject, container);
                TimeIcon timer = instance.GetComponent<TimeIcon>();
                timer.Setup(glyph, sprite);
                _timeIcons.Add(timer);
            }

            AnimateNext();
        }

        public override void UpdateValues()
        {
            primaryNeed?.Increase(_successes / _rounds *NeedData.MAX);
            base.UpdateValues();
        }

        #endregion


        #region Helpers

        protected override void CheckInput(GlyphData input)
        {
            if (_toMatch == input)
            {
                _toMatch.CorrectlyGuessed();
                Success();

                _timeIcons[_order[_orderIndex]].Disable();
                _orderIndex++;
                if (_orderIndex >= _order.Count) return;
                _toMatch = _timeIcons[_order[_orderIndex]].Data;
            }
            else
            {
                _toMatch.WronglyGuessed();
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