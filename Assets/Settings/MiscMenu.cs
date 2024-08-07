using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GlyphaeScripts
{
    public class MiscMenu : MonoBehaviour
    {
        #region Serialized Fields
        
        [SerializeField] private Settings settings;

        [Header("Dropdown Controls")]
        [SerializeField] private TMP_Dropdown language;
        [SerializeField] private TMP_Dropdown difficulty, timerStart, timerEnd;

        [Header("Slind Controls")]
        [SerializeField] private Slider animationSpeed;


        #endregion


        #region Unity Built-Ins

        private void OnEnable()
        {
            difficulty.SetValueWithoutNotify((int)settings.Difficulty);
            language.SetValueWithoutNotify((int)settings.Language);
            timerStart.SetValueWithoutNotify(settings.SilenceStart-18);
            timerEnd.SetValueWithoutNotify(settings.SilenceEnd-6);

            animationSpeed.SetValueWithoutNotify(settings.AnimationSpeed);
        }

        private void OnDisable()
        {
            settings.SaveSettings();
        }

        #endregion


        #region Methods

        public void Reset()
        {
            settings.Difficulty = Difficulty.Easy;
            difficulty.SetValueWithoutNotify(0);
            settings.Language = Language.English;
            language.SetValueWithoutNotify(0);

            settings.AnimationSpeed = 3;
            animationSpeed.SetValueWithoutNotify(3);

            settings.SilenceStart = 20;
            timerStart.SetValueWithoutNotify(2);
            settings.SilenceEnd = 8;
            timerEnd.SetValueWithoutNotify(2);
        }

        #endregion
    }
}