using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class Menu : MonoBehaviour
{
    [SerializeField] private GameObject _menuPanel;
    [SerializeField] private GameObject _customSettings;
    [SerializeField] private TMP_Dropdown _widthDropdown;
    [SerializeField] private TMP_Dropdown _heightDropdown;
    [SerializeField] private Toggle _bonusesToggle;
    [SerializeField] private GameObject _howToPlayMenu;
    [SerializeField] private GameObject _aboutMenu;
    [SerializeField] private TMP_Text _soundsBtnText;
    [SerializeField] private TMP_Text _musicBtnText;
     
    private enum State
    {
        MENU,
        CUSTOM_SETTINGS,
        HOW_TO_PLAY_MENU,
        ABOUT_MENU
    };

    private State _state = State.MENU;
    private Sounds _sounds;
    private Music _music;
    private bool _soundsEnable;
    private bool _musicEnable;

    private void Start()
    {
        _sounds = FindObjectOfType<Sounds>();
        _music = FindObjectOfType<Music>();
        _soundsEnable = PlayerPrefs.GetInt("Sounds", 1) == 1;
        _musicEnable = PlayerPrefs.GetInt("Music", 1) == 1;
        _soundsBtnText.text = _soundsEnable ? "Sounds ON" : "Sounds OFF";
        _musicBtnText.text = _musicEnable ? "Music ON" : "Music OFF";
    }
    
    public void OnClassicBtnClick()
    {
        _sounds.Play();
        SceneManager.LoadScene("ClassicMode");       
    }
    
    public void OnDoubleBtnClick()
    {
        _sounds.Play();
        SceneManager.LoadScene("DoubleMode");
    }
    
    public void OnBonusBtnClick()
    {
        _sounds.Play();
        SceneManager.LoadScene("BonusMode");
    }

    public void OnCustomBtnClick()
    {
        _sounds.Play();
        _state = State.CUSTOM_SETTINGS;
        SwitchMenu();
    }

    public void OnHowToPlayClick()
    {
        _sounds.Play();
        _state = State.HOW_TO_PLAY_MENU;
        SwitchMenu();
    }

    public void OnAboutClick()
    {
        _sounds.Play();
        _state = State.ABOUT_MENU;
        SwitchMenu();
    }

    public void OnSoundsClick()
    {
        _sounds.Play();
        _soundsEnable = !_soundsEnable;
        PlayerPrefs.SetInt("Sounds", _soundsEnable ? 1 : 0);
        _soundsBtnText.text = _soundsEnable ? "Sounds ON" : "Sounds OFF";
    }

    public void OnMusicClick()
    {
        _sounds.Play(); 
        _musicEnable = !_musicEnable;
        PlayerPrefs.SetInt("Music", _musicEnable ? 1 : 0);
        _musicBtnText.text = _musicEnable ? "Music ON" : "Music OFF";

        if (_musicEnable)
            _music.Play();
        else
            _music.Pause();
    }

    public void OnCustomPlayBtnClick()
    {
        _sounds.Play();
        PlayerPrefs.SetInt("Width", int.Parse(_widthDropdown.captionText.text));
        PlayerPrefs.SetInt("Height", int.Parse(_heightDropdown.captionText.text));
        PlayerPrefs.SetInt("Bonuses", _bonusesToggle.isOn ? 1 : 0);
        SceneManager.LoadScene("CustomMode");
    }

    private void SwitchMenu()
    {
        _menuPanel.SetActive(_state == State.MENU);
        _customSettings.SetActive(_state == State.CUSTOM_SETTINGS);
        _howToPlayMenu.SetActive(_state == State.HOW_TO_PLAY_MENU);
        _aboutMenu.SetActive(_state == State.ABOUT_MENU);
    }

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape)) return;
        
        if (_state != State.MENU)
        {
            _state = State.MENU;
            SwitchMenu();
        }
        else 
        {
            Debug.Log("exit");
            Application.Quit();
        }
    }
}
