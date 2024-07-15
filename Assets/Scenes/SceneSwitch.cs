using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GlyphaeScripts
{
    [RequireComponent(typeof(AudioSource))]
    public class SceneSwitch : MonoBehaviour
    {
        #region Serialized Fields



        #endregion


        #region Fields

        private AudioSource _audioSource;

        #endregion


        #region GetSets / Properties



        #endregion


        #region Unity Built-Ins

        void Awake()
        {
            if (_audioSource == null) TryGetComponent(out _audioSource);

            MainMenu.OnGameStart += Load;
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

        void OnDestroy()
        {
            MainMenu.OnGameStart -= Load;
        }

        #endregion


        #region Events

        

        #endregion


        #region Methods

        public void Load(string name)
        {
            SceneManager.LoadScene(name);
        }

        #endregion


        #region Helpers



        #endregion
    }
}
