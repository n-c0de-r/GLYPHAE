using System;
using System.Collections.Generic;
using Unity.VisualScripting;
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
        [Tooltip("GUI panel to turn on and off.")]
        [SerializeField] private GameObject mainPanel;

        [Tooltip("Field where the Pet will 'live' in.")]
        [SerializeField] private RectTransform objectContainer;

        #endregion


        #region Fields

        private Pet _pet;

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

        #endregion


        #region Unity Built-Ins

        void Awake()
        {
            settings.PetInstance = Instantiate(settings.SelectedPet.gameObject, objectContainer);
            _pet = settings.PetInstance.GetComponent<Pet>();
            settings.PetInstance.SetActive(_pet.Level != Evolutions.Egg);
        }

        private void OnEnable()
        {
            Minigame.OnGameClose += CloseMinigame;
            ShellBreaker.OnEggBreak += () =>
            {
                _pet.IncreaseLevel();
                settings.PetInstance.SetActive(_pet.Level != Evolutions.Egg);
            };
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
        }

        void OnDestroy()
        {

        }

        #endregion


        #region Methods

        public void StartGame(Minigame original)
        {
            GameObject instance = Instantiate(original.gameObject, objectContainer);
            Minigame game = instance.GetComponent<Minigame>();

            if (_pet.Energy.Current < game.EnergyCost)
            {
                Destroy(instance);
                return;
            }

            if (_pet.Level != Evolutions.Egg) _pet.Energy.Decrease(game.EnergyCost);
            mainPanel.SetActive(false);
            game.SetupGame(_pet.Literals, CalculateBaselevel());
            game.NextRound();
        }

        #endregion


        #region Helpers

        private void CloseMinigame(GameObject minigame)
        {
            if (!settings.PetInstance.activeInHierarchy) settings.PetInstance.SetActive(!(_pet.Level == Evolutions.Egg));
            Destroy(minigame);
            mainPanel.SetActive(true);
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

        #endregion

        #region Gizmos

        //private void OnDrawGizmos()
        //{
        //    Gizmos.color = new Color(0, 0, 1, 0.2f);
        //    Gizmos.DrawCube(petContainer.localPosition - new Vector3(0, petContainer.anchoredPosition.y + petContainer.pivot.y, 0), petContainer.rect.size);
        //}

        //private void OnDrawGizmosSelected()
        //{
        //    Gizmos.color = new Color(1, 0, 0, 0.3f);
        //    Gizmos.DrawCube(petContainer.localPosition - new Vector3(0, petContainer.anchoredPosition.y + petContainer.pivot.y, 0), petContainer.rect.size);
        //}

        #endregion
    }
}