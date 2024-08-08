using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GlyphaeScripts
{
    /// <summary>
    /// Played in the pre-stage. To break the egg's shell.
    /// </summary>
    public class ShellBreaker : Minigame
    {
        #region Serialized Fields

        [Space]
        [Header("Game Specific")]
        [SerializeField] private FlashOverlay flashOverlay;

        #endregion


        #region Fields

        private GameObject _eggInstance;
        private Pet _egg;
        private bool _hasFailed = false;

        #endregion


        #region Events

        public static event Action OnEggBreak;

        #endregion


        #region Methods

        public override void SetupGame(bool isTeaching, List<GlyphData> glyphs, int baseLevel)
        {
            base.SetupGame(isTeaching, glyphs, baseLevel);
            SetupDragging();
            ResetGame();

            _eggInstance = Instantiate(settings.Egg.gameObject, transform.parent);
            _egg = _eggInstance.GetComponent<Pet>();
        }

        public override void NextRound()
        {
            if (_successes >= baseRounds) return;

            _toMatch = _newGlyphs[UnityEngine.Random.Range(0, _newGlyphs.Count)];
            _newGlyphs.Remove(_toMatch);
            _usedGlyphs.Add(_toMatch);

            // BUGS
            GlyphData wrongGlyph = _newGlyphs[UnityEngine.Random.Range(0, _newGlyphs.Count)];
            _newGlyphs.Remove(wrongGlyph);
            _usedGlyphs.Add(wrongGlyph);

            int rng = UnityEngine.Random.Range(0, _gameInputs.Count);
            Sprite correct = rng == 0 ? _toMatch.Symbol : _toMatch.Letter;

            GlyphData[] glyphs = { _toMatch, wrongGlyph };

            foreach (GameButton button in _gameInputs)
            {
                Sprite icon = correct == _toMatch.Letter ? glyphs[rng].Symbol : glyphs[rng].Letter;
                button.Setup(glyphs[rng], icon);


                GameDrag drag = (GameDrag)button;
                drag.DragColor = Mathf.Abs(rng-1);
                drag.Mark = _hasFailed;

                rng = Mathf.Abs(--rng);
            }

            DisplayRound(correct);
        }

        #endregion


        #region Helpers

        protected override void Win()
        {
            StartCoroutine(AnimateFade(0,1, settings.AnimationSpeed));
        }

        protected override void Success()
        {
            _egg.ChangeSprite(++_successes);
            MessageSuccess(PrimaryNeed.Positive);
            if (_successes >= baseRounds) Win();
        }


        protected override void Fail()
        {
            
            MessageFail(PrimaryNeed.Negative);
            if (++_fails >= _failsToLose)
            {
                _hasFailed = true;
                ResetGame();
            }
        }

        private void ResetGame()
        {
            _fails = 0;
            _successes = 0;

            if (_usedGlyphs == null || _usedGlyphs.Count == 0)
            {
                _usedGlyphs = new();
                return;
            }

            foreach (GlyphData item in _usedGlyphs)
            {
                item.MemoryLevel = MemoryLevels.New;
            }
            
            SetupGylphLists(_usedGlyphs);
        }

        private IEnumerator AnimateFade(float start, float end, float speedFactor)
        {
            yield return flashOverlay.Flash(start, end, speedFactor);
            foreach (GameDrag drag in _gameInputs) drag.gameObject.SetActive(false);

            OnEggBreak?.Invoke();
            yield return new WaitForSeconds(1f / speedFactor);

            Destroy(_eggInstance);
            CloseGame();
        }

        #endregion
    }
}