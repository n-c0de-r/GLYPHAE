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

        public void Next()
        {
            SceneManager.LoadScene(nextScene);
        }

        public void Load(string name)
        {
            SceneManager.LoadScene(name);
        }

        #endregion


        #region Helpers



        #endregion
    }
}
