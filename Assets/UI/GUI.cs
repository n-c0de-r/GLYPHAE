using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GUI : MonoBehaviour
{
    #region Serialized Fields
    
    [SerializeField] private TMP_Text coinCounter;
    [SerializeField] private Slider enemyHealth;
    [SerializeField] private GameObject pauseMenu, menuPanel, volumePanel;
    //[SerializeField] private Settings currentSettings;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider mainVolume, musicVolume, soundVolume;

    [Tooltip("The action used to call up the menu.")]
    [SerializeField] private InputActionReference menuAction;

    #endregion


    #region Built-Ins / MonoBehaviours

    private void Awake()
    {
        ResetAudio();
        menuAction.action.performed += TogglePause;
    }

    #endregion


    #region Methods

    /// <summary>
    /// Toggles the Pause on or off.
    /// </summary>
    public void TogglePause()
    {
        if (pauseMenu == null) return;
        pauseMenu.SetActive(!pauseMenu.activeInHierarchy);
        Time.timeScale = Convert.ToInt32(!pauseMenu.activeInHierarchy);
    }

    /// <summary>
    /// Loads the given scene.
    /// </summary>
    /// <param name="scene">Scene name.</param>
    public void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    /// <summary>
    /// Sets the in-game volume to a given value.
    /// </summary>
    /// <param name="volume">The volume to set to.</param>
    public void SetMainVolume(float volume)
    {
        audioMixer.SetFloat("MainVolume", volume);
        //currentSettings.MainVolume = volume;
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", volume);
        //currentSettings.MusicVolume = volume;
    }

    public void SetSoundVolume(float volume)
    {
        audioMixer.SetFloat("SoundVolume", volume);
        //currentSettings.SoundVolume = volume;
    }

    private void ResetAudio()
    {
        //float main = currentSettings.MainVolume;
        //float music = currentSettings.MusicVolume;
        //float sound = currentSettings.SoundVolume;

        //SetMainVolume(main);
        //SetMusicVolume(music);
        //SetSoundVolume(sound);

        //mainVolume.value = main;
        //musicVolume.value = music;
        //soundVolume.value = sound;
    }
    private void TogglePause(InputAction.CallbackContext context)
    {
        TogglePause();
    }

    #endregion
}
