//using System;
//using System.Collections.Generic;
//using UnityEngine;

//namespace GlyphaeScripts
//{
//    /// <summary>
//    /// A Minigame played when executing basic functions.
//    /// </summary>
//    public class WashGame : Minigame
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


//        #region Unity Built-Ins

//        void Awake()
//        {
            
//        }

//        void Start()
//        {
            
//        }

//        void FixedUpdate()
//        {
            
//        }

//        void Update()
//        {
            
//        }

//        #endregion


//        #region Events



//        #endregion


//        #region Methods

//        public override void SetupGame(List<GlyphData> glyphs, int petLevel)
//        {
//            if (petLevel == 0) return;

//            currentGlyphs = glyphs.ToArray();
//            toMatch = new();

//            int baseline = (int)petLevel / (Enum.GetNames(typeof(Evolutions)).Length / 2);
//            int rounds = minimumRounds + baseline;
//            buttonAmount = (1 + baseline) << 1;

//            while (toMatch.Count < rounds)
//            {
//                int rand = UnityEngine.Random.Range(0, currentGlyphs.Length);
//                GlyphData glyph = currentGlyphs[rand];
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
//            GlyphData glyph = toMatch.Peek();
//            //if (glyph.Symbol.name == message || glyph.Letter.name == message) toMatch.Dequeue();

//            //if (toMatch.Count == 0)
//            //{
//            //    SendMessageUpwards("CloseMinigame");
//            //    Destroy(gameObject);

//            //    //Settings.NeedUpdate(needType, needAmount);
//            //    //Settings.NeedUpdate(Needs.Energy, -energyCost);
//            //}
//            //else
//            //{
//            //    //SetupRound();
//            //}
//        }

//        protected override void SetupRound(GlyphData glyph, Sprite correctIcon, Sprite wrongIcon, List<GlyphData> currentGlyphs)
//        {
//            throw new NotImplementedException();
//        }

//        protected override void SetupRound(Sprite correctIcon, List<GlyphData> allGlyphs)
//        {
//            throw new System.NotImplementedException();
//        }

//        #endregion
//    }
//}