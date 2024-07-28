using System;
using System.Collections.Generic;
using UnityEngine;

namespace GlyphaeScripts
{
    public class GameMenu : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField] private Settings settings;

        [Header("Base values")]
        [Tooltip("The costs of Energy to play a game.")]
        [SerializeField][Range(0, 50)] protected int energyCost;

        [Tooltip("List of Minigames to play.")]
        [SerializeField] private List<Minigame> minigames;

        [Header("Other objects")]
        [Tooltip("GUI panel to turn on and off.")]
        [SerializeField] private GameObject mainPanel;

        [Tooltip("Field where the Pet will 'live' in.")]
        [SerializeField] private RectTransform objectContainer;

        #endregion


        #region Fields

        private GameObject _petInstance;
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
            _petInstance = Instantiate(settings.SelectedPet.gameObject, objectContainer);
            _pet = _petInstance.GetComponent<Pet>();
            _petInstance.SetActive(!(_pet.Level == Evolutions.Egg));
        }

        private void OnEnable()
        {
            Minigame.OnGameClose += CloseMinigame;
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

            if (_pet.Energy.Current >= energyCost)
            {
                GameObject instance = Instantiate(original.gameObject, objectContainer);
                Minigame game = instance.GetComponent<Minigame>();

                game.SetupGame(_pet.Literals, _pet.Level);
                _pet.Energy.setData(-energyCost);
                mainPanel.SetActive(false);
            }
        }

        #endregion


        #region Helpers

        private void CloseMinigame(GameObject minigame)
        {
            if (!_petInstance.activeInHierarchy) _petInstance.SetActive(!(_pet.Level == Evolutions.Egg));
            Destroy(minigame);
            mainPanel.SetActive(true);
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