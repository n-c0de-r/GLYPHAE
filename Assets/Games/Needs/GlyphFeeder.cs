using System.Collections.Generic;

namespace GlyphaeScripts
{
    /// <summary>
    /// Basic game to feed the <see cref="Pet"/> and satisfiy the <see cref="NeedData"/> of hunger.
    /// </summary>
    public class GlyphFeeder : Minigame
    {
        #region Methods

        public override void NextRound()
        {
            SetupDragging();

            List<GlyphData> temp = new(SelectGlyphs());
            _toMatch = temp[UnityEngine.Random.Range(0, temp.Count)];

            for (int i = 0; i < _buttonCount; i++)
            {
                GlyphData glyph = temp[UnityEngine.Random.Range(0, temp.Count)];

                if (_toLearn != null && _toLearn == glyph) _toMatch = _toLearn;

                temp.Remove(glyph);
                _gameInputs[i].Setup(glyph, glyph.Symbol);
            }

            DisplayRound(_toMatch.Letter);
        }

        #endregion
    }
}