using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        public override void SetupGame(List<GlyphData> glyphs, int level)
        {
            base.SetupGame(glyphs, level);

            _eggInstance = Instantiate(settings.Egg.gameObject, transform.parent);
            _egg = _eggInstance.GetComponent<Pet>();
            _egg.Literals = glyphs;
        }

        #endregion


        #region Helpers

        public override void NextRound()
        {
            _usedGlyphs = new();
            _toMatch = _allGlyphs[UnityEngine.Random.Range(0, _allGlyphs.Count)];
            _allGlyphs.Remove(_toMatch);

            GlyphData wrongGlyph = _allGlyphs[UnityEngine.Random.Range(0, _allGlyphs.Count)];
            _allGlyphs.Remove(wrongGlyph);
            _usedGlyphs.Add(wrongGlyph);

            Sprite[] sprites = { _toMatch.Symbol, _toMatch.Letter };
            int rng = UnityEngine.Random.Range(0, sprites.Length);
            Sprite correct = sprites[rng];


            rng = UnityEngine.Random.Range(0, gameInputs.Count);

            GlyphData[] glyphs = { _toMatch, wrongGlyph };
            Color[] colors = { Color.green, Color.red };

            foreach (GameButton button in gameInputs)
            {
                Sprite icon = correct == _toMatch.Letter ? glyphs[rng].Symbol : glyphs[rng].Letter;
                button.Setup(glyphs[rng], icon);

                if (_hasFailed)
                {
                    GameDrag drag = (GameDrag)button;
                    drag.SetSColor(colors[rng]);
                }

                rng--;
                rng = Mathf.Abs(rng);
            }

            DisplayRound(correct);
        }

        protected override void Win()
        {
            StartCoroutine(AnimateFade(0,1, settings.SpeedFactor));
        }

        protected override void Success()
        {
            _egg.IncreaseLevel();
            _successes++;
            if (_successes >= baseRounds) Win();
        }


        protected override void Fail()
        {
            _fails++;
            if (_fails >= _failsToLose) ResetGame();
        }

        private void ResetGame()
        {
            _fails = 0;
            _hasFailed = true;
        }

        private IEnumerator AnimateFade(float start, float end, float speedFactor)
        {
            yield return flashOverlay.Flash(start, end, speedFactor);

            OnEggBreak?.Invoke();
            yield return new WaitForSeconds(1f / speedFactor);

            Destroy(_eggInstance);
            CloseGame();
        }

        #endregion
    }
}