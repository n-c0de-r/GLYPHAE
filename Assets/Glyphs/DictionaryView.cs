using System.Collections.Generic;
using UnityEngine;

namespace GlyphaeScripts
{
    public class DictionaryView : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField] private Transform container;
        [SerializeField] private GameObject iconTemplate;

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

        private void ClearContainer()
        {
            for (int i = 0; i < container.childCount; i++)
            {
                Destroy(container.GetChild(i).gameObject);
            }
        }

        #endregion


        #region Helpers



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