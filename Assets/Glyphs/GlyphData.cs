using System;
using UnityEngine;

namespace GlyphaeScripts
{
    /// <summary>
    /// A scriptable object container holding all relevant data of a specific Hieroglyph.
    /// </summary>
    [CreateAssetMenu(fileName = "Glyph", menuName = "ScriptableObjects/Glyph")]
    public class GlyphData : ScriptableObject
    {
        #region Serialized Fields

        [Tooltip("The heiroglyph symbol of this Glyph.")]
        [SerializeField] private Sprite symbol;

        [Tooltip("The common transiteration character of this Glyph.\r\nBased on 'Werning 2013 (‘Advanced’)'.")]
        [SerializeField] private Sprite letter;

        [Tooltip("The commonly approximated\r\nverbal pronounciation of this Glyph.")]
        [SerializeField] private AudioClip sound;

        [Tooltip("The strength of memorization,\r\nbased on Leitner flashcard system.")]
        [SerializeField] private MemoryLevels memoryLevel;

        [Tooltip("The number of times this was guessed correctly.")]
        [SerializeField] [Range(0, 5)] private int correctGuesses = 0;

        [Tooltip("The number of times this was guessed wrongly.")]
        [SerializeField] [Range(0, 5)] private int wrongGuesses = 0;

        #endregion


        #region GetSets / Properties

        /// <summary>
        /// The heiroglyph symbol of this <see cref="GlyphData"/>.
        /// </summary>
        public Sprite Symbol { get => symbol; }

        /// <summary>
        /// The common transiteration letter of this <see cref="GlyphData"/>.
        /// Based on 'Werning 2013 (‘Advanced’)' in <see href="https://en.wiktionary.org/wiki/Appendix:Egyptian_transliteration_schemes"/>
        /// </summary>
        public Sprite Letter { get => letter; }

        /// <summary>
        /// The commonly approximated verbal
        /// pronounciation of this <see cref="GlyphData"/>.
        /// </summary>
        public AudioClip Sound { get => sound; }

        /// <summary>
        /// The strength of memorization,
        /// based on Leitner flashcard system.
        /// </summary>
        public MemoryLevels MemoryLevel { get => memoryLevel; }

        /// <summary>
        /// The number of times this was guessed correctly.
        /// </summary>
        public int CorrectGuesses { get => correctGuesses; }

        /// <summary>
        /// The number of times this was guessed wrongly.
        /// </summary>
        public int WrongGuesses { get => wrongGuesses; }

        #endregion


        #region Methods

        /// <summary>
        /// Increases the memory level if the <see cref="GlyphData"/> is guessed often enough correctly.
        /// </summary>
        public void CorrectlyGuessed()
        {
            if (++correctGuesses > Enum.GetValues(typeof(MemoryLevels)).Length)
            {
                if (!Enum.IsDefined(typeof(MemoryLevels), ++memoryLevel))
                {
                    memoryLevel--;
                    correctGuesses--; // reset
                    return;
                }

                correctGuesses = 0;
                wrongGuesses = 0;
            }
        }

        /// <summary>
        /// Decreases the memory level if the <see cref="GlyphData"/> is guessed often enough wrongly.
        /// </summary>
        /// <param name="sprite">The sprite to display as feedback.</param>
        public void WronglyGuessed()
        {
            if (++wrongGuesses > Enum.GetValues(typeof(MemoryLevels)).Length)
            {
                if (!Enum.IsDefined(typeof(MemoryLevels), --memoryLevel))
                {
                    memoryLevel++;
                    wrongGuesses--; // reset
                    return;
                }

                correctGuesses = 0;
                wrongGuesses = 0;
            }
        }

        /// <summary>
        /// Sets the glyph data after reload.
        /// </summary>
        /// <param name="level">The glyphs <see cref="MemoryLevels"/></param>
        /// <param name="correct">The number of correct guesses for this glyph.</param>
        /// <param name="wrong">The number of wrong guesses for this glyph.</param>
        public void SetupData(MemoryLevels level, int correct, int wrong)
        {
            memoryLevel = level;
            correctGuesses = correct;
            wrongGuesses = wrong;
        }

        /// <summary>
        /// Resets back all values of this glyph to the start.
        /// </summary>
        public void ResetLevel()
        {
            memoryLevel = MemoryLevels.New;
            correctGuesses = 0;
            wrongGuesses = 0;
        }

        /// <summary>
        /// Increases the <see cref="MemoryLevels"/> of this glyph by 1.
        /// </summary>
        public void LevelUp()
        {
            memoryLevel++;
        }

        #endregion
    }

    #region Enums

    /// <summary>
    /// The strength marker of memorization,
    /// based on Leitner flashcard system.
    /// </summary>
    public enum MemoryLevels
    {
        /// <summary>
        /// Not encountered yet.
        /// </summary>
        New,

        /// <summary>
        /// Seen at least once
        /// </summary>
        Seen,

        /// <summary>
        /// Seen but not yet well remembered.
        /// </summary>
        Unknown,

        /// <summary>
        /// Seen and remembered.
        /// </summary>
        Known,

        /// <summary>
        /// Fully memorized.
        /// </summary>
        Memorized
    }

    #endregion
}