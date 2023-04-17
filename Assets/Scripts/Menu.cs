using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Menu : MonoBehaviour
{
    [SerializeField] private GameObject _modesPanel;
    [SerializeField] private GameObject _customSettings;
    [SerializeField] private TMP_Dropdown _widthDropdown;
    [SerializeField] private TMP_Dropdown _heightDropdown;
    [SerializeField] private Toggle _bonusesToggle;

    private bool _inMenu = true;
    
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
        _inMenu = false;
        SwitchMenu();
    }

    public void OnCustomPlayBtnClick()
    {
        PlayerPrefs.SetInt("Width", int.Parse(_widthDropdown.captionText.text));
        PlayerPrefs.SetInt("Height", int.Parse(_heightDropdown.captionText.text));
        PlayerPrefs.SetInt("Bonuses", _bonusesToggle.isOn ? 1 : 0);
        SceneManager.LoadScene("CustomMode");
    }

    private void SwitchMenu()
    {
        _modesPanel.SetActive(_inMenu);
        _customSettings.SetActive(!_inMenu);
    }

    private void Update()
    {
        if (!Input.GetKey(KeyCode.Escape)) return;
        
        if (!_inMenu)
        {
            _inMenu = true;
            SwitchMenu();
        }
        else 
        {
            Application.Quit();
        }
    }
}
