using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MenuController : MonoBehaviour
{
    [Header("Volume Settings")]
    [SerializeField] TMP_Text volumeTextValue = null;
    [SerializeField] Slider volumeSlider = null;
    [SerializeField] LevelLoader lvlLoader;


    public void SetVolume(float _volume)
    {
        // AudioListener.volume
        AudioListener.volume = _volume;
        volumeTextValue.text = _volume.ToString();
    }

    public void VolumeApply()
    {
        PlayerPrefs.SetFloat("masterVolume", AudioListener.volume);
    }

    public void ExtiGame()
    {
        Application.Quit();
    }

    public void onButtonClick()
    {
        Soundmanager.PlaySound(Soundmanager.Sound.Buttonclick);
    }

    public void startGame()
    {
        Soundmanager.PlaySound(Soundmanager.Sound.StartGame);
        lvlLoader.LoadNextLevel();
    }

}
