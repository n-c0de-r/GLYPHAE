using System;
using UnityEngine;

namespace GlyphaeScripts
{
    [CreateAssetMenu(fileName = "Glyph", menuName = "ScriptableObjects/Glyph")]
    public class Glyph : ScriptableObject
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

        private int _guesses = 0;

        #endregion


        #region GetSets / Properties

        /// <summary>
        /// The heiroglyph symbol of this <see cref="Glyph"/>.
        /// </summary>
        public Sprite Symbol { get => symbol; }

        /// <summary>
        /// The common transiteration letter of this <see cref="Glyph"/>.
        /// Based on 'Werning 2013 (‘Advanced’)' in <see href="https://en.wiktionary.org/wiki/Appendix:Egyptian_transliteration_schemes"/>
        /// </summary>
        public Sprite Letter { get => letter; }

        /// <summary>
        /// The commonly approximated verbal
        /// pronounciation of this <see cref="Glyph"/>.
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


        #region Events

        #endregion


        #region Methods

        /// <summary>
        /// Increases the memory level if the <see cref="Glyph"/> is guessed often enough correctly.
        /// </summary>
        public void CorrectlyGuessed()
        {
            _guesses++;
            int nr = (int)memoryLevel;

            if (_guesses >= nr)
            {
                nr++;
                Array enums = Enum.GetValues(typeof(MemoryLevels));
                if (nr >= enums.Length)
                {
                    _guesses--; // reset
                    return;
                }

                memoryLevel = (MemoryLevels)enums.GetValue(nr % enums.Length);
                _guesses = 0;
            }
        }



        /// <summary>
        /// Decreases the memory level if the <see cref="Glyph"/> is guessed often enough wrongly.
        /// </summary>
        public void WronglyGuessed()
        {
            _guesses--;
            int nr = (int)memoryLevel;

            if (_guesses < 0)
            {
                nr--;
                Array enums = Enum.GetValues(typeof(MemoryLevels));
                if (nr < 1)
                {
                    _guesses++; // reset
                    return;
                }

                memoryLevel = (MemoryLevels)enums.GetValue(nr % enums.Length);
                _guesses = 0;
            }
        }

        #endregion


        #region Helpers



        private void TemplateHelper(bool param)
        {
            
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
        /// Same as null.
        /// </summary>
        None,

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