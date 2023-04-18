using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    [SerializeField] private GameObject _winMenu;
    
    private Sounds _sounds;
    
    private void Start()
    {
        _sounds = FindObjectOfType<Sounds>();
    }
    
    public void OnRestartBtnClick()
    {
        _sounds.Play();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnMenuBtnClick()
    {
        _sounds.Play();
        SceneManager.LoadScene("Menu");
    }

    public void OnContinueBtnClick()
    {
        _sounds.Play();
        _winMenu.SetActive(false);     
    }
    
    private void Update()
    {
        if (!Input.GetKey(KeyCode.Escape)) return;
        OnMenuBtnClick();
    }
}
