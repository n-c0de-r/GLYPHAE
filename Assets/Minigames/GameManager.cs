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

        #endregion


        #region Fields

        private GameObject _petInstance;
        private List<Glyph> _toLearn;

        #endregion


        #region GetSets / Properties



        #endregion


        #region Unity Built-Ins

        void Awake()
        {
            if (petContainer == null) TryGetComponent(out petContainer);
            _petInstance = Instantiate(currentSettings.SelectedPet.gameObject, petContainer);
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
        
        

        public void TemplateMethod(bool param)
        {
            
        }

        #endregion


        #region Helpers

        

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