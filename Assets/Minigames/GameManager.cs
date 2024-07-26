using System;
using System.Collections.Generic;
using UnityEngine;

namespace GlyphaeScripts
{
    public class GameManager : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField] private Settings settings;

        [Tooltip("List of Minigames to play.")]
        [SerializeField] private List<Minigame> minigames;

        [Tooltip("Field where the Pet will 'live' in.")]
        [SerializeField] private RectTransform petContainer;

        #endregion


        #region Fields

        private GameObject _petInstance;
        private Pet _pet;

        #endregion


        #region Events

        public static event Action OnGameStart;
        public static event Action OnGameEnd;

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
            if (petContainer == null) TryGetComponent(out petContainer);

            _petInstance = Instantiate(settings.SelectedPet.gameObject, petContainer);
            _pet = _petInstance.GetComponent<Pet>();
            _petInstance.SetActive(!settings.FirstLevel);
        }

        private void OnEnable()
        {
            InitMixer.OnFinished += () =>
            {
                settings.FirstLevel = false;
                OnGameEnd.Invoke();
                _petInstance.SetActive(!settings.FirstLevel);

            };
            Minigame.OnGameLose += CloseMinigame;
        }

        void Start()
        {
            if (settings.FirstLevel) StartGame(minigames[0]);
            else OnGameEnd.Invoke();
        }

        void FixedUpdate()
        {
            
        }

        void Update()
        {

        }

        private void OnDisable()
        {
            InitMixer.OnFinished -= () =>
            {
                settings.FirstLevel = false;
                OnGameEnd.Invoke();
                _petInstance.SetActive(!settings.FirstLevel);
            };
            Minigame.OnGameLose -= CloseMinigame;
        }

        void OnDestroy()
        {

        }

        #endregion


        #region Methods

        public void StartGame(Minigame original)
        {

            if (settings.FirstLevel || _pet.Energy.Current >= original.EnergyCost)
            {
                GameObject instance = Instantiate(original.gameObject, petContainer);
                Minigame game = instance.GetComponent<Minigame>();

                game.SetupGame(_pet.Literals, _pet.PetLevel);
                OnGameStart?.Invoke();
            }
        }

        #endregion


        #region Helpers

        private void CloseMinigame(GameObject minigame)
        {
            Destroy(minigame);
            OnGameEnd?.Invoke();
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