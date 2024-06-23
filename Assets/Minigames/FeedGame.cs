using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore;

namespace GlyphaeScripts
{
    /// <summary>
    /// A Minigame played when executing basic functions.
    /// </summary>
    public class FeedGame : Minigame
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

        public override void SetupGame(List<Glyph> glyphs, Evolution gameLevel)
        {
            Glyph glyph;
            if (gameLevel == 0) return;
            currentGlyphs = glyphs.ToArray();
            toMatch = new();

            int baseline = (int)gameLevel / (Enum.GetNames(typeof(Evolution)).Length / 2);
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

            SetupRound();
        }

        #endregion


        #region Helpers

        protected override void InputCheck(string message)
        {
            Glyph glyph = toMatch.Dequeue();

            if (glyph.Symbol.name == message || glyph.Character.name == message)
            {
                Success();
            }
            else
            {
                toMatch.Enqueue(glyph);
                Fail();
            }
        }

        protected override void SetupRound()
        {
            if (toMatch.Count == 0)
            {
                Win();
                return;
            }

            List<Glyph> used = new();

            Glyph glyph = toMatch.Peek();
            used.Add(glyph);

            int correct = UnityEngine.Random.Range(0, buttonAmount);
            int index = 0;

            while (index < buttonAmount)
            {
                if (index == correct)
                {
                    gameInputs[index].Setup(glyph.Sound, glyph.Character);
                }
                else
                {
                    int rand = UnityEngine.Random.Range(0, currentGlyphs.Length);
                    Glyph randGlyph = currentGlyphs[rand];
                    if (randGlyph == null || used.Contains(randGlyph)) continue;

                    used.Add(randGlyph);
                    gameInputs[index].Setup(randGlyph.Sound, randGlyph.Character);
                }
                index++;
            }

            needBubble.Setup(glyph.Sound, glyph.Symbol);
            needBubble.Show(null);
        }

        #endregion
    }
}