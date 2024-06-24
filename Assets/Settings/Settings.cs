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
        private Dictionary<string, Glyph> _literals = new();

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
        /// The list of <see cref="Pets"/>s available in the game.
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
            set
            {
                _pet = value;
                PlayerPrefs.SetString("SelectedPet", value.name);
            }
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
                audioMixer.SetFloat("MainVolume", value);
                PlayerPrefs.SetFloat("MainVolume", value);
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
                audioMixer.SetFloat("MusicVolume", value);
                PlayerPrefs.SetFloat("MusicVolume", value);
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
                audioMixer.SetFloat("SoundVolume", value);
                PlayerPrefs.SetFloat("SoundVolume", value);
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
                audioMixer.SetFloat("VoiceVolume", value);
                PlayerPrefs.SetFloat("VoiceVolume", value);
            }
        }

        /// <summary>
        /// The <see cref="Glyph"/> literals found in the saved data.
        /// </summary>
        public Dictionary<string, Glyph> Literals
        {
            get => _literals;
            set =>_literals = value;
        }

        /// <summary>
        /// The speed of message animations animation.
        /// </summary>
        public int SpeedFactor
        {
            get => speedFactor;
            set
            {
                speedFactor = value;
                PlayerPrefs.SetInt("SpeedFactor", value);
            } 
        }

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

        private void OnDisable()
        {
        }

        #endregion


        #region Events

        public static event Action<Need, float> OnNeedUpdate;

        #endregion


        #region Methods

        public static void NeedUpdate(Need need, float value)
        {
            OnNeedUpdate.Invoke(need, value);
        }

        #endregion


        #region Helpers

        /// <summary>
        /// Sets up a new dictionary
        /// </summary>
        public void SetupDictionary()
        {
            if (_pet != null)
            {
                string dict = "";
                foreach (Glyph item in _pet.Literals)
                {
                    // TODO: Check double items
                    _literals.Add(item.name, item);
                    dict += item.name + ":" + item.MemoryLevel.ToString() + ";";
                }
                // Store initial values in PlayerPrefs
                if (!PlayerPrefs.HasKey(_pet.name)) PlayerPrefs.SetString(_pet.name, dict);
            }
        }

        #endregion
    }
}