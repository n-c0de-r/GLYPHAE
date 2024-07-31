using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GlyphaeScripts
{
    public class MainMenu : MonoBehaviour
    {
        #region Serialized Fields
        
        [SerializeField] private Settings settings;

        [Header("Volume Controls")]
        [SerializeField] private Slider main;
        [SerializeField] private Slider music, sound, voice;

        [Header("Dropdown Controls")]
        [SerializeField] private TMP_Dropdown difficulty;
        [SerializeField] private TMP_Dropdown timerStart, timerEnd;

        [Header("Input Objects")]
        [SerializeField] private GameObject buttonContainer;
        [SerializeField] private GameObject templateButton;

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

            difficulty.SetValueWithoutNotify((int)settings.Difficulty);
            timerStart.SetValueWithoutNotify(settings.SilenceStart-18);
            timerEnd.SetValueWithoutNotify(settings.SilenceEnd-6);

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
                Destroy(buttonContainer.transform.GetChild(i).gameObject);
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