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
        [SerializeField] private GameObject buttonContainer, templateButton;

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

            if (buttonContainer.transform.childCount != 0) ResetButtons();
            if (buttonContainer.transform.childCount == 0) SetupButtons();
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
         //=> SceneManager.LoadScene((int)Scenes.GAME);

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

        private void ResetButtons()
        {
            for (int i = 0; i < buttonContainer.transform.childCount; i++)
            {
                Destroy(buttonContainer.transform.GetChild(i));
            }
        }

        private void SetupButtons()
        {
            foreach (Pet pet in settings.Pets)
            {
                GameObject go = Instantiate(templateButton, buttonContainer.transform);
                PetButton button = go.GetComponent<PetButton>();
                go.name = pet.Name;
                if (pet.Unlocked)
                    button.Setup(pet, () => settings.SelectedPet = pet);
                go.SetActive(true);
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