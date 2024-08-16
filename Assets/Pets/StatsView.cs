using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GlyphaeScripts
{
    public class StatsView : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField] private Settings settings;

        [Header("Pet Numbers")]
        [SerializeField] private TMP_Text petName;
        [SerializeField] private TMP_Text birthday;
        [SerializeField] private TMP_Text age;
        [SerializeField] private TMP_Text level;
        [SerializeField] private TMP_Text steps;
        [SerializeField] private TMP_Text learning;

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
            petName.text = settings.SelectedPet.Name;
            birthday.text = settings.SelectedPet.BirthTime.ToShortDateString();
            age.text = (DateTime.Now - settings.SelectedPet.BirthTime).Days.ToString() + " days";

            level.text = settings.SelectedPet.Level.ToString() + " (" + (int)settings.SelectedPet.Level + "/" + Enum.GetValues(typeof(Evolutions)).Length + ")";
            steps.text = settings.SelectedPet.EvolutionCalls + "/" + Enum.GetValues(typeof(Evolutions)).Length;

            int learned = 0;
            foreach (GlyphData item in settings.SelectedPet.Literals)
                if (item.MemoryLevel == MemoryLevels.Memorized)
                    learned++;

            learning.text = learned + "/" + settings.SelectedPet.Literals.Count.ToString();
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

        private void TemplateHelper(bool param)
        {
            
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