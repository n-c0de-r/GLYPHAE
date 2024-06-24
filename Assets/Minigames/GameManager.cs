using System;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Collections.AllocatorManager;

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

        #endregion


        #region Fields

        private GameObject _petInstance;
        private Pet _pet;
        private List<Glyph> _toLearn;
        private int playCost;

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

        public void StartGame(GameObject minigame)
        {
            Minigame game = minigame.GetComponent<Minigame>();

            if (_pet.Needs.TryGetValue(Need.Energy, out float petEnergy) && petEnergy - game.EnergyCost >= 0)
            {
                GameObject gameInstance = Instantiate(minigame, petContainer);
                game = gameInstance.GetComponent<Minigame>();

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