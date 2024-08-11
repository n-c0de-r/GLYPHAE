using System;
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

        public static event Action OnSleep;

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

            for (int i = 0; i < _buttonCount; i++)
                _gameInputs[i].Setup(_usedGlyphs[i], _usedGlyphs[i].Letter);

            _order = new();
            while (_order.Count < _rounds)
            {
                int rng = UnityEngine.Random.Range(0, _rounds);
                if (_order.Contains(rng)) continue;
                _order.Add(rng);
            }

            _timeIcons = new();
            for (int i = 0; i < _rounds; i++)
            {
                GameObject instance = Instantiate(template.gameObject, container);
                TimeIcon timer = instance.GetComponent<TimeIcon>();
                timer.Setup(_usedGlyphs[i % _usedGlyphs.Count], _usedGlyphs[i % _usedGlyphs.Count].Symbol);
                _timeIcons.Add(timer);
            }

            AnimateNext();
        }

        #endregion


        #region Helpers

        protected override void Win()
        {
            OnSleep?.Invoke();
            base.Win();
        }

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