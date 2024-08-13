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
        List<GlyphData> _orderData;
        private List<int> _order;
        private int _orderIndex = 0;

        #endregion


        #region Unity Built-Ins

        private new void OnEnable()
        {
            base.OnEnable();
            TimeIcon.OnAnimationDone += AnimateNext;

        }

        private new void OnDisable()
        {
            base.OnDisable();
            TimeIcon.OnAnimationDone -= AnimateNext;
        }

        #endregion

        #region Methods

        public override void SetupGame(bool isTeaching, List<GlyphData> glyphs, int baseLevel)
        {
            base.SetupGame(isTeaching, glyphs, baseLevel);
            SelectGlyphs();

            List<Sprite> sprites = new();
            List<Sprite> reverses = new();
            string type = Random.Range(0, 2) == 0 ? "letter" : "icon";

            for (int i = 0; i < _buttonCount; i++)
            {
                GlyphData glyph = _usedGlyphs[i];
                Sprite sprite = type.Contains("letter") ? glyph.Symbol : glyph.Letter;

                _gameInputs[i].Setup(glyph, sprite);
                sprites.Add(sprite);
                type = sprite.name;

                sprite = type.Contains("letter") ? glyph.Symbol : glyph.Letter;
                reverses.Add(sprite);
            }

            _order = new();
            _orderData = new();
            while (_order.Count < _rounds)
            {
                int rng = Random.Range(0, _rounds);
                if (_order.Contains(rng)) continue;
                _order.Add(rng);
            }

            _timeIcons = new();
            for (int i = 0; i < _rounds; i++)
            {
                _orderData.Add(_usedGlyphs[_order[i] % _buttonCount]);
                Sprite sprite = reverses[_order[i] % _buttonCount];

                GameObject instance = Instantiate(template.gameObject, container);
                TimeIcon timer = instance.GetComponent<TimeIcon>();
                timer.Setup(sprite);
                timer.name = sprite.name;
                _timeIcons.Add(timer);
            }

            AnimateNext();
        }

        public override void UpdateValues()
        {
            primaryNeed?.Increase(_successes / _rounds * NeedData.MAX);
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
                _toMatch = _orderData[_order[_orderIndex] % _buttonCount];
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
                _toMatch = _orderData[_order[_orderIndex]];
                return;
            }

            _timeIcons[_order[_orderIndex]].Animate();
            _orderIndex++;
        }

        #endregion
    }
}