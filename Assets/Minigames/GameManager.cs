using System;
using System.Collections.Generic;
using UnityEngine;

namespace GlyphaeScripts
{
    public class GameManager : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField] private GameMenu gameMenu;

        [SerializeField] private Settings currentSettings;

        [Tooltip("List of Minigames to play.")]
        [SerializeField] private List<Minigame> minigames;

        [Tooltip("Field where the Pet will 'live' in.")]
        [SerializeField] private RectTransform petContainer;

        #endregion


        #region Fields

        private GameObject _petInstance, _gameInstance;
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

        #endregion


        #region Events
        
        

        #endregion


        #region Methods
        
        public void StartGame(GameObject minigame)
        {
            if (_gameInstance != null) return;

            Minigame game = minigame.GetComponent<Minigame>();
            playCost = game.energyCost;

            if (_pet.Needs.TryGetValue(Need.Energy, out float petEnergy) && petEnergy - playCost >= 0)
            {
                gameMenu.gameObject.SetActive(false);
                _gameInstance = Instantiate(minigame, petContainer);
                game = _gameInstance.GetComponent<Minigame>();

                game.SetupGame(_pet.Literals, _pet.CurrentLevel);
            }
        }

        #endregion


        #region Helpers

        private void CloseMinigame()
        {
            Destroy(_gameInstance);
            gameMenu.gameObject.SetActive(true);
            Settings.NeedUpdate(Need.Energy, -playCost);
        }

        #endregion

        #region Gizmos

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0, 0, 1, 0.2f);
            Gizmos.DrawCube(petContainer.position, petContainer.rect.size);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1, 0, 0, 0.3f);
            Gizmos.DrawCube(petContainer.position, petContainer.rect.size);
        }

        #endregion
    }
}