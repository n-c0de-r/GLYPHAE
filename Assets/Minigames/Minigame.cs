using System;
using System.Collections.Generic;
using UnityEngine;

namespace GlyphaeScripts
{
    /// <summary>
    /// Represents an abstract idea of a game.
    /// Encapsulates the basic values and functions
    /// each <see cref="Minigame"/> should have.
    /// </summary>
    public abstract class Minigame : MonoBehaviour
    {
        #region Serialized Fields

        [Tooltip("The current Settings for display values.")]
        [SerializeField] protected Settings settings;

        [Header("Base Values")]
        [Tooltip("The match type \r\nthis game belongs to.")]
        [SerializeField] protected GameType type;

        [Tooltip("The Inputs to set up at start.")]
        [SerializeField] protected List<GameButton> gameInputs;

        [Tooltip("Minimum number of rounds to play this game.")]
        [SerializeField][Range(1, 3)] protected int minimumRounds = 1;

        [Space]
        [Header("Need Values")]
        [Tooltip("The type of need this game fills.")]
        [SerializeField] protected NeedData need;

        [Tooltip("The strength of need filling by the game.")]
        [SerializeField][Range(10, 50)] protected int needFill;

        [Space]
        [Header("Help Data")]
        [Tooltip("Container to display the following data.")]
        [SerializeField] protected GameHelp helpContainer;

        [Tooltip("A short description of this Minigame.")]
        [SerializeField][TextArea(3, 10)] protected string gameDescription;

        [Tooltip("Instructions how to play the game.")]
        [SerializeField][TextArea(3, 10)] protected string gameInstructions;

        #endregion Serialized Fields


        #region Fields

        protected GlyphData _toMatch;
        protected List<GlyphData> _allGlyphs, _usedGlyphs;
        protected int _level, _buttonCount;
        protected int _successes, _fails, _failsToLose;

        #endregion Fields


        #region Events

        public static event Action<GameObject> OnGameClose;
        public static event Action<Sprite> OnNextRound, OnCorrectGuess, OnWrongGuess;

        #endregion


        #region Unity Built-Ins

        void Awake()
        {
            
        }

        private void OnEnable()
        {
            GameButton.OnInput += CheckInput;

            NeedBubble.OnFeedbackDone += NextRound;

            helpContainer.Setup(this.GetType().Name, gameDescription, gameInstructions);
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

        private void OnDisable()
        {
            GameButton.OnInput -= CheckInput;

            NeedBubble.OnFeedbackDone -= NextRound;
        }

        private void OnDestroy()
        {
            
        }

        #endregion


        #region GetSets / Properties

        #endregion


        #region Methods

        /// <summary>
        /// Sets up initial values for the game through the <see cref="GameManager"/>.
        /// </summary>
        /// <param name="glyphs">The current list of glyphs the <see cref="Pet"/> holds.</param>
        /// <param name="level"><The <see cref="Pet"/>'s current <see cref="Evolutions"/> level.</param>
        public virtual void SetupGame(List<GlyphData> glyphs, int level)
        {
            _allGlyphs = new(glyphs);
            _level = level;
            _buttonCount = ++_level << 1;

            NextRound();
        }

        /// <summary>
        /// Informs the BaseGame Controller, that the game
        /// has ended and can be closed and destroyed.
        /// </summary>
        public virtual void CloseGame()
        {
            OnGameClose?.Invoke(gameObject);
        }

        #endregion


        #region Helpers

        /// <summary>
        /// Sets up the next round internally after the <see cref="Pet"/> has messaged its next <see cref="NeedData"/>.
        /// </summary>
        /// <param name="glyph"></param>
        /// <param name="correctIcon"></param>
        /// <param name="wrongIcon"></param>
        /// <param name="allGlyphs"></param>
        //protected abstract void SetupRound(GlyphData glyph, Sprite correctIcon, Sprite wrongIcon, List<GlyphData> allGlyphs);

        //protected abstract void SetupRound(Sprite correctIcon, List<GlyphData> allGlyphs);

        protected abstract void NextRound();

        protected virtual void DisplayRound(Sprite correct)
        {
            OnNextRound?.Invoke(correct);
        }

        protected virtual void CheckInput(GlyphData input)
        {
            if (_toMatch == input)
            {
                _toMatch.CorrectlyGuessed();
                OnCorrectGuess?.Invoke(need.Positive);
                Success();
            }
            else
            {
                _toMatch.WronglyGuessed();
                OnWrongGuess?.Invoke(need.Negative);
                Fail();
            }
            _allGlyphs.AddRange(_usedGlyphs);
        }

        /// <summary>
        /// Trigger this when you achieved a success.
        /// It counts and manages everything else.
        /// </summary>
        protected virtual void Success()
        {
            if (++_successes >= minimumRounds) Win();
        }

        /// <summary>
        /// Use this when you made a mistake.
        /// It counts and manages everything else.
        /// </summary>
        protected virtual void Fail()
        {
            if (++_fails >= _failsToLose) CloseGame();
        }

        /// <summary>
        /// Informs the BaseGame Controller, that the game
        /// triggered a win condition and stops the game.
        /// </summary>
        protected virtual void Win()
        {
            need.SetData(needFill);
            CloseGame();
        }

        #endregion
    }


    #region Enums

    /// <summary>
    /// The match type this  <see cref="Minigame"/> belongs to.
    /// </summary>
    public enum GameType
    {
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