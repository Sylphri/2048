using UnityEngine;

public class Music : MonoBehaviour
{
    [SerializeField] private AudioSource _source;
    
    public void Pause()
    {
        _source.Pause();     
    }
    
    public void Play()
    {
        _source.Play(); 
    }
    
    private void Awake()
    {
        Object[] objs = FindObjectsOfType<Music>();

        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
            return;
        }

        DontDestroyOnLoad(this.gameObject);
        
        if (PlayerPrefs.GetInt("Music", 1) == 1)
            Play();
        else 
            Pause();
    }
}
