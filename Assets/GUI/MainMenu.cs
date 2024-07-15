using System;
using UnityEngine;
using UnityEngine.SceneManagement;
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
            settings.LoadSettings();

            main.SetValueWithoutNotify(settings.MainVolume);
            music.SetValueWithoutNotify(settings.MusicVolume);
            sound.SetValueWithoutNotify(settings.SoundVolume);
            voice.SetValueWithoutNotify(settings.VoiceVolume);

            if (settings.SelectedPet != null) StartGame();
        }

        void FixedUpdate()
        {
            
        }

        void Update()
        {
            
        }

        #endregion


        #region Events

        public static event Action<string> OnGameStart;

        #endregion


        #region Methods

        /// <summary>
        /// Loads the given scene.
        /// </summary>
        public void StartGame()
        => OnGameStart.Invoke("2_Game");

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