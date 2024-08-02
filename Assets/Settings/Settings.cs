using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace GlyphaeScripts
{
    [CreateAssetMenu(fileName = "Settings", menuName = "ScriptableObjects/Settings")]
    /// <summary>
    /// Represents all relevant settings and data needed
    /// in the whole game as a scriptable object.
    /// </summary>
    public class Settings : ScriptableObject
    {
        #region Serialized Fields

        [Header("General Objects")]
        [SerializeField] private AudioMixer audioMixer;

        [Tooltip("The egg of any Pet to take care of.")]
        [SerializeField] private Pet egg;

        [Tooltip("The list of Pets available in the whole game.")]
        [SerializeField] private List<Pet> pets;

        [Header("Volume Settings")]
        [SerializeField][Range(-40,0)] private float main = -20.0f;
        [SerializeField][Range(-40,0)] private float music;
        [SerializeField][Range(-40,0)] private float sound;
        [SerializeField][Range(-40,0)] private float voice;

        [Header("Display Values")]
        [Tooltip("The speed of animations.")]
        [SerializeField][Range(1, 5)] private float animationSpeed = 3;

        [Header("Other Values")]
        [Tooltip("If the game has ever run.")]
        [SerializeField] private bool firstRun = true;
        [SerializeField] private Difficulty difficulty = Difficulty.Easy;
        [SerializeField] private Language language = Language.English;

        [Header("Debugging Values")]
        [SerializeField] private Pet _selectedPet;
        [SerializeField] private int _silenceStart = 20;
        [SerializeField] private int _silenceEnd = 8;
        [SerializeField] private int debugCount = 3;
        [SerializeField] private bool _isDebugMode = false;

        #endregion


        #region Fields


        private const char GLYPH_SPLIT = ';';
        private const char MEMORY_SPLIT = ':';


        #endregion


        #region Events

        //public static event Action<Needs, float> OnNeedUpdate;

        #endregion


        #region GetSets / Properties

        /// <summary>
        /// The egg of any <see cref="Pet"/> to take care of.
        /// </summary>
        public Pet Egg
        {
            get => egg;
        }

        /// <summary>
        /// The <see cref="Pet"/>s available in the game.
        /// </summary>
        public List<Pet> Pets
        {
            get => pets;
        }

        /// <summary>
        /// The main volume value set by the player.
        /// </summary>
        public float MainVolume
        {
            get => main;
            set
            {
                main = value;
                audioMixer.SetFloat(nameof(Keys.MainVolume), value);
            }
        }

        /// <summary>
        /// The music volume value set by the player.
        /// </summary>
        public float MusicVolume
        {
            get => music;
            set
            {
                music = value;
                audioMixer.SetFloat(nameof(Keys.MusicVolume), value);
            }
        }

        /// <summary>
        /// The sound volume value set by the player.
        /// </summary>
        public float SoundVolume
        {
            get => sound;
            set
            {
                sound = value;
                audioMixer.SetFloat(nameof(Keys.SoundVolume), value);
            }
        }

        /// <summary>
        /// The sound volume value set by the player.
        /// </summary>
        public float VoiceVolume
        {
            get => voice;
            set
            {
                voice = value;
                audioMixer.SetFloat(nameof(Keys.VoiceVolume), value);
            }
        }

        /// <summary>
        /// The speed of animations.
        /// </summary>
        public float AnimationSpeed
        {
            get => animationSpeed;
            set => animationSpeed = value;
        }

        /// <summary>
        /// If the game has ever run.
        /// </summary>
        public bool FirstRun
        {
            get => firstRun;
            set => firstRun = value;
        }

        /// <summary>
        /// The ramp up of level difficulty.
        /// Easier starts with two easy levels, two middle levels and only the final level is hard.
        /// Harder has only one easy level, 2 mid levels, and 2 hard ones.
        /// </summary>
        public Difficulty Difficulty
        {
            get => difficulty;
            set => difficulty = value;
        }

        /// <summary>
        /// Overload for Dropdown menus. The ramp up of level difficulty.
        /// Easier starts with two easy levels, two middle levels and only the final level is hard.
        /// Harder has only one easy level, 2 mid levels, and 2 hard ones.
        /// </summary>
        public int DifficultyValue
        {
            set => difficulty = (Difficulty)value;
        }

        /// <summary>
        /// The ramp up of level difficulty.
        /// Easier starts with two easy levels, two middle levels and only the final level is hard.
        /// Harder has only one easy level, 2 mid levels, and 2 hard ones.
        /// </summary>
        public Language Language
        {
            get => language;
            set => language = value;
        }

        /// <summary>
        /// Overload for Dropdown menus. The ramp up of level difficulty.
        /// Easier starts with two easy levels, two middle levels and only the final level is hard.
        /// Harder has only one easy level, 2 mid levels, and 2 hard ones.
        /// </summary>
        public int LanguageValue
        {
            set => language = (Language)value;
        }

        /// <summary>
        /// The selected <see cref="Pet"/> to take care of.
        /// </summary>
        public Pet SelectedPet
        {
            get => _selectedPet;
            set => _selectedPet = value;
        }

        /// <summary>
        /// The lower time limit of notification mute.
        /// When the silence will start in the evening.
        /// </summary>
        public int SilenceStart
        {
            get => _silenceStart;
            set => _silenceStart = Mathf.Clamp(value, 0, 4) + 18;
        }

        /// <summary>
        /// The upper time limit of notification mute.
        /// When the silence will end in the morning.
        /// </summary>
        public int SilenceEnd
        {
            get => _silenceEnd;
            set => _silenceEnd = Mathf.Clamp(value, 0, 4) + 6;
        }

        /// <summary>
        /// Sets the debug counter. Only for editor access.
        /// </summary>
        public int DebugCount
        {
            get => debugCount;
            set
            {
                if (_isDebugMode) return;
                debugCount = Mathf.Clamp(debugCount + value, 0, 3);
            }
        }

        /// <summary>
        /// Debug mode offers more options to test on hardware.
        /// </summary>
        public bool DebugMode
        {
            get => _isDebugMode;
            set => _isDebugMode = value;
        }

        /// <summary>
        /// Set the current <see cref="Evolutions"/> level of the selected <see cref="Pet"/>.
        /// Only for debugging on hardware.
        /// </summary>
        public int DebugLevel { set => _selectedPet.LevelValue = value; }

        /// <summary>
        /// Sets the time factor value to speed up display.
        /// Only for debugging on hardware.
        /// </summary>
        public float DebugTime { set => _selectedPet.TimeFactor = value; }

        #endregion


        #region Methods

        /// <summary>
        /// Loads the settings from <see cref="PlayerPrefs"/>
        /// into the game's settings asset.
        /// </summary>
        public void LoadSettings()
        {
            if (PlayerPrefs.HasKey(nameof(Keys.SelectedPet)))
                _selectedPet = pets.Find(pet => pet.name == PlayerPrefs.GetString(nameof(Keys.SelectedPet)));

            if (PlayerPrefs.HasKey(nameof(Keys.GlyphList)))
            {
                List<GlyphData> glyphs = _selectedPet.Literals;
                foreach (string item in PlayerPrefs.GetString(nameof(Keys.GlyphList)).Split(GLYPH_SPLIT))
                {
                    if (item == "") continue;

                    string[] glyphData = item.Split(MEMORY_SPLIT);
                    if (Enum.TryParse(glyphData[1], out MemoryLevels level))
                    {
                        int.TryParse(glyphData[0][..3], out int index);
                        glyphs[index].MemoryLevel = level;
                    }
                }
            }


            // Volume values
            if (PlayerPrefs.HasKey(nameof(Keys.MainVolume)))
                MainVolume = PlayerPrefs.GetFloat(nameof(Keys.MainVolume));

            if (PlayerPrefs.HasKey(nameof(Keys.MusicVolume)))
                MusicVolume = PlayerPrefs.GetFloat(nameof(Keys.MusicVolume));

            if (PlayerPrefs.HasKey(nameof(Keys.SoundVolume)))
                SoundVolume = PlayerPrefs.GetFloat(nameof(Keys.SoundVolume));

            if (PlayerPrefs.HasKey(nameof(Keys.VoiceVolume)))
                VoiceVolume = PlayerPrefs.GetFloat(nameof(Keys.VoiceVolume));

            if (PlayerPrefs.HasKey(nameof(Keys.FirstRun)))
                FirstRun = PlayerPrefs.GetString(nameof(Keys.FirstRun)).Equals("True");
        }

        /// <summary>
        /// Saves the settings to <see cref="PlayerPrefs"/>
        /// from the game's settings asset.
        /// </summary>
        public void SaveSettings()
        {
            PlayerPrefs.SetString(nameof(Keys.SelectedPet), _selectedPet.name);

            string glyphs = "";
            foreach (GlyphData item in _selectedPet.Literals)
            {
                glyphs += item.name + MEMORY_SPLIT + item.MemoryLevel.ToString() + GLYPH_SPLIT;
            }
            PlayerPrefs.SetString(nameof(Keys.GlyphList), glyphs);

            PlayerPrefs.SetFloat(nameof(Keys.MainVolume), main);
            PlayerPrefs.SetFloat(nameof(Keys.MusicVolume), music);
            PlayerPrefs.SetFloat(nameof(Keys.SoundVolume), sound);
            PlayerPrefs.SetFloat(nameof(Keys.VoiceVolume), voice);

            PlayerPrefs.SetFloat(nameof(Keys.AnimationSpeed), animationSpeed);

            PlayerPrefs.SetString(nameof(Keys.FirstRun), firstRun.ToString());

        }

        #endregion


        #region Helpers

        #endregion


        /// <summary>
        /// The <see cref="Settings"/>'s keywords.
        /// </summary>
        public enum Keys
        {
            SelectedPet, GlyphList, MainVolume, MusicVolume, SoundVolume, VoiceVolume, AnimationSpeed, FirstRun
        }
    }

    /// <summary>
    /// The ramp up of level difficulty.
    /// </summary>
    public enum Difficulty
    {
        /// <summary>
        /// Easy: starts with two easy levels, then three middle levels, no hard ones.
        /// </summary>
        Easy,

        /// <summary>
        /// Starts with two easy levels, and two middle levels and one hard.
        /// </summary>
        Medium,

        /// <summary>
        /// Start with 1 easy level, 2 mid levels, and 2 hard ones.
        /// </summary>
        Hard
    }

    /// <summary>
    /// The language set for the game.
    /// </summary>
    public enum Language
    {
        English, Deutsch
    }
}