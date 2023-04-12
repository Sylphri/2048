using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void OnPlayButtonClick() 
    {
        SceneManager.LoadScene("ClassicMode");     
    }
}
