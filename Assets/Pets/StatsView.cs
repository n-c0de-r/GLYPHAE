using System;
using TMPro;
using UnityEngine;

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


        #region Unity Built-Ins

        private void OnEnable()
        {
            petName.text = settings.SelectedPet.Name;
            birthday.text = settings.SelectedPet.BirthTime.ToShortDateString();
            age.text = (DateTime.Now - settings.SelectedPet.BirthTime).Days.ToString() + " days";

            level.text = settings.SelectedPet.Level.ToString() + " (" + (int)settings.SelectedPet.Level + "/" + Enum.GetValues(typeof(Evolutions)).Length + ")";
            steps.text = settings.SelectedPet.EvolutionCalls + "/" + Enum.GetValues(typeof(Evolutions)).Length;

            int learned = 0;
            foreach (GlyphData item in settings.SelectedPet.Literals)
                if (item.MemoryLevel == MemoryLevels.Seen)
                    learned++;

            learning.text = learned + "/" + settings.SelectedPet.Literals.Count.ToString();
        }

        #endregion
    }
}