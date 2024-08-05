using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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
        [Tooltip("The Inputs to set up at start.")]
        [SerializeField] protected List<GameButton> gameInputs;

        [Tooltip("Minimum number of rounds to play this game.")]
        [SerializeField][Range(1, 3)] protected int baseRounds = 1;

        [Tooltip("The base costs of Energy to play a game.")]
        [SerializeField][Range(0, 10)] private int energyCost;

        [Space]
        [Header("Need Values")]
        [Tooltip("The type of need this game fills.")]
        [SerializeField] protected NeedData primaryNeed;

        [Tooltip("The type of need this game depletes.")]
        [SerializeField] protected NeedData secondaryNeed;

        [Tooltip("The strength of need filling by the game.")]
        [SerializeField][Range(0, 10)] protected int fillAmount;

        [Tooltip("Secondary need is depleted on win or loss either way.")]
        [SerializeField][Range(0, 5)] protected int lossAmount;

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
        protected List<GlyphData> _newGlyphs, _allOtherGlyphs, _usedGlyphs;
        protected float _primaryValue = 0, _secondValue;
        protected int _successes, _fails, _failsToLose;
        protected int _level, _rounds, _buttonCount;
        protected bool _isTeaching = false, _hasLearned = false;


        #endregion Fields


        #region Events

        public static event Action<NeedData> OnGameWin;
        public static event Action<Minigame> OnGameClose;
        public static event Action<Sprite> OnNextRound, OnCorrectGuess, OnWrongGuess;

        #endregion


        #region GetSets / Properties

        /// <summary>
        /// The base costs of Energy to play a game.
        /// </summary>
        public int EnergyCost { get => energyCost; }
        public NeedData PrimaryNeed { get => primaryNeed; }

        #endregion


        #region Unity Built-Ins

        protected void Awake()
        {
            
        }

        protected void OnEnable()
        {
            GameButton.OnInput += CheckInput;

            NeedBubble.OnFeedbackDone += NextRound;

            //source https://stackoverflow.com/a/4489031
            string normalizedName = string.Join(" ", Regex.Split(this.GetType().Name, @"(?<!^)(?=[A-Z])"));
            helpContainer.Setup(normalizedName, gameDescription, gameInstructions);
        }

        protected void Start()
        {

        }

        protected void FixedUpdate()
        {

        }

        protected void Update()
        {

        }

        protected void OnDisable()
        {
            GameButton.OnInput -= CheckInput;

            NeedBubble.OnFeedbackDone -= NextRound;
        }

        protected void OnDestroy()
        {
            
        }

        #endregion


        #region Methods

        /// <summary>
        /// Sets up initial values for the game through the <see cref="GameManager"/>.
        /// </summary>
        /// <param name="glyphs">The current list of glyphs the <see cref="Pet"/> holds.</param>
        /// <param name="baseLevel"><The <see cref="Minigame"/> base level, based on the <see cref="Pet"/>'s current <see cref="Evolutions"/> level.</param>
        public virtual void SetupGame(bool isTeaching, List<GlyphData> glyphs, int baseLevel)
        {
            _isTeaching = isTeaching;
            SetupGylphLists(new(glyphs));
            _level = baseLevel;
            _rounds = baseRounds + baseLevel;
            _failsToLose = _rounds;
            _buttonCount = (baseLevel+1) << 1;
        }

        /// <summary>
        /// Informs the BaseGame Controller, that the game
        /// has ended and can be closed and destroyed.
        /// Also reduces the secondary need by 1/3rd.
        /// </summary>
        public virtual void CloseGame()
        {
            _secondValue = lossAmount;
            OnGameClose?.Invoke(this);
        }

        public virtual void UpdateValues()
        {            
            primaryNeed?.Increase(_primaryValue);
            secondaryNeed?.Decrease(_secondValue);
        }

        /// <summary>
        /// Sets up the next round after the <see cref="Pet"/> has messaged its next <see cref="NeedData"/>.
        /// </summary>
        public abstract void NextRound();

        #endregion


        #region Helpers

        protected virtual void DisplayRound(Sprite correct)
        {
            OnNextRound?.Invoke(correct);
            ActivateButtons(true);
        }

        protected virtual void CheckInput(GlyphData input)
        {
            ActivateButtons(false);

            if (_toMatch == input)
            {
                _toMatch.CorrectlyGuessed();
                Success();
            }
            else
            {
                _toMatch.WronglyGuessed();
                Fail();
            }
            SetupGylphLists(_usedGlyphs);
        }

        /// <summary>
        /// Trigger this when you achieved a success.
        /// It counts and manages everything else.
        /// </summary>
        protected virtual void Success()
        {
            OnCorrectGuess?.Invoke(primaryNeed.Positive);
            if (++_successes >= _rounds) Win();
        }

        /// <summary>
        /// Use this when you made a mistake.
        /// It counts and manages everything else.
        /// </summary>
        protected virtual void Fail()
        {
            OnWrongGuess?.Invoke(primaryNeed.Negative);
            if (++_fails >= _failsToLose) CloseGame();
        }

        /// <summary>
        /// Informs the BaseGame Controller, that the game
        /// triggered a win condition and stops the game.
        /// </summary>
        protected virtual void Win()
        {
            OnGameWin?.Invoke(primaryNeed);
            _primaryValue = fillAmount;
            CloseGame();
        }

        public void MessageSuccess() => OnCorrectGuess?.Invoke(primaryNeed.Positive);
        public void MessageFail() => OnWrongGuess?.Invoke(primaryNeed.Negative);

        /// <summary>
        /// Activates all relevant buttons of a game.
        /// </summary>
        /// <param name="state">The state to switch to: true/on, false/off.</param>
        protected void ActivateButtons(bool state)
        {
            for (int i = 0; i < _buttonCount; i++)
            {
                gameInputs[i].Switch = state;
            }
        }

        /// <summary>
        /// Prepares lists divided by <see cref="MemoryLevels"/> so games can access different values for learning.
        /// </summary>
        /// <param name="glyphs">The current list of glyphs the <see cref="Pet"/> holds.</param>
        private void SetupGylphLists(List<GlyphData> glyphs)
        {
            if (_allOtherGlyphs == null) _allOtherGlyphs = new();
            if (_newGlyphs == null) _newGlyphs = new();

            foreach (GlyphData glyph in glyphs)
            {
                switch (glyph.MemoryLevel)
                {
                    case MemoryLevels.New:
                        _newGlyphs.Add(glyph);
                        break;
                    default:
                        _allOtherGlyphs.Add(glyph);
                        break;
                }
            }
        }

        #endregion
    }
}