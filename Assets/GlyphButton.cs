using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Scripts
{
    public class GlyphButton : MonoBehaviour, IClickable
    {
        #region Serialized Fields


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

        public void OnClick()
        {
            Debug.Log(name + " clicked");
        }

        public void OnStartDrag()
        {
            throw new NotImplementedException();
        }

        public void OnEndDrag()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}