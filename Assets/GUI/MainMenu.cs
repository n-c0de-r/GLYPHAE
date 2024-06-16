using UnityEngine;
using UnityEngine.Events;

public class MainMenu : MonoBehaviour
{
    //[SerializeField] private Settings dafaultSettings;
    [SerializeField] private UnityEvent onInit;

    private void Awake()
    {
    }

    private void Start()
    {
        onInit.Invoke();
    }

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
}