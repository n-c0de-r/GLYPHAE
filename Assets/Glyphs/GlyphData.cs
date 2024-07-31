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

        #endregion


        #region Fields

        [Range(0,5)]public int _guesses = 0;

        #endregion


        #region Events

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
        public MemoryLevels MemoryLevel { get => memoryLevel; set => memoryLevel = value; }

        #endregion


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


        #region Methods

        /// <summary>
        /// Increases the memory level if the <see cref="GlyphData"/> is guessed often enough correctly.
        /// </summary>
        public void CorrectlyGuessed()
        {
            if (++_guesses > (int)memoryLevel)
            {
                if (!Enum.IsDefined(typeof(MemoryLevels), ++memoryLevel))
                {
                    memoryLevel--;
                    _guesses--; // reset
                    return;
                }

                _guesses = 0;
            }
        }

        /// <summary>
        /// Decreases the memory level if the <see cref="GlyphData"/> is guessed often enough wrongly.
        /// </summary>
        public void WronglyGuessed()
        {
            if (--_guesses < 0)
            {
                if (!Enum.IsDefined(typeof(MemoryLevels), --memoryLevel))
                {
                    memoryLevel++;
                    _guesses++; // reset
                    return;
                }

                _guesses = (int)memoryLevel;
            }
        }

        #endregion


        #region Helpers



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