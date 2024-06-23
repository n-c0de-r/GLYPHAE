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

        [Header("Base Values")]
        [Tooltip("A short description of the game\r\nthat can be shown in the help menu.")]
        [SerializeField][TextArea(2, 10)] protected string description;

        [Tooltip("A short instruction how the game\r\nis played that can be shown in the help menu.")]
        [SerializeField][TextArea(3, 10)] protected string instructionText;

        [Tooltip("Minimum number of rounds to play this game.")]
        [SerializeField][Range(1, 3)] protected int minimumRounds = 1;

        [Space]
        [Tooltip("The Evolution level\r\nthis game is played at.")]
        [SerializeField] protected Evolution level;

        [Tooltip("The Inputs to set up at start.")]
        [SerializeField] protected GameInput[] gameInputs;

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

        public static event Action<GameObject> OnWin, OnLose;
        //public static event Action<(string side, int score, float timer, int toWin, int toLose)> OnSetVariables;
        //public static event Action<Transform, AnimType, int, float, float> OnPlayAnimations;

        protected int successesToWin, failsToLose;
        protected int _successes, _fails;


        #endregion Fields

        #region GetSets / Properties

        /// <summary>
        /// The Evolution level this game is played at.
        /// </summary>
        public Evolution Level
        {
            get => level;
        }

        /// <summary>
        /// The game's description.
        /// </summary>
        public string Description
        {
            get => description;
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

        public abstract void SetupGame(List<Glyph> glyphs, Evolution rounds);

        protected abstract void InputCheck(string message);

        protected void InitializeButtons()
        {
            //if (correct == null) continue;
            //glyphs[rand] = null;
            //gameInputs[index].Setup(correct.Sound, correct.Symbol, correct.Character);

            //rand = Random.Range(0, glyphs.Count);

            //if (wrong == null) continue;
            //glyphs[rand] = null;
            //gameInputs[index + 1].Setup(wrong.Sound, wrong.Symbol, wrong.Character);
            //index++;
        }

        public void RestartGame()
        {
            StopAllCoroutines();
        }

        /// <summary>
        /// Overload method.
        /// Trigger this when you achieved a success.
        /// It counts and manages everything else.
        /// </summary>
        /// <param name="score">Pass a different score.</param>
        protected void Success(int score)
        {
            _successes++;
            AnimateSuccess(_successes, successesToWin);
            
            if (_successes >= successesToWin)
            {
                _successes = 0;
                _fails = failsToLose;
               
                Win();
            }
        }

        /// <summary>
        /// Overload Method.
        /// Use this when you made a mistake.
        /// It counts and manages everything else.
        /// </summary>
        /// <param name="parent">The place to play the animation.</param>
        /// <param name="score">The score to reduce on fail.</param>
        protected void Fail(int score)
        {
            _fails--;
            AnimateFail(_fails, failsToLose);

            if (_fails <= 0)
            {
                _successes = 0;
                _fails = failsToLose;
                Lose();
            }
        }

        /// <summary>
        /// Informs the BaseGame Controller, that the game triggered a win condition
        /// </summary>
        protected void Win() =>
            OnWin?.Invoke(gameObject);

        /// <summary>
        /// Informs the BaseGame Controller, that the game triggered a lose condition
        /// </summary>
        protected void Lose() =>
            OnLose?.Invoke(gameObject);

        /// <summary>
        /// Overload method.
        /// Runs the Win animation at a given parent position.
        /// </summary>
        /// <param name="parent">The parent object to attatch and play the animation at.</param>
        /// <param name="successes">Current increasing count of successes achieved (or their equivalent in your game).</param>
        /// <param name="successesToWin">The max number of successes to win (or their equivalent in your game).</param>
        protected void AnimateSuccess(int successes, int successesToWin)
        {
            //OnPlayAnimations?.Invoke(parent, AnimType.Win, (int)difficulty, (float)successes, (float)successesToWin);
        }

        /// <summary>
        /// Overload method.
        /// Runs the Lose animation at a given parent position.
        /// </summary>
        /// <param name="parent">The parent object to attatch and play the animation at.</param>
        /// <param name="fails">Current decreasing count of fails left (or their equivalent in your game).</param>
        /// <param name="failsToLose">The max number of fails to lose (or their equivalent in your game).</param>
        protected void AnimateFail(int fails, int failsToLose)
        {
            //OnPlayAnimations?.Invoke(parent, AnimType.Lose, (int)difficulty, (float)(failsToLose - fails), (float)failsToLose);
        }

        /// <summary>
        /// Does the actual animation.
        /// </summary>
        /// <param name="container">The GameObject holding the TMP_Text.</param>
        /// <returns></returns>
        protected IEnumerator AnimateInstruction(GameObject container)
        {
            GameObject obj = Instantiate(container, transform.position, Quaternion.identity, transform);
            obj.SetActive(true);
            TMP_Text tmp = obj.GetComponent<TMP_Text>();

            float countdown = 3.0f;
            tmp.text = countdown.ToString();
            yield return new WaitForSeconds(Time.deltaTime);
            while (0 < countdown)
            {
                countdown -= Time.deltaTime;
                tmp.text = ((int)countdown+1).ToString();
                yield return new WaitForSeconds(Time.deltaTime);
            }
            obj.SetActive(false);
        }

        #endregion  Methods

        #region GetSets

        #endregion GetSets
    }
}