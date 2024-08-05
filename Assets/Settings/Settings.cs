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
        [SerializeField][Range(VOL_MIN, VOL_MAX)] private float main = VOL_MIN / 2;
        [SerializeField][Range(VOL_MIN, VOL_MAX)] private float music = VOL_MAX;
        [SerializeField][Range(VOL_MIN, VOL_MAX)] private float sound = VOL_MAX;
        [SerializeField][Range(VOL_MIN, VOL_MAX)] private float voice = VOL_MAX;

        [Header("Display Values")]
        [Tooltip("The speed of animations.")]
        [SerializeField][Range(1, 5)] private float animationSpeed = 3;

        [Header("Other Values")]
        [Tooltip("The difficulty of the game,\r\nchanges game behavior slightly.")]
        [SerializeField] private Difficulty difficulty = Difficulty.Easy;
        [Tooltip("The selected language.")]
        [SerializeField] private Language language = Language.English;
        [Tooltip("If the game has ever run.")]
        [SerializeField] private bool firstRun = true;

        [Header("Debugging Values")]
        [SerializeField] private Pet _selectedPet;
        [SerializeField] private int _silenceStart = 20;
        [SerializeField] private int _silenceEnd = 8;
        [SerializeField] private int debugCount = 3;
        [SerializeField] private bool _isDebugMode = false;
        [SerializeField] private bool _hasNotificationPermission = false;

        #endregion


        #region Fields

        public const float VOL_MIN = -50, VOL_MAX = 0;


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
        public float VolumeMain
        {
            get => main;
            set
            {
                main = value;
                audioMixer.SetFloat(nameof(VolumeMain), value);
            }
        }

        /// <summary>
        /// The music volume value set by the player.
        /// </summary>
        public float VolumeMusic
        {
            get => music;
            set
            {
                music = value;
                audioMixer.SetFloat(nameof(VolumeMusic), value);
            }
        }

        /// <summary>
        /// The sound volume value set by the player.
        /// </summary>
        public float VolumeSound
        {
            get => sound;
            set
            {
                sound = value;
                audioMixer.SetFloat(nameof(VolumeSound), value);
            }
        }

        /// <summary>
        /// The sound volume value set by the player.
        /// </summary>
        public float VolumeVoice
        {
            get => voice;
            set
            {
                voice = value;
                audioMixer.SetFloat(nameof(VolumeVoice), value);
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
        /// If the game has ever run.
        /// </summary>
        public bool FirstRun
        {
            get => firstRun;
            set => firstRun = value;
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
            set => _silenceStart = value > 4 ? value : Mathf.Clamp(value, 0, 4) + 18;
        }

        /// <summary>
        /// The upper time limit of notification mute.
        /// When the silence will end in the morning.
        /// </summary>
        public int SilenceEnd
        {
            get => _silenceEnd;
            set => _silenceEnd = value > 4 ? value : Mathf.Clamp(value, 0, 4) + 6;
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
        /// Set when permission is granted.
        /// </summary>
        public bool NotificationPermission
        {
            get => _hasNotificationPermission;
            set => _hasNotificationPermission = value;
        }

        #endregion


        #region Methods

        /// <summary>
        /// Loads the settings from <see cref="PlayerPrefs"/>
        /// into the game's <see cref="Settings"/> asset.
        /// </summary>
        public void LoadSettings()
        {
            if (PlayerPrefs.HasKey(nameof(SelectedPet)))
            {
                _selectedPet = pets.Find(pet => pet.Name == PlayerPrefs.GetString(nameof(SelectedPet)));
            }

            // Volume values
            VolumeMain = PlayerPrefs.HasKey(nameof(VolumeMain)) ? PlayerPrefs.GetFloat(nameof(VolumeMain)) : VOL_MIN / 2;
            VolumeMusic = PlayerPrefs.HasKey(nameof(VolumeMusic)) ? PlayerPrefs.GetFloat(nameof(VolumeMusic)) : VOL_MAX;
            VolumeSound = PlayerPrefs.HasKey(nameof(VolumeSound)) ? PlayerPrefs.GetFloat(nameof(VolumeSound)) : VOL_MAX;
            VolumeVoice = PlayerPrefs.HasKey(nameof(VolumeVoice)) ? PlayerPrefs.GetFloat(nameof(VolumeVoice)) : VOL_MAX;

            if (PlayerPrefs.HasKey(nameof(FirstRun)))
                FirstRun = PlayerPrefs.GetString(nameof(FirstRun)).Equals(true.ToString());
        }

        /// <summary>
        /// Saves the settings to <see cref="PlayerPrefs"/>
        /// from the game's <see cref="Settings"/> asset.
        /// </summary>
        public void SaveSettings()
        {
            if (_selectedPet != null)
            {
                PlayerPrefs.SetString(nameof(SelectedPet), _selectedPet.Name);
            }
            
            PlayerPrefs.SetFloat(nameof(VolumeMain), main);
            PlayerPrefs.SetFloat(nameof(VolumeMusic), music);
            PlayerPrefs.SetFloat(nameof(VolumeSound), sound);
            PlayerPrefs.SetFloat(nameof(VolumeVoice), voice);

            PlayerPrefs.SetFloat(nameof(AnimationSpeed), animationSpeed);

            PlayerPrefs.SetString(nameof(FirstRun), firstRun.ToString());
        }

        #endregion


        #region Helpers

        #endregion
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