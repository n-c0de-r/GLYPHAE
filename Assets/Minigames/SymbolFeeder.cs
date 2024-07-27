using System;
using System.Collections.Generic;
using UnityEngine;

namespace GlyphaeScripts
{
    /// <summary>
    /// A Minigame played when executing basic functions.
    /// </summary>
    public class SymbolFeeder : Minigame
    {
        #region Serialized Fields

        [Tooltip("The display of the current need.")]
        [SerializeField] private NeedBubble needBubble;

        #endregion


        #region Fields

        private Queue<Glyph> toMatch;
        private Glyph[] currentGlyphs;
        private int buttonAmount;

        #endregion


        #region Methods

        public override void SetupGame(List<Glyph> glyphs, Evolutions petLevel)
        {
            if (petLevel == Evolutions.Egg) return;

            Glyph glyph;
            currentGlyphs = glyphs.ToArray();
            toMatch = new();

            int baseline = (int)petLevel / (Enum.GetNames(typeof(Evolutions)).Length / 2);
            _failsToLose = baseline;
            buttonAmount = (1 + baseline) << 1;

            while (toMatch.Count < minimumRounds + baseline)
            {
                int rand = UnityEngine.Random.Range(0, currentGlyphs.Length);
                glyph = currentGlyphs[rand];
                if (glyph == null) continue;

                toMatch.Enqueue(glyph);
                currentGlyphs[rand] = null;
            }

            //SetupRound();
        }

        #endregion


        #region Helpers

        protected void InputCheck(string message)
        {
            Glyph glyph = toMatch.Dequeue();

            if (glyph.Symbol.name == message || glyph.Letter.name == message)
            {
                Success();
            }
            else
            {
                toMatch.Enqueue(glyph);
                Fail();
            }
        }

        protected override void SetupRound(Glyph glyph, Sprite correctIcon, Sprite wrongIcon, List<Glyph> currentGlyphs)
        {
            if (toMatch.Count == 0)
            {
                Win();
                return;
            }

            List<Glyph> used = new();

            Glyph glyph1 = toMatch.Peek();
            used.Add(glyph);

            int correct = UnityEngine.Random.Range(0, buttonAmount);
            int index = 0;

            while (index < buttonAmount)
            {
                if (index == correct)
                {
                    gameInputs[index].Setup(glyph.Sound, glyph.Symbol);
                }
                else
                {
                    int rand = UnityEngine.Random.Range(0, currentGlyphs.Count);
                    Glyph randGlyph = currentGlyphs[rand];
                    if (randGlyph == null || used.Contains(randGlyph)) continue;

                    used.Add(randGlyph);
                    gameInputs[index].Setup(randGlyph.Sound, randGlyph.Symbol);
                }
                index++;
            }
            // TODO: pet event
            needBubble.Setup(glyph.Sound, glyph.Letter);
            //needBubble.Show(null);
        }

        #endregion
    }
}