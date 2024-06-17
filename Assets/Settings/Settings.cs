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
        [SerializeField] private List<GameObject> games, pets;

        [Header("Volume Settings")]
        [SerializeField][Range(-40,0)] private float main = -20.0f;
        [SerializeField][Range(-40,0)] private float music;
        [SerializeField][Range(-40,0)] private float sound;
        [SerializeField][Range(-40,0)] private float voice;

        #endregion


        #region Fields

        private GameObject _pet = null, _game = null;

        #endregion


        #region GetSets / Properties

        /// <summary>
        /// The selected game to play once in single or tutorial mode.
        /// </summary>
        public GameObject SelectedPet
        {
            get => _pet;
            set
            {
                _pet = value;
                PlayerPrefs.SetString("SelectedPet", value.name);
            }
        }

        /// <summary>
        /// The selected game to play once in single or tutorial mode.
        /// </summary>
        public GameObject SelectedGame
        {
            get => _game;
            set => _game = value;
        }

        /// <summary>
        /// The list of Minigames able to play in combination with others.
        /// </summary>
        public List<GameObject> Games
        {
            get => games;
            set => games = value;
        }

        /// <summary>
        /// The list of Minigames able to play in combination with others.
        /// </summary>
        public List<GameObject> Pets
        {
            get => pets;
            set => pets = value;
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

        public void OnHungerUpdate(float value)
        {
            OnNeedUpdate.Invoke(Need.Hunger, value);
        }

        #endregion


        #region Helpers



        private void TemplateHelper(bool param)
        {
            
        }

        #endregion
    }
}