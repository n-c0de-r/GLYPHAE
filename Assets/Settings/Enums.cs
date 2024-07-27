

namespace GlyphaeScripts
{
    /// <summary>
    /// The <see cref="Evolutions"/> levels a <see cref="Pet"/> goes through.
    /// </summary>
    public enum Evolutions
    {
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
    /// The match type this  <see cref="Minigame"/> belongs to.
    /// </summary>
    public enum GameType
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



    /// <summary>
    /// The <see cref="Settings"/>'s keywords.
    /// </summary>
    public enum Keys
    {
        SelectedPet, GlyphList, MainVolume, MusicVolume, SoundVolume, VoiceVolume, AnimationSpeed, FirstRun
    }



    /// <summary>
    /// Marks the different <see cref="NeedData"/> types a <see cref="Pet"/> has.
    /// </summary>
    public enum NeedTypes
    {
        /// <summary>
        /// <see cref="NeedData"/> is stilled when feeding the <see cref="Pet"/>.
        /// </summary>
        Hunger,

        /// <summary>
        /// <see cref="NeedData"/> is stilled when washing the <see cref="Pet"/>.
        /// </summary>
        Health,

        /// <summary>
        /// <see cref="NeedData"/> is stilled when playing with the <see cref="Pet"/>.
        /// </summary>
        Joy,

        /// <summary>
        /// <see cref="NeedData"/> is stilled when putting the <see cref="Pet"/> to sleep.
        /// </summary>
        Energy
    }
}
