using UnityEngine;

namespace GlyphaeScripts
{
    public class PetSelect : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField] private Settings settings;

        [Header("Input Objects")]
        [SerializeField] private GameObject buttonContainer;
        [SerializeField] private GameObject templateButton;

        #endregion


        #region Fields



        #endregion


        #region Events



        #endregion


        #region GetSets / Properties



        #endregion


        #region Unity Built-Ins

        void Awake()
        {
            
        }

        private void OnEnable()
        {
            SetupButtons();
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
            ResetButtons();
        }

        private void OnDestroy()
        {
            
        }

        #endregion


        #region Methods

        public void TemplateMethod(bool param)
        {
            
        }

        #endregion


        #region Helpers

        private void ResetButtons()
        {
            for (int i = 0; i < buttonContainer.transform.childCount; i++)
            {
                Destroy(buttonContainer.transform.GetChild(i).gameObject);
            }
        }

        private void SetupButtons()
        {
            foreach (Pet pet in settings.Pets)
            {
                GameObject go = Instantiate(templateButton, buttonContainer.transform);
                PetButton button = go.GetComponent<PetButton>();
                go.name = pet.Name;
                if (pet.Unlocked)
                    button.Setup(pet, () => settings.SelectedPet = pet);
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