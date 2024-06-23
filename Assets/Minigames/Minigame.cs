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
        [Tooltip("The Evolution level\r\nthis game is played at.")]
        [SerializeField] protected Evolution level;

        [Tooltip("The Inputs to set up at start.")]
        [SerializeField] protected GameInput[] gameInputs;

        [Tooltip("Minimum number of rounds to play this game.")]
        [SerializeField][Range(1, 3)] protected int minimumRounds = 1;

        [Space]
        [Header("Animation Values")]
        [Tooltip("Display feedback emoji.")]
        [SerializeField] protected NeedBubble reactionBubble;

        [Tooltip("The display of the positive feedback.")]
        [SerializeField] protected Sprite positiveFeedback;

        [Tooltip("The display of the negative feedback.")]
        [SerializeField] protected Sprite negativeFeedback;

        [Space]
        [Header("Need Values")]
        [Tooltip("The type of need this game fills the current need.")]
        [SerializeField] protected Need needType;

        [Tooltip("The strength of need filling by the game.")]
        [SerializeField][Range(10, 50)] protected int needAmount;

        [Tooltip("Minimum number of rounds to play this game.")]
        [SerializeField][Range(10, 50)] public int energyCost = 10;

        #endregion Serialized Fields


        #region Fields

        protected int _successes, _fails, _failsToLose;

        #endregion Fields


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


        #region GetSets / Properties

        /// <summary>
        /// The Evolution level this game is played at.
        /// </summary>
        public Evolution Level
        {
            get => level;
        }

        /// <summary>
        /// The text to display at game start.
        /// </summary>
        public string InstructionText
        {
            get => instructionText;
        }

        /// <summary>
        /// Minimum number of rounds to play this game.
        /// </summary>
        public int MinimumRounds
        {
            get => minimumRounds;
        }

        #endregion


        #region Methods

        private void ShowInstructions()
        {
            if (helpText.text != instructionText) helpText.text = instructionText;
        }

        public abstract void SetupGame(List<Glyph> glyphs, Evolution rounds);

        protected abstract void SetupRound();

        protected abstract void InputCheck(string message);

        /// <summary>
        /// Trigger this when you achieved a success.
        /// It counts and manages everything else.
        /// </summary>
        protected void Success()
        {
            _successes++;
            AnimateSuccess();
            
            if (_successes >= minimumRounds) Win();
        }

        /// <summary>
        /// Use this when you made a mistake.
        /// It counts and manages everything else.
        /// </summary>
        protected void Fail()
        {
            _fails++;
            AnimateFail();

            if (_fails > _failsToLose) Lose();
        }

        /// <summary>
        /// Informs the BaseGame Controller, that the game
        /// triggered a win condition and stops the game.
        /// </summary>
        protected void Win()
        {
            SendMessageUpwards("CloseMinigame");
            Settings.NeedUpdate(needType, needAmount);
        }

        /// <summary>
        /// Informs the BaseGame Controller, that the game
        /// triggered a lose condition and stops the game.
        /// </summary>
        protected void Lose()
        {
            SendMessageUpwards("CloseMinigame");
        }

        /// <summary>
        /// Runs the Win animation.
        /// </summary>
        protected void AnimateSuccess()
        {
            reactionBubble.Setup(positiveFeedback);
            reactionBubble.Show(nameof(SetupRound));
        }

        /// <summary>
        /// Runs the Lose animation.
        /// </summary>
        protected void AnimateFail()
        {
            reactionBubble.Setup(negativeFeedback);
            reactionBubble.Show(nameof(SetupRound));
        }

        #endregion  Methods
    }
}