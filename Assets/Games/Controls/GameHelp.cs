using TMPro;
using UnityEngine;

namespace GlyphaeScripts
{
    public class GameHelp : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField] private TMP_Text gameName;

        [Tooltip("A short description of this Minigame.")]
        [SerializeField] private TMP_Text description;

        [Tooltip("Instructions how to play the game.")]
        [SerializeField] private TMP_Text instructions;

        #endregion


        #region Methods

        public void Setup(string newName, string newDescription, string newInstcructions)
        {
            gameName.text = newName;
            description.text = newDescription;
            instructions.text = newInstcructions;
        }

        #endregion
    }
}