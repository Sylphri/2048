using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Menu : MonoBehaviour
{
    [SerializeField] private GameObject _playButton;
    [SerializeField] private GameObject _modesPanel;
    [SerializeField] private GameObject _customSettings;
    [SerializeField] private TMP_Dropdown _widthDropdown;
    [SerializeField] private TMP_Dropdown _heightDropdown;
    [SerializeField] private Toggle _bonusesToggle;
    
    public void OnPlayBtnClick() 
    {
        _playButton.SetActive(false);
        _modesPanel.SetActive(true);
    }

    public void OnClassicBtnClick()
    {
        SceneManager.LoadScene("ClassicMode");
    }
    
    public void OnDoubleBtnClick()
    {
        SceneManager.LoadScene("DoubleMode");
    }
    
    public void OnBonusBtnClick()
    {
        SceneManager.LoadScene("BonusMode");
    }

    public void OnCustomBtnClick()
    {
        _modesPanel.SetActive(false);
        _customSettings.SetActive(true);
    }

    public void OnCustomPlayBtnClick()
    {
        PlayerPrefs.SetInt("Width", int.Parse(_widthDropdown.captionText.text));
        PlayerPrefs.SetInt("Height", int.Parse(_heightDropdown.captionText.text));
        PlayerPrefs.SetInt("Bonuses", _bonusesToggle.isOn ? 1 : 0);
        SceneManager.LoadScene("CustomMode");
    }
}
