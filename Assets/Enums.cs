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
        None, New, Seen, Unknown, Known, Memorized
    }

    /// <summary>
    /// Marks the different needs a <see cref="Pet"/> has.
    /// </summary>
    public enum Need
    {
        None, Hunger, Health, Joy, Energy
    }
}