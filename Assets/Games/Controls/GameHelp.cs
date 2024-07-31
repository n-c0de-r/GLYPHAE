using TMPro;
using UnityEngine;

namespace GlyphaeScripts
{
    public class GameHelp : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField] private TMP_Text gameName;

        [Tooltip("A short description of this Minigame.")]
        [SerializeField] private TMP_Text description;

        [Tooltip("Instructions how to play the game.")]
        [SerializeField] private TMP_Text instructions;


        #endregion


        #region Fields
        
        

        #endregion


        #region Events
        
        

        #endregion


        #region GetSets / Properties
        
        

        #endregion


        #region Unity Built-Ins

        void Awake()
        {
            
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


        #region Methods

        public void Setup(string newName, string newDescription, string newInstcructions)
        {
            gameName.text = newName;
            description.text = newDescription;
            instructions.text = newInstcructions;
        }

        #endregion


        #region Helpers

        private void TemplateHelper(bool param)
        {
            
        }

        #endregion


        #region Gizmos

        private void OnDrawGizmos()
        {
            
        }

        private void OnDrawGizmosSelected()
        {
             
        }

        #endregion
    }
}