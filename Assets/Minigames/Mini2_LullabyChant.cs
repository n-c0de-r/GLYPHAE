using UnityEngine;

namespace GlyphaeScripts
{
    /// <summary>
    /// Basic game to put the <see cref="Pet"/> to sleep, reduces <see cref="NeedData"/> loss and keeps it silent.
    /// </summary>
    public class LullabyChant : Minigame
    {
        #region Serialized Fields



        #endregion


        #region Fields

        #endregion


        #region Events

        //public static event Action OnEggBreak;

        #endregion


        #region Methods

        public override void NextRound()
        {
            _usedGlyphs = new();
            _toMatch = _allGlyphs[Random.Range(0, _allGlyphs.Count)];
            _allGlyphs.Remove(_toMatch);

            int correctPosition = UnityEngine.Random.Range(0, _buttonCount);

            for (int i = 0; i < _buttonCount; i++)
            {
                if (i == correctPosition)
                {
                    gameInputs[i].Setup(_toMatch, _toMatch.Symbol);
                }
                else
                {
                    GlyphData wrongGlyph = _allGlyphs[Random.Range(0, _allGlyphs.Count)];
                    _allGlyphs.Remove(wrongGlyph);
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