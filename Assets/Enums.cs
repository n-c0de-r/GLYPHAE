namespace GlyphaeScripts
{
    /// <summary>
    /// The <see cref="Evolution"/> levels a <see cref="Pet"/> goes through.
    /// </summary>
    public enum Evolution
    {
        None, Egg, Baby, Kid, Teen, Adult, God
    }

    /// <summary>
    /// The strength marker of memorization,
    /// based on Leitner flashcard system.
    /// </summary>
    public enum MemoryLevel
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

    /// <summary>
    /// Marks the different needs a <see cref="Pet"/> has.
    /// </summary>
    public enum Need
    {
        None, Hunger, Health, Joy, Energy
    }
}