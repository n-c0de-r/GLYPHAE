using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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

        [Header("Description Values")]
        [Tooltip("The Text Mesh object dscription\r\nand instruction will be shown.")]
        [SerializeField] protected TMP_Text helpText;

        [Tooltip("A short instruction how the game\r\nis played that can be shown in the help menu.")]
        [SerializeField][TextArea(3, 10)] protected string instructionText;

        [Space]
        [Header("Base Values")]
        [Tooltip("The match type \r\nthis game belongs to.")]
        [SerializeField] protected Type type;

        [Tooltip("The Evolution level\r\nthis game is played at.")]
        [SerializeField] protected Evolutions level;

        [Tooltip("The Inputs to set up at start.")]
        [SerializeField] protected GameButton[] gameInputs;

        [Tooltip("Minimum number of rounds to play this game.")]
        [SerializeField][Range(1, 3)] protected int minimumRounds = 1;

        [Space]
        [Header("Need Values")]
        [Tooltip("The type of need this game fills the current need.")]
        [SerializeField] protected Needs needType;

        [Tooltip("The strength of need filling by the game.")]
        [SerializeField][Range(10, 50)] protected int needAmount;

        [Tooltip("The costs of Energy to play this game.")]
        [SerializeField][Range(0, 50)] protected int energyCost;

        #endregion Serialized Fields


        #region Fields

        protected int _successes, _fails, _failsToLose;

        #endregion Fields


        #region Unity Built-Ins

        void Awake()
        {
            OnGameStart?.Invoke(needType, - energyCost);

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

        #endregion


        #region Events

        public static event Action<Needs, float> OnGameStart, OnGameWin;
        public static event Action<GameObject> OnGameLose;
        public static event Action<int, Type> OnGameInit;

        #endregion


        #region GetSets / Properties

        /// <summary>
        /// The Evolution level this game is played at.
        /// </summary>
        public Evolutions Level { get => level; }

        /// <summary>
        /// The text to display at game start.
        /// </summary>
        public string InstructionText { get => instructionText; }

        /// <summary>
        /// Minimum number of rounds to play this game.
        /// </summary>
        public int MinimumRounds { get => minimumRounds; }

        /// <summary>
        /// The costs of <see cref="Needs.Energy"/> to play this game.
        /// </summary>
        public int EnergyCost { get => energyCost; }

        #endregion


        #region Methods

        public void ShowInstructions()
        {
            if (helpText.text != instructionText) helpText.text = instructionText;
        }

        public void Close()
        {
            OnGameLose?.Invoke(gameObject);
        }

        protected void Init(int rounds)
        {
            OnGameInit?.Invoke(rounds, type);
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
            OnGameWin?.Invoke(needType, needAmount);
            Close();
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

        public abstract void SetupGame(List<Glyph> glyphs, Evolutions petLevel);

        protected abstract void SetupRound(Glyph glyph, Sprite correctIcon, Sprite wrongIcon, Glyph[] allGlyphs);

        protected abstract void InputCheck(string message);

        #endregion  Methods

        #region Enums

        /// <summary>
        /// The match type this  <see cref="Minigame"/> belongs to.
        /// </summary>
        public enum Type
        {

            /// <summary>
            /// Same as null.
            /// </summary>
            None,

            /// <summary>
            /// Match the Egyptian symbol of the <see cref="Glyph"/> shown by the <see cref="Pet"/>.
            /// </summary>
            Symbols,

            /// <summary>
            /// Match the transliteration letter of the <see cref="Glyph"/> shown by the <see cref="Pet"/>.
            /// </summary>
            Letters,

            /// <summary>
            /// Match the changing icon of the <see cref="Glyph"/> shown by the <see cref="Pet"/>.
            /// </summary>
            Alternate,

            /// <summary>
            /// Match multiple parts of the <see cref="Glyph"/> shown by the <see cref="Pet"/>.
            /// </summary>
            Multiple,

            /// <summary>
            /// Match a random part of the <see cref="Glyph"/> shown by the <see cref="Pet"/>.
            /// </summary>
            Random
        }

        #endregion
    }
}