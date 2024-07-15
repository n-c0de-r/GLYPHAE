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

        [Tooltip("List of Minigames to play.")]
        [SerializeField] private List<Minigame> games;

        [Tooltip("The list of Pets available in the whole game.")]
        [SerializeField] private List<Pet> pets;

        [Header("Volume Settings")]
        [SerializeField][Range(-40,0)] private float main = -20.0f;
        [SerializeField][Range(-40,0)] private float music;
        [SerializeField][Range(-40,0)] private float sound;
        [SerializeField][Range(-40,0)] private float voice;

        [Header("Display Values")]
        [Tooltip("The speed of message animations animation.")]
        [SerializeField][Range(1, 5)] private int speedFactor = 3;

        #endregion


        #region Fields

        private Pet _pet;
        private List<Glyph> _literals;

        public const char GLYPH_SPLIT = ';';
        public const char MEMORY_SPLIT = ':';

        #endregion


        #region GetSets / Properties

        /// <summary>
        /// The list of <see cref="Minigame"/>s able to play.
        /// </summary>
        public List<Minigame> Games
        {
            get => games;
            set => games = value;
        }

        /// <summary>
        /// The list of <see cref="Pet"/>s available in the game.
        /// </summary>
        public List<Pet> Pets
        {
            get => pets;
            set => pets = value;
        }

        /// <summary>
        /// The selected <see cref="Pet"/> to take care of.
        /// </summary>
        public Pet SelectedPet
        {
            get => _pet;
            set =>  _pet = value;
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
        /// The speed of message animations animation.
        /// </summary>
        public int SpeedFactor
        {
            get => speedFactor;
            set => speedFactor = value;
        }

        #endregion


        #region Unity Built-Ins

        void Awake()
        {
            Pet.OnNeedUpdate += NeedUpdate;
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

        private void OnDisable()
        {
            //SaveSettings();
        }

        void OnDestroy()
        {
            Pet.OnNeedUpdate -= NeedUpdate;
        }

        #endregion


        #region Events

        //public static event Action<Needs, float> OnNeedUpdate;

        #endregion


        #region Methods



        /// <summary>
        /// Loads the settings from <see cref="PlayerPrefs"/>
        /// into the game's settings asset.
        /// </summary>
        public void LoadSettings()
        {
            if (PlayerPrefs.HasKey(nameof(Keys.SelectedPet)))
                _pet = pets.Find(pet => pet.name == PlayerPrefs.GetString(nameof(Keys.SelectedPet)));

            if (PlayerPrefs.HasKey(nameof(Keys.GlyphList)))
            {
                List<Glyph> glyphs = _pet.Literals;
                foreach (string item in PlayerPrefs.GetString(nameof(Keys.GlyphList)).Split(GLYPH_SPLIT))
                {
                    if (item == "") continue;

                    string[] glyphData = item.Split(MEMORY_SPLIT);
                    if (Enum.TryParse(glyphData[1], out MemoryLevels level))
                    {
                        int.TryParse(glyphData[0].Substring(0, 3), out int index);
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
        }

        /// <summary>
        /// Saves the settings to <see cref="PlayerPrefs"/>
        /// from the game's settings asset.
        /// </summary>
        public void SaveSettings()
        {
            PlayerPrefs.SetString(nameof(Keys.SelectedPet), _pet.name);

            string glyphs = "";
            foreach (Glyph item in _pet.Literals)
            {
                glyphs += item.name + MEMORY_SPLIT + item.MemoryLevel.ToString() + GLYPH_SPLIT;
            }
            PlayerPrefs.SetString(nameof(Keys.GlyphList), glyphs);

            PlayerPrefs.SetFloat(nameof(Keys.MainVolume), main);
            PlayerPrefs.SetFloat(nameof(Keys.MusicVolume), music);
            PlayerPrefs.SetFloat(nameof(Keys.SoundVolume), sound);
            PlayerPrefs.SetFloat(nameof(Keys.VoiceVolume), voice);

            PlayerPrefs.SetInt(nameof(Keys.AnimationSpeed), speedFactor);

        }

        public void NeedUpdate(Needs need, float value)
        {
            //OnNeedUpdate.Invoke(need, value);
            Debug.Log(need);
        }

        #endregion


        #region Helpers

        /// <summary>
        /// Sets up a new <see cref="Glyph"> list.
        /// </summary>
        public void SetupGlyphList()
        {
            if (_pet != null)
            {
                List<Glyph> glyphs = new();

                foreach (Glyph item in _pet.Literals) glyphs.Add(item);
            }
        }

        #endregion

        #region Enums 

        /// <summary>
        /// The setting's keywords.
        /// </summary>
        public enum Keys
        {
            SelectedPet, GlyphList, MainVolume, MusicVolume, SoundVolume, VoiceVolume, AnimationSpeed
        }

        #endregion
    }
}