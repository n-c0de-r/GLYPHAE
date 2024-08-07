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
    }
}