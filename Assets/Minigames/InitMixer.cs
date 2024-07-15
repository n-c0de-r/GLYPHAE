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

        protected override void SetupRound(Glyph correctGlyph, Sprite correctIcon, Sprite wrongIcon, Glyph[] allGlyphs)
        {
            List<Glyph> used = new() { correctGlyph };

            int correct = Random.Range(0, buttonAmount);
            int index = 0;

            //int iconType = Random.Range(0, 2);

            while (index < buttonAmount)
            {
                if (index == correct)
                {
                    gameInputs[index].Setup(correctGlyph.Sound, correctIcon);
                }
                else
                {
                    int rand = Random.Range(0, allGlyphs.Length);
                    Glyph randGlyph = allGlyphs[rand];
                    if (used.Contains(randGlyph)) continue;

                    used.Add(randGlyph);

                    gameInputs[index].Setup(randGlyph.Sound, wrongIcon);
                }
                index++;
            }
        }

        protected override void Win()
        {
            Debug.Log("win");
        }

        #endregion
    }
}