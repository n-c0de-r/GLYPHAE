using System;
using System.Collections;
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
        [Tooltip("The Evolution level\r\nthis game is played at.")]
        [SerializeField] private Evolution level;

        [Tooltip("A short description of the game\r\nthat can be shown in the help menu.")]
        [SerializeField][TextArea(2, 10)] private string description;
        
        [Tooltip("A short instruction how the game\r\nis played that can be shown in the help menu.")]
        [SerializeField][TextArea(3, 10)] private string instructionText;

        [Space]
        [Tooltip("Rounds needed to win\r\nto pass the game.")]
        [SerializeField][Range(0,5)] protected int successesToWin = 2;

        [Tooltip("Rounds to play this game.")]
        [SerializeField][Range(0,5)] protected int roundsToPlay = 4;

        [Tooltip("Rounds to play this game.")]
        [SerializeField][Range(0,5)] protected int failsToLose = 2;

        #endregion Serialized Fields

        #region Fields

        public static event Action<GameObject> OnWin, OnLose;
        //public static event Action<(string side, int score, float timer, int toWin, int toLose)> OnSetVariables;
        //public static event Action<Transform, AnimType, int, float, float> OnPlayAnimations;

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

        #endregion

        #region Methods

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