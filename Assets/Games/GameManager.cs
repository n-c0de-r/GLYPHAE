using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GlyphaeScripts
{
    public class GameManager : MonoBehaviour
    {
        #region Serialized Fields

        [Tooltip("The current Settings for display values.")]
        [SerializeField] private Settings settings;

        [Header("Base values")]
        [Tooltip("List of Minigames to play.")]
        [SerializeField] private List<Minigame> minigames;

        [Header("Other objects")]
        [Tooltip("GUI buttons left panel to turn on and off.")]
        [SerializeField] private GameObject leftButtons;

        [Tooltip("GUI buttons right panel to turn on and off.")]
        [SerializeField] private GameObject rightButtons;

        [Tooltip("Used for evolution and sleep.")]
        [SerializeField] private FlashOverlay flashOverlay;

        [Tooltip("Button to wake the pet.")]
        [SerializeField] private GameObject wakeButton;

        [Tooltip("Field where the Pet will 'live' in.")]
        [SerializeField] private RectTransform objectContainer;

        [Tooltip("Hidden button to activate debug mode.")]
        [SerializeField] private GameObject debugActivator;

        [Tooltip("The actual button to show the debug view.")]
        [SerializeField] private GameObject debugButton;

        #endregion


        #region Fields

        private Pet _pet;
        public static HashSet<NeedData> _criticals = new();

        #endregion


        #region Events



        #endregion


        #region GetSets / Properties

        /// <summary>
        /// The list of <see cref="Minigame"/>s able to play.
        /// </summary>
        public List<Minigame> Games
        {
            get => minigames;
            set => minigames = value;
        }

        /// <summary>
        /// Sets the debug counter. Only for editor access.
        /// </summary>
        public int DebugClick
        {
            set
            {
                if (settings.DebugMode) return;
                settings.DebugCount = value;
                debugActivator.SetActive(settings.DebugCount == 0);
            }
        }

        #endregion


        #region Unity Built-Ins

        void Awake()
        {
            if (settings.SelectedPet != null)
            {
                GameObject instance = Instantiate(settings.SelectedPet.gameObject, objectContainer);
                _pet = instance.GetComponent<Pet>();
                settings.SelectedPet = _pet;
                instance.SetActive(_pet.Level != Evolutions.Egg);
            }
        }

        private void OnEnable()
        {
            Minigame.OnGameClose += CloseMinigame;
            Minigame.OnGameWin += (need) => _criticals.Remove(need);

            NeedData.OnNeedCritical += SetCiticals;

            LullabyChant.OnSleep += Sleep;
            Pet.OnEvolve += Flash;

            ShellBreaker.OnEggBreak += () =>
            {
                _pet.IncreaseLevel();
                settings.SelectedPet.gameObject.SetActive(_pet.Level != Evolutions.Egg);
                settings.FirstRun = false;
            };

            debugButton.SetActive(settings.DebugMode);
        }

        void Start()
        {
            if (_pet.Level == Evolutions.Egg) StartGame(minigames[0]);
        }

        void FixedUpdate()
        {

        }

        void Update()
        {

        }

        private void OnDisable()
        {
            Minigame.OnGameClose -= CloseMinigame;

            NeedData.OnNeedCritical -= SetCiticals;

            LullabyChant.OnSleep -= Sleep;
            Pet.OnEvolve -= Flash;

            settings.SaveSettings();
        }

        void OnDestroy()
        {

        }

        #endregion


        #region Methods

        public void StartGame(Minigame picked)
        {
            int baseLevel = CalculateBaselevel();
            if (_pet.Energy.Current < picked.EnergyCost + baseLevel) return;

            if (!picked.GetType().Equals(typeof(LullabyChant)) && picked.PrimaryNeed.Current > picked.PrimaryNeed.SatisfiedLimit)
            {
                picked.MessageSuccess(picked.PrimaryNeed.Positive);
                return;
            }

            if (picked.GetType().Equals(typeof(LullabyChant)) && picked.SecondaryNeed.Current < picked.LossAmount)
            {
                picked.MessageFail(picked.SecondaryNeed.Negative);
                return;
            }

            GameObject instance = Instantiate(picked.gameObject, objectContainer);
            Minigame game = instance.GetComponent<Minigame>();

            leftButtons.SetActive(false);
            rightButtons.SetActive(false);
            game.SetupGame(_criticals.Contains(game.PrimaryNeed), _pet.Literals, baseLevel);
            game.NextRound();
        }

        public void WakeUp()
        {
            StartCoroutine(AnimateWake(1, 0, settings.AnimationSpeed));
            leftButtons.SetActive(true);
            rightButtons.SetActive(true);
            wakeButton.SetActive(false);
            _pet.WakeUp();
        }

        #endregion


        #region Helpers

        private void CloseMinigame(Minigame game)
        {
            if (!settings.SelectedPet.gameObject.activeInHierarchy) settings.SelectedPet.gameObject.SetActive(!(_pet.Level == Evolutions.Egg));
            settings.SelectedPet.GetComponent<SpriteRenderer>().enabled = true;
            leftButtons.SetActive(true);
            rightButtons.SetActive(true);
            _pet.Energy.Decrease(game.EnergyCost);
            game.UpdateValues();
            Destroy(game.gameObject);
        }

        private int CalculateBaselevel()
        {
            int halfLevels = (Enum.GetValues(typeof(Evolutions)).Length / 2);
            switch (settings.Difficulty)
            {
                case Difficulty.Easy:
                    return (int)_pet.Level / halfLevels;
                case Difficulty.Medium:
                    return (int)_pet.Level / (halfLevels - (int)_pet.Level / (int)Evolutions.God);
                case Difficulty.Hard:
                    return (int)_pet.Level >> 1;
            }
            return -1;
        }

        private void SetCiticals(NeedData data, bool state)
        {
            _criticals.Add(data);
        }

        private void Flash()
        {
            StartCoroutine(AnimateEvolution(0, 1, settings.AnimationSpeed));
        }

        private void Sleep()
        {
            StartCoroutine(AnimateSleep(0, 1, settings.AnimationSpeed));
            leftButtons.SetActive(false);
            rightButtons.SetActive(false);
            wakeButton.SetActive(true);
        }

        private IEnumerator AnimateEvolution(float start, float end, float speedFactor)
        {
            yield return flashOverlay.Flash(Color.white, start, end, speedFactor);

            yield return new WaitForSeconds(1f / speedFactor);

            yield return flashOverlay.Flash(Color.clear, end, start, speedFactor);
        }

        private IEnumerator AnimateSleep(float start, float end, float speedFactor)
        {
            yield return flashOverlay.Flash(Color.black, start, end, speedFactor);

            yield return new WaitForSeconds(1f / speedFactor);
        }

        private IEnumerator AnimateWake(float start, float end, float speedFactor)
        {
            yield return flashOverlay.Flash(Color.clear, start, end, speedFactor);

            yield return new WaitForSeconds(1f / speedFactor);
        }

        #endregion
    }
}