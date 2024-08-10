using System;
using System.Collections;
using UnityEngine;

namespace GlyphaeScripts
{
    public class GameMenu : MonoBehaviour
    {
        #region Serialized Fields

        [Tooltip("The current Settings for display values.")]
        [SerializeField] private Settings settings;

        [Header("Base values")]
        [Header("Other objects")]
        [Tooltip("GUI buttons panel .")]
        [SerializeField] private GameObject buttonsPanel;

        [Tooltip("Used for evolution and sleep.")]
        [SerializeField] private FlashOverlay flashOverlay;

        [Tooltip("Button to wake the pet.")]
        [SerializeField] private GameObject wakeButton;

        [Tooltip("Hidden button to activate debug mode.")]
        [SerializeField] private GameObject debugActivator;

        [Tooltip("The actual button to show the debug view.")]
        [SerializeField] private GameObject debugButton;

        #endregion


        #region Events

        public static event Action<Minigame> OnGameSelected;
        public static event Action OnGameRandom;

        #endregion


        #region GetSets / Properties

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

        private void OnEnable()
        {
            GameManager.OnGameFinished += ToggleButtons;

            LullabyChant.OnSleep += Sleep;
            Pet.OnEvolve += Flash;

            debugButton.SetActive(settings.DebugMode);
        }

        private void OnDisable()
        {
            GameManager.OnGameFinished -= ToggleButtons;

            LullabyChant.OnSleep -= Sleep;
            Pet.OnEvolve -= Flash;

            settings.SaveSettings();
        }

        #endregion


        #region Methods

        public void PlayGame(Minigame selected)
        {
            OnGameSelected?.Invoke(selected);
        }


        public void PlayGame()
        {
            OnGameRandom?.Invoke();
        }

        public void WakeUp()
        {
            StartCoroutine(AnimateWake(1, 0, settings.AnimationSpeed));
            buttonsPanel.SetActive(true);
            wakeButton.SetActive(false);
            settings.SelectedPet.WakeUp();
        }

        #endregion


        #region Helpers

        private void ToggleButtons(bool state)
        {
            buttonsPanel.SetActive(state);
        }

        private void Flash()
        {
            StartCoroutine(AnimateEvolution(0, 1, settings.AnimationSpeed));
        }

        private void Sleep()
        {
            StartCoroutine(AnimateSleep(0, 1, settings.AnimationSpeed));
            buttonsPanel.SetActive(false);
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