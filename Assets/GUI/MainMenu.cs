using System;
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

        private void OnEnable()
        {
            main.SetValueWithoutNotify(settings.MainVolume);
            music.SetValueWithoutNotify(settings.MusicVolume);
            sound.SetValueWithoutNotify(settings.SoundVolume);
            voice.SetValueWithoutNotify(settings.VoiceVolume);

            settings.FirstRun = false;
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

        #endregion


        #region Events



        #endregion


        #region Methods

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