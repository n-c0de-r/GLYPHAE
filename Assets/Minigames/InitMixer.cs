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

        #endregion


        #region Fields

        private int buttonAmount;

        #endregion


        #region Methods

        public override void SetupGame(List<Glyph> glyphs, Evolutions petLevel)
        {
            SetupGame(petLevel);
        }

        public void SetupGame(Evolutions petLevel)
        {
            if (petLevel == Evolutions.None) return;

            buttonAmount = 2;

            _failsToLose = minimumRounds << 1;

            Init(minimumRounds);
        }

        #endregion


        #region Helpers

        protected override void InputCheck(string message)
        {
            
        }

        protected override void SetupRound(Glyph correctGlyph, Glyph[] allGlyphs)
        {
            if (_successes >= minimumRounds)
            {
                Win();
                return;
            }

            List<Glyph> used = new() { correctGlyph };

            int correct = Random.Range(0, buttonAmount);
            int index = 0;

            int selector = Random.Range(0, 2);

            while (index < buttonAmount)
            {
                if (index == correct)
                {
                    if (selector == 0) gameInputs[index].Setup(correctGlyph.Sound, correctGlyph.Character);
                    else gameInputs[index].Setup(correctGlyph.Sound, correctGlyph.Symbol);
                }
                else
                {
                    int rand = Random.Range(0, allGlyphs.Length);
                    Glyph randGlyph = allGlyphs[rand];
                    if (used.Contains(randGlyph)) continue;

                    used.Add(randGlyph);

                    if (selector == 0) gameInputs[index].Setup(randGlyph.Sound, randGlyph.Character);
                    else gameInputs[index].Setup(randGlyph.Sound, randGlyph.Symbol);
                }
                index++;
            }
        }

        #endregion
    }
}