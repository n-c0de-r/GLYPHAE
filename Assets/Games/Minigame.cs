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
        [Tooltip("The Input this game uses.")]
        [SerializeField] protected GameButton gameInput;

        [Tooltip("Object refereces where buttons should be set.")]
        [SerializeField] protected RectTransform inputPositions;

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

        [Tooltip("The Inputs to set up at start.")]
        protected List<GameButton> _gameInputs;
        protected GlyphData _toMatch, _toLearn;
        protected List<GlyphData> _newGlyphs, _allOtherGlyphs, _usedGlyphs;
        protected float _primaryValue = 0, _secondValue;
        protected int _successes, _fails, _failsToLose;
        protected int _level, _rounds, _buttonCount;
        protected bool _isTeaching = false;

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

        protected void OnEnable()
        {
            GameButton.OnInput += CheckInput;

            NeedBubble.OnFeedbackDone += NextRound;

            //source https://stackoverflow.com/a/4489031
            string normalizedName = string.Join(" ", Regex.Split(this.GetType().Name, @"(?<!^)(?=[A-Z])"));
            helpContainer.Setup(normalizedName, gameDescription, gameInstructions);
        }

        protected void OnDisable()
        {
            GameButton.OnInput -= CheckInput;

            NeedBubble.OnFeedbackDone -= NextRound;
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

            SetupButtons();
        }

        /// <summary>
        /// Informs the BaseGame Controller, that the game
        /// has ended and can be closed and destroyed.
        /// Also reduces the secondary need by 1/3rd.
        /// </summary>
        public virtual void CloseGame()
        {
            ActivateButtons(false);
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
        public virtual void NextRound() { }

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
                _toLearn = null;
                _isTeaching = false;
                _toMatch.CorrectlyGuessed();
                Success();
            }
            else
            {
                _toMatch.WronglyGuessed();
                Fail();
            }
            _toMatch = null;
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
        /// Instantiate the buttons needed to play the game.
        /// </summary>
        protected void SetupButtons()
        {
            _gameInputs = new();

            for (int i = 0; i < _buttonCount; i++)
            {
                Vector3 pos = inputPositions.GetChild(i).position;
                GameButton button = Instantiate(gameInput, transform);
                button.GetComponent<RectTransform>().position = pos;
                _gameInputs.Add(button);
            }
        }

        /// <summary>
        /// Set up the dragging buttons' target.
        /// </summary>
        protected void SetupDragging()
        {
            for (int i = 0; i < _gameInputs.Count; i++)
            {
                GameDrag drag = (GameDrag)_gameInputs[i];
                drag.SetTarget(inputPositions.GetChild(inputPositions.childCount - 1));
            }
        }

        /// <summary>
        /// Activates all relevant buttons of a game.
        /// </summary>
        /// <param name="state">The state to switch to: true/on, false/off.</param>
        protected void ActivateButtons(bool state)
        {
            for (int i = 0; i < _buttonCount; i++)
            {
                _gameInputs[i].Switch = state;
            }
        }

        /// <summary>
        /// Prepares lists divided by <see cref="MemoryLevels"/> so games can access different values for learning.
        /// </summary>
        /// <param name="glyphs">The current list of glyphs the <see cref="Pet"/> holds.</param>
        protected void SetupGylphLists(List<GlyphData> glyphs)
        {
            if (glyphs == null || glyphs.Count == 0) return;

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

        /// <summary>
        /// Select the glyphs for the next round.
        /// </summary>
        protected List<GlyphData> SelectGlyphs()
        {
            if (_usedGlyphs != null && _usedGlyphs.Count > 0)
                SetupGylphLists(_usedGlyphs);

            _usedGlyphs = new();

            for (int i = 0; i < _buttonCount; i++)
            {
                if (_isTeaching && _toLearn == null && _newGlyphs.Count > 0)
                {
                    // On criticals prefer new glyphs, to teach
                    _toLearn = _newGlyphs[UnityEngine.Random.Range(0, _newGlyphs.Count)];
                    _newGlyphs.Remove(_toLearn);
                    _usedGlyphs.Add(_toLearn);
                }
                else if (_allOtherGlyphs.Count > 0)
                {
                    // Normally pick known ones
                    GlyphData temp = _allOtherGlyphs[UnityEngine.Random.Range(0, _allOtherGlyphs.Count)];
                    _allOtherGlyphs.Remove(temp);
                    _usedGlyphs.Add(temp);
                }
            }
            return _usedGlyphs;
        }

        #endregion
    }
}