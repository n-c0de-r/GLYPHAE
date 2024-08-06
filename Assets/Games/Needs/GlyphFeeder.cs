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
            SelectGlyphs();

            SetupDragging();

            int correctPosition = Random.Range(0, _buttonCount);

            for (int i = 0; i < _buttonCount; i++)
            {
                _gameInputs[i].Setup(_usedGlyphs[i], _usedGlyphs[i].Symbol);

                if (i == correctPosition) _toMatch = _usedGlyphs[i];
            }

            DisplayRound(_toMatch.Letter);
        }

        #endregion


        #region Helpers



        #endregion
    }
}