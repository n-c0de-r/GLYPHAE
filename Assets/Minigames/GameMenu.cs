using UnityEngine;
using static Unity.Collections.AllocatorManager;

namespace GlyphaeScripts
{
    public class GameMenu : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField] private GameObject[] sidePanels;
        [SerializeField] private GameObject mainPanel;

        #endregion


        #region Fields
        
        

        #endregion


        #region GetSets / Properties
        
        

        #endregion


        #region Unity Built-Ins

        void Awake()
        {
        }

        private void OnEnable()
        {
            GameManager.OnGameStart += () => mainPanel.SetActive(false);
            GameManager.OnGameEnd += () => mainPanel.SetActive(true);
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

        private void OnDisable()
        {
            GameManager.OnGameStart -= () => mainPanel.SetActive(false);
            GameManager.OnGameEnd -= () => mainPanel.SetActive(true);
        }

        void OnDestroy()
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

        private void HideSides()
        {
            foreach (GameObject go in sidePanels)
            {
                go.SetActive(false);
            }
        }

        private void ShowSides()
        {
            foreach (GameObject go in sidePanels)
            {
                go.SetActive(true);
            }
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