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
