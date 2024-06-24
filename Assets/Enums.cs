using System;
using UnityEngine;

namespace GlyphaeScripts
{
    #region Enums 

    /// <summary>
    /// The <see cref="Evolution"/> levels a <see cref="Pet"/> goes through.
    /// </summary>
    public enum Evolution
    {
        /// <summary>
        /// Same as null.
        /// </summary>
        None,

        /// <summary>
        /// Initial starting form. Has no interactions.
        /// </summary>
        Egg,

        /// <summary>
        /// 
        /// </summary>
        Baby,

        /// <summary>
        /// 
        /// </summary>
        Kid,

        /// <summary>
        /// 
        /// </summary>
        Teen,

        /// <summary>
        /// 
        /// </summary>
        Adult,

        /// <summary>
        /// Final form. Can play any game.
        /// </summary>
        God
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

        /// <summary>
        /// Same as null.
        /// </summary>
        None,

        /// <summary>
        /// Need is stilled when feeding the <see cref="Pet"/>.
        /// </summary>
        Hunger,

        /// <summary>
        /// Need is stilled when washing the <see cref="Pet"/>.
        /// </summary>
        Health,

        /// <summary>
        /// Need is stilled when playing with the <see cref="Pet"/>.
        /// </summary>
        Joy,

        /// <summary>
        /// Need is stilled when putting the <see cref="Pet"/> to sleep.
        /// </summary>
        Energy
    }

    #endregion
}