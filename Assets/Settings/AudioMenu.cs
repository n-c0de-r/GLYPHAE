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


        #region Unity Built-Ins

        void Start()
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

        #endregion


        #region Methods

        public void Reset()
        {
            settings.VolumeMain = Settings.VOL_MAX;
            main.SetValueWithoutNotify(Settings.VOL_MAX);
            settings.VolumeMusic = Settings.VOL_MIN / 2;
            music.SetValueWithoutNotify(Settings.VOL_MIN / 2);
            settings.VolumeSound = Settings.VOL_MAX;
            sound.SetValueWithoutNotify(Settings.VOL_MAX);
            settings.VolumeVoice = Settings.VOL_MIN / 5;
            voice.SetValueWithoutNotify(Settings.VOL_MIN / 5);
        }

        #endregion
    }
}