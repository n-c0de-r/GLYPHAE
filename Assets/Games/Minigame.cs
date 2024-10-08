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

        [Tooltip("Object refereces where buttons will spawn.")]
        [SerializeField] protected Transform inputContainer;

        [Tooltip("Object refereces where drop checks are done.")]
        [SerializeField] protected Transform dropPoint;

        [Tooltip("Minimum number of rounds to play this game.")]
        [SerializeField][Range(1, 3)] protected int baseRounds = 1;

        [Tooltip("The base costs of Energy to play a game.")]
        [SerializeField][Range(0, 10)] protected int energyCost;

        [Space]
        [Header("Need Values")]
        [Tooltip("The type of need this game fills.")]
        [SerializeField] protected NeedData primaryNeed;

        [Tooltip("The type of need this game depletes.")]
        [SerializeField] protected NeedData secondaryNeed;

        [Tooltip("The strength of need filling by the game.")]
        [SerializeField][Range(0, 10)] protected int fillAmount;

        [Tooltip("The amount the secondary need is depleted\r\non win or loss either way.")]
        [SerializeField][Range(0, 10)] protected int lossAmount;

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
        protected List<GlyphData> _newGlyphs, _usedGlyphs, _seenGlyphs, _unknownGlyphs, _knownGlyphs, _memorizedGlyphs;
        protected HashSet<GlyphData> _correctGuesses;
        protected float _primaryValue = 0, _secondValue;
        protected int _successes, _fails, _failsToLose;
        protected int _level, _rounds, _buttonCount;
        protected bool _isTeaching = false;

        #endregion Fields


        #region Events

        public static event Action<NeedData> OnGameWin;
        public static event Action<Minigame> OnGameClose;
        public static event Action<AudioClip, Sprite> OnNextRound;
        public static event Action<Sprite> OnCorrectGuess, OnWrongGuess;

        #endregion


        #region GetSets / Properties

        /// <summary>
        /// The base costs of Energy to play a game.
        /// </summary>
        public int EnergyCost { get => energyCost; }

        /// <summary>
        /// The type of need this game fills.
        /// </summary>
        public NeedData PrimaryNeed { get => primaryNeed; }

        /// <summary>
        /// The type of need this game depletes.
        /// </summary>
        public NeedData SecondaryNeed { get => secondaryNeed; }

        /// <summary>
        /// The amount the secondary need is depleted on win or loss either way.
        /// </summary>
        public int LossAmount { get => lossAmount; }

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
            _correctGuesses = new();
            _isTeaching = isTeaching;
            SetupGylphLists(new(glyphs));
            _level = baseLevel;
            _rounds = baseRounds + baseLevel;
            _failsToLose = _rounds;
            _buttonCount = (baseLevel+1) << 1;

            SetupButtons(_buttonCount);
        }

        /// <summary>
        /// Informs the BaseGame Controller, that the game
        /// has ended and can be closed and destroyed.
        /// Also reduces the secondary need by 1/3rd.
        /// </summary>
        public virtual void CloseGame()
        {
            SetupGylphLists(_usedGlyphs);
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

        /// <summary>
        /// Displays the next <see cref="NeedData"/> sprite.
        /// </summary>
        /// <param name="clip">The sound to play on match.</param>
        /// <param name="correct">The correct sprite to match.</param>
        protected virtual void DisplayRound(AudioClip clip, Sprite correct)
        {
            OnNextRound?.Invoke(clip, correct);
            ActivateButtons(true);
        }

        /// <summary>
        /// Displays the next <see cref="NeedData"/> sprite.
        /// </summary>
        /// <param name="correct">The correct sprite to match.</param>
        //protected virtual void DisplayRound(Sprite correct)
        //{
        //    OnNextRound?.Invoke(correct);
        //    ActivateButtons(true);
        //}

        /// <summary>
        /// General input check method, can be overridden.
        /// </summary>
        /// <param name="input">The incoming input data to match.</param>
        protected virtual void CheckInput(GlyphData input)
        {
            if (_toMatch == null) return;

            ActivateButtons(false);

            if (_toMatch == input)
            {
                _correctGuesses.Add(_toMatch);
                if (_toLearn != null)
                {
                    _toLearn.LevelUp();
                    _correctGuesses.Remove(_toLearn);
                }
                _toLearn = null;
                _isTeaching = false;
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
            switch (settings.Difficulty)
            {
                case Difficulty.Easy:
                    _failsToLose = int.MaxValue;
                    break;
                case Difficulty.Hard:
                    _failsToLose /= 2;
                    break;
            }
            if (++_fails >= _failsToLose) CloseGame();
        }

        /// <summary>
        /// Informs the BaseGame Controller, that the game
        /// triggered a win condition and stops the game.
        /// </summary>
        protected virtual void Win()
        {
            foreach (GlyphData item in _correctGuesses)
                item.CorrectlyGuessed();
            OnGameWin?.Invoke(primaryNeed);
            _primaryValue = fillAmount;
            CloseGame();
        }

        public void MessageSuccess(Sprite sprite) => OnCorrectGuess?.Invoke(sprite);
        public void MessageFail(Sprite sprite) => OnWrongGuess?.Invoke(sprite);

        /// <summary>
        /// Instantiate the buttons needed to play the game.
        /// </summary>
        protected virtual void SetupButtons(int count)
        {
            _gameInputs = new();

            for (int i = 0; i < count; i++)
            {
                Vector3 pos = inputPositions.GetChild(i % inputPositions.childCount).position;
                GameButton button = Instantiate(gameInput, inputContainer);
                button.GetComponent<RectTransform>().position = pos;
                _gameInputs.Add(button);
            }
        }

        /// <summary>
        /// Set up the dragging buttons' target.
        /// </summary>
        protected void SetupDragging()
        {
            SetupDragging(dropPoint);
        }

        /// <summary>
        /// Overload method.
        /// Set up the dragging buttons' target.
        /// </summary>
        protected void SetupDragging(Transform target)
        {
            for (int i = 0; i < _gameInputs.Count; i++)
            {
                GameDrag drag = (GameDrag)_gameInputs[i];
                drag.Target = target;
            }
        }

        /// <summary>
        /// Activates all relevant buttons of a game.
        /// </summary>
        /// <param name="state">The state to switch to: true/on, false/off.</param>
        protected void ActivateButtons(bool state)
        {
            for (int i = 0; i < _gameInputs.Count; i++)
                _gameInputs[i].Switch = state;
        }

        /// <summary>
        /// Prepares lists divided by <see cref="MemoryLevels"/> so games can access different values for learning.
        /// </summary>
        /// <param name="glyphs">The current list of glyphs the <see cref="Pet"/> holds.</param>
        protected void SetupGylphLists(List<GlyphData> glyphs)
        {
            if (glyphs == null || glyphs.Count == 0) return;

            // Same as: if (_allOtherGlyphs == null) _allOtherGlyphs = new();
            _newGlyphs ??= new();
            _seenGlyphs ??= new();
            _unknownGlyphs ??= new();
            _knownGlyphs ??= new();
            _memorizedGlyphs ??= new();

            foreach (GlyphData glyph in glyphs)
            {
                switch (glyph.MemoryLevel)
                {
                    case MemoryLevels.New:
                        _newGlyphs.Add(glyph);
                        break;
                    case MemoryLevels.Seen:
                        _seenGlyphs.Add(glyph);
                        break;
                    case MemoryLevels.Unknown:
                        _unknownGlyphs.Add(glyph);
                        break;
                    case MemoryLevels.Known:
                        _knownGlyphs.Add(glyph);
                        break;
                    case MemoryLevels.Memorized:
                        _memorizedGlyphs.Add(glyph);
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
                else if (_seenGlyphs.Count > 0)
                {
                    // Normally pick known ones
                    GlyphData temp = _seenGlyphs[UnityEngine.Random.Range(0, _seenGlyphs.Count)];
                    _seenGlyphs.Remove(temp);
                    _usedGlyphs.Add(temp);
                }
                else if (_unknownGlyphs.Count > 0)
                {
                    GlyphData temp = _unknownGlyphs[UnityEngine.Random.Range(0, _unknownGlyphs.Count)];
                    _unknownGlyphs.Remove(temp);
                    _usedGlyphs.Add(temp);
                }
                else if (_knownGlyphs.Count > 0)
                {
                    GlyphData temp = _knownGlyphs[UnityEngine.Random.Range(0, _knownGlyphs.Count)];
                    _knownGlyphs.Remove(temp);
                    _usedGlyphs.Add(temp);
                }
                else if (_memorizedGlyphs.Count > 0)
                {
                    GlyphData temp = _memorizedGlyphs[UnityEngine.Random.Range(0, _memorizedGlyphs.Count)];
                    _memorizedGlyphs.Remove(temp);
                    _usedGlyphs.Add(temp);
                }
            }
            return _usedGlyphs;
        }

        #endregion
    }
}