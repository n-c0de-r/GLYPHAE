using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSlider : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    public void Reset()
    {
        string key = gameObject.transform.parent.name + gameObject.name;
        if (PlayerPrefs.HasKey(key))
        {
            Debug.Log(key + " " + PlayerPrefs.GetFloat(key));
            float value = PlayerPrefs.GetFloat(key);
            audioMixer.SetFloat(key, value);
            gameObject.GetComponent<Slider>().SetValueWithoutNotify(value);
        }
    }

    public void ChangeVolume(float volume)
    {
        string key = gameObject.transform.parent.name + gameObject.name;
        audioMixer.SetFloat(key, volume);
        PlayerPrefs.SetFloat(key, volume);
        gameObject.GetComponent<Slider>().SetValueWithoutNotify(volume);
    }

    private void OnDestroy()
    {
        string key = gameObject.transform.parent.name + gameObject.name;

        Debug.Log(key + " " + PlayerPrefs.GetFloat(key));
    }
}