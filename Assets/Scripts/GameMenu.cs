using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    public void OnRestartBtnClick()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnMenuBtnClick()
    {
        SceneManager.LoadScene("Menu");
    }
    
    private void Update()
    {
        if (!Input.GetKey(KeyCode.Escape)) return;
        OnMenuBtnClick();
    }
}
