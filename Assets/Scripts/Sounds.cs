using UnityEngine;

public class Sounds : MonoBehaviour
{
    [SerializeField] private AudioSource _source;

    public void Play()
    {
        if (PlayerPrefs.GetInt("Sounds", 1) == 1)
            _source.Play();
    }
    
    private void Awake()
    {
        Object[] objs = FindObjectsOfType<Sounds>();

        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }
}
