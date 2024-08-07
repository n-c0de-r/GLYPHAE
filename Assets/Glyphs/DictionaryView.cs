using UnityEngine;

namespace GlyphaeScripts
{
    public class DictionaryView : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField] private Transform container;
        [SerializeField] private GameObject iconTemplate;

        #endregion


        #region Methods
        
        public void SetupGlyphs(Pet pet)
        {
            if (container.childCount != 0) ClearContainer();

            foreach (GlyphData glyph in pet.Literals)
            {
                GameObject instance = Instantiate(iconTemplate, container);
                instance.TryGetComponent(out DictButton dictbutton);
                dictbutton.SetupButton(glyph);
            }
        }

        #endregion


        #region Helpers

        private void ClearContainer()
        {
            for (int i = 0; i < container.childCount; i++)
                Destroy(container.GetChild(i).gameObject);
        }

        #endregion
    }
}