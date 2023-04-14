using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField] private GameObject _playButton;
    [SerializeField] private GameObject _modesPanel;
    
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
}
