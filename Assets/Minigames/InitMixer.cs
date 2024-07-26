using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GlyphaeScripts
{
    /// <summary>
    /// A Minigame played when executing basic functions.
    /// </summary>
    public class InitMixer : Minigame
    {
        #region Serialized Fields

        [SerializeField] private Pet egg;
        [SerializeField] private Settings settings;
        [SerializeField] private Color[] spCols;

        #endregion


        #region Fields

        private GameObject _instance;
        private bool _hasFailed = false;

        #endregion


        #region Events

        public static event Action OnFinished;

        #endregion


        #region Methods

        public override void SetupGame(List<Glyph> glyphs, Evolutions petLevel)
        {
            _instance = Instantiate(egg.gameObject, transform.parent);
            _instance.GetComponent<Pet>().Literals = glyphs;
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

        protected override void InputCheck(string message)
        {
            
        }

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
            Debug.Log("win");
            OnFinished?.Invoke();
            Destroy(_instance);
            Destroy(gameObject);
        }

        protected override void Success()
        {
            _instance.GetComponent<SpriteRenderer>().color = spCols[_successes];
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
                //float value = Mathf.Abs(i);
                //color = back.color;
                //color.a = value;
                //back.color = color;

                //color = iconBack.color;
                //color.a = value;
                //iconBack.color = color;

                //color = iconLine.color;
                //color.a = value;
                //iconLine.color = color;

                //color = outline.color;
                //color.a = value;
                //outline.color = color;
                //yield return new WaitForEndOfFrame();
            }

            if (end > 0)
            {
                yield return new WaitForSeconds(1f / speedFactor);
                yield return AnimateFade(-1, 0, settings.SpeedFactor * 2);
            }
            //OnAnimationDone?.Invoke();
        }

        #endregion
    }
}