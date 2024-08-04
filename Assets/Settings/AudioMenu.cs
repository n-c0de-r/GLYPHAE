using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GlyphaeScripts
{
    public class AudioMenu : MonoBehaviour
    {
        #region Serialized Fields
        
        [SerializeField] private Settings settings;

        [Header("Volume Controls")]
        [SerializeField] private Slider main;
        [SerializeField] private Slider music, sound, voice;

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

        public void Reset()
        {
            settings.MainVolume = -20;
            main.SetValueWithoutNotify(-20);
            settings.MusicVolume = 0;
            music.SetValueWithoutNotify(0);
            settings.SoundVolume = 0;
            sound.SetValueWithoutNotify(0);
            settings.VoiceVolume = 0;
            voice.SetValueWithoutNotify(0);
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