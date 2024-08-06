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
        private List<Sprite> previousSprites;
        private Sprite previous;
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

        public override void SetupGame(bool isTeaching, List<GlyphData> glyphs, int baseLevel)
        {
            base.SetupGame(isTeaching, glyphs, baseLevel);

            _usedGlyphs = new();
            _timeIcons = new(new TimeIcon[_rounds]);
            previousSprites = new();

            _order = new();
            while (_order.Count < _rounds)
            {
                int rng = Random.Range(0, _rounds);
                if (_order.Contains(rng)) continue;
                _order.Add(rng);
            }

            for (int i = 0; i < _buttonCount; i++)
            {
                if (_isTeaching && !_hasLearned && _newGlyphs.Count > 0)
                {
                    // On criticals prefer new glyphs, to teach
                    _toMatch = _newGlyphs[Random.Range(0, _newGlyphs.Count)];
                    _newGlyphs.Remove(_toMatch);
                    _hasLearned = true;
                }
                else if (_allOtherGlyphs.Count > 0)
                {
                    // Normally pick known ones
                    _toMatch = _allOtherGlyphs[Random.Range(0, _allOtherGlyphs.Count)];
                    _allOtherGlyphs.Remove(_toMatch);
                }
                _usedGlyphs.Add(_toMatch);
            }

            for (int i = 0; i < _buttonCount; i++)
            {
                if (previous == null)
                {
                    int rng = Random.Range(0, 2);

                    gameInputs[i].Setup(_toMatch, rng == 0 ? _toMatch.Symbol : _toMatch.Letter);
                    previousSprites.Add(rng == 0 ? _toMatch.Letter : _toMatch.Symbol);
                    previous = rng == 0 ? _toMatch.Symbol : _toMatch.Letter;
                }
                else if (previous.name.Contains("letter"))
                {
                    gameInputs[i].Setup(_toMatch, _toMatch.Symbol);
                    previousSprites.Add(_toMatch.Letter);
                    previous = _toMatch.Symbol;
                }
                else
                {
                    gameInputs[i].Setup(_toMatch, _toMatch.Letter);
                    previousSprites.Add(_toMatch.Symbol);
                    previous = _toMatch.Letter;
                }
            }

            for (int i = 0; i < _rounds; i++)
            {
                GameObject instance = Instantiate(template.gameObject, container);
                instance.SetActive(true);
                TimeIcon timer = instance.GetComponent<TimeIcon>();

                timer.Setup(_usedGlyphs[_order[i] % _usedGlyphs.Count], previousSprites[_order[i] % previousSprites.Count]);
                instance.name = _order[i] + "_" + previousSprites[_order[i] % previousSprites.Count].name;
                _timeIcons[i] = timer;
            }

            AnimateNext();
        }

        public override void NextRound()
        {

        }

        public override void UpdateValues()
        {
            primaryNeed?.Increase(NeedData.MAX);
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