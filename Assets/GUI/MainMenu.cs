using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    #region Serialized Fields

    //[SerializeField] private Settings dafaultSettings;

    #endregion


    #region Methods

    /// <summary>
    /// Loads the given scene.
    /// </summary>
    /// <param name="scene">Scene name.</param>
    public void StartGame() { }
       // => SceneManager.LoadScene((int)Scenes.GAME);

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
}