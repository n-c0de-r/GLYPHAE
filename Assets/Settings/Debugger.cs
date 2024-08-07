using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GlyphaeScripts
{
    public class Debugger : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField] private Settings settings;

        [Header("Dropdown Controls")]
        [SerializeField] private TMP_Dropdown level;

        [Header("Need values")]
        [Tooltip("Basic needs of the pet:\r\nHunger, Health, Joy, Energy.")]
        [SerializeField] private NeedData[] needs;

        [Header("Slider Controls")]
        [SerializeField] private Slider TimeFactor;
        [SerializeField] private Slider hunger, health, joy, energy;

        [Header("Pet Numbers")]
        [SerializeField] private TMP_Text evoCalls;
        [SerializeField] private TMP_Text placeholder;
        [SerializeField] private TMP_Text sickFactor;

        [Header("Hunher Numbers")]
        [SerializeField] private TMP_Text hungerUp;
        [SerializeField] private TMP_Text hungerDown;
        [SerializeField] private TMP_Text hungerRng;
        [SerializeField] private TMP_Text hungerInc;

        [Header("Health Numbers")]
        [SerializeField] private TMP_Text healthUp;
        [SerializeField] private TMP_Text healthDown;
        [SerializeField] private TMP_Text healthRng;
        [SerializeField] private TMP_Text healthInc;


        [Header("Hunber Numbers")]
        [SerializeField] private TMP_Text joyUp;
        [SerializeField] private TMP_Text joyDown;
        [SerializeField] private TMP_Text joyRng;
        [SerializeField] private TMP_Text joyInc;


        [Header("Hunber Numbers")]
        [SerializeField] private TMP_Text energyUp;
        [SerializeField] private TMP_Text energyDown;
        [SerializeField] private TMP_Text energyRng;
        [SerializeField] private TMP_Text energyInc;

        #endregion


        #region GetSets / Properties

        /// <summary>
        /// The current Hunger <see cref="NeedData"/> value.
        /// Only for debugging on hardware.
        /// </summary>
        public float Hunger { set => needs[0].SetValue(value); }

        /// <summary>
        /// The current Health <see cref="NeedData"/> value.
        /// Only for debugging on hardware.
        /// </summary>
        public float Health { set => needs[1].SetValue(value); }

        /// <summary>
        /// The current Joy <see cref="NeedData"/> value.
        /// Only for debugging on hardware.
        /// </summary>
        public float Joy { set => needs[2].SetValue(value); }

        /// <summary>
        /// The current Energy <see cref="NeedData"/> value.
        /// Only for debugging on hardware.
        /// </summary>
        public float Energy { set => needs[3].SetValue(value); }

        /// <summary>
        /// Set the current <see cref="Evolutions"/> level of the selected <see cref="Pet"/>.
        /// Only for debugging on hardware.
        /// </summary>
        public int DebugLevel { set => settings.SelectedPet.LevelValue = value; }

        #endregion


        #region Unity Built-Ins

        private void OnEnable()
        {
            hunger.SetValueWithoutNotify(needs[0].Current);
            health.SetValueWithoutNotify(needs[1].Current);
            joy.SetValueWithoutNotify(needs[2].Current);
            energy.SetValueWithoutNotify(needs[3].Current);

            level.SetValueWithoutNotify((int)settings.SelectedPet.Level);
            UpdateNumbers();
        }

        #endregion


        #region Methods

        public void UpdateNumbers()
        {
            evoCalls.text = settings.SelectedPet.EvolutionCalls.ToString();
            //placeholder;
            sickFactor.text = settings.SelectedPet.SicknessChanceFactor.ToString();

            hungerUp.text = needs[0].UpFactor.ToString();
            hungerDown.text = needs[0].DownFactor.ToString();
            hungerRng.text = needs[0].RandomOffset.ToString("n2");
            hungerInc.text = settings.SelectedPet.HungerIncrement.ToString("n2");

            healthUp.text = needs[1].UpFactor.ToString();
            healthDown.text = needs[1].DownFactor.ToString();
            healthRng.text = needs[1].RandomOffset.ToString("n2");
            healthInc.text = settings.SelectedPet.HealthIncrement.ToString("n2");

            joyUp.text = needs[2].UpFactor.ToString();
            joyDown.text = needs[2].DownFactor.ToString();
            joyRng.text = needs[2].RandomOffset.ToString("n2");
            joyInc.text = settings.SelectedPet.JoyIncrement.ToString("n2");

            energyUp.text = needs[3].UpFactor.ToString();
            energyDown.text = needs[3].DownFactor.ToString();
            energyRng.text = needs[3].RandomOffset.ToString("n2");
            energyInc.text = settings.SelectedPet.EnergyIncrement.ToString("n2");
        }

        #endregion
    }
}