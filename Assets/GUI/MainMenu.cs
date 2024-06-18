using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GlyphaeScripts
{
    public class MainMenu : MonoBehaviour
    {
        #region Serialized Fields
        
        [SerializeField] private Settings settings;
        [SerializeField] private Slider main, music, sound, voice;

        #endregion


        #region Fields



        #endregion


        #region GetSets / Properties



        #endregion


        #region Unity Built-Ins

        void Awake()
        {
        }

        void Start()
        {
            LoadSettings();

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
        /// Saves the settings to PlayerPrefs
        /// from the game's settings asset.
        /// </summary>
        public void SaveSettings()
        {

        }

        /// <summary>
        /// Loads the given scene.
        /// </summary>
        /// <param name="scene">Scene name.</param>
        public void StartGame() { }
        // => SceneManager.LoadScene((int)Scenes.GAME);

        /// <summary>
        /// Quits the application or editor.
        /// </summary>
        public void Quit()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #endif
                Application.Quit();
        }

        #endregion


        #region Helpers

        /// <summary>
        /// Loads the settings from PlayerPrefs
        /// into the game's settings asset.
        /// </summary>
        private void LoadSettings()
        {
            if (PlayerPrefs.HasKey("SelectedPet"))
            {
                string petName = PlayerPrefs.GetString("SelectedPet");
                foreach (Pet pet in settings.Pets)
                {
                    if (pet.name == petName)
                    {
                        settings.SelectedPet = pet;
                        settings.SetupDictionary();
                        break;
                    }
                }

                if (PlayerPrefs.HasKey(petName))
                {
                    foreach (string item in PlayerPrefs.GetString(petName).Split(';'))
                    {
                        if (item == "") continue;

                        Enum.TryParse(item.Split(":")[1], out MemoryLevel level);
                        settings.Literals[item.Split(':')[0]].MemoryLevel = level;
                    }
                }
            }

            if (PlayerPrefs.HasKey("MainVolume"))
            {
                float value = PlayerPrefs.GetFloat("MainVolume");
                main.SetValueWithoutNotify(value);
                settings.MainVolume = value;
            }

            if (PlayerPrefs.HasKey("MusicVolume"))
            {
                float value = PlayerPrefs.GetFloat("MusicVolume");
                music.SetValueWithoutNotify(value);
                settings.MusicVolume = value;
            }

            if (PlayerPrefs.HasKey("SoundVolume"))
            {
                float value = PlayerPrefs.GetFloat("SoundVolume");
                sound.SetValueWithoutNotify(value);
                settings.SoundVolume = value;
            }

            if (PlayerPrefs.HasKey("VoiceVolume"))
            {
                float value = PlayerPrefs.GetFloat("VoiceVolume");
                voice.SetValueWithoutNotify(value);
                settings.VoiceVolume = value;
            }
        }

        #endregion


        #region Gizmos

        private void OnDrawGizmos()
        {
            
        }

        private void OnDrawGizmosSelected()
        {
             
        }

        #endregion
    }
}