using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GlyphaeScripts
{
    public class PetButton : MonoBehaviour
    {
        #region Serialized Fields

        [Header("Button values")]
        [SerializeField] Button petButton;
        [SerializeField] Image petIcon;
        [SerializeField] TMP_Text petName;
        [SerializeField] TMP_Text petCategory;

        [Header("View values")]
        [SerializeField] GameObject viewButton;
        [SerializeField] Image viewIcon;
        [SerializeField] TMP_Text viewName;
        [SerializeField] TMP_Text viewCategory;
        [SerializeField] TMP_Text viewDescription;

        #endregion


        #region Fields

        private string description;

        #endregion


        #region Methods

        public void Setup(Pet pet, UnityAction action)
        {
            petIcon.sprite = pet.Symbol;
            petName.text = pet.Name;
            petCategory.text = pet.Category;
            petButton.interactable = pet.Unlocked;
            description = pet.Description;

            petButton.onClick.AddListener(action);
            petButton.onClick.AddListener(SetupView);
        }

        #endregion


        #region Helpers

        private void SetupView()
        {
            viewIcon.sprite = petIcon.sprite;
            viewName.text = petName.text;
            viewCategory.text = petCategory.text;
            viewDescription.text = description;
            // Remove later
            if (petName.text.Equals("Horus")) viewButton.SetActive(false);
            else viewButton.SetActive(true);
        }

        #endregion
    }
}