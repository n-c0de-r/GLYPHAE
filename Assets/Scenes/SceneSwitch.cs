using UnityEngine;
using UnityEngine.SceneManagement;

namespace GlyphaeScripts
{
    public class SceneSwitch : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField] private string nextScene;

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

        void OnDestroy()
        {

        }

        #endregion


        #region Events



        #endregion


        #region Methods

        /// <summary>
        /// Loads the next scene after this.
        /// </summary>
        public void Next()
        {
            SceneManager.LoadScene(nextScene);
        }

        /// <summary>
        /// Loads the scene with the given name.
        /// </summary>
        /// <param name="name">The scene name to load.</param>
        public void Load(string name)
        {
            SceneManager.LoadScene(name);
        }

        /// <summary>
        /// Quits the application or editor.
        /// </summary>
        public void Quit()
        {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
            Application.Quit();
        }

        #endregion


        #region Helpers



        #endregion
    }
}
