using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GlyphaeScripts
{
    public class DictButton : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField] private Button button;
        [SerializeField] private AudioSource speech;
        [SerializeField] private Image back;
        [SerializeField] private Image symbol;
        [SerializeField] private Image character;

        #endregion


        #region Fields
        
        

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


        #region Events
        
        

        #endregion


        #region Methods
        
        

        public void SetupButton(GlyphData glyph)
        {
            MemoryLevels level = glyph.MemoryLevel;

            if (level >= MemoryLevels.Seen) symbol.sprite = glyph.Symbol;

            if (level >= MemoryLevels.Unknown) character.sprite = glyph.Letter;

            if (level >= MemoryLevels.Known) speech.clip = glyph.Sound;

            button.interactable = level >= MemoryLevels.Memorized;

            gameObject.name = glyph.name;

            gameObject.SetActive(true);
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