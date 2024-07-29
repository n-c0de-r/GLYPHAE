//using System;
//using System.Collections.Generic;
//using UnityEngine;

//namespace GlyphaeScripts
//{
//    /// <summary>
//    /// A Minigame played when executing basic functions.
//    /// </summary>
//    public class SymbolFeeder : Minigame
//    {
//        #region Serialized Fields

//        [Tooltip("The display of the current need.")]
//        [SerializeField] private NeedBubble needBubble;

//        #endregion


//        #region Fields

//        private Queue<GlyphData> toMatch;
//        private GlyphData[] currentGlyphs;
//        private int buttonAmount;

//        #endregion


//        #region Methods

//        public override void SetupGame(List<GlyphData> glyphs, int petLevel)
//        {
//            //if (petLevel == Evolutions.Egg) return;

//            GlyphData glyph;
//            currentGlyphs = glyphs.ToArray();
//            toMatch = new();

//            int baseline = (int)petLevel / (Enum.GetValues(typeof(Evolutions)).Length / 2);
//            _failsToLose = baseline;
//            buttonAmount = (1 + baseline) << 1;

//            while (toMatch.Count < minimumRounds + baseline)
//            {
//                int rand = UnityEngine.Random.Range(0, currentGlyphs.Length);
//                glyph = currentGlyphs[rand];
//                if (glyph == null) continue;

//                toMatch.Enqueue(glyph);
//                currentGlyphs[rand] = null;
//            }

//            //SetupRound();
//        }

//        #endregion


//        #region Helpers

//        protected override void CheckInput(GlyphData message)
//        {
//            GlyphData glyph = toMatch.Dequeue();

//            //if (glyph.Symbol.name == message || glyph.Letter.name == message)
//            //{
//            //    Success();
//            //}
//            //else
//            //{
//            //    toMatch.Enqueue(glyph);
//            //    Fail();
//            //}
//        }

//        protected override void SetupRound(GlyphData glyph, Sprite correctIcon, Sprite wrongIcon, List<GlyphData> currentGlyphs)
//        {
//            if (toMatch.Count == 0)
//            {
//                Win();
//                return;
//            }

//            List<GlyphData> used = new();

//            GlyphData glyph1 = toMatch.Peek();
//            used.Add(glyph);

//            int correct = UnityEngine.Random.Range(0, buttonAmount);
//            int index = 0;

//            while (index < buttonAmount)
//            {
//                if (index == correct)
//                {
//                    gameInputs[index].Setup(glyph, glyph.Symbol);
//                }
//                else
//                {
//                    int rand = UnityEngine.Random.Range(0, currentGlyphs.Count);
//                    GlyphData randGlyph = currentGlyphs[rand];
//                    if (randGlyph == null || used.Contains(randGlyph)) continue;

//                    used.Add(randGlyph);
//                    gameInputs[index].Setup(randGlyph, randGlyph.Symbol);
//                }
//                index++;
//            }
//            // TODO: pet event
//            needBubble.Setup(glyph.Sound, glyph.Letter);
//            //needBubble.Show(null);
//        }
//        protected override void SetupRound(Sprite correctIcon, List<GlyphData> allGlyphs)
//        {
//            throw new System.NotImplementedException();
//        }

//        #endregion
//    }
//}