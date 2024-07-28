using System;
using System.Collections.Generic;
using UnityEngine;

namespace GlyphaeScripts
{
    /// <summary>
    /// Represents an abstract idea of a game.
    /// Encapsulates the basic values and functions
    /// each <see cref="Minigame"/> should have to function.
    /// </summary>
    public abstract class Minigame : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField] protected Settings settings;

        [Header("Base Values")]
        [Tooltip("The match type \r\nthis game belongs to.")]
        [SerializeField] protected GameType type;

        [Tooltip("The Inputs to set up at start.")]
        [SerializeField] protected GameButton[] gameInputs;

        [Tooltip("Minimum number of rounds to play this game.")]
        [SerializeField][Range(1, 3)] protected int minimumRounds = 1;

        [Space]
        [Header("Need Values")]
        [Tooltip("The type of need this game fills.")]
        [SerializeField] protected NeedData need;

        [Tooltip("The strength of need filling by the game.")]
        [SerializeField][Range(10, 50)] protected int needFill;

        #endregion Serialized Fields


        #region Fields

        protected int _successes, _fails, _failsToLose;

        #endregion Fields


        #region Events

        public static event Action<GameObject> OnGameClose;
        public static event Action<GameType, int> OnGameInit;

        #endregion


        #region Unity Built-Ins

        void Awake()
        {
            Pet.OnNeedCall += SetupRound;
            Pet.OnNeedSatisfied += (state) => { if (state) Success(); else Fail(); };
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

        private void OnDestroy()
        {
            Pet.OnNeedCall -= SetupRound;
            Pet.OnNeedSatisfied -= (state) => { if (state) Success(); else Fail(); };
        }

        #endregion


        #region GetSets / Properties

        /// <summary>
        /// Minimum number of rounds to play this game.
        /// </summary>
        public int MinimumRounds { get => minimumRounds; }

        #endregion


        #region Methods

        /// <summary>
        /// Wrapper method to be able to call the event from subclasses.
        /// </summary>
        /// <param name="actualRounds">The number of actual rounds to play after all calculations are done.</param>
        protected void Init(int actualRounds)
        {
            OnGameInit?.Invoke(type, actualRounds);
        }

        /// <summary>
        /// Trigger this when you achieved a success.
        /// It counts and manages everything else.
        /// </summary>
        protected virtual void Success()
        {
            _successes++;
            if (_successes >= minimumRounds) Win();
        }

        /// <summary>
        /// Use this when you made a mistake.
        /// It counts and manages everything else.
        /// </summary>
        protected virtual void Fail()
        {
            _fails++;
            if (_fails > _failsToLose) Close();
        }

        /// <summary>
        /// Informs the BaseGame Controller, that the game
        /// triggered a win condition and stops the game.
        /// </summary>
        protected virtual void Win()
        {
            need.setData(needFill);
            Close();
        }

        /// <summary>
        /// Informs the BaseGame Controller, that the game
        /// has ended and can be closed and destroyed.
        /// </summary>
        protected void Close()
        {
            OnGameClose?.Invoke(gameObject);
        }

        /// <summary>
        /// Runs the Win animation.
        /// </summary>
        protected void AnimateSuccess()
        {
            // TODO: Pet event calls
            //reactionBubble.Setup(positiveFeedback);
            //reactionBubble.Show(nameof(SetupRound));
        }

        /// <summary>
        /// Runs the Lose animation.
        /// </summary>
        protected void AnimateFail()
        {
            //reactionBubble.Setup(negativeFeedback);
            //reactionBubble.Show(nameof(SetupRound));
        }

        /// <summary>
        /// Sets up initial values for the game through the <see cref="GameMenu"/>.
        /// </summary>
        /// <param name="glyphs">The current list of glyphs the <see cref="Pet"/> holds.</param>
        /// <param name="petLevel"><The <see cref="Pet"/>'s current <see cref="Evolutions"/> level.</param>
        public abstract void SetupGame(List<GlyphData> glyphs, Evolutions petLevel);

        /// <summary>
        /// Sets up the next round internally after the <see cref="Pet"/> has messaged its next <see cref="NeedData"/>.
        /// </summary>
        /// <param name="glyph"></param>
        /// <param name="correctIcon"></param>
        /// <param name="wrongIcon"></param>
        /// <param name="allGlyphs"></param>
        protected abstract void SetupRound(GlyphData glyph, Sprite correctIcon, Sprite wrongIcon, List<GlyphData> allGlyphs);
        protected virtual void SetupRound(Sprite correctIcon, List<GlyphData> allGlyphs) { }

        #endregion
    }


    #region Enums

    /// <summary>
    /// The match type this  <see cref="Minigame"/> belongs to.
    /// </summary>
    public enum GameType
    {

        /// <summary>
        /// Same as null.
        /// </summary>
        None,

        /// <summary>
        /// Match the Egyptian symbol of the <see cref="GlyphData"/> shown by the <see cref="Pet"/>.
        /// </summary>
        Symbols,

        /// <summary>
        /// Match the transliteration letter of the <see cref="GlyphData"/> shown by the <see cref="Pet"/>.
        /// </summary>
        Letters,

        /// <summary>
        /// Match the changing icon of the <see cref="GlyphData"/> shown by the <see cref="Pet"/>.
        /// </summary>
        Alternate,

        /// <summary>
        /// Match multiple parts of the <see cref="GlyphData"/> shown by the <see cref="Pet"/>.
        /// </summary>
        Multiple,

        /// <summary>
        /// Match a random part of the <see cref="GlyphData"/> shown by the <see cref="Pet"/>.
        /// </summary>
        Random
    }

    #endregion
}