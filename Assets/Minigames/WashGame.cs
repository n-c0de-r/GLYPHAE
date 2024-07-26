using System;
using System.Collections.Generic;
using UnityEngine;

namespace GlyphaeScripts
{
    /// <summary>
    /// A Minigame played when executing basic functions.
    /// </summary>
    public class WashGame : Minigame
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


        #region Unity Built-Ins

        void Awake()
        {
            
        }

        void Start()
        {
            
        }

        void FixedUpdate()
        {
            
        }

        void Update()
        {
            
        }

        #endregion


        #region Events



        #endregion


        #region Methods

        public override void SetupGame(List<Glyph> glyphs, Evolutions petLevel)
        {
            if (petLevel == 0) return;

            currentGlyphs = glyphs.ToArray();
            toMatch = new();

            int baseline = (int)petLevel / (Enum.GetNames(typeof(Evolutions)).Length / 2);
            int rounds = minimumRounds + baseline;
            buttonAmount = (1 + baseline) << 1;

            while (toMatch.Count < rounds)
            {
                int rand = UnityEngine.Random.Range(0, currentGlyphs.Length);
                Glyph glyph = currentGlyphs[rand];
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
            Glyph glyph = toMatch.Peek();
            if (glyph.Symbol.name == message || glyph.Letter.name == message) toMatch.Dequeue();

            if (toMatch.Count == 0)
            {
                SendMessageUpwards("CloseMinigame");
                Destroy(gameObject);

                //Settings.NeedUpdate(needType, needAmount);
                //Settings.NeedUpdate(Needs.Energy, -energyCost);
            }
            else
            {
                //SetupRound();
            }
        }

        protected override void SetupRound(Glyph glyph, Sprite correctIcon, Sprite wrongIcon, List<Glyph> currentGlyphs)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}