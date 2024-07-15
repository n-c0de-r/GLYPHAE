using System;
using System.Collections.Generic;
using UnityEngine;

namespace GlyphaeScripts
{
    public class GameManager : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField] private Settings currentSettings;

        [Tooltip("List of Minigames to play.")]
        [SerializeField] private List<Minigame> minigames;

        [Tooltip("Field where the Pet will 'live' in.")]
        [SerializeField] private RectTransform petContainer;

        [Tooltip("The intro game to play on start.")]
        [SerializeField] private Minigame initGame;

        #endregion


        #region Fields

        private GameObject _petInstance;
        private Pet _pet;

        #endregion


        #region GetSets / Properties



        #endregion


        #region Unity Built-Ins

        void Awake()
        {
            if (petContainer == null) TryGetComponent(out petContainer);
            _petInstance = Instantiate(currentSettings.SelectedPet.Prefab, petContainer);
            _pet = _petInstance.GetComponent<Pet>();

            Minigame.OnGameLose += CloseMinigame;
        }

        void Start()
        {
            if (_pet.PetLevel == Evolutions.Egg) StartGame(initGame);
        }

        void FixedUpdate()
        {
            
        }

        void Update()
        {

        }

        void OnDestroy()
        {
            Minigame.OnGameLose -= CloseMinigame;
        }

        #endregion


        #region Events

        public static event Action OnGameStart;
        public static event Action OnGameEnd;

        #endregion


        #region Methods

        public void StartGame(Minigame original)
        {

            if (_pet.Energy.Current >= original.EnergyCost)
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