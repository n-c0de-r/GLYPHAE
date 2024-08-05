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
            main.SetValueWithoutNotify(settings.VolumeMain);
            main.minValue = Settings.VOL_MIN;
            main.maxValue = Settings.VOL_MAX;
            music.SetValueWithoutNotify(settings.VolumeMusic);
            music.minValue = Settings.VOL_MIN;
            music.maxValue = Settings.VOL_MAX;
            sound.SetValueWithoutNotify(settings.VolumeSound);
            sound.minValue = Settings.VOL_MIN;
            sound.maxValue = Settings.VOL_MAX;
            voice.SetValueWithoutNotify(settings.VolumeVoice);
            voice.minValue = Settings.VOL_MIN;
            voice.maxValue = Settings.VOL_MAX;
        }

        void Start()
        {
            Reset();
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
            settings.VolumeMain = Settings.VOL_MIN / 2;
            main.SetValueWithoutNotify(Settings.VOL_MIN / 2);
            settings.VolumeMusic = Settings.VOL_MAX;
            music.SetValueWithoutNotify(Settings.VOL_MAX);
            settings.VolumeSound = Settings.VOL_MAX;
            sound.SetValueWithoutNotify(Settings.VOL_MAX);
            settings.VolumeVoice = Settings.VOL_MAX;
            voice.SetValueWithoutNotify(Settings.VOL_MAX);
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