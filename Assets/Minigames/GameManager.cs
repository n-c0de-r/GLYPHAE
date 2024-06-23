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

        private GameObject _petInstance;
        private Pet _pet;
        private List<Glyph> _toLearn;

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
            int cost = minigame.GetComponent<Minigame>().energyCost;
            if (_pet.Needs.TryGetValue(Need.Energy, out float petEnergy) && petEnergy - cost >= 0)
            {
                _pet.Needs[Need.Energy] = Mathf.Clamp(petEnergy - cost, Pet.MIN, Pet.MAX);

                gameMenu.gameObject.SetActive(false);
                GameObject gameInstance = Instantiate(minigame, petContainer);
                Minigame game = gameInstance.GetComponent<Minigame>();

                game.SetupGame(_pet.Literals, _pet.CurrentLevel);
                return;
            }
            Debug.Log("Not enough Energy.");
        }

        public void TemplateMethod(bool param)
        {
            
        }

        #endregion


        #region Helpers

        private void CloseMinigame()
        {
            gameMenu.gameObject.SetActive(true);
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