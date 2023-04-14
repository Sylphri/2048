using UnityEngine;
using UnityEngine.SceneManagement;

public class LoseMenu : MonoBehaviour
{
    [SerializeField] private string _sceneName;

    public void OnRestartBtnClick()
    {
        SceneManager.LoadScene(_sceneName);
    }
}
