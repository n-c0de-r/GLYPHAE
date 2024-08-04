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

        #endregion


        #region Methods

        public override void NextRound()
        {
            _usedGlyphs = new();

            if (_allOtherGlyphs.Count > 0)
            {
                _toMatch = _allOtherGlyphs[Random.Range(0, _allOtherGlyphs.Count)];
                _allOtherGlyphs.Remove(_toMatch);
            }
            else
            {
                _toMatch = _newGlyphs[Random.Range(0, _newGlyphs.Count)];
                _newGlyphs.Remove(_toMatch);
            }
            _usedGlyphs.Add(_toMatch);

            int correctPosition = Random.Range(0, _buttonCount);

            for (int i = 0; i < _buttonCount; i++)
            {
                if (i == correctPosition)
                {
                    gameInputs[i].Setup(_toMatch, _toMatch.Symbol);
                }
                else
                {
                    GlyphData wrongGlyph;
                    if (_allOtherGlyphs.Count > 0)
                    {
                        wrongGlyph = _allOtherGlyphs[Random.Range(0, _allOtherGlyphs.Count)];
                        _allOtherGlyphs.Remove(wrongGlyph);
                    }
                    else
                    {
                        wrongGlyph = _newGlyphs[Random.Range(0, _newGlyphs.Count)];
                        _newGlyphs.Remove(wrongGlyph);
                    }
                    _usedGlyphs.Add(wrongGlyph);
                    gameInputs[i].Setup(wrongGlyph, wrongGlyph.Symbol);
                }
            }

            DisplayRound(_toMatch.Letter);
        }

        #endregion


        #region Helpers



        #endregion
    }
}