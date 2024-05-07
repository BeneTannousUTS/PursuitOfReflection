using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Settings : MonoBehaviour
{

    [SerializeField] Slider musicSlider;
    [SerializeField] Slider soundSlider;

    [SerializeField] Image musicImage;
    [SerializeField] Image soundImage;

    [SerializeField] Sprite musicIcon;
    [SerializeField] Sprite musicMuteIcon;
    [SerializeField] Sprite soundIcon;
    [SerializeField] Sprite soundMuteIcon;

    public void Start()
    {
        Time.timeScale = 0;

        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", BackgroundMusic.instance.audioSource.volume);
        soundSlider.value = PlayerPrefs.GetFloat("SoundVolume", 1f);

        VolumeMusicChanged();
        VolumeSoundChanged();
    }


    public void MuteMusic()
    {
        musicSlider.value = 0;
        VolumeMusicChanged();
    }


    public void VolumeMusicChanged()
    {
        BackgroundMusic.ChangeVolume(musicSlider.value);
        musicImage.sprite = musicSlider.value == 0 ? musicMuteIcon : musicIcon;
    }


    public void MuteSound()
    {
        soundSlider.value = 0;
        VolumeSoundChanged();
    }


    public void VolumeSoundChanged()
    {
        soundImage.sprite = soundSlider.value == 0 ? soundMuteIcon : soundIcon;
    }


    public void Close()
    {
        PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
        PlayerPrefs.SetFloat("SoundVolume", soundSlider.value);

        Time.timeScale = 1;
        SceneManager.UnloadSceneAsync("Settings");
    }
}
