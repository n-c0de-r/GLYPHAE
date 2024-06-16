using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{
    [CreateAssetMenu(fileName = "Pet", menuName = "ScriptableObjects/Pet")]
    public class PetAsset : ScriptableObject
    {
        #region Serialized Fields

        [SerializeField] private Sprite[] levelSprites;
        [SerializeField] private List<GlyphAsset> literals;

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
        
        

        public void TemplateMethod(bool param)
        {
            
        }

        #endregion


        #region Helpers
        
        

        private void TemplateHelper(bool param)
        {
            
        }

        #endregion
    }
}