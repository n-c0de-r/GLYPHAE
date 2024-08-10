using Random = UnityEngine.Random;
using System;
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

        #endregion


        #region Fields

        private Pet _pet;
        public static HashSet<NeedData> _criticals = new();

        #endregion


        #region Events

        public static event Action<bool> OnGameFinished;

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

        #endregion


        #region Unity Built-Ins

        private void OnEnable()
        {
            GameMenu.OnGameSelected += StartGame;
            GameMenu.OnGameRandom += StartGame;

            Minigame.OnGameClose += CloseMinigame;
            Minigame.OnGameWin += (need) => _criticals.Remove(need);

            NeedData.OnNeedCritical += SetCiticals;

            ShellBreaker.OnEggBreak += () =>
            {
                _pet.IncreaseLevel();
                settings.SelectedPet.gameObject.SetActive(_pet.Level != Evolutions.Egg);
                settings.FirstRun = false;
            };
        }

        void Start()
        {
            if (settings.SelectedPet != null)
            {
                GameObject instance = Instantiate(settings.SelectedPet.gameObject, transform);
                _pet = instance.GetComponent<Pet>();
                settings.SelectedPet = _pet;
                instance.SetActive(_pet.Level != Evolutions.Egg);
            }

            if (_pet.Level == Evolutions.Egg) StartGame(minigames[0]);
        }

        private void OnDisable()
        {
            GameMenu.OnGameSelected -= StartGame;
            GameMenu.OnGameRandom -= StartGame;

            Minigame.OnGameClose -= CloseMinigame;

            NeedData.OnNeedCritical -= SetCiticals;
        }

        #endregion


        #region Methods

        public void StartGame()
        {
            StartGame(minigames[Random.Range(1, minigames.Count)]);
        }

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

            GameObject instance = Instantiate(picked.gameObject, transform);
            Minigame game = instance.GetComponent<Minigame>();

            OnGameFinished?.Invoke(false);

            game.SetupGame(_criticals.Contains(game.PrimaryNeed), _pet.Literals, baseLevel);
            game.NextRound();
        }

        #endregion


        #region Helpers

        private void CloseMinigame(Minigame game)
        {
            if (!settings.SelectedPet.gameObject.activeInHierarchy) settings.SelectedPet.gameObject.SetActive(!(_pet.Level == Evolutions.Egg));
            settings.SelectedPet.GetComponent<SpriteRenderer>().enabled = true;
            OnGameFinished?.Invoke(true);
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

        #endregion
    }
}