using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GlyphaeScripts
{
    /// <summary>
    /// A Minigame played when executing basic functions.
    /// </summary>
    public class InitMixer : Minigame
    {
        #region Serialized Fields

        [SerializeField] private Settings settings;
        [SerializeField] private GameObject prefab;
        [SerializeField] private Image overlay;

        #endregion


        #region Fields

        private GameObject _instance;
        private Pet _egg;
        private bool _hasFailed = false;

        #endregion


        #region Events

        #endregion


        #region Methods

        public override void SetupGame(List<Glyph> glyphs, Evolutions petLevel)
        {
            _instance = Instantiate(prefab, transform.parent);
            _egg = _instance.GetComponent<Pet>();
            _egg.Literals = glyphs;
            foreach (GameButton button in gameInputs) button.SetupDrag(_instance.GetComponent<Transform>());

            SetupGame(petLevel);
        }

        public void SetupGame(Evolutions petLevel)
        {
            _failsToLose = minimumRounds << 1;

            Init(minimumRounds);
        }

        #endregion


        #region Helpers

        protected override void SetupRound(Glyph correctGlyph, Sprite correctIcon, Sprite wrongIcon, List<Glyph> allGlyphs)
        {
            string original = correctIcon.name;
            allGlyphs.Remove(correctGlyph);
            Glyph wrongGlyph = allGlyphs[UnityEngine.Random.Range(0, allGlyphs.Count)];

            int rng = UnityEngine.Random.Range(0, gameInputs.Length);

            Glyph[] glyphs = { correctGlyph, wrongGlyph };
            Color[] colors = { Color.green, Color.red };

            foreach (GameButton button in gameInputs)
            {
                Sprite icon = original.Contains("letter") ? glyphs[rng].Symbol : glyphs[rng].Letter;
                button.Setup(glyphs[rng].Sound, icon);
                if (_hasFailed) button.SetSColor(colors[rng]);
                
                rng--;
                rng = Mathf.Abs(rng);
            }
        }

        protected override void Win()
        {
            StartCoroutine(AnimateFade(0,1, settings.SpeedFactor));
        }

        protected override void Success()
        {
            _egg.IncreaseLevel();
            _successes++;
            if (_successes >= minimumRounds) Win();
        }


        protected override void Fail()
        {
            _fails++;
            if (_fails > _failsToLose) ResetGame();
        }

        private void ResetGame()
        {
            _fails = 0;
            _hasFailed = true;
        }

        private IEnumerator AnimateFade(float start, float end, float speedFactor)
        {
            Color color;

            for (float i = start; i <= end; i += Time.deltaTime * speedFactor)
            {
                color = overlay.color;
                color.a = i;
                overlay.color = color;
                yield return new WaitForEndOfFrame();
            }

            settings.FirstLevel = false;
            Destroy(_instance);
            Close();
        }

        #endregion
    }
}