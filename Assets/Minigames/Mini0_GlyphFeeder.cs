using System.Collections.Generic;
using UnityEngine;

namespace GlyphaeScripts
{
    /// <summary>
    /// Basic game to feed the <see cref="Pet"/> and satisfiy the <see cref="NeedData"/> of hunger.
    /// </summary>
    public class GlyphFeeder : Minigame
    {
        #region Serialized Fields



        #endregion


        #region Fields

        #endregion


        #region Events

        //public static event Action OnEggBreak;

        #endregion


        #region Methods

        public override void SetupGame(List<GlyphData> glyphs, int level)
        {

            for (int i = 0; i < _buttonCount; i++)
            {
                GameDrag button = (GameDrag)gameInputs[i];
                button.SetupDrag(settings.PetInstance.GetComponent<Transform>());
            }

            _failsToLose = minimumRounds;

            base.SetupGame(glyphs, level);
        }

        #endregion


        #region Helpers

        protected override void NextRound()
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

                rng--;
                rng = Mathf.Abs(rng);
            }

            DisplayRound(correct);
        }

        #endregion
    }
}