using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] UIManager uiManager;

    //Vibration
    public bool Vib { get; private set; }
    [SerializeField] private Toggle vibToggle;

    // Music and SFXs
    [SerializeField] private AudioSource music;
    [SerializeField] private AudioClip normalAudioClip;
    [SerializeField] private AudioClip impossibleAudioClip;

    [SerializeField] private AudioMixer SFXmixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider SFXSlider;
    private void Awake()
    {
        // Vibration
        Vib = PlayerPrefs.GetInt("vib", 1) == 1;
        vibToggle.isOn = Vib;

        // Set Music/SFX
        float musicVolume = PlayerPrefs.GetFloat("Music", 0.5f);
        music.volume = musicVolume;
        musicSlider.value = musicVolume;
    }

    private void Start()
    {
        float sfxVolume = PlayerPrefs.GetFloat("SFX", 0f);
        SFXmixer.SetFloat("SFXVolume", sfxVolume);
        SFXSlider.value = sfxVolume;
        ImpossibleDifficultyMusic();
    }

    public void UpdateVib()
    {
        PlayerPrefs.SetInt("vib",vibToggle.isOn ? 1 : 0);
        Vib = PlayerPrefs.GetInt("vib") == 1;
    }

    public void NextDifficulty()
    {
        gameManager.GameDifficulty = (GameDifficultyEnum)(((int) gameManager.GameDifficulty + 1) % GameDifficultyEnum.GetNames(typeof(GameDifficultyEnum)).Length);
        uiManager. UpdateDifficultyText();
        ImpossibleDifficultyMusic();
    }

    public void PreviousDifficulty()
    {
        if((int)gameManager.GameDifficulty == 0)
        {
            gameManager.GameDifficulty = (GameDifficultyEnum)((int)GameDifficultyEnum.GetNames(typeof(GameDifficultyEnum)).Length) - 1;
        }
        else
        {
            gameManager.GameDifficulty = (GameDifficultyEnum)((int)gameManager.GameDifficulty - 1);
        }
        uiManager.UpdateDifficultyText();
        ImpossibleDifficultyMusic();
    }

    private void ImpossibleDifficultyMusic()
    {
        if(gameManager.GameDifficulty != GameDifficultyEnum.Impossible)
        {
            if(music.clip == impossibleAudioClip)
            {
                music.clip = normalAudioClip;
                music.loop = true;
                music.Play();
            }
            return;
        }
        music.clip = impossibleAudioClip;
        music.loop = true;
        music.Play();
    }

    public void OnMusicChanged()
    {
        music.volume = musicSlider.value;
        PlayerPrefs.SetFloat("Music", musicSlider.value);
    }

    public void OnSFXChanged()
    {
        float volume = SFXSlider.value;
        SFXmixer.SetFloat("SFXVolume", SFXSlider.value);
        if(SFXSlider.value == -40f)
        {
            volume = -80f;
        }
        SFXmixer.SetFloat("SFXVolume", volume);
        PlayerPrefs.SetFloat("SFX", volume);
    }
}
